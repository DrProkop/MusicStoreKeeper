using MusicStoreKeeper.Model;
using System.Threading.Tasks;

namespace Common
{
    public interface ICollectionManager
    {
        string MusicSearchDirectory { get; set; }
        string MusicCollectionDirectory { get; set; }

        Task<Artist> SearchArtistAndAllAlbumsOnDiscogs(IMusicDirInfo mDirInfo);

        Task<Album> SearchFullAlbumOnDiscogs(Artist artist, IMusicDirInfo mDirInfo);

        void MoveToCollectionManually(ISimpleFileInfo sFi);
    }
}