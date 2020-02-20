using Common;
using Discogs;
using Discogs.Entity;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    //TODO: Create base class for vms, add logging to it.
    public class MusicSearchScreenVm : BaseScreenVm
    {
        public MusicSearchScreenVm(DiscogsClient client, IRepository repository, IMusicFileAnalyzer musicFileAnalyzer, IMusicDirAnalyzer musicDirAnalyzer, IFileManager fileManager, PreviewFactory previewFactory, ILoggerManager manager) : base(manager)
        {
            _discogsClient = client;
            _repo = repository;
            _musicFileAnalyzer = musicFileAnalyzer;
            _musicDirAnalyzer = musicDirAnalyzer;
            _fileManager = fileManager;
            _previewFactory = previewFactory;
            _discogsConverter = new DiscogsConverter();
            Initialize();
        }

        #region [  fields  ]

        private readonly DiscogsClient _discogsClient;
        private readonly IRepository _repo;
        private readonly IMusicFileAnalyzer _musicFileAnalyzer;
        private readonly IMusicDirAnalyzer _musicDirAnalyzer;
        private readonly IFileManager _fileManager;
        private readonly PreviewFactory _previewFactory;
        private readonly DiscogsConverter _discogsConverter;

        #endregion [  fields  ]

        #region [  properties  ]

        private ObservableCollection<SimpleFileInfoWrap> _musicDirectories = new ObservableCollection<SimpleFileInfoWrap>();

        public ObservableCollection<SimpleFileInfoWrap> MusicDirectories
        {
            get => _musicDirectories;
            set { _musicDirectories = value; OnPropertyChanged(); }
        }

        private SimpleFileInfoWrap _selectedItem;

        public SimpleFileInfoWrap SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        private IPreviewVm _filePreview;

        public IPreviewVm FilePreview
        {
            get => _filePreview;
            set { _filePreview = value; OnPropertyChanged(); }
        }

        private string _musicSearchDirectory;

        public string MusicSearchDirectory
        {
            get => _musicSearchDirectory;
            set
            {
                _musicSearchDirectory = value;
                OnPropertyChanged();
            }
        }

        private string _musicCollectionDirectory;

        public string MusicCollectionDirectory
        {
            get => _musicCollectionDirectory;
            set
            {
                _musicCollectionDirectory = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]

        #region [  commands  ]

        private ICommand _selectedItemChangedCommand;

        public ICommand SelectedItemChangedCommand => _selectedItemChangedCommand ?? (_selectedItemChangedCommand = new RelayCommand<SimpleFileInfoWrap>(arg =>
        {
            ChangeSelectedItem(arg);
        }));

        private ICommand _itemExpandedCommand;

        public ICommand ItemExpandedCommand => _itemExpandedCommand ?? (_itemExpandedCommand = new RelayCommand<RoutedEventArgs>(ExpandTreeViewItem));

        private ICommand _scanDirectoryCommand;

        public ICommand ScanDirectoryCommand => _scanDirectoryCommand ?? (_scanDirectoryCommand = new RelayCommand<object>(arg =>
        {
            ScanDirectory();
        }));

        private ICommand _getArtistFromDiscogsCommand;

        public ICommand GetArtistFromDiscogsCommand => _getArtistFromDiscogsCommand ?? (_getArtistFromDiscogsCommand = new RelayCommand<object>(async arg =>
        {
            await AddMusicDirectoryToCollection(SelectedItem.Value);
        }));

        private ICommand _moveToCollectionManuallyCommand;

        public ICommand MoveToCollectionManuallyCommand => _moveToCollectionManuallyCommand ?? (_moveToCollectionManuallyCommand = new RelayCommand<object>(MoveToCollectionManually));

        #endregion [  commands  ]

        #region [  private methods  ]

        private async Task AddMusicDirectoryToCollection(ISimpleFileInfo dirSfi)
        {
            if (dirSfi == null) throw new ArgumentNullException(nameof(dirSfi));

            if (dirSfi.Type != SfiType.Directory) return;// если передали муз файл, добавить поиск каталога, в котором находится музыкальный файл

            LoadDirectoryWithSubdirectories(dirSfi);
            //получение информации о выбранном каталоге с музыкой
            var mdi = _musicDirAnalyzer.AnalyzeMusicDirectory(dirSfi);
            //поиск информации на дискогс
            await SearchArtistAndAllAlbumsOnDiscogs(mdi);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mDirInfo"></param>
        /// <returns></returns>
        private async Task<Artist> SearchArtistAndAllAlbumsOnDiscogs(IMusicDirInfo mDirInfo)
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
            DownloadArtistImages(dArtist.images, imagePath);

            //Получаю информацию о выбранном альбоме
            await SearchFullAlbumOnDiscogs(allDArtistReleases, artistId, mDirInfo);

            return artist;
        }

        private void DownloadArtistImages(DiscogsImage[] dImages, string dirPath)
        {
            var number = 1;
            foreach (var discogsImage in dImages)
            {
                var photoName = $"photo_{number}";
                _discogsClient.SaveImage(discogsImage, dirPath, photoName);
            }
        }

        private async Task<Album> SearchFullAlbumOnDiscogs(IEnumerable<DiscogsArtistRelease> dArtReleases, int artistId, IMusicDirInfo mDirInfo)
        {
            //получение имени артиста. переделать
            var artistTrackInfo = mDirInfo.TrackInfos.FirstOrDefault();
            if (artistTrackInfo == null) return null;
            //поиск заданного альбома в списке всех альбомов исполнителя
            var selectedArtistRelease = dArtReleases.FirstOrDefault(arg =>
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
            var albumId = _repo.AddOrUpdateAlbum(artistId, albumToCollection);
            var storedArtist = _repo.FindArtistById(artistId);
            var storedAlbum = _repo.FindAlbumById(albumId);
            //TODO:Получаю путь для сохранения альбома. Переделать
            var albumStorageName = CreateAlbumDirectoryName(storedAlbum);
            var albumStoragePath = _fileManager.CreateAlbumStorageDirectory(storedArtist.StoragePath, albumStorageName);
            //Сохраняю физическую копию альбома в хранилище.
            _fileManager.MoveMusicDirectory(mDirInfo, albumStoragePath);
            //TODO: Сохраняю фотографии из дискогс

            //добавляю запись про место сохранения физической копии альбома
            _repo.AddAlbumToStorage(storedAlbum, albumStoragePath);

            return albumToCollection;
        }

        //TODO:Move to another class
        private string CreateAlbumDirectoryName(Album album)
        {
            return $"({album.ReleaseDate}) {album.Title}";
        }

        private void ExpandTreeViewItem(RoutedEventArgs arg)
        {
            if (!(arg.OriginalSource is TreeViewItem item)) return;
            if (item.DataContext is SimpleFileInfoWrap wrap)
            {
                LoadDirectory(wrap);
            }
        }

        private void LoadDirectoryWithSubdirectories(ISimpleFileInfo dirInfo)
        {
            if (dirInfo == null) throw new ArgumentNullException(nameof(dirInfo));
            if (dirInfo.Type != SfiType.Directory) return;
            dirInfo.Children.Clear();

            try
            {
                var path = dirInfo.Info.FullName;
                var subDirs = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);
                if (!subDirs.Any() & !files.Any())
                {
                    return;
                }
                foreach (var subDir in subDirs)
                {
                    var subDirFi = SimpleFileInfoFactory.Create(new FileInfo(subDir));
                    LoadDirectoryWithSubdirectories(subDirFi);
                    dirInfo.Children.Add(subDirFi);
                }
                foreach (var file in files)
                {
                    dirInfo.Children.Add(SimpleFileInfoFactory.Create(new FileInfo(file)));
                }
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Loads the list of files and subdirectories in given directory.
        /// </summary>
        /// <param name="wrap"></param>
        private void LoadDirectory(SimpleFileInfoWrap wrap)
        {
            if (wrap == null) throw new ArgumentNullException(nameof(wrap));
            if (wrap.Value.Type != SfiType.Directory || wrap.Value is DummySimpleFileInfo) return;

            wrap.Children.Clear();
            try
            {
                var path = wrap.Value.Info.FullName;
                var subDirs = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);
                if (!subDirs.Any() & !files.Any())
                {
                    wrap.Children.Add(new SimpleFileInfoWrap(new DummySimpleFileInfo()));
                    return;
                }
                foreach (var subDir in subDirs)
                {
                    var subDirFi = SimpleFileInfoFactory.Create(new FileInfo(subDir));
                    var subWrap = new SimpleFileInfoWrap(subDirFi);
                    subWrap.Children.Add(new SimpleFileInfoWrap(new DummySimpleFileInfo()));

                    wrap.Children.Add(subWrap);
                }
                foreach (var file in files)
                {
                    wrap.Children.Add(new SimpleFileInfoWrap(SimpleFileInfoFactory.Create(new FileInfo(file))));
                }
            }
            catch (Exception e)
            {
            }
        }

        //TODO: Rewrite of add to LoadDirectory method
        private void ScanDirectory()
        {
            MusicDirectories.Clear();
            var musicDirs = _fileManager.ScanDirectory(MusicSearchDirectory, "*.MP3");
            foreach (var di in musicDirs)
            {
                var fi = new FileInfo(di.FullName);
                var simpleFi = SimpleFileInfoFactory.Create(fi);
                var wrap = new SimpleFileInfoWrap(simpleFi);
                wrap.Children.Add(new SimpleFileInfoWrap(new DummySimpleFileInfo()));
                MusicDirectories.Add(wrap);
            }
        }

        private void MoveToCollectionManually()
        {
            var dirInfo = new DirectoryInfo(SelectedItem.Value.Info.FullName);
            var musicFiles = dirInfo.GetFiles("*.mp3");
            if (musicFiles.Length == 0) return;
            //get album information
            var albumInfo = _musicFileAnalyzer.GetBasicAlbumInfoFromDirectory(dirInfo);
            //create destination path
            var collectionDirectory = Properties.Settings.Default.MusicCollectionDirectory;
            var destPath = Path.Combine(collectionDirectory, albumInfo.ArtistName, albumInfo.AlbumTitle);

            _fileManager.MoveMusicDirectory(dirInfo.FullName, destPath);
            MusicDirectories.Remove(SelectedItem);
        }

        private void ChangeSelectedItem(SimpleFileInfoWrap arg)
        {
            SelectedItem = arg;
            FilePreview = _previewFactory.CreatePreviewVm(arg.Value);
        }

        private void Initialize()
        {
            MusicSearchDirectory = Properties.Settings.Default.MusicSearchDirectory;
            MusicCollectionDirectory = Properties.Settings.Default.MusicCollectionDirectory;
        }

        #endregion [  private methods  ]
    }
}