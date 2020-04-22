using Common;
using Serilog;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public abstract class BaseScreenVm:NotifyPropertyChangedBase, IScreenVm
    {
        

        protected BaseScreenVm(ILongOperationService longOperationService, ILoggerManager manager)
        {
            LongOperationService = longOperationService;
            log = manager.GetLogger(this);
        }

        #region [  fields  ]

        protected readonly ILogger log;

        #endregion

        #region [  properties  ]

        public ILongOperationService LongOperationService { get; private set; }

        private string _message;

        public string StatusBarMessage
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region [  commands  ]

        #endregion

        #region [  public methods  ]

        #endregion

        #region [  private methods  ]

        #endregion


    }
}