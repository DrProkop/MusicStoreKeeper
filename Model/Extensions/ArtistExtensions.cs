using System.Collections.Generic;

namespace MusicStoreKeeper.Model
{
    public static class ArtistExtensions
    {
        public static void Merge(this Artist artistA, Artist artistB)
        {
            if (artistB == null) return;

            if (artistB.Name != null)
            {
                artistA.Name = artistB.Name;
            }
            if (artistB.Profile != null)
            {
                artistA.Profile = artistB.Profile;
            }
            if (artistB.RealName != null)
            {
                artistA.RealName = artistB.RealName;
            }
            if (artistB.StoragePath != null)
            {
                artistA.StoragePath = artistB.StoragePath;
            }
        }

        #region [  styles  ]

        public static void AddStyle(this Artist artist, string style)
        {
            if (string.IsNullOrEmpty(style) || artist.Styles.Contains(style)) return;

            artist.Styles.Add(style);
        }

        public static void AddStyles(this Artist artist, IEnumerable<string> styles)
        {
            foreach (var style in styles)
            {
                artist.AddStyle(style);
            }
        }

        public static void RemoveStyle(this Artist artist, string style)
        {
            artist.Styles.Remove(style);
        }

        #endregion [  styles  ]

        #region [  genres  ]

        public static void AddGenre(this Artist artist, string genre)
        {
            if (artist.Genres.Contains(genre)) return;
            artist.Genres.Add(genre);
        }

        public static void AddGenres(this Artist artist, IEnumerable<string> genres)
        {
            foreach (var genre in genres)
            {
                artist.AddGenre(genre);
            }
        }

        public static void RemoveGenre(this Artist artist, string genre)
        {
            artist.Genres.Remove(genre);
        }

        #endregion [  genres  ]
    }
}