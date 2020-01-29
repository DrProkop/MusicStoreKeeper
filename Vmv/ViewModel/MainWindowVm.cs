using System.Configuration;
using System.Windows.Input;
using Common;
using Serilog;

namespace Vmv.ViewModel
{
    public class MainWindowVm : NotifyPropertyChangedBase
    {
        private ILogger log;
        public MainWindowVm(ILoggerManager manager)
        {
            log = manager.GetLogger(this);
            ScreenA=new ScreenAVm();
            ScreenB=new ScreenBVm();
            log.Information("Application started");
        }

        private IScreenVm _screenA;

        public IScreenVm ScreenA
        {
            get => _screenA;
            set { _screenA = value;OnPropertyChanged(); }
        }

        private IScreenVm _screenB;

        public IScreenVm ScreenB
        {
            get => _screenB;
            set { _screenB = value; OnPropertyChanged(); }
        }

        private object _currentScreen;

        public object CurrentScreen
        {
            get => _currentScreen;
            set{_currentScreen = value;OnPropertyChanged();}
        }

        private ICommand _changeScreenCommand;

        public ICommand ChangeScreenCommand => _changeScreenCommand ??
                                               (_changeScreenCommand = new RelayCommand<IScreenVm>(screen => CurrentScreen = screen));
    }
}