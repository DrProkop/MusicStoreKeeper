using MusicStoreKeeper.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public interface IMusicCollectionManager
    {
        string MusicSearchDirectory { get; set; }
        string MusicCollectionDirectory { get; set; }

        Task<Artist> SearchArtistAndAllAlbumsOnDiscogs(IMusicDirInfo mDirInfo, bool updateExisting, CancellationToken token);

        Task<Album> SearchFullAlbumOnDiscogs(Artist artist, IMusicDirInfo mDirInfo, bool updateExisting, CancellationToken token);

        void MoveToCollectionManually(ISimpleFileInfo sFi);

        IEnumerable<Artist> GetAllArtists();

        IEnumerable<Artist> GetRecentArtists();

        IEnumerable<Album> GetAllArtistAlbums(int artistId);

        Album GetAlbum(int albumId);

        void DeleteAlbumFromCollection(Album album);

        void DeleteArtistFromCollection(Artist artist);

        List<string> GetMusicStylesList();

        List<string> GetMusicGenresList();
    }
}