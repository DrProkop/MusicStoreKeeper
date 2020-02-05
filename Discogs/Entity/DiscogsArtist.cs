namespace Discogs.Entity
{
    public class DiscogsArtist:DiscogsEntity
    {
        public string Name { get; set; }
        public string[] NameVariations { get; set; }
        public string Profile { get; set; }
    }
}