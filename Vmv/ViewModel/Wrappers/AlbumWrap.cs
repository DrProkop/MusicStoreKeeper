using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public sealed class AlbumWrap : TreeViewItemWrap<Album>
    {
        public AlbumWrap(Album value , ArtistWrap parent)
        {
            Value = value;
            Parent = parent;
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

        public ArtistWrap Parent { get;}

        public override string Icon => "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/album_icon.png";
    }
}