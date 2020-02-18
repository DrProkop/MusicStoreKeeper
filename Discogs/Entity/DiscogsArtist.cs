namespace Discogs.Entity
{
    public class DiscogsArtist:DiscogsEntity
    {
        public DiscogsImage[] images { get; set; }

        public string name { get; set; }
        public string realname { get; set; }
        public string[] nameVariations { get; set; }
        public string profile { get; set; }

    }
}