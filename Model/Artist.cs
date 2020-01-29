using System.Collections.Generic;

namespace Model
{
    public class Artist
    {
        public Artist()
        {
            Albums = new List<Album>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Album> Albums { get; set; }
    }
}
