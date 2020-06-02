using System.Collections.Generic;
using System.Drawing;
using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.ImageCollectionManager
{
    /// <summary>
    /// Defines methods for identifying duplicate images.
    /// </summary>
    public interface IImageDuplicateFinder
    {
        /// <summary>
        /// Returns a list of duplicate image paths.
        /// </summary>
        /// <param name="imagePaths">List of image paths to compare</param>
        /// <param name="imageComparer">Comparer for selecting a better image from duplicates.</param>
        /// <param name="threshold">How big a difference in a pair of pixels (out of 255) will be ignored.</param>
        /// <param name="imageDifferenceLimit">Max difference between image gray scales.</param>
        /// <returns>List of duplicate image paths.</returns>
        IEnumerable<string> GetDuplicateImagePaths(IEnumerable<string> imagePaths, IComparer<Image> imageComparer, byte threshold = 3, float imageDifferenceLimit = 0.1f);
    }
}