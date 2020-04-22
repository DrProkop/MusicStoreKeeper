using System.Windows.Input;
using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class MainWindowVm : BaseScreenVm
    {
        

        public MainWindowVm(MusicCollectionScreenVm musicCollectionScreen,
            MusicSearchScreenVm musicSearchScreen,
            SettingsScreenVm settingsScreen,
            ILongOperationService longOperationService,
            ILoggerManager manager):base(longOperationService, manager)
        {
            MusicCollectionScreen = musicCollectionScreen;
            MusicSearchScreen = musicSearchScreen;
            SettingsScreen = settingsScreen;
            log.Information("Application started");
        }

        #region [  properties  ]

        private IScreenVm _musicCollectionScreen;

        public IScreenVm MusicCollectionScreen
        {
            get => _musicCollectionScreen;
            set { _musicCollectionScreen = value; OnPropertyChanged(); }
        }

        private IScreenVm _musicSearchScreen;

        public IScreenVm MusicSearchScreen
        {
            get => _musicSearchScreen;
            set { _musicSearchScreen = value; OnPropertyChanged(); }
        }

        private IScreenVm _settingsScreen;

        public IScreenVm SettingsScreen
        {
            get => _settingsScreen;
            set
            {
                _settingsScreen = value;
                OnPropertyChanged();
            }
        }

        private IScreenVm _currentScreen;

        public IScreenVm CurrentScreen
        {
            get => _currentScreen;
            set { _currentScreen = value; OnPropertyChanged(); }
        }

        #region [  tab item selected  ]

        private bool _musicCollectionScreenSelected;

        public bool MusicCollectionScreenSelected
        {
            get => _musicCollectionScreenSelected;
            set
            {
                if (_musicCollectionScreenSelected == value) return;
                _musicCollectionScreenSelected = value;
                if (value)
                {
                    SetCurrentScreen(MusicCollectionScreen);
                }
            }
        }

        private bool _searchMusicScreenSelected;

        public bool SearchMusicScreenSelected
        {
            get => _searchMusicScreenSelected;
            set
            {
                if (_searchMusicScreenSelected == value) return;
                _searchMusicScreenSelected = value;
                if (value)
                {
                    SetCurrentScreen(MusicSearchScreen);
                }
            }
        }

        private bool _settingsScreenSelected;

        public bool SettingsScreenSelected
        {
            get => _settingsScreenSelected;
            set
            {
                if (_settingsScreenSelected == value) return;
                _settingsScreenSelected = value;
                if (value)
                {
                    SetCurrentScreen(SettingsScreen);
                }
                
            }
        }

        #endregion [  tab item selected  ]

        #endregion [  properties  ]

        #region [  commands  ]

        private ICommand _changeScreenCommand;

        public ICommand ChangeScreenCommand => _changeScreenCommand ??
                                               (_changeScreenCommand = new RelayCommand<IScreenVm>(screen => CurrentScreen = screen));

        #endregion [  commands  ]

        #region [  private methods  ]

        private void SetCurrentScreen(IScreenVm screen)
        {
            CurrentScreen = screen;
        }

        #endregion [  private methods  ]
    }
}