using System.Windows.Input;

namespace Vmv.ViewModel
{
    public class MainWindowVm : NotifyPropertyChangedBase
    {
        public MainWindowVm()
        {
            ScreenA=new ScreenAVm();
            ScreenB=new ScreenBVm();
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