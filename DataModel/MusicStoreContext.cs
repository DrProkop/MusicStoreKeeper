using System.Data.Entity;
using Model;

namespace DataModel
{
    public class MusicStoreContext:DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
    }
}
