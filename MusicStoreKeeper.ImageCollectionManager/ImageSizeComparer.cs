using System;
using System.Collections.Generic;
using System.Drawing;

namespace MusicStoreKeeper.ImageCollectionManager
{
    /// <summary>
    /// Compares two images by their area.
    /// </summary>
    internal class ImageSizeComparer : IComparer<Image>
    {
        /// <summary>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
        public int Compare(Image x, Image y)
        {
            var xArea = x.Height * x.Width;
            var yArea = y.Height * y.Width;
            if (xArea > yArea)
            {
                return +1;
            }

            if (xArea < yArea)
            {
                return -1;
            }

            return 0;
        }

        internal Image GetHigherResolutionImage(Image imageA, Image imageB)
        {
            var comparisonResult = Compare(imageA, imageB);
            if (comparisonResult > 1)
            {
                return imageA;
            }

            if (comparisonResult < 1)
            {
                return imageB;
            }

            return imageA;
        }

        internal Tuple<string, Image> GetHigherResolutionImage(Tuple<string, Image> imageTupleA,
            Tuple<string, Image> imageTupleB)
        {
            var comparisonResult = Compare(imageTupleA.Item2, imageTupleB.Item2);
            if (comparisonResult > 1)
            {
                return imageTupleA;
            }

            if (comparisonResult < 1)
            {
                return imageTupleB;
            }

            return imageTupleA;
        }
    }
}