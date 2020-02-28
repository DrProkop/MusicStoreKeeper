using System.Collections.ObjectModel;
using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.Vmv.ViewModel
{
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

        public override bool IsChecked
        {
            get => base.IsChecked;
            set
            {
                foreach (var albumWrap in Children)
                {
                    albumWrap.IsChecked = value;
                }

                base.IsChecked = value;
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
}