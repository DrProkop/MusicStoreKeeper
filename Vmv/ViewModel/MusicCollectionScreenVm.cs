using System.Collections.Generic;
using Common;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class MusicCollectionScreenVm : BaseScreenVm
    {
        public MusicCollectionScreenVm(IRepository repository, PreviewFactory previewFactory, ILoggerManager manager) : base(manager)
        {
            _repo = repository;
            _previewFactory = previewFactory;
            LoadArtistsInCollection();
            ShowAlbumsNotInCollection = false;
            //  SetupTestArtistData();
            
        }

        #region [  fields  ]

        private readonly IRepository _repo;
        private readonly PreviewFactory _previewFactory;

        #endregion [  fields  ]

        #region [  properties  ]

        private ObservableCollection<ArtistWrap> _artistsCollection;

        public ObservableCollection<ArtistWrap> ArtistsCollection
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

        private Album _selectedAlbum;

        public Album SelectedAlbum
        {
            get => _selectedAlbum;
            set { _selectedAlbum = value; OnPropertyChanged(); }
        }

        private IPreviewVm _collectionItemPreview;

        public IPreviewVm CollectionItemPreview
        {
            get => _collectionItemPreview;
            set
            {
                _collectionItemPreview = value;
                OnPropertyChanged();
            }
        }

        private  bool _showAlbumsNotInCollection;

        public bool ShowAlbumsNotInCollection
        {
            get => _showAlbumsNotInCollection;
            set
            {
                _showAlbumsNotInCollection = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]

        #region [  commands  ]

        private ICommand _artistExpandedCommand;

        public ICommand ArtistExpandedCommand => _artistExpandedCommand ?? (_artistExpandedCommand = new RelayCommand<RoutedEventArgs>(
                                                   arg => { ExpandTreeViewItem(arg); }));

        private ICommand _selectedItemChangedCommand;

        public ICommand SelectedItemChangedCommand => _selectedItemChangedCommand ?? (_selectedItemChangedCommand = new RelayCommand<object>(arg =>
                                                          {
                                                              ChangeSelectedItem(arg);
                                                          }));

        #endregion [  commands  ]

        #region [  private methods  ]

        private void ChangeSelectedItem(object arg)
        {

            switch (arg)
            {
                case AlbumWrap albumWrap when albumWrap.Value.InCollection:
                    albumWrap.Value = _repo.GetAlbumWithTracks(albumWrap.Value.Id);
                    CollectionItemPreview = _previewFactory.CreateAlbumPreviewVm(albumWrap.Value);
                    return;
                case AlbumWrap albumWrap:
                    CollectionItemPreview = _previewFactory.CreateAlbumPreviewVm(albumWrap.Value);
                    break;
                case ArtistWrap artistWrap:
                    CollectionItemPreview = _previewFactory.CreateArtistPreviewVm(artistWrap.Value);
                    break;
            }
        }

        private void ExpandTreeViewItem(RoutedEventArgs arg)
        {
            if (!(arg.OriginalSource is TreeViewItem item)) return;
            if (item.DataContext is ArtistWrap wrap)
            {
                LoadArtistAlbums(wrap);
            }
        }

        private void LoadArtistAlbums(ArtistWrap artWrap)
        {
            artWrap.Children.Clear();
            var albums= _repo.GetAllArtistAlbums(artWrap.Value.Id).ToList();
            foreach (var album in albums)
            {
                artWrap.Children.Add(new AlbumWrap(album));
            }
        }

        private void LoadArtistsInCollection()
        {
            var dbArtists = _repo.GetAllArtists().ToList();
            var dbArtistWrap = new List<ArtistWrap>();
            foreach (var dbArtist in dbArtists)
            {
                var artWrap=new ArtistWrap(dbArtist);
                artWrap.Children.Add(new AlbumWrap(new Album()));
                dbArtistWrap.Add(artWrap);
                
            }
            ArtistsCollection = new ObservableCollection<ArtistWrap>(dbArtistWrap);
        }

        #endregion [  private methods  ]

    }
}