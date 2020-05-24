using Common;
using MusicStoreKeeper.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using XnaFan.ImageComparison;

namespace ImageAnalyzer
{
    public class ImageService : IImageService
    {

        /// <summary>
        /// Returns number of images which have ImageStatus.InCollection.
        /// </summary>
        /// <param name="imageDataList">List of ImageData of artist or album</param>
        /// <returns></returns>
        public int GetNumberOfImagesInCollection(IEnumerable<ImageData> imageDataList)
        {
            return imageDataList.Count(img => img.Status == ImageStatus.InCollection);
        }

        /// <summary>
        /// Returns a list of lower resolution duplicate image paths.
        /// </summary>
        /// <param name="imageFileInfos">List of image file infos to compare</param>
        /// <param name="threshold">How big a difference in a pair of pixels (out of 255) will be ignored.</param>
        /// <param name="imageDifferenceLimit"></param>
        /// <returns>List of of lower resolution duplicate image paths.</returns>
        public IEnumerable<string> GetDuplicateImagePaths(IEnumerable<FileInfo> imageFileInfos, byte threshold = 3, float imageDifferenceLimit = 0.1f)
        {
            var imageSizeComparer = new ImageSizeComparer();
            var duplicateImagePathsList = new List<string>();
            // create list of yobaImages
            var imageContainers = new List<ImageContainer>();
            foreach (var imageFileInfo in imageFileInfos)
            {
                var imageContainer = new ImageContainer();
                imageContainer.ImagePath = imageFileInfo.FullName;
                imageContainer.Image = Image.FromFile(imageContainer.ImagePath);
                imageContainer.Image16X16GrayScale = imageContainer.Image.GetGrayScaleValues();
                imageContainers.Add(imageContainer);
            }
            //compare every image with others in the imageContainers list
            for (var i = 0; i < imageContainers.Count - 1; i++)
            {
                var lowResolutionDuplicateImages = new List<ImageContainer>();
                //set first image to compare as default best image
                var bestImage = imageContainers[i];
                for (var j = i + 1; j < imageContainers.Count; j++)
                {
                    //get percentage difference for images 16x16 gray scales
                    var difference = GetPercentageDifferenceFor16X16GrayScales(imageContainers[i].Image16X16GrayScale,
                        imageContainers[j].Image16X16GrayScale, threshold);
                    if (difference < imageDifferenceLimit)
                    {
                        //select image with higher resolution. the lower resolution image goes to lowResolutionDuplicateImages list
                        var result = imageSizeComparer.Compare(bestImage.Image, imageContainers[j].Image);
                        switch (result)
                        {
                            case -1:
                                lowResolutionDuplicateImages.Add(bestImage);
                                bestImage = imageContainers[j];
                                break;

                            case 0:
                                lowResolutionDuplicateImages.Add(imageContainers[j]);
                                break;

                            case 1:
                                lowResolutionDuplicateImages.Add(imageContainers[j]);
                                break;
                        }
                    }
                }
                //no duplicates were found for current imageContainers[i]
                if (lowResolutionDuplicateImages.Count == 0)
                {
                    continue;
                }
                //add paths of lower resolution duplicate images to return list
                foreach (var lowResolutionDuplicateImage in lowResolutionDuplicateImages)
                {
                    if (duplicateImagePathsList.Contains(lowResolutionDuplicateImage.ImagePath))
                    {
                        continue;
                    }
                    duplicateImagePathsList.Add(lowResolutionDuplicateImage.ImagePath);
                }
            }
            //dispose created images to avoid access exceptions
            foreach (var imageContainer in imageContainers)
            {
                imageContainer.Image.Dispose();
            }
            return duplicateImagePathsList;
        }

        private static float GetPercentageDifferenceFor16X16GrayScales(byte[,] firstImageGrayScale, byte[,] secondImageGrayScale, byte threshold = 3)
        {
            var differences = new byte[16, 16];

            for (var y = 0; y < 16; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    differences[x, y] = (byte)Math.Abs(firstImageGrayScale[x, y] - secondImageGrayScale[x, y]);
                }
            }

            var diffPixels = 0;

            foreach (byte b in differences)
            {
                if (b > threshold) { diffPixels++; }
            }

            return diffPixels / 256f;
        }
    }
}