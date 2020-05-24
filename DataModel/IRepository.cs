using MusicStoreKeeper.Model;
using System.Collections.Generic;

namespace MusicStoreKeeper.DataModel
{
    /// <summary>
    /// Stores  music collection info in db.
    /// </summary>
    public interface IRepository
    {
        //artist
        int AddNewArtist(Artist artist);

        void UpdateArtist(int artistCollectionId, Artist artist);

        void AddArtistToStorage(Artist artist, string storagePath);

        void AddArtistToStorage(int artistId, string storagePath);

        void DeleteArtist(Artist artist);

        void DeleteArtist(int artistId);

        Artist FindArtistById(int id);

        Artist FindArtistByDiscogsId(int discogsId);

        Artist FindArtistByName(string artistName);

        Artist FindArtistByNameOrDiscogsId(string artistName, int discogsId);

        IEnumerable<Artist> GetAllArtists();

        Artist GetArtistWithAlbums(int id);

        IEnumerable<int> GetRecentlyAddedArtists();

        //album
        void UpdateArtistDiscography(int artistCollectionId, IEnumerable<Album> newAlbums);

        int AddNewAlbum(int artistId, Album album);

        int UpdateAlbum(int artistId, Album album);

        void AddAlbumToStorage(Album album, string storagePath);

        void AddAlbumToStorage(int albumId, string storagePath);

        IEnumerable<Album> GetAllArtistAlbums(int artistId);

        Album GetAlbumWithTracks(int albumId);

        void DeleteAlbum(Album album);

        void DeleteAlbum(int albumId);

        Album FindAlbumById(int albumId);

        Album FindAlbumByTitleOrDiscogsId(string title, int discogsId);

        // imagedata

        void AddImageData(ImageData imageData, int ownerId);

        void AddImagesData(IEnumerable<ImageData> imagesData, int ownerId);

        // save
        void Save();
    }
}