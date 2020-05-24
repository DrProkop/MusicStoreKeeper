using Common;
using Discogs;
using Discogs.Entity;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStoreKeeper.CollectionManager
{
    public class CollectionManager : ICollectionManager
    {
        public CollectionManager(DiscogsClient client,
            IFileManager fileManager,
            IMusicFileAnalyzer musicFileAnalyzer,
            IMusicDirAnalyzer musicDirAnalyzer,
            IImageService imageService,
            IRepository repository)
        {
            _discogsClient = client;
            _fileManager = fileManager;
            _musicFileAnalyzer = musicFileAnalyzer;
            _musicDirAnalyzer = musicDirAnalyzer;
            _imageService = imageService;
            _repo = repository;
            _genreAndStyleProvider = new GenreAndStyleProvider();
            _discogsConverter = new DiscogsConverter(_genreAndStyleProvider);
            CreateTempImageDirectory();
        }

        #region [  fields  ]

        private readonly DiscogsClient _discogsClient;
        private readonly IFileManager _fileManager;
        private readonly IMusicFileAnalyzer _musicFileAnalyzer;
        private readonly IMusicDirAnalyzer _musicDirAnalyzer;
        private readonly IImageService _imageService;
        private readonly IRepository _repo;
        private readonly DiscogsConverter _discogsConverter;
        private readonly GenreAndStyleProvider _genreAndStyleProvider;
        private string tempImageDirectoryPath;

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

        public async Task<Artist> SearchArtistAndAllAlbumsOnDiscogs(IMusicDirInfo mDirInfo, bool updateExisting, CancellationToken token)
        {
            if (mDirInfo == null) throw new ArgumentNullException(nameof(mDirInfo));
            token.ThrowIfCancellationRequested();
            //получение имени артиста. переделать
            var artistTrackInfo = mDirInfo.TrackInfos.FirstOrDefault();
            if (artistTrackInfo == null) return null;
            //получение DiscogsArtist
            var dArtist = await _discogsClient.GetArtistByName(artistTrackInfo.ArtistName, token);
            //поиск исполнителя в коллекции
            var artistInCollection = _repo.FindArtistByNameOrDiscogsId(artistTrackInfo.ArtistName, dArtist.id);
            //возврат, если обновление не требуется
            if (!updateExisting && artistInCollection != null)
            {
                return artistInCollection;
            }
            //получение DiscogsArtistReleases
            var allDArtistReleases = await _discogsClient.GetArtistReleases(dArtist.id, token);
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
            DownloadArtistOrAlbumImages(dArtist.images, dArtist.name, artist.ImageDataList, imagePath);
            //добавляю путь к папке исполнителя в базу
            _repo.AddArtistToStorage(artistId, artPath);
            return artist;
        }

        public async Task<Album> SearchFullAlbumOnDiscogs(Artist artist, IMusicDirInfo mDirInfo, bool updateExisting, CancellationToken token)
        {
            if (artist == null) throw new ArgumentNullException(nameof(artist));
            if (mDirInfo == null) throw new ArgumentNullException(nameof(mDirInfo));
            var albumCopied = false;
            var albumStoragePath = string.Empty;
            try
            {
                //получение имени артиста. переделать
                var artistTrackInfo = mDirInfo.TrackInfos.FirstOrDefault();
                if (artistTrackInfo == null) return null;

                //получение списка всех альбомов исполнителя
                var allDArtistReleases = await _discogsClient.GetArtistReleases(artist.DiscogsId, token);
                //поиск заданного альбома в списке всех альбомов исполнителя
                var matchingDArtistReleases = allDArtistReleases.Where(arg =>
                    arg.title.Equals(artistTrackInfo.AlbumTitle, StringComparison.InvariantCultureIgnoreCase) ||
                    arg.title.Contains(artistTrackInfo.AlbumTitle)).ToList();
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
                            var dReleaseToCompare = await _discogsClient.GetReleaseById(dArtistRelease.id, token);
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
                    var dMasterRelease = await _discogsClient.GetMatserReleaseById(selectedDArtistRelease.id, token);
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
                var dRelease = await _discogsClient.GetReleaseById(releaseId, token);
                //создаю альбом с полной информацией
                var albumToCollection = _discogsConverter.CreateAlbum(dRelease);
                //добавляю стили исполнителю
                artist.AddStyles(albumToCollection.Styles);
                artist.AddGenres(albumToCollection.Genres);
                //save album data in db
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
                albumStoragePath = _fileManager.CreateAlbumStorageDirectory(artist.StoragePath, albumStorageName);
                //save album copy to collection directory
                albumCopied = _fileManager.CopyMusicDirectory(mDirInfo, albumStoragePath);
                //save pictures from Discogs to album images directory
                var albumImageDirectoryPath = Path.Combine(albumStoragePath, _fileManager.DefaultAlbumImagesDirectory);
                DownloadArtistOrAlbumImages(dRelease.images, storedAlbum.Title, storedAlbum.ImageDataList, albumImageDirectoryPath);
                //scan album images directory to get all images data
                RefreshImageDirectory(storedAlbum.ImageDataList, albumImageDirectoryPath);
                //delete duplicate images from album image directory and db
                DeleteDuplicateImagesFromDirectoryAndDb(storedAlbum.ImageDataList, albumImageDirectoryPath);
                //add album storage path to db and change album status to "InCollection"
                _repo.AddAlbumToStorage(storedAlbum, albumStoragePath);

                return albumToCollection;
            }
            finally
            {
                //delete source music directory if it was successfully copied
                if (albumCopied)
                {
                    _fileManager.DeleteSourceMusicDirectoryFiles(mDirInfo);
                }
                //delete album directory in collection directory if something went wrong
                else
                {
                    if (!string.IsNullOrEmpty(albumStoragePath))
                    {
                        _fileManager.TryDeleteDirectory(albumStoragePath);
                    }
                }
            }
        }

        //TODO:Rework MoveToCollectionManually
        public void MoveToCollectionManually(ISimpleFileInfo sFi)
        {
            //if (sFi == null) throw new ArgumentNullException(nameof(sFi));
            //if (sFi.Type != SfiType.Directory) return;

            //var dirInfo = new DirectoryInfo(sFi.Path);
            //var musicFiles = dirInfo.GetFiles("*.mp3");
            //if (musicFiles.Length == 0) return;
            ////get album information
            //var albumInfo = _musicFileAnalyzer.GetBasicAlbumInfoFromDirectory(dirInfo);
            ////create destination path
            //var destPath = Path.Combine(MusicCollectionDirectory, albumInfo.ArtistName, albumInfo.AlbumTitle);

            //_fileManager.MoveMusicDirectory(dirInfo.FullName, destPath);
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

        private void DownloadArtistOrAlbumImages(DiscogsImage[] discogsImages, string imageNamePattern, List<ImageData> imageDataList, string targetDirPath)
        {
            var number = 1;
            //list of image names in target directory
            var imageNamesInTargetDirectory = _fileManager.GetFileNamesFromDirectory(targetDirPath);
            try
            {
                foreach (var discogsImage in discogsImages)
                {
                    //check imageDataList for exact image copies with the same not null source
                    if (!string.IsNullOrEmpty(discogsImage.uri) &&
                        imageDataList.Any(arg => arg.Source.Equals(discogsImage.uri)))
                    {
                        continue;
                    }

                    //generate name for new image
                    var newImageNameAndNumber = _fileManager.GenerateNameForDownloadedImage(imageNamesInTargetDirectory, imageNamePattern, number);
                    var newImageName = newImageNameAndNumber.Item1;
                    imageNamesInTargetDirectory.Add(newImageName);
                    number = newImageNameAndNumber.Item2 + 1;
                    //save image to temp directory
                    _discogsClient.SaveImage(discogsImage, tempImageDirectoryPath, newImageName);
                    //create new ImageData (important to add url to source property)
                    var tempImagePath = Path.Combine(tempImageDirectoryPath, newImageName);
                    //move image to target directory
                    _fileManager.MoveFile(tempImagePath, targetDirPath);
                    //add new imageData to imageDataList
                    var downloadedImageData = new ImageData() { Name = newImageName, Source = discogsImage.uri, Status = ImageStatus.InCollection };
                    imageDataList.Add(downloadedImageData);
                }
            }
            //TODO: Add exception handling to CollectionManager
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _fileManager.ClearDirectory(tempImageDirectoryPath);
            }
        }

        /// <summary>
        /// Check target directory for files with no appropriate info in collection db. Add new files to db.
        /// </summary>
        /// <param name="imageDataList">List of image data from db</param>
        /// <param name="targetDirPath">Directory to check</param>
        /// <returns>True whether any new images were found</returns>
        private bool RefreshImageDirectory(List<ImageData> imageDataList, string targetDirPath)
        {
            var newImagesWereFound = false;
            var imagesInTargetDirectory = _fileManager.GetImageFileInfosFromDirectory(targetDirPath);
            var imagesInCollection = _imageService.GetNumberOfImagesInCollection(imageDataList);
            //check number of images in collection and in target directory
            if (imagesInTargetDirectory.Count != imagesInCollection)
            {
                newImagesWereFound = true;
                //find new images
                foreach (var imageInTargetDirectory in imagesInTargetDirectory)
                {
                    //get name of each image in target directory
                    var imgName = imageInTargetDirectory.Name;
                    //check for matches in imageDataList
                    if (!imageDataList.Any(img => img.Name.Equals(imgName)))
                    {
                        //create new imageData
                        var newImageData = new ImageData { Name = imgName, Status = ImageStatus.InCollection };
                        //add new images to collection
                        imageDataList.Add(newImageData);
                    }
                }
            }
            return newImagesWereFound;
        }

        private void DeleteDuplicateImagesFromDirectoryAndDb(ICollection<ImageData> imageData, string directoryPath)
        {
            //search for duplicates in album images directory.
            var imagePaths = _fileManager.GetImageFileInfosFromDirectory(directoryPath);
            var duplicateImagePaths = _imageService.GetDuplicateImagePaths(imagePaths);
            //delete lower resolution image duplicates
            foreach (var duplicateImagePath in duplicateImagePaths)
            {
                var duplicateImageName = Path.GetFileName(duplicateImagePath);
                //delete file from image directory
                //TODO: Add TryDeleteFile to IFileManager
                _fileManager.DeleteFile(duplicateImagePath);
                //delete imageData from db
                var imgDataToDelete = imageData.FirstOrDefault(img => img.Name.Equals(duplicateImageName));
                if (imgDataToDelete != null)
                {
                    if (string.IsNullOrEmpty(imgDataToDelete.Source))
                    {
                        imageData.Remove(imgDataToDelete);
                    }
                    else
                    {
                        imgDataToDelete.Status = ImageStatus.Deleted;
                    }
                }
            }
        }

        private string CreateAlbumDirectoryName(Album album)
        {
            return $"({album.ReleaseDate}) {album.Title}";
        }

        private void CreateTempImageDirectory()
        {
            tempImageDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempImage");
            Directory.CreateDirectory(tempImageDirectoryPath);
        }

        #endregion [  private methods  ]
    }
}