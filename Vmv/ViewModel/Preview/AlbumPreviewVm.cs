using MusicStoreKeeper.Model;
using System.Collections.ObjectModel;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class AlbumPreviewVm : ModelEntityPreviewVmBase
    {
        public AlbumPreviewVm(Album album)
        {
            Album = album;
            ItemName = album.Title;
            AlbumTracks = new ObservableCollection<Track>(album.Tracks);
        }

        #region [  fields  ]



        #endregion [  fields  ]

        #region [  properties  ]

        public Album Album { get; }

        private ObservableCollection<Track> _albumTracks;

        public ObservableCollection<Track> AlbumTracks
        {
            get => _albumTracks;
            set
            {
                _albumTracks = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }
}