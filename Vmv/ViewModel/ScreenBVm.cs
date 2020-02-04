using Common;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Vmv.ViewModel
{
    //TODO: Create base class for vms, add logging to it.
    public class ScreenBVm : NotifyPropertyChangedBase, IScreenVm
    {
        public ScreenBVm(IFileAnalyzer fileAnalyzer)
        {
            _fileAnalyzer = fileAnalyzer;
        }

        #region [  fields  ]

        private readonly IFileAnalyzer _fileAnalyzer;

        #endregion

        #region [  properties  ]

        private ObservableCollection<ISimpleFileInfo> _musicDirectories = new ObservableCollection<ISimpleFileInfo>();

        public ObservableCollection<ISimpleFileInfo> MusicDirectories
        {
            get => _musicDirectories;
            set { _musicDirectories = value; OnPropertyChanged(); }
        }

        private ISimpleFileInfo _selectedMusicDirectory;

        public ISimpleFileInfo SelectedMusicDirectory
        {
            get => _selectedMusicDirectory;
            set { _selectedMusicDirectory = value; OnPropertyChanged(); }
        }

        private IPreviewVm _filePreview;

        public IPreviewVm FilePreview
        {
            get { return _filePreview; }
            set { _filePreview = value;OnPropertyChanged(); }
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
            TempInit();
        }));

        #endregion [  commands  ]

        #region [  private methods  ]

        private IPreviewVm CreatePreview(ISimpleFileInfo file)
        {
            //TODO: Add enum PreviewableFiles to ISimpleFileInfo
            if (file.IsAudioFile)
            {
                var basicTrackInfo = _fileAnalyzer.GetBasicAlbumInfoFromAudioFile(file.Info);
                var audioPreview = new AudioFilePreviewVm(file,basicTrackInfo);
                return audioPreview;
            }
            if (file.IsImage)
            {
                var imageSource=new ImageSourceConverter().ConvertFromString(file.Info.FullName) as ImageSource;
              //  var imageSource = new ImageSourceConverter().
                var imagePreview=new ImagePreviewVm(file, imageSource);
                return imagePreview;
            }

            if (file.IsTextDocument)
            {
                return new TextFilePreviewVm(file);
            }
            return null;
        }

        private void ExpandTreeViewItem(RoutedEventArgs arg)
        {
            if (!(arg.OriginalSource is TreeViewItem item)) return;
            var sfi = item.DataContext as ISimpleFileInfo;
            LoadDirectory(sfi);
        }

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
            SelectedMusicDirectory = arg;
            FilePreview = CreatePreview(arg);
        }

        #endregion [  private methods  ]
    }
}