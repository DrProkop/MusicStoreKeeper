namespace Common
{
    public interface IBasicTrackInfo
    {
        string ArtistName { get; set; }
        string AlbumTitle { get; set; }
        string TrackName { get; set; }
        uint Year { get; set; }
    }
}