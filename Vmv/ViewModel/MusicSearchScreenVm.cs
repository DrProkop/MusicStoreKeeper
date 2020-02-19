using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Common;
using Discogs;
using MusicStoreKeeper.Model;
using MusicStoreKeeper.DataModel;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    //TODO: Create base class for vms, add logging to it.
    public class MusicSearchScreenVm : BaseScreenVm
    {
        public MusicSearchScreenVm(DiscogsClient client,IRepository repository, IFileAnalyzer fileAnalyzer, IFileManager fileManager, PreviewFactory previewFactory, ILoggerManager manager) : base(manager)
        {
            _discogsClient = client;
            _repo = repository;
            _fileAnalyzer = fileAnalyzer;
            _fileManager = fileManager;
            _previewFactory = previewFactory;
            _discogsConverter = new DiscogsConverter();
            Initialize();
        }

        #region [  fields  ]

        private readonly DiscogsClient _discogsClient;
        private readonly IRepository _repo;
        private readonly IFileAnalyzer _fileAnalyzer;
        private readonly IFileManager _fileManager;
        private readonly PreviewFactory _previewFactory;
        private readonly DiscogsConverter _discogsConverter;
       #endregion [  fields  ]

        #region [  properties  ]

        private ObservableCollection<ISimpleFileInfo> _musicDirectories = new ObservableCollection<ISimpleFileInfo>();

        public ObservableCollection<ISimpleFileInfo> MusicDirectories
        {
            get => _musicDirectories;
            set { _musicDirectories = value; OnPropertyChanged(); }
        }

        private ISimpleFileInfo _selectedItem;

        public ISimpleFileInfo SelectedItem
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

        #endregion [  properties  ]

        #region [  commands  ]

        private ICommand _selectedItemChangedCommand;

        public ICommand SelectedItemChangedCommand => _selectedItemChangedCommand ?? (_selectedItemChangedCommand = new RelayCommand<ISimpleFileInfo>(arg =>
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
            var artist = await SearchArtistAndAlbumOnDiscogs();
        }));

        private ICommand _moveToCollectionManuallyCommand;

        public ICommand MoveToCollectionManuallyCommand => _moveToCollectionManuallyCommand ?? (_moveToCollectionManuallyCommand = new RelayCommand<object>(MoveToCollectionManually));

        #endregion [  commands  ]

        #region [  private methods  ]

        private async Task<Artist> SearchArtistAndAlbumOnDiscogs()
        {
            if (!SelectedItem.IsAudioFile) return null;

            var basicTrackInfo = _fileAnalyzer.GetBasicAlbumInfoFromAudioFile(SelectedItem.Info);
            var dArtist = await _discogsClient.GetArtistByName(basicTrackInfo.ArtistName);
            var allDArtistReleases = await _discogsClient.GetArtistReleases(dArtist.id);
            
            var selectedArtistRelease = allDArtistReleases.FirstOrDefault(arg =>
                 arg.title.Equals(basicTrackInfo.AlbumTitle, StringComparison.InvariantCultureIgnoreCase));
            if (selectedArtistRelease == null)
            {
                //TODO: Try searching by album and track names
                return null;
            }

            var releaseId=0;
            if (selectedArtistRelease.type == "master")
            {
                //Always searches for main release
                var dMasterRelease = await _discogsClient.GetMaterReleaseById(selectedArtistRelease.id);
                releaseId = dMasterRelease.main_release;
            }
            else
            {
                releaseId = selectedArtistRelease.id;
            }
            //создаю исполнителя
            var artist = _discogsConverter.CreateArtist(dArtist);
            //создаю коллекцию всех альбомов исполнителя
            var allArtistAlbums = _discogsConverter.CreateArtistAlbums(allDArtistReleases);
            artist.Albums = allArtistAlbums;
            //сохраняю исполнителя в базе
            var artistId=_repo.AddOrUpdateArtistFull(artist); 
            //создаю альбом с полной информацией
            var dRelease = await _discogsClient.GetReleaseById(releaseId);
            var albumToCollection = _discogsConverter.CreateAlbum(dRelease);
            //сохраняю альбом в базе
            var albumId = _repo.AddOrUpdateAlbum(artistId, albumToCollection);
            //добавляю запись про место сохранения физической копии альбома
            _repo.AddAlbumToStorage(albumId, "ololo azaza");
            
            return artist;
        }

        private void ExpandTreeViewItem(RoutedEventArgs arg)
        {
            if (!(arg.OriginalSource is TreeViewItem item)) return;
            if (item.DataContext is ISimpleFileInfo sfi)
            {
                LoadDirectory(sfi);
            }
        }

        /// <summary>
        /// Loads the list of files and subdirectories in given directory.
        /// </summary>
        /// <param name="dir"></param>
        private void LoadDirectory(ISimpleFileInfo dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            if (!dir.IsDirectory || dir is DummySimpleFileInfo) return;

            dir.Children.Clear();
            try
            {
                var path = dir.Info.FullName;
                var subDirs = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);
                if (!subDirs.Any() & !files.Any())
                {
                    dir.Children.Add(new DummySimpleFileInfo());
                    return;
                }
                foreach (var subDir in subDirs)
                {
                    var subDirFi = new SimpleFileInfo(new FileInfo(subDir));
                    subDirFi.Children.Add(new DummySimpleFileInfo());
                    dir.Children.Add(subDirFi);
                }
                foreach (var file in files)
                {
                    dir.Children.Add(new SimpleFileInfo(new FileInfo(file)));
                }
            }
            catch (Exception e)
            {
            }
        }

        private void ScanDirectory()
        {
            MusicDirectories.Clear();
            var musicDirs = _fileManager.ScanDirectory(MusicSearchDirectory, "*.MP3");
            foreach (var di in musicDirs)
            {
                var fi = new FileInfo(di.FullName);
                var simpleFi = new SimpleFileInfo(fi);
                simpleFi.Children.Add(new DummySimpleFileInfo());
                MusicDirectories.Add(simpleFi);
            }
        }

        private void MoveToCollectionManually()
        {
            var dirInfo = new DirectoryInfo(SelectedItem.Info.FullName);
            var musicFiles = dirInfo.GetFiles("*.mp3");
            if (musicFiles.Length == 0) return;
            //get album information
            var albumInfo = _fileAnalyzer.GetBasicAlbumInfoFromDirectory(dirInfo);
            //create destination path
            var collectionDirectory = Properties.Settings.Default.MusicCollectionDirectory;
            var destPath = Path.Combine(collectionDirectory, albumInfo.ArtistName, albumInfo.AlbumTitle);

            _fileManager.MoveMusicDirectory(dirInfo.FullName, destPath);
            MusicDirectories.Remove(SelectedItem);
        }

        private void TempInit()
        {
            var testPath = ConfigurationManager.AppSettings.Get("MusicStorage");
            var dInfos = new DirectoryInfo(testPath).GetDirectories().ToList();
            foreach (var di in dInfos)
            {
                var fi = new FileInfo(di.FullName);
                var simpleFi = new SimpleFileInfo(fi);
                simpleFi.Children.Add(new DummySimpleFileInfo());
                MusicDirectories.Add(simpleFi);
            }
        }

        private void ChangeSelectedItem(ISimpleFileInfo arg)
        {
            SelectedItem = arg;
            FilePreview = _previewFactory.CreatePreviewVm(arg);
        }

        private void Initialize()
        {
            MusicSearchDirectory = Properties.Settings.Default.MusicSearchDirectory;
        }

        #endregion [  private methods  ]
    }
}