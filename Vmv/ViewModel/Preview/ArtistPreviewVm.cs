using System.Windows.Media;
using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class ArtistPreviewVm:ModelPreviewVmBase
    {
        public ArtistPreviewVm(Artist artist)
        {
            _artist = artist;
            ItemName = artist.Name;
            Profile = artist.Profile;
        }

        #region [  fields  ]

        private readonly Artist _artist;

        #endregion

        #region [  properties  ]

        private  string _profile;

        public string Profile
        {
            get => _profile;
            set
            {
                _profile = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _artistImage;

        public ImageSource ArtistImage
        {
            get => _artistImage;
            set
            {
                _artistImage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region [  commands  ]

        #endregion

        #region [  public methods  ]

        #endregion

        #region [  private methods  ]

        #endregion

    }
}