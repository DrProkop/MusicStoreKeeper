using System.Collections.Generic;

namespace MusicStoreKeeper.Model
{
    public class Album : ArtistAndAlbumBase
    {
        public Album()
        {
        }

        public string Title { get; set; }

        public int ReleaseDate { get; set; }

        public List<Track> Tracks { get; set; } = new List<Track>();

        public List<ImageData> ImageDataList { get; set; } = new List<ImageData>();

        public int ArtistId { get; set; }

        public bool InCollection { get; set; }

        public bool Partial { get; set; }
    }
}