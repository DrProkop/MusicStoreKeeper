using System.Windows;
using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class UserNotificationService:NotifyPropertyChangedBase, IUserNotificationService
    {
        public UserNotificationService()
        {
            
        }

        #region [  fields  ]

        #endregion

        #region [  properties  ]

        private string _statusBarMessage;

        public string StatusBarMessage
        {
            get => _statusBarMessage;
            set
            {
                _statusBarMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region [  public methods  ]
        public void ShowUserMessage(string message)
        {
            MessageBox.Show(message, "ololo", MessageBoxButton.OK);
        }
        #endregion

        #region [  private methods  ]

        #endregion

        
        
    }
}