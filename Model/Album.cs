using System.Collections.Generic;

namespace MusicStoreKeeper.Model
{
    public class Album:BaseEntity
    {
        public Album()
        {
            Tracks=new List<Track>();
        }
        
        public int DiscogsId { get; set; }
        public string Title { get; set; }
        public int ReleaseDate { get; set; }
        public List<Track> Tracks {get; set; }
        public int ArtistId { get; set; }
        public bool InCollection { get; set; }
        public string StoragePath { get; set; }
    }
}
