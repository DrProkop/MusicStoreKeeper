using System.Collections.Generic;
using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.DataModel
{
    /// <summary>
    /// Stores collection info in db.
    /// </summary>
    public interface IRepository
    {
        //artist
        int AddNewArtist(Artist artist);
        int AddOrUpdateArtistFull(Artist artist);
        void DeleteArtist(Artist artist);
        void DeleteArtist(int artistId);
        Artist FindArtistById(int id);
        Artist FindArtistByDiscogsId(int discogsId);
        Artist FindArtistByName(string artistName);
        Artist FindArtistByNameAndDiscogsId(string artistName, int discogsId);
        IEnumerable<Artist> GetAllArtists();
        Artist GetArtistWithAlbums(int id);
        //album
        int AddNewAlbum(int artistId, Album album);
        int AddOrUpdateAlbum(int artistId, Album album);
        void AddAlbumToStorage(Album album, string storagePath);
        void AddAlbumToStorage(int albumId, string storagePath);
        IEnumerable<Album> GetAllArtistAlbums(int artistId);
        Album GetAlbumWithTracks(int albumId);
        void DeleteAlbum(Album album);
        void DeleteAlbum(int albumId);
        Album FindAlbumById(int albumId);

        void Save();
    }
}