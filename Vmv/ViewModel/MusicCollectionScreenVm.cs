using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Common;
using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class MusicCollectionScreenVm: BaseScreenVm
    {
        public MusicCollectionScreenVm(ILoggerManager manager):base(manager)
        {
            ArtistsCollection = new ObservableCollection<Artist>();
            SetupTestArtistData();

        }

        #region [  properties  ]

        private ObservableCollection<Artist> _artistsCollection;

        public ObservableCollection<Artist> ArtistsCollection
        {
            get => _artistsCollection;
            set { _artistsCollection = value; OnPropertyChanged(); }
        }

        private Artist _selectedArtist;

        public Artist SelectedArtist
        {
            get => _selectedArtist;
            set { _selectedArtist = value; OnPropertyChanged(); }
        }

        private  Album _selectedAlbum;

        public Album SelectedAlbum
        {
            get => _selectedAlbum;
            set { _selectedAlbum = value; OnPropertyChanged(); }
        }

        #endregion

        #region [  commands  ]

        private ICommand _artistExpandedCommand;

        public ICommand ArtistExpandedCommand => _artistExpandedCommand ?? (_artistExpandedCommand = new RelayCommand<RoutedEventArgs>(
                                                   arg =>
                                                   {

                                                   }));

        #endregion

        private void SetupTestArtistData()
        {
            var artistA=new Artist();
            var artistB=new Artist();
            var artistC=new Artist();

            var albumA=new Album();
            var albumB = new Album();
            var albumC = new Album();

            var trackA=new Track();
            var trackB=new Track();
            var trackC=new Track();

            trackA.Name = "track A";
            trackB.Name = "track B";
            trackC.Name = "track C";

            albumA.Title = "Album A";
            albumB.Title = "Album B";
            albumC.Title = "Album C";

            artistA.Name = "Artist A";
            artistB.Name = "Artist B";
            artistC.Name = "Artist C";

            albumA.Tracks.Add(trackA);
            albumB.Tracks.Add(trackB);
            albumC.Tracks.Add(trackC);

            artistA.Albums.Add(albumA);
            artistB.Albums.Add(albumB);
            artistC.Albums.Add(albumC);

            ArtistsCollection.Add(artistA);
            ArtistsCollection.Add(artistB);
            ArtistsCollection.Add(artistC);
        }
    }
    
}
