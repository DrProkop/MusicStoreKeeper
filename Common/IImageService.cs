using System.Collections.Generic;
using MusicStoreKeeper.Model;

namespace Common
{
    public interface IImageService
    {
        int CompareImageDataGrayScales(ImageData imgDataA, ImageData imgDataB);
        int NumberOfImagesInCollection(IEnumerable<ImageData> imageDataList);
        ImageData CreateImageData(string imagePath, string imageName, string source = default);
    }
}