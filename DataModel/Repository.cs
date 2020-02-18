using System.Collections.Generic;
using System.Linq;
using MusicStoreKeeper.Model;
using Common;
using Serilog;
using System.Data.Entity;

namespace MusicStoreKeeper.DataModel
{
    /// <summary>
    /// Stores collection info in db.
    /// </summary>
    public class Repository : IRepository
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Repository(ILoggerManager manager)
        {
            _log = manager.GetLogger(this);
        }

        #region [  fields  ]

        private readonly ILogger _log;
        private readonly MusicStoreContext _musicStoreContext = new MusicStoreContext();

        #endregion

        #region [  properties  ]

        #endregion

        #region [  public methods  ]
        #region [  artist  ]

        public void AddNewArtist(Artist artist)
        {
            _musicStoreContext.Artists.Add(artist);
            _log.Information("Added artist {ArtistName} to music collection", artist.Name);
        }

        public void DeleteArtist(Artist artist)
        {
            _musicStoreContext.Artists.Remove(artist);
        }

        public Artist FindArtistById(int id)
        {
            return _musicStoreContext.Artists.Find(id);
        }

        public IEnumerable<Artist> GetAllArtists()
        {
            return _musicStoreContext.Artists.ToList();
        }

        public Artist GetArtistWithAlbums(int id)
        {
            return _musicStoreContext.Artists.Include(art => art.Albums).FirstOrDefault( art => art.Id == id);
        }

        #endregion

        #region [  album  ]

        public IEnumerable<Album> GetAllArtistAlbums(int artistId)
        {
            return _musicStoreContext.Albums.Where(alb => alb.ArtistId == artistId);
        }

        #endregion

        public void Save()
        {
            _musicStoreContext.SaveChanges();
        }
        #endregion

        #region [  private methods  ]

        #endregion

    }
}