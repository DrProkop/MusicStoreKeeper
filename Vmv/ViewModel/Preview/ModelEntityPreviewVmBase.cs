using Common;
using System.Collections.Generic;
using System.Windows.Media;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public abstract class ModelEntityPreviewVmBase : NotifyPropertyChangedBase, IPreviewVm
    {
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

        private ImageSource _selectedImage;

        public ImageSource SelectedImage
        {
            get => _selectedImage;
            set
            {
                _selectedImage = value;
                OnPropertyChanged();
            }
        }

        private List<ImageSource> _imageCollection = new List<ImageSource>();

        public List<ImageSource> ImageCollection
        {
            get => _imageCollection;
            set
            {
                _imageCollection = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }
}