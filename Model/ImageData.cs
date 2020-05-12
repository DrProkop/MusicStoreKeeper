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

        /// <summary>
        /// Property for storing GrayScale16X16 in db.
        /// </summary>
        public byte[] GrayScaleDb
        {
            get => ImageDataConverter.ConvertFrom16X16GrayScale(GrayScale16X16);

            set => GrayScale16X16 = ImageDataConverter.ConvertTo16X16GrayScale(value);
        }

        [NotMapped]
        public byte[,] GrayScale16X16 { get; set; } = new byte[16, 16];
    }

    public enum ImageStatus
    {
        InCollection, 
        Deleted, 
        Unknown
    }
}