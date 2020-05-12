using Common;
using MusicStoreKeeper.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using XnaFan.ImageComparison;

namespace ImageAnalyzer
{
    public class ImageService : IImageService
    {
        #region [  fields  ]

        private readonly ArrayComparer<byte> _arrayComparer = new ArrayComparer<byte>();

        #endregion [  fields  ]



        #region [  public methods  ]

        public ImageData CreateImageData(string imagePath, string imageName, string source = default)
        {
            var image = Image.FromFile(imagePath);
            var grayScale16X16 = image.GetGrayScaleValues();
            image.Dispose();
            return new ImageData() { GrayScale16X16 = grayScale16X16, Source = source };
        }

        public int CompareImageDataGrayScales(ImageData imgDataA, ImageData imgDataB)
        {
            return _arrayComparer.Compare(imgDataA.GrayScale16X16, imgDataB.GrayScale16X16);
        }

        public int NumberOfImagesInCollection(IEnumerable<ImageData> imageDataList)
        {
            return imageDataList.Count(img => img.Status == ImageStatus.InCollection);
        }

        #endregion [  public methods  ]
    }
}