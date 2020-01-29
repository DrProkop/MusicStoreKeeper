using System.Collections.Generic;

namespace Model
{
    public class Album
    {
        public Album()
        {
            Tracks=new List<Track>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReleaseDate { get; set; }
        public List<Track> Tracks {get; set; }
        public Artist Artist { get; set; }
        public int ArtistId { get; set; }
    }
}
