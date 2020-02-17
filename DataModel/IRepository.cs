using System.Collections.Generic;
using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.DataModel
{
    /// <summary>
    /// Stores collection info in db.
    /// </summary>
    public interface IRepository
    {
        void AddNewArtist(Artist artist);
        void DeleteArtist(Artist artist);
        Artist FindArtistById(int id);
        IEnumerable<Artist> GetAllArtists();
        Artist GetArtistWithAlbums(int id);
        void Save();
    }
}