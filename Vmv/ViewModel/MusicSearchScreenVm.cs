using Common;
using Discogs;
using MusicStoreKeeper.DataModel;
using System;
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
        public MusicSearchScreenVm(DiscogsClient client, ICollectionManager collectionManager, IRepository repository, IMusicFileAnalyzer musicFileAnalyzer, IMusicDirAnalyzer musicDirAnalyzer, IFileManager fileManager, PreviewFactory previewFactory, ILoggerManager manager) : base(manager)
        {
            _discogsClient = client;
            _collectionManager = collectionManager;
            _repo = repository;
            _musicFileAnalyzer = musicFileAnalyzer;
            _musicDirAnalyzer = musicDirAnalyzer;
            _fileManager = fileManager;
            _previewFactory = previewFactory;

            
        }

        #region [  fields  ]

        private readonly DiscogsClient _discogsClient;
        private readonly ICollectionManager _collectionManager;
        private readonly IRepository _repo;
        private readonly IMusicFileAnalyzer _musicFileAnalyzer;
        private readonly IMusicDirAnalyzer _musicDirAnalyzer;
        private readonly IFileManager _fileManager;
        private readonly PreviewFactory _previewFactory;

        #endregion [  fields  ]

        #region [  properties  ]

        private ObservableCollection<SimpleFileInfoWrap> _musicDirectories = new ObservableCollection<SimpleFileInfoWrap>();

        public ObservableCollection<SimpleFileInfoWrap> MusicDirectories
        {
            get => _musicDirectories;
            set { _musicDirectories = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SimpleFileInfoWrap> _directoriesForManualMovement = new ObservableCollection<SimpleFileInfoWrap>();

        public ObservableCollection<SimpleFileInfoWrap> DirectoriesForManualMovement
        {
            get => _directoriesForManualMovement;
            set { _directoriesForManualMovement = value; OnPropertyChanged(); }
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

        private bool _isSelectionEnabled;

        public bool IsSelectionEnabled
        {
            get => _isSelectionEnabled;
            set
            {
                _isSelectionEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _musicSearchDirectory;

        public string MusicSearchDirectory
        {
            get => _collectionManager.MusicSearchDirectory;
            set
            {
                _collectionManager.MusicSearchDirectory = value;
                OnPropertyChanged();
            }
        }

        private string _musicCollectionDirectory;

        public string MusicCollectionDirectory
        {
            get => _collectionManager.MusicCollectionDirectory;
            set
            {
                _collectionManager.MusicCollectionDirectory = value;
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

        public ICommand MoveToCollectionManuallyCommand => _moveToCollectionManuallyCommand ?? (_moveToCollectionManuallyCommand = new RelayCommand<object>(AddAlbumToCollectionManually));

        private ICommand _enableSelectionCommand;

        public ICommand EnableSelectionCommand => _enableSelectionCommand ?? (_enableSelectionCommand = new RelayCommand<object>(arg =>
        {
            EnableSelection();
        }));

        private void EnableSelection()
        {
            IsSelectionEnabled = !IsSelectionEnabled;
        }

        #endregion [  commands  ]

        #region [  private methods  ]

        private async Task AddMusicDirectoryToCollection(ISimpleFileInfo dirSfi)
        {
            if (dirSfi == null) throw new ArgumentNullException(nameof(dirSfi));

            if (dirSfi.Type != SfiType.Directory) return;//TODO: если передали муз файл, добавить поиск каталога, в котором находится музыкальный файл

            LoadDirectoryWithSubdirectories(dirSfi);
            //получение информации о выбранном каталоге с музыкой
            var mdi = _musicDirAnalyzer.AnalyzeMusicDirectory(dirSfi);
            //поиск информации на дискогс
            var artist=await _collectionManager.SearchArtistAndAllAlbumsOnDiscogs(mdi, true);
            await _collectionManager.SearchFullAlbumOnDiscogs(artist, mdi, true);
        }

        private void AddAlbumToCollectionManually()
        {
            _collectionManager.MoveToCollectionManually(SelectedItem.Value);
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



        private void ChangeSelectedItem(SimpleFileInfoWrap arg)
        {
            if (arg == null) return;
            SelectedItem = arg;
            FilePreview = _previewFactory.CreatePreviewVm(arg.Value);
        }

        #endregion [  private methods  ]
    }
}