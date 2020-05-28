using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using Common;
using Discogs;
using Discogs.Entity;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using XnaFan.ImageComparison;

namespace MusicStoreKeeper.ImageCollectionManager
{
    public class ImageCollectionManager : LoggingBase, IImageCollectionManager
    {
        public ImageCollectionManager(
            DiscogsClient client,
            IFileManager fileManager,
            IRepository repository,
            ILoggerManager manager) : base(manager)
        {
            _discogsClient = client;
            _fileManager = fileManager;
            _repository = repository;
            CreateTempImageDirectory();
        }

        #region [  fields  ]

        private readonly DiscogsClient _discogsClient;
        private readonly IFileManager _fileManager;
        private readonly IRepository _repository;
        private string _tempImageDirectoryPath;

        #endregion [  fields  ]

        #region [  public methods  ]

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
        /// Check target directory for files with no appropriate info in collection db. Add new files to db.
        /// </summary>
        /// <param name="imageDataCollection">List of image data from db.</param>
        /// <param name="ownerId">Id of imageDataCollection owner.</param>
        /// <param name="directoryPath">Directory to check.</param>
        /// <returns>True whether any changes in target directory were found.</returns>
        public bool RefreshImageDirectory(ICollection<ImageData> imageDataCollection, int ownerId, string directoryPath)
        {
            var imageNamesInTargetDirectory = _fileManager.GetImageNamesFromDirectory(directoryPath);
            var imageNamesInCollection = Enumerable.ToList<string>(imageDataCollection
                    .Where(img => img.Status == ImageStatus.InCollection)
                    .Select(img => img.Name));
            //find difference in image names in db and target image directory
            //TODO: write custom method for collections comparison
            var commonNames = Enumerable.Intersect(imageNamesInTargetDirectory, imageNamesInCollection).ToList();
            var difference1 = Enumerable.Except(imageNamesInTargetDirectory, commonNames).ToList();
            var difference2= imageNamesInCollection.Except(commonNames).ToList();
            var difference = difference1.Concat(difference2).ToList();
            //image directory hasn't changed changed
            if (difference.Count == 0) return false;
            //something changed in image directory
            foreach (var diff in difference)
            {
                //image added to image directory
                if (imageNamesInTargetDirectory.Contains(diff))
                {
                    //create new imageDataCollection
                    var newImageData = new ImageData { Name = diff };
                    //add new image data to db
                    _repository.AddImageData(newImageData, ownerId);
                    //add new image to collection
                    imageDataCollection.Add(newImageData);

                    continue;
                }
                //image removed from image directory or renamed
                var imageDataToRemove = imageDataCollection.First(img => img.Name.Equals(diff));
                DeleteImageDataFromCollectionAndDb(imageDataCollection, imageDataToRemove);
                
            }

            return true;
            ////check number of images in collection and in target directory
            ////new images were added
            //if (imagesInTargetDirectory.Count > imagesInCollection)
            //{
            //    imageQuantityChanged = true;
            //    var imagesToAdd = imagesInTargetDirectory.Count - imagesInCollection;
            //    //find new images
            //    foreach (var imageInTargetDirectory in imagesInTargetDirectory)
            //    {
            //        //get name of each image in target directory
            //        var imgName = imageInTargetDirectory.Name;
            //        //check for image name matches in imageDataCollection
            //        if (!imageDataCollection.Any(data => data.Name.Equals(imgName)))
            //        {
            //            //create new imageDataCollection
            //            var newImageData = new ImageData { Name = imgName, Status = ImageStatus.InCollection };
            //            //add new images to collection
            //            imageDataCollection.Add(newImageData);
            //            //add new image data to db
            //            _repository.AddImageData(newImageData, ownerId);
            //        }
            //    }
            //    _repository.Save();
            //}
            ////some images were deleted
            //if (imagesInTargetDirectory.Count < imagesInCollection)
            //{
            //}
            //return imageQuantityChanged;
        }

        /// <summary>
        /// Deletes lower resolution copies of images from directory.
        /// </summary>
        /// <param name="imageDataCollection">List of image data from db.</param>
        /// <param name="ownerId">Id of imageDataCollection owner.</param>
        /// <param name="directoryPath">Directory to check.</param>
        public void DeleteDuplicateImagesFromDirectoryAndDb(ICollection<ImageData> imageDataCollection, int ownerId, string directoryPath)
        {
            //search for duplicates in album images directory.
            var imagePaths = Enumerable.ToList<string>(_fileManager.GetImageSimpleFileInfosFromDirectory(directoryPath).Select(sfi=>sfi.Path));
            var duplicateImagePaths = GetDuplicateImagePaths(imagePaths);
            //delete lower resolution image duplicates
            foreach (var duplicateImagePath in duplicateImagePaths)
            {
                var duplicateImageName = Path.GetFileName(duplicateImagePath);
                //delete file from image directory
                //TODO: Add TryDeleteFile to IFileManager
                _fileManager.DeleteFile(duplicateImagePath);
                //delete imageDataCollection from image data collection
                var imageDataToRemove = imageDataCollection.FirstOrDefault(img => img.Name.Equals(duplicateImageName));
                //skip images with no records in collection
                if (imageDataToRemove==null)
                {
                    continue;
                }
                //delete imageDataCollection from db and collection
                DeleteImageDataFromCollectionAndDb(imageDataCollection, imageDataToRemove);
            }
        }

        /// <summary>
        /// Refreshes content of image directory and db. Deletes duplicate images if necessary.
        /// Performs IImageCollectionManager.RefreshImageDirectory and IImageCollectionManager.DeleteDuplicateImagesFromDirectoryAndDb.
        /// </summary>
        /// <param name="imageData">List of image data from db.</param>
        /// <param name="ownerId">Id of imageDataCollection owner.</param>
        /// <param name="directoryPath">Directory to check.</param>
        public void CleanupImageDirectory(ICollection<ImageData> imageData, int ownerId, string directoryPath)
        {
            var directoryStateChanged = RefreshImageDirectory(imageData, ownerId, directoryPath);
            if (directoryStateChanged)
            {
                DeleteDuplicateImagesFromDirectoryAndDb(imageData, ownerId, directoryPath);
            }
        }

        /// <summary>
        /// Downloads artist or album images from Discogs.
        /// </summary>
        /// <param name="discogsImages">Array of DiscogsImage of artist or album.</param>
        /// <param name="imageNamePattern">Name of album or artist generally.</param>
        /// <param name="imageData">List of image data from db.</param>
        /// <param name="ownerId">Id of imageDataCollection owner.</param>
        /// <param name="targetDirPath">Directory path to store images.</param>
        public void DownloadArtistOrAlbumImages(DiscogsImage[] discogsImages, string imageNamePattern, ICollection<ImageData> imageData, int ownerId, string targetDirPath)
        {
            var number = 1;
            //list of image names in target directory
            var imageNamesInTargetDirectory = _fileManager.GetFileNamesFromDirectory(targetDirPath);
            try
            {
                foreach (var discogsImage in discogsImages)
                {
                    //check imageDataCollection for exact image copies with the same not null source
                    if (!String.IsNullOrEmpty(discogsImage.uri) && imageData.Any(arg => arg.Source.Equals(discogsImage.uri)))
                    {
                        continue;
                    }
                    //generate name for new image
                    var newImageNameAndNumber = _fileManager.GenerateNameForDownloadedImage(imageNamesInTargetDirectory, imageNamePattern, number);
                    var newImageName = newImageNameAndNumber.Item1;
                    imageNamesInTargetDirectory.Add(newImageName);
                    number = newImageNameAndNumber.Item2 + 1;
                    //save image to temp directory
                    _discogsClient.SaveImage(discogsImage, _tempImageDirectoryPath, newImageName);
                    //create new ImageData (important to add url to source property)
                    var tempImagePath = Path.Combine(_tempImageDirectoryPath, newImageName);
                    //move image to target directory
                    _fileManager.MoveFile(tempImagePath, targetDirPath);
                    //add new imageDataCollection to imageDataCollection
                    var downloadedImageData = new ImageData() { Name = newImageName, Source = discogsImage.uri, Status = ImageStatus.InCollection };
                    imageData.Add(downloadedImageData);
                }
                //save image data for downloaded discogs images
                _repository.AddImagesData(imageData, ownerId);
            }
            //TODO: Add exception handling to ImageCollectionManager
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _fileManager.ClearDirectory(_tempImageDirectoryPath);
            }
        }

        /// <summary>
        /// Returns a list of lower resolution duplicate image paths.
        /// </summary>
        /// <param name="imagePaths">List of image paths to compare</param>
        /// <param name="threshold">How big a difference in a pair of pixels (out of 255) will be ignored.</param>
        /// <param name="imageDifferenceLimit"></param>
        /// <returns>List of of lower resolution duplicate image paths.</returns>
        public IEnumerable<string> GetDuplicateImagePaths(IEnumerable<string> imagePaths, byte threshold = 3, float imageDifferenceLimit = 0.1f)
        {
            var imageSizeComparer = new ImageSizeComparer();
            var duplicateImagePathsList = new List<string>();
            // create list of yobaImages
            var imageContainers = new List<ImageContainer>();
            foreach (var imagePath in imagePaths)
            {
                var imageContainer = new ImageContainer();
                imageContainer.ImagePath = imagePath;
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

        #endregion [  public methods  ]

        #region [  private methods  ]

        public static float GetPercentageDifferenceFor16X16GrayScales(byte[,] firstImageGrayScale, byte[,] secondImageGrayScale, byte threshold = 3)
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

        private void CreateTempImageDirectory()
        {
            _tempImageDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempImage");
            Directory.CreateDirectory(_tempImageDirectoryPath);
        }

        /// <summary>
        /// Deletes imageDataCollection from db and from collection, if imageDataCollection.Source is string.Empty.
        /// </summary>
        /// <param name="imageDataCollection"></param>
        /// <param name="imageDataToRemove"></param>
        private void DeleteImageDataFromCollectionAndDb(ICollection<ImageData> imageDataCollection, ImageData imageDataToRemove)
        {
            _repository.DeleteImageData(imageDataToRemove);
            //delete imageDataCollection from imageDataCollection collection
            if (String.IsNullOrEmpty(imageDataToRemove.Source))
            {
                imageDataCollection.Remove(imageDataToRemove);
            }
        }

        #endregion [  private methods  ]
    }
}