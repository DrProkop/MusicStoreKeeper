using System.Collections.Generic;

namespace MusicStoreKeeper.Model
{
    public class Artist:BaseEntity
    {
        public Artist()
        {
            Albums = new List<Album>();
        }

        public int DiscogsId { get; set; }
        public string Name { get; set; }
        public List<Album> Albums { get; set; }
        public string StoragePath { get; set; }
    }
}
