using Common;
using MusicStoreKeeper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class MusicCollectionScreenVm : BaseScreenVm
    {
        public MusicCollectionScreenVm(ICollectionManager collectionManager, PreviewFactory previewFactory, ILoggerManager manager) : base(manager)
        {
            _collectionManager = collectionManager;
            _previewFactory = previewFactory;

            _allArtists = _collectionManager.GetAllArtists().ToList();
            LoadAllArtistsInCollection();
            ShowAlbumsNotInCollection = false;
            IsSelectionEnabled = false;
            MusicStyles = _collectionManager.GetMusicStylesList();
            MusicGenres = _collectionManager.GetMusicGenresList();
        }

        #region [  fields  ]

        private readonly ICollectionManager _collectionManager;
        private readonly PreviewFactory _previewFactory;
        private readonly List<Artist> _allArtists;
        private readonly List<string> _selectedStyles = new List<string>();
        private readonly List<string> _selectedGenres = new List<string>();

        #endregion [  fields  ]

        #region [  properties  ]

        private ObservableCollection<ArtistWrap> _fullArtistsCollection;

        public ObservableCollection<ArtistWrap> FullArtistsCollection
        {
            get => _fullArtistsCollection;
            set { _fullArtistsCollection = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ArtistWrap> _artistsCollectionToShow;

        public ObservableCollection<ArtistWrap> ArtistsCollectionToShow
        {
            get => _artistsCollectionToShow;
            set { _artistsCollectionToShow = value; OnPropertyChanged(); }
        }

        private Artist _selectedArtist;

        public Artist SelectedArtist
        {
            get => _selectedArtist;
            set
            {
                _selectedArtist = value;
                OnPropertyChanged();
            }
        }

        private Album _selectedAlbum;

        public Album SelectedAlbum
        {
            get => _selectedAlbum;
            set
            {
                _selectedAlbum = value;
                OnPropertyChanged();
            }
        }

        public object SelectedItemWrap { get; set; }

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

        private bool _isSelectionEnabled;

        public bool IsSelectionEnabled
        {
            get => _isSelectionEnabled;
            set
            {
                _isSelectionEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _showAlbumsNotInCollection;

        public bool ShowAlbumsNotInCollection
        {
            get => _showAlbumsNotInCollection;
            set
            {
                _showAlbumsNotInCollection = value;
                OnPropertyChanged();
            }
        }

        public List<string> MusicStyles { get; set; }

        public List<string> MusicGenres { get; set; }

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

        private ICommand _refreshCollectionCommand;

        public ICommand RefreshCollectionCommand => _refreshCollectionCommand ?? (_refreshCollectionCommand = new RelayCommand<object>(arg =>
                                                        {
                                                            RefreshCollection();
                                                        }));

        private ICommand _enableSelectionCommand;

        public ICommand EnableSelectionCommand => _enableSelectionCommand ?? (_enableSelectionCommand = new RelayCommand<object>(arg =>
                                                      {
                                                          EnableSelection();
                                                      }));

        private ICommand _showMissingAlbumsCommand;

        public ICommand ShowMissingAlbumsCommand => _showMissingAlbumsCommand ?? (_showMissingAlbumsCommand = new RelayCommand<object>(arg =>
       {
           ShowMissingAlbums();
       }));

        private ICommand _deleteFromCollectionCommand;

        public ICommand DeleteFromCollectionCommand =>
            _deleteFromCollectionCommand ?? (_deleteFromCollectionCommand = new RelayCommand<object>(
                (arg) => { DeleteFromCollection(); }, (arg) => SelectedItemWrap != null));

        private ICommand _selectStylesCommand;

        public ICommand SelectStylesCommand => _selectStylesCommand ?? (_selectStylesCommand = new RelayCommand<string>(
                                                  (arg) => UpdateSearchParametersList(_selectedStyles, arg)));

        private ICommand _selectGenresCommand;

        public ICommand SelectGenresCommand => _selectGenresCommand ?? (_selectGenresCommand = new RelayCommand<string>(
                                                   (arg) => UpdateSearchParametersList(_selectedGenres, arg)));

        private ICommand _sortBySelectedStylesAndGenresCommand;

        public ICommand SortBySelectedStylesAndGenresCommand => _sortBySelectedStylesAndGenresCommand ?? (_sortBySelectedStylesAndGenresCommand = new RelayCommand<object>(
                                                   (arg) => SortBySelectedStylesAndGenres()));

        #endregion [  commands  ]

        #region [  private methods  ]

        private void ChangeSelectedItem(object arg)
        {
            SelectedItemWrap = arg;
            switch (arg)
            {
                case AlbumWrap albumWrap when albumWrap.Value.InCollection:
                    albumWrap.Value = _collectionManager.GetAlbum(albumWrap.Value.Id);
                    CollectionItemPreview = _previewFactory.CreateAlbumPreviewVm(albumWrap.Value);
                    SelectedAlbum = albumWrap.Value;
                    SelectedArtist = null;
                    return;

                case AlbumWrap albumWrap:
                    CollectionItemPreview = _previewFactory.CreateAlbumPreviewVm(albumWrap.Value);
                    SelectedAlbum = albumWrap.Value;
                    SelectedArtist = null;
                    break;

                case ArtistWrap artistWrap:
                    CollectionItemPreview = _previewFactory.CreateArtistPreviewVm(artistWrap.Value);
                    SelectedArtist = artistWrap.Value;
                    SelectedAlbum = null;
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

        private void ShowMissingAlbums()
        {
            ShowAlbumsNotInCollection = !ShowAlbumsNotInCollection;
        }

        private void EnableSelection()
        {
            IsSelectionEnabled = !IsSelectionEnabled;
        }

        #region [  loading  ]

        private void LoadArtistAlbums(ArtistWrap artWrap)
        {
            artWrap.Children.Clear();
            var parent = artWrap;
            var albums = _collectionManager.GetAllArtistAlbums(artWrap.Value.Id);
            foreach (var album in albums)
            {
                artWrap.Children.Add(new AlbumWrap(album, parent));
            }
        }

        private void LoadAllArtistsInCollection()
        {
            var artistWrapCollection = new List<ArtistWrap>();
            foreach (var dbArtist in _allArtists)
            {
                //переделать
                var artWrap = CreateArtistWrap(dbArtist);
                artistWrapCollection.Add(artWrap);
            }

            FullArtistsCollection = new ObservableCollection<ArtistWrap>(artistWrapCollection);
            ArtistsCollectionToShow = FullArtistsCollection;
        }

        private ArtistWrap CreateArtistWrap(Artist artist)
        {
            var artWrap = new ArtistWrap(artist);
            artWrap.Children.Add(CreateAlbumWrap(new Album(), artWrap));
            return artWrap;
        }

        private AlbumWrap CreateAlbumWrap(Album album, ArtistWrap artistWrap)
        {
            return new AlbumWrap(album, artistWrap);
        }

        #endregion [  loading  ]

        #region [  selecting  ]

        private void SortBySelectedStylesAndGenres()
        {
            if (!_selectedStyles.Any() && !_selectedGenres.Any())
            {
                ArtistsCollectionToShow = FullArtistsCollection;
                return;
            }

            var selectedArtistsList = new List<ArtistWrap>();

            foreach (var artist in _allArtists)
            {
                //styles are selected
                if (_selectedStyles.Any() && !_selectedGenres.Any())
                {
                    if (artist.Styles.Intersect(_selectedStyles).Any())
                    {
                        selectedArtistsList.Add(CreateArtistWrap(artist));
                    }
                }

                //genres are selected
                else if (_selectedGenres.Any() && !_selectedStyles.Any())
                {
                    if (artist.Genres.Intersect(_selectedGenres).Any())
                    {
                        selectedArtistsList.Add(CreateArtistWrap(artist));
                    }
                }

                //genres and styles are selected
                else if (artist.Styles.Intersect(_selectedStyles).Any() && artist.Styles.Intersect(_selectedGenres).Any())
                {
                    selectedArtistsList.Add(CreateArtistWrap(artist));
                }
                
            }

            ArtistsCollectionToShow = new ObservableCollection<ArtistWrap>(selectedArtistsList);
        }

        private void UpdateSearchParametersList(List<string> parametersList, string newParameter)
        {
            if (parametersList == null) throw new ArgumentNullException(nameof(parametersList));
            if (string.IsNullOrEmpty(newParameter)) throw new ArgumentNullException(nameof(newParameter));

            if (parametersList.Contains(newParameter))
            {
                parametersList.Remove(newParameter);
                return;
            }
            parametersList.Add(newParameter);
        }

        #endregion [  selecting  ]

        #region [  updating  ]

        private void RefreshCollection()
        {
            var newArtists = _collectionManager.GetRecentArtists();
            foreach (var newArtist in newArtists)
            {
                var artWrap = new ArtistWrap(newArtist);
                artWrap.Children.Add(new AlbumWrap(new Album(), artWrap));
                ArtistsCollectionToShow.Add(artWrap);
            }
        }

        #endregion [  updating  ]

        #region [  deleting  ]

        private void DeleteFromCollection()
        {
            if (SelectedItemWrap == null) return;

            if (SelectedItemWrap is ArtistWrap artWrap)
            {
                var result = MessageBox.Show($"Do you want to delete artist [ {artWrap.Value.Name} ] from your music collection?", "Artist delete", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.No:
                        return;

                    case MessageBoxResult.Yes:
                        DeleteSelectedArtistFromCollection(artWrap);
                        return;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (SelectedItemWrap is AlbumWrap albWrap)
            {
                var result = MessageBox.Show($"Do you want to delete album  [ {albWrap.Value.Title} ] by [ {albWrap.Parent.Value.Name} ] from your music collection?", "Album delete", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.No:
                        return;

                    case MessageBoxResult.Yes:
                        DeleteSelectedAlbumFromCollection(albWrap);
                        return;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void DeleteSelectedArtistFromCollection(ArtistWrap artistWrap)
        {
            _collectionManager.DeleteArtistFromCollection(artistWrap.Value);
            ArtistsCollectionToShow.Remove(artistWrap);
        }

        private void DeleteSelectedAlbumFromCollection(AlbumWrap albumWrap)
        {
            _collectionManager.DeleteAlbumFromCollection(albumWrap.Value);
            var artWrap = albumWrap.Parent;
            artWrap.Children.Remove(albumWrap);
        }

        #endregion [  deleting  ]

        #endregion [  private methods  ]
    }
}