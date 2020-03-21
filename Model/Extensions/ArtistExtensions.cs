namespace MusicStoreKeeper.Model
{
    public static class ArtistExtensions
    {
        public static void Merge(this Artist artistA, Artist artistB)
        {
            if (artistB == null) return;

            if (!string.IsNullOrEmpty(artistB.Name))
            {
                artistA.Name = artistB.Name;
            }
            if (!string.IsNullOrEmpty(artistB.Profile))
            {
                artistA.Profile = artistB.Profile;
            }
            if (!string.IsNullOrEmpty(artistB.RealName))
            {
                artistA.RealName = artistB.RealName;
            }
            if (!string.IsNullOrEmpty(artistB.StoragePath))
            {
                artistA.StoragePath = artistB.StoragePath;
            }
        }
    }
}