using System.Windows.Media;

namespace Common
{
    public class ImagePreviewVm : PreviewVmBase
    {
        public ImagePreviewVm(ISimpleFileInfo fileInfo, ImageSource imageSource) : base(fileInfo)
        {
            Image = imageSource;
        }

        #region [  properties  ]

        private ImageSource _image;

        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }
}