using System.Drawing;

namespace MusicStoreKeeper.ImageCollectionManager
{
    internal class ImageContainer
    {
        public string ImagePath { get; set; }
        public Image Image { get; set; }
        public byte[,] Image16X16GrayScale { get; set; }
    }
}