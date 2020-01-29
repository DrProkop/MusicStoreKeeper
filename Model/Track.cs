namespace Model
{
    public class Track
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public Album Album { get; set; }
        public Artist Artist { get; set; }
    }
}
