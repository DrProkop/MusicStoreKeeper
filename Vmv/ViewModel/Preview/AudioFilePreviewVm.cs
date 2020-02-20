using Common;

namespace MusicStoreKeeper.Vmv.ViewModel

{
    public class AudioFilePreviewVm : FilePreviewVmBase
    {
        public AudioFilePreviewVm(ISimpleFileInfo fileInfo, IBasicTrackInfo trackInfo) : base(fileInfo)
        {
            ArtistName = trackInfo.ArtistName;
            AlbumTitle = trackInfo.AlbumTitle;
            Year = trackInfo.Year;
            TrackName = trackInfo.TrackName;
        }

        #region [  properties  ]

        private string _artistName;

        public string ArtistName
        {
            get => _artistName;
            set
            {
                if (_artistName == value) return;
                _artistName = value;
                OnPropertyChanged();
            }
        }

        private string _albumTitle;

        public string AlbumTitle
        {
            get => _albumTitle;
            set
            {
                if (_albumTitle == value) return;
                _albumTitle = value;
                OnPropertyChanged();
            }
        }

        private string _trackName;

        public string TrackName
        {
            get => _trackName;
            set
            {
                if (_trackName == value) return;
                _trackName = value;
                OnPropertyChanged();
            }
        }

        private uint _year;

        public uint Year
        {
            get => _year;
            set
            {
                if (_year == value) return;
                _year = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }
}