using System.ComponentModel;
using System.Text;
using Discogs.Enums;
using RestSharp.Extensions;

namespace Discogs
{
    /// <summary>
    /// Contains search query and additional parameters for search on Discogs.
    /// </summary>
    public class DiscogsSearchParameters
    {
        /// <summary>
        /// Search query. Example: nirvana
        /// </summary>
        [Description("q")]
        public string query { get; set; }

        /// <summary>
        /// Search object type. One of release, master, artist, label.
        /// </summary>
        public DiscogsSearchObjectType? type { get; set; }

        /// <summary>
        /// Search by combined “Artist Name - Release Title” title field. Example: nirvana - nevermind.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Release title. Example: nevermind.
        /// </summary>
        [Description("release_title")]
        public string releaseTitle { get; set; }

        /// <summary>
        /// Release credits. Example: kurt.
        /// </summary>
        public string credit { get; set; }

        /// <summary>
        /// Artist name. Example: nirvana
        /// </summary>
        public string artist { get; set; }

        /// <summary>
        /// Artist ANV. Example: nirvana.
        /// </summary>
        public string anv { get; set; }

        /// <summary>
        /// Label name. Example: dgc.
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// Genre.  Example: rock.
        /// </summary>
        public string genre { get; set; }

        /// <summary>
        /// Style. Example: grunge.
        /// </summary>
        public string style { get; set; }

        /// <summary>
        /// Release country. Example: canada.
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// Release year. Example: 1991.
        /// </summary>
        public string year { get; set; }

        /// <summary>
        /// Format. Example: album.
        /// </summary>
        public string format { get; set; }

        /// <summary>
        /// Catalog number. Example: DGCD-24425.
        /// </summary>
        public string catno { get; set; }

        /// <summary>
        /// Barcode. Example: 7 2064-24425-2 4.
        /// </summary>
        public string barcode { get; set; }

        /// <summary>
        /// Track title. Example: smells like teen spirit.
        /// </summary>
        public string track { get; set; }

        /// <summary>
        /// Submitter username.
        /// </summary>
        public string submitter { get; set; }

        /// <summary>
        /// Contributor username.
        /// </summary>
        public string contributor { get; set; }

        /// <summary>
        /// Returns a formatted string with all set search parameters.
        /// </summary>
        /// <returns></returns>
        public string GetSearchString()
        {
            return SearchParameterReader.Instance.GetSearchParameters(this);
        }
    }

    internal class SearchParameterReader
    {
        private const string SearchStringStart = "/database/search?";

        private static SearchParameterReader _reader;
        private readonly StringBuilder _str = new StringBuilder();

        private SearchParameterReader()
        {
        }

        public static SearchParameterReader Instance => _reader ?? new SearchParameterReader();

        /// <summary>
        /// Returns a formatted string made from properties names and values.
        /// </summary>
        internal string GetSearchParameters(object obj)
        {
            _str.Clear();
            _str.Append(SearchStringStart);
            foreach (var propInfo in obj.GetType().GetProperties())
            {
                var value = propInfo.GetValue(obj, null);
                if (value == null) continue;
                var attribute = propInfo.GetAttribute<DescriptionAttribute>();
                var name = (attribute == null) ? propInfo.Name : attribute.Description;
                _str.Append($"{name}={value}&");
            }

            _str.Remove(_str.Length - 1, 1);
            return _str.ToString();
        }
    }
}