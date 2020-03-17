using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStoreKeeper.Model
{
    public abstract class ArtistAndAlbumBase : BaseEntity
    {
        protected ArtistAndAlbumBase()
        {
        }

        #region [  properties  ]

        public int DiscogsId { get; set; }

        public string StoragePath { get; set; }

        public string StylesString
        {
            get => JsonConvert.SerializeObject(Styles);
            set
            {
                if (value == null) Genres.Clear();
                Styles = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

        public string GenresString
        {
            get => JsonConvert.SerializeObject(Genres);
            set
            {
                if (value == null) Genres.Clear();
                Genres = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

        [NotMapped]
        public List<string> Styles { get; set; } = new List<string>();

        [NotMapped]
        public List<string> Genres { get; set; } = new List<string>();

        #endregion [  properties  ]
    }
}