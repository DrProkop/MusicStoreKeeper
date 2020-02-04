using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Model;

namespace Vmv.ViewModel
{
    public class ScreenAVm: NotifyPropertyChangedBase, IScreenVm
    {
        public ScreenAVm()
        {
            ArtistsList = new ObservableCollection<Artist>();
            SetupTestArtistData();

        }

        #region [  properties  ]

        private ObservableCollection<Artist> _artistsList;

        public ObservableCollection<Artist> ArtistsList
        {
            get => _artistsList;
            set { _artistsList = value; OnPropertyChanged(); }
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

            ArtistsList.Add(artistA);
            ArtistsList.Add(artistB);
            ArtistsList.Add(artistC);
        }
    }
    
}
