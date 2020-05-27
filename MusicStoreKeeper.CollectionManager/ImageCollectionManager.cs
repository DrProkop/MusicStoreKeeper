using Common;
using Discogs;
using Discogs.Entity;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicStoreKeeper.CollectionManager
{
    public class ImageCollectionManager : LoggingBase, IImageCollectionManager
    {
        public ImageCollectionManager(
            DiscogsClient client,
            IFileManager fileManager,
            IRepository repository,
            IImageService imageService,
            ILoggerManager manager) : base(manager)
        {
            _discogsClient = client;
            _fileManager = fileManager;
            _repository = repository;
            _imageService = imageService;

            CreateTempImageDirectory();
        }

        #region [  fields  ]

        private readonly DiscogsClient _discogsClient;
        private readonly IFileManager _fileManager;
        private readonly IRepository _repository;
        private readonly IImageService _imageService;

        private string _tempImageDirectoryPath;

        #endregion [  fields  ]

        #region [  public methods  ]

        /// <summary>
        /// Check target directory for files with no appropriate info in collection db. Add new files to db.
        /// </summary>
        /// <param name="imageDataList">List of image data from db.</param>
        /// <param name="ownerId">Id of imageData owner.</param>
        /// <param name="directoryPath">Directory to check.</param>
        /// <returns>True whether any changes in target directory were found.</returns>
        public bool RefreshImageDirectory(ICollection<ImageData> imageDataList, int ownerId, string directoryPath)
        {
            var imageNamesInTargetDirectory = _fileManager.GetImageNamesFromDirectory(directoryPath);
            var imageNamesInCollection = imageDataList
                .Where(img => img.Status == ImageStatus.InCollection)
                .Select(img => img.Name).ToList();
            //find difference in image names in db and target image directory
            //TODO: write custom method for collections comparison
            var commonNames = imageNamesInTargetDirectory.Intersect(imageNamesInCollection).ToList();
            var difference1 = imageNamesInTargetDirectory.Except(commonNames).ToList();
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
                    //create new imageData
                    var newImageData = new ImageData { Name = diff };
                    //add new image data to db
                    _repository.AddImageData(newImageData, ownerId);
                    //add new image to collection
                    imageDataList.Add(newImageData);

                    continue;
                }
                //image removed from image directory or renamed
                var imageDataToRemove = imageDataList.First(img => img.Name.Equals(diff));
                _repository.DeleteImageData(imageDataToRemove);
                if (string.IsNullOrEmpty(imageDataToRemove.Source))
                {
                    imageDataList.Remove(imageDataToRemove);
                }
                
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
            //        //check for image name matches in imageData
            //        if (!imageData.Any(data => data.Name.Equals(imgName)))
            //        {
            //            //create new imageData
            //            var newImageData = new ImageData { Name = imgName, Status = ImageStatus.InCollection };
            //            //add new images to collection
            //            imageData.Add(newImageData);
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
        /// <param name="imageData">List of image data from db.</param>
        /// <param name="ownerId">Id of imageData owner.</param>
        /// <param name="directoryPath">Directory to check.</param>
        public void DeleteDuplicateImagesFromDirectoryAndDb(ICollection<ImageData> imageData, int ownerId, string directoryPath)
        {
            //search for duplicates in album images directory.
            var imagePaths = _fileManager.GetImageSimpleFileInfosFromDirectory(directoryPath).Select(sfi=>sfi.Path);
            var duplicateImagePaths = _imageService.GetDuplicateImagePaths(imagePaths);
            //delete lower resolution image duplicates
            foreach (var duplicateImagePath in duplicateImagePaths)
            {
                var duplicateImageName = Path.GetFileName(duplicateImagePath);
                //delete file from image directory
                //TODO: Add TryDeleteFile to IFileManager
                _fileManager.DeleteFile(duplicateImagePath);
                //delete imageData from image data collection
                var imageDataToRemove = imageData.FirstOrDefault(img => img.Name.Equals(duplicateImageName));
                //skip images with no records in collection
                if (imageDataToRemove==null)
                {
                    continue;
                }
                //delete imageData from db
                _repository.DeleteImageData(imageDataToRemove);
                //delete imageData from imageData collection
                if (string.IsNullOrEmpty(imageDataToRemove.Source))
                {
                    imageData.Remove(imageDataToRemove);
                }
            }
        }

        /// <summary>
        /// Refreshes content of image directory and db. Deletes duplicate images if necessary.
        /// Performs IImageCollectionManager.RefreshImageDirectory and IImageCollectionManager.DeleteDuplicateImagesFromDirectoryAndDb.
        /// </summary>
        /// <param name="imageData">List of image data from db.</param>
        /// <param name="ownerId">Id of imageData owner.</param>
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
        /// <param name="ownerId">Id of imageData owner.</param>
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
                    //check imageData for exact image copies with the same not null source
                    if (!string.IsNullOrEmpty(discogsImage.uri) && imageData.Any(arg => arg.Source.Equals(discogsImage.uri)))
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
                    //add new imageData to imageData
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

        #endregion [  public methods  ]

        #region [  private methods  ]

        private void CreateTempImageDirectory()
        {
            _tempImageDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempImage");
            Directory.CreateDirectory(_tempImageDirectoryPath);
        }

        #endregion [  private methods  ]
    }
}