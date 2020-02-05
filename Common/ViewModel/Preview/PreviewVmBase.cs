namespace Common
{
    public abstract class PreviewVmBase : NotifyPropertyChangedBase, IPreviewVm
    {
        protected PreviewVmBase(ISimpleFileInfo fileInfo)
        {
            FileName = fileInfo.Name;
        }

        #region [  properties  ]

        private string _fileName;

        public string FileName
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