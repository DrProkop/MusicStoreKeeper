namespace MusicStoreKeeper.Model
{
    public static class AlbumExtensions
    {
        public static void Merge(this Album albumA, Album albumB)
        {
            if (albumB == null) return;

            if (!string.IsNullOrEmpty(albumB.Title))
            {
                albumA.Title = albumB.Title;
            }

            if (albumB.DiscogsId != 0)
            {
                albumA.DiscogsId = albumB.DiscogsId;
            }

            if (albumB.ReleaseDate != 0)
            {
                albumA.ReleaseDate = albumB.ReleaseDate;
            }

            if (!string.IsNullOrEmpty(albumB.GenresString))
            {
                albumA.GenresString = albumB.GenresString;
            }

            if (!string.IsNullOrEmpty(albumB.StylesString))
            {
                albumA.StylesString = albumB.StylesString;
            }

            albumA.Partial = albumB.Partial;
        }
    }
}