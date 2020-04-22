using Common;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class SettingsScreenVm : BaseScreenVm
    {
        public SettingsScreenVm(
            ICollectionManager collectionManager,
            ILongOperationService longOperationService,
            ILoggerManager manager) : base(longOperationService, manager)
        {
            _collectionManager = collectionManager;
        }

        #region [  fields  ]

        private readonly ICollectionManager _collectionManager;

        #endregion

        #region [  properties  ]

        private string _musicCollectionDirectoryPath;

        public string MusicCollectionDirectoryPath
        {
            get => _musicCollectionDirectoryPath ?? _collectionManager.MusicCollectionDirectory;
            set
            {
                if (MusicCollectionDirectoryPath.Equals(value)) return;
                _collectionManager.MusicCollectionDirectory = value;
                _musicCollectionDirectoryPath = value;
                OnPropertyChanged();
            }
        }

        private string _musicSearchDirectoryPath;

        public string MusicSearchDirectoryPath
        {
            get => _musicSearchDirectoryPath ?? _collectionManager.MusicSearchDirectory;
            set
            {
                if (MusicSearchDirectoryPath.Equals(value)) return;
                _collectionManager.MusicSearchDirectory = value;
                _musicSearchDirectoryPath = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]

        #region [  commands  ]

        private ICommand _selectCollectionDirectoryCommand;

        public ICommand SelectCollectionDirectoryCommand => _selectCollectionDirectoryCommand ?? (_selectCollectionDirectoryCommand = new RelayCommand<RoutedEventArgs>(
                                                             arg => { SelectCollectionDirectory(); }));

        private ICommand _selectMusicSearchDirectoryCommand;

        public ICommand SelectMusicSearchDirectoryCommand => _selectMusicSearchDirectoryCommand ?? (_selectMusicSearchDirectoryCommand = new RelayCommand<RoutedEventArgs>(
                                                                arg => { SelectMusicSearchDirectory(); }));

        #endregion [  commands  ]


        #region [  private methods  ]

        private void SelectCollectionDirectory()
        {
            using (var dialog = InitializeFolderSelectionDialogue("Select a folder for your music collection"))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    MusicCollectionDirectoryPath = dialog.FileName;
                }
            }
        }

        private void SelectMusicSearchDirectory()
        {
            using (var dialog = InitializeFolderSelectionDialogue("Select a folder to search for music"))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    MusicSearchDirectoryPath = dialog.FileName;
                }
            }
        }

        private CommonOpenFileDialog InitializeFolderSelectionDialogue(string title)
        {
            var selectFolderDialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\",
                IsFolderPicker = true,
                Title = title
            };
            return selectFolderDialog;
        }

        #endregion [  private methods  ]
    }
}