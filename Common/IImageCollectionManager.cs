using Discogs.Entity;
using MusicStoreKeeper.Model;
using System.Collections.Generic;

namespace Common
{
    public interface IImageCollectionManager
    {
        /// <summary>
        /// Check target directory for files with no appropriate info in collection db. Add new files to db.
        /// </summary>
        /// <param name="imageDataList">List of image data from db.</param>
        /// <param name="ownerId">Id of imageDataList owner.</param>
        /// <param name="directoryPath">Directory to check.</param>
        /// <returns>True whether any changes in target directory were found.</returns>
        bool RefreshImageDirectory(ICollection<ImageData> imageDataList, int ownerId, string directoryPath);

        /// <summary>
        /// Deletes lower resolution copies of images from directory.
        /// </summary>
        /// <param name="imageData">List of image data from db.</param>
        /// <param name="ownerId">Id of imageData owner.</param>
        /// <param name="directoryPath">Directory to check.</param>
        void DeleteDuplicateImagesFromDirectoryAndDb(ICollection<ImageData> imageData, int ownerId, string directoryPath);

        /// <summary>
        /// Refreshes content of image directory and db. Deletes duplicate images if necessary.
        /// Performs IImageCollectionManager.RefreshImageDirectory and IImageCollectionManager.DeleteDuplicateImagesFromDirectoryAndDb.
        /// </summary>
        /// <param name="imageData">List of image data from db.</param>
        /// <param name="ownerId">Id of imageData owner.</param>
        /// <param name="directoryPath">Directory to check.</param>
        void CleanupImageDirectory(ICollection<ImageData> imageData, int ownerId, string directoryPath);

        /// <summary>
        /// Downloads artist or album images from Discogs.
        /// </summary>
        /// <param name="discogsImages">Array of DiscogsImage of artist or album.</param>
        /// <param name="imageNamePattern">Name of album or artist generally.</param>
        /// <param name="imageData">List of image data from db.</param>
        /// <param name="ownerId">Id of imageData owner.</param>
        /// <param name="targetDirPath">Directory path to store images.</param>
        void DownloadArtistOrAlbumImages(DiscogsImage[] discogsImages, string imageNamePattern, ICollection<ImageData> imageData, int ownerId, string targetDirPath);
    }
}