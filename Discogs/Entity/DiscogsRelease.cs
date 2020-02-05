namespace Discogs.Entity
{
    public class DiscogsRelease:DiscogsReleaseBase
    {
        public DiscogsReleaseArtist[] extraartists { get; set; }
        public DiscogsReleaseLabel[] labels { get; set; }

        public string resource_url { get; set; }
        public string uri { get; set; }
        public int master_id { get; set; }
        public string master_url { get; set; }
        public string country { get; set; }
    }
}