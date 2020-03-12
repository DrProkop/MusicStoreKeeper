using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStoreKeeper.Model
{
    public class Album : BaseEntity
    {
        public Album()
        {
            Tracks = new List<Track>();
        }

        public int DiscogsId { get; set; }
        public string Title { get; set; }
        public int ReleaseDate { get; set; }
        public List<Track> Tracks { get; set; }
        public int ArtistId { get; set; }
        public bool InCollection { get; set; }
        public bool Partial { get; set; }
        public string StoragePath { get; set; }
        public string StylesString { get; set; }
        public string GenresString { get; set; }

        private List<string> _styles;

        [NotMapped]
        public List<string> Styles
        {
            get => _styles ?? (_styles = new List<string>(JsonConvert.DeserializeObject<List<string>>(StylesString)));
            set
            {
                StylesString = JsonConvert.SerializeObject(value);
                _styles = value;
            }
        }

        private List<string> _genres;

        [NotMapped]
        public List<string> Genres
        {
            get => _genres ?? (_genres = new List<string>(JsonConvert.DeserializeObject<List<string>>(GenresString)));
            set
            {
                GenresString = JsonConvert.SerializeObject(value);
                _genres = value;
            }
        }
    }
}