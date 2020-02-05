using Common;
using Serilog;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public abstract class BaseScreenVm:NotifyPropertyChangedBase, IScreenVm
    {
        protected BaseScreenVm(ILoggerManager manager)
        {
            log = manager.GetLogger(this);
        }

        #region [  fields  ]

        protected readonly ILogger log;

        #endregion

        #region [  properties  ]

        #endregion

        #region [  commands  ]

        #endregion

        #region [  public methods  ]

        #endregion

        #region [  private methods  ]

        #endregion

    }
}