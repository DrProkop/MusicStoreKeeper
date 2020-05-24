using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStoreKeeper.Model
{
    /// <summary>
    /// Info about artist and album images in collection.
    /// </summary>
    public class ImageData : BaseEntity
    {
        public string Name { get; set; }

        public string Source { get; set; }

        public ImageStatus Status { get; set; } = ImageStatus.Unknown;

        public int ImageOwnerId { get; set; }
    }

    public enum ImageStatus
    {
        InCollection,
        Deleted,
        Unknown
    }
}