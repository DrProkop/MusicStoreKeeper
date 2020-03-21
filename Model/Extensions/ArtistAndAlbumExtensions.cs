using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStoreKeeper.Model
{
    public static class ArtistAndAlbumExtensions 
    {
        #region [  styles  ]

        public static void AddStyle<T>(this T entity, string style) where T:ArtistAndAlbumBase
        {
            if (string.IsNullOrEmpty(style) || entity.Styles.Contains(style)) return;

            entity.Styles.Add(style);
        }

        public static void AddStyles<T>(this T entity, IEnumerable<string> styles) where T : ArtistAndAlbumBase
        {
            foreach (var style in styles)
            {
                entity.AddStyle(style);
            }
        }

        public static void RemoveStyle<T>(this T entity, string style) where T : ArtistAndAlbumBase
        {
            entity.Styles.Remove(style);
        }

        #endregion [  styles  ]

        #region [  genres  ]

        public static void AddGenre<T>(this T entity, string genre) where T : ArtistAndAlbumBase
        {
            if (entity.Genres.Contains(genre)) return;
            entity.Genres.Add(genre);
        }

        public static void AddGenres<T>(this T entity, IEnumerable<string> genres) where T : ArtistAndAlbumBase
        {
            foreach (var genre in genres)
            {
                entity.AddGenre(genre);
            }
        }

        public static void RemoveGenre<T>(this T entity, string genre) where T : ArtistAndAlbumBase
        {
            entity.Genres.Remove(genre);
        }

        #endregion [  genres  ]
    }
}
