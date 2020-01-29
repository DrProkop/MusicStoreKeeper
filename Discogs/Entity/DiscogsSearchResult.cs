using Discogs.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Discogs.Entity
{
    public class DiscogsSearchResult : DiscogsEntity
    {
        public string[] genre { get; set; }
        public string[] style { get; set; }
        public string[] format { get; set; }
        public string[] label { get; set; }
        public string[] barcode { get; set; }
        public int? year { get; set; }
        public string title { get; set; }
        public string thumb { get; set; }
        public string country { get; set; }
        public string catno { get; set; }
        public string uri { get; set; }
        public string resource_url { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DiscogsSearchObjectType type { get; set; }
    }
}
