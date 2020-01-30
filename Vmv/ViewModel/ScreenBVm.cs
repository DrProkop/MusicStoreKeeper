using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using Common;

namespace Vmv.ViewModel
{
    //TODO: Create base class for vms, add logging to it.
    public class ScreenBVm:NotifyPropertyChangedBase, IScreenVm
    {
        public ScreenBVm()
        {
           


        }


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

        #endregion

        #region [  commands  ]

        private ICommand _selectedItemChangedCommand;

        public ICommand SelectedItemChangedCommand => _selectedItemChangedCommand?? (_selectedItemChangedCommand = new RelayCommand<ISimpleFileInfo>( arg =>
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

        #endregion

        #region [  public methods  ]

        

        #endregion

        #region [  private methods  ]

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
                    var subDirFi=new SimpleFileInfo(new FileInfo(subDir));
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
        }

        #endregion




    }

}
