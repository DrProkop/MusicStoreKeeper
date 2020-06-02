using System;
using System.Collections.Generic;
using System.Drawing;
using XnaFan.ImageComparison;

namespace MusicStoreKeeper.ImageCollectionManager
{
    public class ImageDuplicateFinder : IImageDuplicateFinder
    {
        /// <summary>
        /// Returns a list of duplicate image paths.
        /// </summary>
        /// <param name="imagePaths">List of image paths to compare</param>
        /// <param name="imageComparer">Comparer for selecting a better image from duplicates.</param>
        /// <param name="threshold">How big a difference in a pair of pixels (out of 255) will be ignored.</param>
        /// <param name="imageDifferenceLimit">Max difference between image gray scales.</param>
        /// <returns>List of duplicate image paths.</returns>
        public IEnumerable<string> GetDuplicateImagePaths(IEnumerable<string> imagePaths, IComparer<Image> imageComparer, byte threshold = 3, float imageDifferenceLimit = 0.1f)
        {
            if (imageComparer == null) throw new ArgumentNullException(nameof(imageComparer));
            // list of duplicate images
            var duplicateImagePathsList = new List<string>();
            // list of path,grayscale tuples
            var imagePathGrayList = new List<Tuple<string, byte[,]>>();
            foreach (var imagePath in imagePaths)
            {
                var image = Image.FromFile(imagePath);
                var image16X16GrayScale = image.GetGrayScaleValues();
                imagePathGrayList.Add(new Tuple<string, byte[,]>(imagePath, image16X16GrayScale));
                image.Dispose();
            }
            Image bestImage = null;
            //compare every image with others in the imagePathGrayList
            for (var i = 0; i < imagePathGrayList.Count - 1; i++)
            {
                var inferiorDuplicateImages = new List<Tuple<string, byte[,]>>();
                //set first image to compare as default best image
                var bestImageTuple = imagePathGrayList[i];

                for (var j = i + 1; j < imagePathGrayList.Count; j++)
                {
                    //get percentage difference for images 16x16 gray scales
                    var difference = GetPercentageDifferenceFor16X16GrayScales(
                        imagePathGrayList[i].Item2,
                        imagePathGrayList[j].Item2, threshold);
                    if (difference < imageDifferenceLimit)
                    {
                        if (bestImage == null)
                        {
                            bestImage = Image.FromFile(imagePathGrayList[i].Item1);
                        }
                        using (var imageToCompare = Image.FromFile(imagePathGrayList[j].Item1))
                        {
                            //compare images. the lower resolution image goes to inferiorDuplicateImages list
                            var result = imageComparer.Compare(bestImage, imageToCompare);
                            if (result == -1)
                            {
                                inferiorDuplicateImages.Add(bestImageTuple);
                                bestImageTuple = imagePathGrayList[j];
                                bestImage.Dispose();
                                bestImage = (Image)imageToCompare.Clone();
                            }
                            else
                            {
                                inferiorDuplicateImages.Add(imagePathGrayList[j]);
                            }
                        }
                    }
                }
                //no duplicates were found for current imagePathGrayList[i]
                if (inferiorDuplicateImages.Count == 0)
                {
                    continue;
                }
                //add paths of inferior duplicate images to return list
                foreach (var inferiorDuplicateImage in inferiorDuplicateImages)
                {
                    if (duplicateImagePathsList.Contains(inferiorDuplicateImage.Item1))
                    {
                        continue;
                    }
                    duplicateImagePathsList.Add(inferiorDuplicateImage.Item1);
                }
            }
            bestImage?.Dispose();
            return duplicateImagePathsList;
        }

        /// <summary>
        /// Returns percentage difference for two provided image gray scales.
        /// </summary>
        /// <param name="firstImageGrayScale">Gray scale of first image.</param>
        /// <param name="secondImageGrayScale">Gray scale of second image.</param>
        /// <param name="threshold">How big a difference in a pair of pixels (out of 255) will be ignored.</param>
        /// <returns></returns>
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