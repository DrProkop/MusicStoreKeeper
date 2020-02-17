using System.Windows.Media;
using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class ImageFilePreviewVm : FilePreviewVmBase
    {
        public ImageFilePreviewVm(ISimpleFileInfo fileInfo, ImageSource imageSource) : base(fileInfo)
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