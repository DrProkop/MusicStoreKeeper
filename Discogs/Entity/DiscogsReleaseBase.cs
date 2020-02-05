namespace Discogs.Entity
{
    public class DiscogsReleaseBase : DiscogsEntity
    {
        public DiscogsVideo[] videos { get; set; }
        public DiscogsImage[] images { get; set; }
        public DiscogsTrack[] tracklist { get; set; }
        public DiscogsReleaseArtist[] artists { get; set; }

        public string data_quality { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public string[] styles { get; set; }
        public string[] genres { get; set; }
        public string resource_url { get; set; }
        public string uri { get; set; }
    }
}