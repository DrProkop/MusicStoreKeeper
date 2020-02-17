using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public abstract class FilePreviewVmBase : NotifyPropertyChangedBase, IPreviewVm
    {
        protected FilePreviewVmBase(ISimpleFileInfo fileInfo)
        {
            ItemName = fileInfo.Name;
        }

        #region [  properties  ]

        private string _fileName;

        public string ItemName
        {
            get => _fileName;
            set
            {
                if (_fileName == value) return;
                _fileName = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }
}