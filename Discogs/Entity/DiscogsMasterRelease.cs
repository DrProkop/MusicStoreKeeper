namespace Discogs.Entity
{
    public class DiscogsMasterRelease : DiscogsReleaseBase
    {
        public int most_recent_release { get; set; }
        public string most_recent_release_url { get; set; }
        public int main_release { get; set; }
        public string main_release_url { get; set; }
        public string versions_url { get; set; }
    }
}