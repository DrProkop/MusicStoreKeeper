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

        Artist GetArtistWithAlbumsAndImageData(int id);

        IEnumerable<int> GetRecentlyAddedArtists();

        //album
        void UpdateArtistDiscography(int artistCollectionId, IEnumerable<Album> newAlbums);

        int AddNewAlbum(int artistId, Album album);

        int UpdateAlbum(int artistId, Album album);

        void AddAlbumToStorage(Album album, string storagePath);

        void AddAlbumToStorage(int albumId, string storagePath);

        IEnumerable<Album> GetAllArtistAlbums(int artistId);

        Album GetAlbumWithTracksAndImageData(int albumId);

        void DeleteAlbum(Album album);

        void DeleteAlbum(int albumId);

        Album FindAlbumById(int albumId);

        Album FindAlbumByTitleOrDiscogsId(string title, int discogsId);

        // imagedata

        IEnumerable<ImageData> FindArtistOrAlbumImagesData(int ownerId);

        void AddImageData(ImageData imageData, int ownerId);

        void AddImagesData(IEnumerable<ImageData> imagesData, int ownerId);

        void DeleteImageData(ImageData imageData);

        void DeleteImageData(int id);

        // save
        void Save();
    }
}