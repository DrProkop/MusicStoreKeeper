namespace Discogs.Entity
{
    public class DiscogsRelease:DiscogsEntity
    {
        public DiscogsImage[] images { get; set; }
        public DiscogsTrack[] tracklist { get; set; }
        public DiscogsReleaseArtist[] artists { get; set; }
        public DiscogsReleaseArtist[] extraartists { get; set; }
        public DiscogsReleaseLabel[] labels { get; set; }

        public string title { get; set; }
        public int year { get; set; }
        public string[] styles { get; set; }
        public string[] genres { get; set; }
        public string resource_url { get; set; }
        public string uri { get; set; }
        public string master_url { get; set; }
        public string country { get; set; }
    }
}