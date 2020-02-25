using Common;
using Discogs;
using Discogs.Entity;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreKeeper.CollectionManager
{
    public class CollectionManager : ICollectionManager
    {
        public CollectionManager(DiscogsClient client, IFileManager fileManager, IMusicFileAnalyzer musicFileAnalyzer, IRepository repository)
        {
            _discogsClient = client;
            _fileManager = fileManager;
            _musicFileAnalyzer = musicFileAnalyzer;
            _repo = repository;
            _discogsConverter = new DiscogsConverter();
        }

        #region [  fields  ]

        private readonly DiscogsClient _discogsClient;
        private readonly IFileManager _fileManager;
        private readonly IMusicFileAnalyzer _musicFileAnalyzer;
        private readonly IRepository _repo;
        private readonly DiscogsConverter _discogsConverter;

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="mDirInfo"></param>
        /// <returns></returns>
        public async Task<Artist> SearchArtistAndAllAlbumsOnDiscogs(IMusicDirInfo mDirInfo)
        {
            //получение имени артиста. переделать
            var artistTrackInfo = mDirInfo.TrackInfos.FirstOrDefault();
            if (artistTrackInfo == null) return null;
            //получение DiscogsArtist
            var dArtist = await _discogsClient.GetArtistByName(artistTrackInfo.ArtistName);
            //получение DiscogsArtistReleases
            var allDArtistReleases = await _discogsClient.GetArtistReleases(dArtist.id);
            //создание исполнителя и списка его альбомов
            //создаю исполнителя
            var artist = _discogsConverter.CreateArtist(dArtist);
            //создаю коллекцию всех альбомов исполнителя
            var allArtistAlbums = _discogsConverter.CreateArtistAlbums(allDArtistReleases);
            artist.Albums = allArtistAlbums;
            //сохраняю исполнителя в базе
            var artistId = _repo.AddOrUpdateArtistFull(artist);
            //создаю папку с исполнителем на диске
            var artPath = _fileManager.CreateArtistStorageDirectory(MusicCollectionDirectory, artist.Name);
            //добавляю путь к папке исполнителя в базу
            _repo.AddArtistToStorage(artistId, artPath);
            //TODO:добавляю фотографии
            var imagePath = Path.Combine(artPath, "photos");
            DownloadArtistImages(dArtist, imagePath);

            return artist;
        }

        public async Task<Album> SearchFullAlbumOnDiscogs(Artist artist, IMusicDirInfo mDirInfo)
        {
            //получение имени артиста. переделать
            var artistTrackInfo = mDirInfo.TrackInfos.FirstOrDefault();
            if (artistTrackInfo == null) return null;
            //получение списка всех альбомов исполнителя
            var allDArtistReleases = await _discogsClient.GetArtistReleases(artist.DiscogsId);
            //поиск заданного альбома в списке всех альбомов исполнителя
            var selectedArtistRelease = allDArtistReleases.FirstOrDefault(arg =>
                arg.title.Equals(artistTrackInfo.AlbumTitle, StringComparison.InvariantCultureIgnoreCase));
            if (selectedArtistRelease == null)
            {
                //TODO: Try searching by album and track names
                return null;
            }
            //получение discogsId для заданного альбома. Пока пропускаю master release
            var releaseId = 0;
            if (selectedArtistRelease.type == "master")
            {
                //Always searches for main release
                var dMasterRelease = await _discogsClient.GetMatserReleaseById(selectedArtistRelease.id);
                releaseId = dMasterRelease.main_release;
            }
            else
            {
                releaseId = selectedArtistRelease.id;
            }
            //получение информации об альбоме с дискогс
            var dRelease = await _discogsClient.GetReleaseById(releaseId);
            //создаю альбом с полной информацией
            var albumToCollection = _discogsConverter.CreateAlbum(dRelease);
            //сохраняю альбом в базе
            var albumId = _repo.AddOrUpdateAlbum(artist.Id, albumToCollection);
            //переделать эту жуть
            var storedAlbum = _repo.FindAlbumById(albumId);
            //TODO:Получаю путь для сохранения альбома. Переделать
            var albumStorageName = CreateAlbumDirectoryName(storedAlbum);
            var albumStoragePath = _fileManager.CreateAlbumStorageDirectory(artist.StoragePath, albumStorageName);
            //Сохраняю физическую копию альбома в хранилище.
            _fileManager.MoveMusicDirectory(mDirInfo, albumStoragePath);
            //TODO: Сохраняю фотографии из дискогс
            DownloadAlbumImages(dRelease, Path.Combine(albumStoragePath, "images"));
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