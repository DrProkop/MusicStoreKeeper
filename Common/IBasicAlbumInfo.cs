namespace Common
{
    public interface IBasicAlbumInfo
    {
        string ArtistName { get; set; }
        string AlbumTitle { get; set; }
        string TrackName { get; set; }
        uint Year { get; set; }
    }
}