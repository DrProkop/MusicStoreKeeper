using System.Collections.Generic;
using System.IO;
using MusicStoreKeeper.Model;

namespace Common
{
    public interface IImageService
    {
        int CompareImageDataGrayScales(ImageData imgDataA, ImageData imgDataB);
        int NumberOfImagesInCollection(IEnumerable<ImageData> imageDataList);
        ImageData CreateImageData(string imagePath, string imageName, string source = default);
        IEnumerable<string> GetDuplicateImagePaths(IEnumerable<FileInfo> imageFileInfos, float imageDifferenceLimit = default);
    }
}