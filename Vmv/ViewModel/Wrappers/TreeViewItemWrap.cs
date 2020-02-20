using Common;
using MusicStoreKeeper.Model;
using System.Collections.ObjectModel;

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

    public sealed class ArtistWrap : TreeViewItemWrap<Artist>
    {
        public ArtistWrap(Artist value)
        {
            Value = value;
        }

        private Artist _value;

        public override Artist Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AlbumWrap> _children = new ObservableCollection<AlbumWrap>();

        public ObservableCollection<AlbumWrap> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public override string Icon => "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/artist_icon.png";
    }

    public sealed class AlbumWrap : TreeViewItemWrap<Album>
    {
        public AlbumWrap(Album value)
        {
            Value = value;
        }

        private Album _value;

        public override Album Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public override string Icon => "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/album_icon.png";
    }
}

public abstract class TreeViewItemWrap<T> : NotifyPropertyChangedBase
{
    public abstract T Value { get; set; }

    public abstract string Icon { get; }

    private bool _isChecked;

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            OnPropertyChanged();
        }
    }
}