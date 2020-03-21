using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MusicStoreKeeper.CollectionManager
{
    public class GenreAndStyleProvider
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public GenreAndStyleProvider()
        {
            InitializeGenresAndStylesCollections();
        }

        #region [  constants  ]

        private const string GenresFileName = "Musicgenres.json";
        private const string StylesFileName = "Musicstyles.json";

        #endregion [  constants  ]

        #region [  fields  ]

        private string _genresFilePath;
        private string _stylesFilePath;

        #endregion [  fields  ]

        #region [  properties  ]

        private HashSet<String> GenresSet { get; } = new HashSet<string>();
        private HashSet<String> StylesSet { get; } = new HashSet<string>();

        #endregion [  properties  ]

        #region [  public methods  ]

        #region [  genres  ]

        public void AddGenre(string genre)
        {
            GenresSet.Add(genre);
        }

        public void AddGenres(IEnumerable<string> genres)
        {
            foreach (var genre in genres)
            {
                AddGenre(genre);
            }
        }

        public void SaveGenres()
        {
            SaveToFile(_genresFilePath, GenresSet);
        }

        public IEnumerable<string> GetGenres()
        {
            return GenresSet;
        }

        #endregion [  genres  ]

        #region [  styles  ]

        public void AddStyle(string style)
        {
            StylesSet.Add(style);
        }

        public void AddStyles(IEnumerable<string> styles)
        {
            foreach (var style in styles)
            {
                AddStyle(style);
            }
        }

        public IEnumerable<string> GetStyles()
        {
            return StylesSet;
        }

        public void SaveStyles()
        {
            SaveToFile(_stylesFilePath, StylesSet);
        }

        #endregion [  styles  ]

        public void SaveAll()
        {
            SaveGenres();
            SaveStyles();
        }

        #endregion [  public methods  ]

        #region [  private methods  ]

        private void InitializeGenresAndStylesCollections()
        {
            _genresFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GenresFileName);
            _stylesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, StylesFileName);
            AddGenres(GetDataFromFile(_genresFilePath));
            AddStyles(GetDataFromFile(_stylesFilePath));
        }

        private IEnumerable<string> GetDataFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return new List<string>();
            }

            using (var file = File.OpenText(path))
            {
                using (var jsonReader = new JsonTextReader(file))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<List<string>>(jsonReader);
                }
            }
        }

        private void SaveToFile(string path, IEnumerable<string> data)
        {
            using (var file = File.CreateText(path))
            using (var jsonWriter = new JsonTextWriter(file))
            {
                jsonWriter.Formatting = Formatting.Indented;
                //jsonWriter.WriteStartObject();
                //jsonWriter.WriteEndObject();
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, data);
            }
        }

        #endregion [  private methods  ]
    }
}