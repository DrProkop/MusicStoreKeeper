using System.Collections.Generic;

namespace MusicStoreKeeper.Model
{
    public static class AlbumExtensions
    {
        public static void Merge(this Album albumA, Album albumB)
        {
            if (albumA == null || albumB == null) return;

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

        #region [  styles  ]

        public static void AddStyle(this Album album, string style)
        {
            if (album.Styles.Contains(style)) return;
            album.Styles.Add(style);
        }

        public static void AddStyles(this Album album, IEnumerable<string> styles)
        {
            foreach (var style in styles)
            {
                album.AddStyle(style);
            }
        }

        public static void RemoveStyle(this Album album, string style)
        {
            album.Styles.Remove(style);
        }

        #endregion [  styles  ]

        #region [  genres  ]

        public static void AddGenre(this Album album, string genre)
        {
            if (album.Genres.Contains(genre)) return;
            album.Genres.Add(genre);
        }

        public static void AddGenres(this Album album, IEnumerable<string> genres)
        {
            foreach (var genre in genres)
            {
                album.AddGenre(genre);
            }
        }

        public static void RemoveGenre(this Album album, string genre)
        {
            album.Genres.Remove(genre);
        }

        #endregion [  genres  ]
    }
}