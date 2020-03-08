﻿using Common;
using MusicStoreKeeper.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MusicStoreKeeper.DataModel
{
    /// <summary>
    /// Stores  music collection info in db.
    /// </summary>
    public class Repository : IRepository
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Repository(ILoggerManager manager)
        {
            _log = manager.GetLogger(this);
            _addedArtists = new HashSet<int>();
        }

        #region [  fields  ]

        private readonly ILogger _log;
        private readonly MusicStoreContextLite _musicStoreContext = new MusicStoreContextLite();
        private readonly HashSet<int> _addedArtists;

        #endregion [  fields  ]

        #region [  public methods  ]

        #region [  artist  ]

        public int AddNewArtist(Artist artist)
        {
            _musicStoreContext.Artists.Add(artist);
            Save();
            _log.Information("Added artist {ArtistName} to music collection", artist.Name);
            _addedArtists.Add(artist.Id);
            return artist.Id;
        }

        public void UpdateArtist(int artistCollectionId, Artist artist)
        {
            var existingArtist = GetArtistWithAlbums(artistCollectionId);
            existingArtist.Merge(artist);
            foreach (var album in artist.Albums)
            {
                Album existingAlbum;
                if (album.DiscogsId == 0)
                {
                    existingAlbum = existingArtist.Albums.SingleOrDefault(alb => alb.Title.Equals(album.Title, StringComparison.InvariantCultureIgnoreCase) && alb.ReleaseDate==album.ReleaseDate);
                }
                else
                {
                    existingAlbum = existingArtist.Albums.SingleOrDefault(alb => alb.DiscogsId == album.DiscogsId);
                }

                if (existingAlbum != null)
                {
                    existingAlbum.Merge(album);
                }
                else
                {
                    existingArtist.Albums.Add(album);
                }
            }
            Save();
        }

        public int AddOrUpdateArtistFull(Artist artist)
        {
            Artist existingArtist;
            if (artist.DiscogsId == 0)
            {
                existingArtist = _musicStoreContext.Artists
                    .Where(art => art.Name.Equals(artist.Name))
                    .Include(art => art.Albums).SingleOrDefault();
            }
            else
            {
                existingArtist = _musicStoreContext.Artists
                    .Where(art => art.DiscogsId == artist.DiscogsId)
                    .Include(art => art.Albums).SingleOrDefault();
            }

            if (existingArtist == null) return AddNewArtist(artist);
            //logic for updating artist here
            existingArtist.Merge(artist);
            foreach (var album in artist.Albums)
            {
                Album existingAlbum;
                if (album.DiscogsId == 0)
                {
                    existingAlbum = existingArtist.Albums.SingleOrDefault(alb => alb.Title.Equals(album.Title));
                }
                else
                {
                    existingAlbum = existingArtist.Albums.SingleOrDefault(alb => alb.DiscogsId == album.DiscogsId);
                }

                if (existingAlbum != null)
                {
                    existingAlbum.Merge(album);
                }
                else
                {
                    existingArtist.Albums.Add(album);
                }
            }
            Save();

            return existingArtist.Id;
        }

        public void AddArtistToStorage(Artist artist, string storagePath)
        {
            if (artist == null) throw new ArgumentNullException(nameof(artist));
            _musicStoreContext.Entry(artist).Property(p => p.StoragePath).CurrentValue = storagePath ?? throw new ArgumentNullException(nameof(storagePath));

            Save();
        }

        public void AddArtistToStorage(int artistId, string storagePath)
        {
            var artist = _musicStoreContext.Artists.Find(artistId);
            if (artist != null)
            {
                AddArtistToStorage(artist, storagePath);
            }
        }

        public void DeleteArtist(Artist artist)
        {
            if (artist == null) throw new ArgumentNullException(nameof(artist));

            _musicStoreContext.Artists.Remove(artist);
            Save();
        }

        public void DeleteArtist(int artistId)
        {
            var artist = _musicStoreContext.Artists.Find(artistId);
            if (artist != null)
            {
                DeleteArtist(artist);
            }
        }

        public Artist FindArtistById(int id)
        {
            return _musicStoreContext.Artists.Find(id);
        }

        public Artist FindArtistByDiscogsId(int discogsId)
        {
            return _musicStoreContext.Artists.FirstOrDefault(art => art.DiscogsId == discogsId);
        }

        public Artist FindArtistByName(string artistName)
        {
            return _musicStoreContext.Artists.FirstOrDefault(art => art.Name == artistName);
        }

        public Artist FindArtistByNameOrDiscogsId(string artistName, int discogsId)
        {
            return _musicStoreContext.Artists.FirstOrDefault(art => art.DiscogsId == discogsId) ?? _musicStoreContext.Artists.FirstOrDefault(art => art.Name == artistName);
        }

        public IEnumerable<Artist> GetAllArtists()
        {
            return _musicStoreContext.Artists.ToList();
        }

        public Artist GetArtistWithAlbums(int id)
        {
            return _musicStoreContext.Artists.Include(art => art.Albums).FirstOrDefault(art => art.Id == id);
        }

        public IEnumerable<int> GetRecentlyAddedArtists()
        {
            var artists = _addedArtists.ToList();
            _addedArtists.Clear();
            return artists;
        }

        #endregion [  artist  ]

        #region [  album  ]

        public int AddNewAlbum(int artistId, Album album)
        {
            album.ArtistId = artistId;
            _musicStoreContext.Albums.Add(album);
            Save();
            return album.Id;
        }

        public int AddOrUpdateAlbum(int artistId, Album album)
        {
            var artistAlbums = _musicStoreContext.Albums.Where(a => a.ArtistId == artistId).ToList();
            var updatedAlbum = artistAlbums.FirstOrDefault(alb => alb.Title.Equals(album.Title));
            if (updatedAlbum == null) return AddNewAlbum(artistId, album);
            //logic for updating album here
            foreach (var track in album.Tracks)
            {
                var existingTrack = updatedAlbum.Tracks.FirstOrDefault(et =>
                    et.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase));
                if (existingTrack != null)
                {
                    existingTrack.Merge(track);
                }
                else
                {
                    updatedAlbum.Tracks.Add(track);
                }
            }
            Save();
            return updatedAlbum.Id;
        }

        public int UpdateAlbum(int artistId, Album album)
        {
            if (album == null) throw new ArgumentNullException(nameof(album));

            var updatedAlbum = _musicStoreContext.Albums.FirstOrDefault(alb => alb.ArtistId==artistId && alb.Title.Equals(album.Title, StringComparison.InvariantCultureIgnoreCase) );
            if (updatedAlbum == null) return 0;
            updatedAlbum.Merge(album);
            foreach (var track in album.Tracks)
            {
                var existingTrack = updatedAlbum.Tracks.FirstOrDefault(et =>
                    et.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase));
                if (existingTrack != null)
                {
                    existingTrack.Merge(track);
                }
                else
                {
                    updatedAlbum.Tracks.Add(track);
                }
            }
            Save();
            return updatedAlbum.Id;
        }

        public void AddAlbumToStorage(Album album, string storagePath)
        {
            if (album == null) throw new ArgumentNullException(nameof(album));
            _musicStoreContext.Entry(album).Property(p => p.StoragePath).CurrentValue = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
            _musicStoreContext.Entry(album).Property(p => p.InCollection).CurrentValue = true;
            Save();
        }

        public void AddAlbumToStorage(int albumId, string storagePath)
        {
            var album = _musicStoreContext.Albums.Find(albumId);
            if (album != null)
            {
                AddAlbumToStorage(album, storagePath);
            }
        }

        public IEnumerable<Album> GetAllArtistAlbums(int artistId)
        {
            return _musicStoreContext.Albums.Where(alb => alb.ArtistId == artistId).ToList();
        }

        public Album GetAlbumWithTracks(int albumId)
        {
            return _musicStoreContext.Albums.Include(alb => alb.Tracks).FirstOrDefault(alb => alb.Id == albumId);
        }

        public Album FindAlbumById(int albumId)
        {
            return _musicStoreContext.Albums.Find(albumId);
        }

        public Album FindAlbumByTitleOrDiscogsId(string title, int discogsId)
        {
            return _musicStoreContext.Albums.FirstOrDefault(alb => alb.DiscogsId == discogsId) ?? _musicStoreContext.Albums.FirstOrDefault(alb => alb.Title == title);
        }

        public void DeleteAlbum(Album album)
        {
            if (album == null) throw new ArgumentNullException(nameof(album));

            _musicStoreContext.Albums.Remove(album);
            Save();
        }

        public void DeleteAlbum(int albumId)
        {
            var album = _musicStoreContext.Albums.Find(albumId);
            if (album != null)
            {
                DeleteAlbum(album);
            }
        }

        #endregion [  album  ]

        public void Save()
        {
            _musicStoreContext.SaveChanges();
        }

        #endregion [  public methods  ]
    }
}