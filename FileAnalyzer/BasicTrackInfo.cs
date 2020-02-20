using Common;

namespace FileAnalyzer
{

    public class BasicTrackInfo : IBasicTrackInfo
    {
        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }
        public string TrackName { get; set; }
        public uint Year { get; set; }
    }
}