using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class ArtistPreviewVm : ModelEntityPreviewVmBase
    {
        public ArtistPreviewVm(Artist artist)
        {
            _artist = artist;
            ItemName = artist.Name;
            Profile = artist.Profile;
        }

        #region [  fields  ]

        private readonly Artist _artist;

        #endregion [  fields  ]

        #region [  properties  ]

        private string _profile;

        public string Profile
        {
            get => _profile;
            set
            {
                _profile = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }
}