namespace MusicStoreKeeper.Model
{
    public class Track:BaseEntity
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public int AlbumId { get; set; }
        public int ArtistId { get; set; }
    }
}
