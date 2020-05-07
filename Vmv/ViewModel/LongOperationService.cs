using Common;
using Serilog;
using System;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class LongOperationService : NotifyPropertyChangedBase, ILongOperationService
    {
        public LongOperationService(IUserNotificationService userNotificationService, ILoggerManager manager)
        {
            _userNotificationService = userNotificationService;
            log = manager.GetLogger(this);
        }

        #region [  fields  ]

        private readonly ILogger log;
        private readonly IUserNotificationService _userNotificationService;

        #endregion [  fields  ]

        #region [  properties  ]

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private bool _isBlockingBusy;

        public bool IsBlockingBusy
        {
            get => _isBlockingBusy;
            private set
            {
                _isBlockingBusy = value;
                OnPropertyChanged();
            }
        }

        private string _blockingMessage;

        public string BlockingMessage
        {
            get => _blockingMessage;
            private set
            {
                _blockingMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]

        #region [  public methods  ]

        public void StartLongBlockingOperation(string message)
        {
            IsBlockingBusy = true;
            BlockingMessage = message;
        }

        public void FinishLongBlockingOperation()
        {
            IsBlockingBusy = false;
        }

        public void StartLongOperation(string message)
        {
            IsBusy = true;
            _userNotificationService.StatusBarMessage = message;
        }

        public void FinishLongOperation(string message)
        {
            IsBusy = false;
            _userNotificationService.StatusBarMessage = string.Empty;
        }

        public void HandleCancellation(Exception ex, string errorMessage = default)
        {
            log.Information(ex, errorMessage);
        }

        public void HandleException(Exception ex, string errorMessage = default)
        {
            log.Error(ex, errorMessage);
        }

        public void ShowMessage(string message)
        {
            _userNotificationService.ShowUserMessage(message);
        }

        #endregion [  public methods  ]
    }
}