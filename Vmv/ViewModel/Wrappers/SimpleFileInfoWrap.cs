using System.Collections.ObjectModel;
using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public sealed class SimpleFileInfoWrap : TreeViewItemWrap<ISimpleFileInfo>
    {
        public SimpleFileInfoWrap(ISimpleFileInfo value)
        {
            Value = value;
        }

        private ISimpleFileInfo _value;

        public override ISimpleFileInfo Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public override bool IsChecked
        {
            get => base.IsChecked;
            set
            {
                foreach (var child in Children)
                {
                    child.IsChecked = value;
                }

                base.IsChecked = value;
            }
        }

        private ObservableCollection<SimpleFileInfoWrap> _children = new ObservableCollection<SimpleFileInfoWrap>();

        public ObservableCollection<SimpleFileInfoWrap> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public override string Icon
        {
            get
            {
                switch (Value.Type)
                {
                    case SfiType.Directory:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/close_folder_icon.png";
                    case SfiType.AudioFile:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/audio_file_icon.png";
                    case SfiType.ImageFile:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/image_file_icon.png";
                    case SfiType.TextFile:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/document_icon.png";
                    case SfiType.Unknown:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/document_blank_icon.png";
                    default:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/document_blank_icon.png";
                }
            }
        }
    }
}