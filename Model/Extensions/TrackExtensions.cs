namespace MusicStoreKeeper.Model
{
    public static class TrackExtensions
    {
        public static void Merge(this Track trackA, Track trackB)
        {
            if (trackA == null || trackB == null) return;

            if (!string.IsNullOrEmpty(trackB.Name))
            {
                trackA.Name = trackB.Name;
            }
            if (trackB.Number != 0)
            {
                trackA.Number = trackB.Number;
            }
            if (!string.IsNullOrEmpty(trackB.Duration))
            {
                trackA.Duration = trackB.Duration;
            }
        }
    }
}