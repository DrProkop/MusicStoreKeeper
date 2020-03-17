using System.Collections.Generic;

namespace MusicStoreKeeper.Model
{
    public class Artist : ArtistAndAlbumBase
    {
        public Artist()
        {
        }

        public string Name { get; set; }

        public string RealName { get; set; }

        public string Profile { get; set; }

        public List<Album> Albums { get; set; } = new List<Album>();
    }
}