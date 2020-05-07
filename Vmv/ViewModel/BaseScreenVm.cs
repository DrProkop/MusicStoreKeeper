using Common;
using Serilog;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public abstract class BaseScreenVm : NotifyPropertyChangedBase, IScreenVm
    {
        protected BaseScreenVm(ILongOperationService longOperationService, IUserNotificationService userNotificationService, ILoggerManager manager)
        {
            LongOperationService = longOperationService;
            UserNotificationService = userNotificationService;
            log = manager.GetLogger(this);
        }

        #region [  fields  ]

        protected readonly ILogger log;

        #endregion [  fields  ]

        #region [  properties  ]

        public IUserNotificationService UserNotificationService { get; }
        public ILongOperationService LongOperationService { get; }

        #endregion [  properties  ]
    }
}