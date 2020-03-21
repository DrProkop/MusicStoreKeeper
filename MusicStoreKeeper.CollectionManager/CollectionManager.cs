using Common;
using Discogs;
using Discogs.Entity;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreKeeper.CollectionManager
{
    public class CollectionManager : ICollectionManager
    {
        public CollectionManager(DiscogsClient client, IFileManager fileManager, IMusicFileAnalyzer musicFileAnalyzer, IMusicDirAnalyzer musicDirAnalyzer, IRepository repository)
        {
            _discogsClient = client;
            _fileManager = fileManager;
            _musicFileAnalyzer = musicFileAnalyzer;
            _musicDirAnalyzer = musicDirAnalyzer;
            _repo = repository;
            _genreAndStyleProvider = new GenreAndStyleProvider();
            _discogsConverter = new DiscogsConverter(_genreAndStyleProvider);
            
        }

        #region [  fields  ]

        private readonly DiscogsClient _discogsClient;
        private readonly IFileManager _fileManager;
        private readonly IMusicFileAnalyzer _musicFileAnalyzer;
        private readonly IMusicDirAnalyzer _musicDirAnalyzer;
        private readonly IRepository _repo;
        private readonly DiscogsConverter _discogsConverter;
        private readonly GenreAndStyleProvider _genreAndStyleProvider;

        #endregion [  fields  ]

        #region [  properties  ]

        public string MusicSearchDirectory
        {
            get => Properties.Settings.Default.MusicSearchDirectory;
            set
            {
                Properties.Settings.Default.MusicSearchDirectory = value;
                Properties.Settings.Default.Save();
            }
        }

        public string MusicCollectionDirectory
        {
            get => Properties.Settings.Default.MusicCollectionDirectory;
            set
            {
                Properties.Settings.Default.MusicCollectionDirectory = value;
                Properties.Settings.Default.Save();
            }
        }

        #endregion [  properties  ]

        #region [  public methods  ]

        public async Task<Artist> SearchArtistAndAllAlbumsOnDiscogs(IMusicDirInfo mDirInfo, bool updateExisting = true)
        {
            if (mDirInfo == null) throw new ArgumentNullException(nameof(mDirInfo));
            //получение имени артиста. переделать
            var artistTrackInfo = mDirInfo.TrackInfos.FirstOrDefault();
            if (artistTrackInfo == null) return null;
            //получение DiscogsArtist
            var dArtist = await _discogsClient.GetArtistByName(artistTrackInfo.ArtistName);
            //поиск исполнителя в коллекции
            var artistInCollection = _repo.FindArtistByNameOrDiscogsId(artistTrackInfo.ArtistName, dArtist.id);
            //возврат, если обновление не требуется
            if (!updateExisting && artistInCollection != null)
            {
                return artistInCollection;
            }
            //получение DiscogsArtistReleases
            var allDArtistReleases = await _discogsClient.GetArtistReleases(dArtist.id);
            //создание исполнителя и списка его альбомов
            //создаю исполнителя
            var artist = _discogsConverter.CreateArtist(dArtist);
            //создаю коллекцию всех альбомов исполнителя
            var allArtistAlbums = _discogsConverter.CreateArtistAlbums(allDArtistReleases);
            artist.Albums = allArtistAlbums;
            //обновление исполнителя
            if (artistInCollection != null)
            {
                _repo.UpdateArtist(artistInCollection.Id, artist);
                return artistInCollection;
            }
            //добавление исполнителя
            //сохраняю исполнителя в базе
            var artistId = _repo.AddNewArtist(artist);
            //создаю папку с исполнителем на диске
            var artPath = _fileManager.CreateArtistStorageDirectory(MusicCollectionDirectory, artist.Name);
            var imagePath = Path.Combine(artPath, _fileManager.DefaultArtistPhotosDirectory);
            DownloadArtistImages(dArtist, imagePath);
            //добавляю путь к папке исполнителя в базу
            _repo.AddArtistToStorage(artistId, artPath);
            return artist;
        }

        public async Task<Album> SearchFullAlbumOnDiscogs(Artist artist, IMusicDirInfo mDirInfo, bool updateExisting = false)
        {
            if (artist == null) throw new ArgumentNullException(nameof(artist));
            if (mDirInfo == null) throw new ArgumentNullException(nameof(mDirInfo));

            //получение имени артиста. переделать
            var artistTrackInfo = mDirInfo.TrackInfos.FirstOrDefault();
            if (artistTrackInfo == null) return null;

            //получение списка всех альбомов исполнителя
            var allDArtistReleases = await _discogsClient.GetArtistReleases(artist.DiscogsId);
            //поиск заданного альбома в списке всех альбомов исполнителя
            var matchingDArtistReleases = allDArtistReleases.Where(arg =>
                arg.title.Equals(artistTrackInfo.AlbumTitle, StringComparison.InvariantCultureIgnoreCase) || arg.title.Contains(artistTrackInfo.AlbumTitle)).ToList();
            if (!matchingDArtistReleases.Any())
            {
                //TODO: Try searching by album and track names. Manage EPs and singles
                return null;
            }

            DiscogsArtistRelease selectedDArtistRelease = null;
            var releaseId = 0;
            if (matchingDArtistReleases.Count() > 1)
            {
                //поиск по мастер релизу
                var masterRelease = matchingDArtistReleases.FirstOrDefault(rel => rel.type == "master");
                if (masterRelease != null)
                {
                    selectedDArtistRelease = masterRelease;
                }
                //сравнение директории с мазыкой и информации о релизе на дискогс
                else
                {
                    foreach (var dArtistRelease in matchingDArtistReleases)
                    {
                        var dReleaseToCompare = await _discogsClient.GetReleaseById(dArtistRelease.id);
                        if (!_musicDirAnalyzer.CompareAlbums(mDirInfo, dReleaseToCompare)) continue;
                        selectedDArtistRelease = dArtistRelease;
                        break;
                    }
                }
            }
            else
            {
                selectedDArtistRelease = matchingDArtistReleases.First();
            }

            if (selectedDArtistRelease == null) return null;

            //получение discogsId для заданного альбома. Пока пропускаю master release
            if (selectedDArtistRelease.type == "master")
            {
                //Always searches for main release
                var dMasterRelease = await _discogsClient.GetMatserReleaseById(selectedDArtistRelease.id);
                releaseId = dMasterRelease.main_release;
            }
            else
            {
                releaseId = selectedDArtistRelease.id;
            }

            //поиск релиза в коллекции
            var storedAlbum = _repo.FindAlbumByTitleOrDiscogsId(artistTrackInfo.AlbumTitle, releaseId);
            var albumExistsInCollection = (storedAlbum != null && storedAlbum.InCollection);
            //продумать удаление музыкальной папки
            if (albumExistsInCollection && !updateExisting) return storedAlbum;

            //получение информации об альбоме с дискогс
            var dRelease = await _discogsClient.GetReleaseById(releaseId);
            //создаю альбом с полной информацией
            var albumToCollection = _discogsConverter.CreateAlbum(dRelease);
            //добавляю стили исполнителю
            artist.AddStyles(albumToCollection.Styles);
            artist.AddGenres(albumToCollection.Genres);
            // сохраняю альбом в базе
            var albumId = 0;
            if (storedAlbum == null)
            {
                albumId = _repo.AddNewAlbum(artist.Id, albumToCollection);
            }
            else
            {
                albumId = _repo.UpdateAlbum(artist.Id, albumToCollection);
            }

            //TODO: переделать эту жуть
            storedAlbum = _repo.FindAlbumById(albumId);
            //TODO:Получаю путь для сохранения альбома. Переделать
            var albumStorageName = CreateAlbumDirectoryName(storedAlbum);
            var albumStoragePath = _fileManager.CreateAlbumStorageDirectory(artist.StoragePath, albumStorageName);
            //Сохраняю физическую копию альбома в хранилище.
            _fileManager.MoveMusicDirectory(mDirInfo, albumStoragePath);
            DownloadAlbumImages(dRelease, Path.Combine(albumStoragePath, _fileManager.DefaultAlbumImagesDirectory));
            //добавляю запись про место сохранения физической копии альбома
            _repo.AddAlbumToStorage(storedAlbum, albumStoragePath);

            return albumToCollection;
        }

        //TODO:Rework MoveToCollectionManually
        public void MoveToCollectionManually(ISimpleFileInfo sFi)
        {
            if (sFi == null) throw new ArgumentNullException(nameof(sFi));
            if (sFi.Type != SfiType.Directory) return;

            var dirInfo = new DirectoryInfo(sFi.Path);
            var musicFiles = dirInfo.GetFiles("*.mp3");
            if (musicFiles.Length == 0) return;
            //get album information
            var albumInfo = _musicFileAnalyzer.GetBasicAlbumInfoFromDirectory(dirInfo);
            //create destination path
            var destPath = Path.Combine(MusicCollectionDirectory, albumInfo.ArtistName, albumInfo.AlbumTitle);

            _fileManager.MoveMusicDirectory(dirInfo.FullName, destPath);
        }

        public IEnumerable<Artist> GetAllArtists()
        {
            return _repo.GetAllArtists();
        }

        public IEnumerable<Artist> GetRecentArtists()
        {
            var artistsIds = _repo.GetRecentlyAddedArtists();
            return artistsIds.Select(artistId => _repo.FindArtistById(artistId)).ToList();
        }

        public IEnumerable<Album> GetAllArtistAlbums(int artistId)
        {
            return _repo.GetAllArtistAlbums(artistId);
        }

        public Album GetAlbum(int albumId)
        {
            return _repo.GetAlbumWithTracks(albumId);
        }

        public void DeleteAlbumFromCollection(Album album)
        {
            _repo.DeleteAlbum(album);
        }

        public void DeleteArtistFromCollection(Artist artist)
        {
            _repo.DeleteArtist(artist);
        }

        public List<string> GetMusicStylesList()
        {
            return _genreAndStyleProvider.GetStyles().ToList();
        }

        public List<string> GetMusicGenresList()
        {
            return _genreAndStyleProvider.GetGenres().ToList();
        }
        #endregion [  public methods  ]

        #region [  private methods  ]

        private void DownloadArtistImages(DiscogsArtist dArtist, string dirPath)
        {
            if (dArtist == null) throw new ArgumentNullException(nameof(dArtist));
            if (string.IsNullOrEmpty(dirPath)) throw new ArgumentNullException(nameof(dirPath));

            var number = 1;
            foreach (var discogsImage in dArtist.images)
            {
                var photoName = $"{dArtist.name}_photo_{number}.jpg";
                _discogsClient.SaveImage(discogsImage, dirPath, photoName);
                number += 1;
            }
        }

        private void DownloadAlbumImages(DiscogsReleaseBase dRelease, string dirPath)
        {
            if (dRelease == null) throw new ArgumentNullException(nameof(dRelease));
            if (string.IsNullOrEmpty(dirPath)) throw new ArgumentNullException(nameof(dirPath));

            var number = 1;
            foreach (var discogsImage in dRelease.images)
            {
                var photoName = $"{dRelease.title}_img_{number}.jpg";
                _discogsClient.SaveImage(discogsImage, dirPath, photoName);
                number += 1;
            }
        }

        private string CreateAlbumDirectoryName(Album album)
        {
            return $"({album.ReleaseDate}) {album.Title}";
        }

        #endregion [  private methods  ]
    }
}