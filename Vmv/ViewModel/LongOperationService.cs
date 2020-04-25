using Common;
using Serilog;
using System;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class LongOperationService : NotifyPropertyChangedBase, ILongOperationService
    {
        public LongOperationService(ILoggerManager manager)
        {
            log = manager.GetLogger(this);
        }

        #region [  fields  ]

        private readonly ILogger log;

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

        private string _generalMessage;

        public string GeneralMessage
        {
            get => _generalMessage;
            private set
            {
                _generalMessage = value;
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
            GeneralMessage = message;
        }

        public void FinishLongOperation(string message)
        {
            IsBusy = false;
        }

        public void HandleException(Exception ex, string errorMessage = default)
        {
            log.Error(ex, errorMessage);
        }

        public void ShowMessage(string message)
        {
            throw new System.NotImplementedException();
        }

        #endregion [  public methods  ]
    }
}