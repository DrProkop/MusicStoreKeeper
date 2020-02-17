using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public abstract class ModelPreviewVmBase:NotifyPropertyChangedBase ,IPreviewVm
    {
        protected ModelPreviewVmBase()
        {
            
        }

        #region [  fields  ]

        #endregion

        #region [  properties  ]

        private string _itemName;
        public string ItemName
        {
            get => _itemName;
            set
            {
                if (_itemName == value) return;
                _itemName = value;
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