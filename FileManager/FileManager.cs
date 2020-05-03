using Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;

namespace FileManager
{
    public sealed class FileManager : IFileManager
    {
        private readonly ILogger _logger;

        public FileManager(ILoggerManager loggerManager)
        {
            _logger = loggerManager.GetLogger(this);
        }

        #region [constants]

        private const long MaxFileSize = 500000000;
        public string DefaultArtistPhotosDirectory => "artist photos";
        public string DefaultAlbumImagesDirectory => "images";
        public string DefaultAlbumDocsDirectory => "doc";
        public string DefaultAlbumUnknownFilesDirectory => "other";

        #endregion [constants]

        #region [  public methods  ]

        #region [  common methods  ]

        public void CreateDirectory(string path)
        {
            var dI = Directory.CreateDirectory(path);
            _logger.Information("Created directory {DirectoryName:l} at {DirectoryPath:l}.", dI.Name, dI.FullName);
        }

        public void CopyDirectory(string sourcePath, string destPath, bool copySubDirs = true)
        {
            var sourceDi = new DirectoryInfo(sourcePath);
            if (!sourceDi.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory doesn't exist: {sourcePath}. ");
            }

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            //files
            var files = sourceDi.GetFiles();
            foreach (var file in files)
            {
                var destFilePath = Path.Combine(destPath, file.Name);
                file.CopyTo(destFilePath, false);
            }

            if (!copySubDirs)
            {
                _logger.Information("Copied files from {DirectoryCopySource:l} to {DirectoryCopyDestination:l}.", sourcePath, destPath);
                return;
            }
            //directories
            var subDirs = sourceDi.GetDirectories();
            foreach (var subDir in subDirs)
            {
                var destDirPath = Path.Combine(destPath, subDir.Name);
                CopyDirectory(subDir.FullName, destDirPath);
            }
            _logger.Information("Copied files and subdirectories from {DirectoryCopySource:l} to {DirectoryCopyDestination:l}.", sourcePath, destPath);
        }

        public void MoveDirectory(string sourcePath, string destPath, bool moveSubDirs = true)
        {
            //move with subdirectories
            if (moveSubDirs)
            {
                MoveDirectoryWithSubdirectories(sourcePath, destPath);
                return;
            }

            //move files only
            DirectoryInfo sourceDi = new DirectoryInfo(sourcePath);
            if (!sourceDi.Exists)
            {
                throw new DirectoryNotFoundException($"Directory to move doesn't exist: {sourceDi}. ");
            }
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            var files = sourceDi.GetFiles();
            foreach (var file in files)
            {
                var destFilePath = Path.Combine(destPath, file.Name);
                file.MoveTo(destFilePath);
            }
            _logger.Information("Moved files from {DirectoryMoveSource:l} to {DirectoryMoveDestination:l}.", sourcePath, destPath);
        }

        public void ClearDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            if (!di.Exists)
            {
                throw new DirectoryNotFoundException($"Directory to clear doesn't exist: {path}. ");
            }

            var deleteAttempts = 0;
            var files = di.GetFiles();
            var directories = di.GetDirectories();
            try
            {
                foreach (var file in files)
                {
                    file.Delete();
                }

                foreach (var dir in directories)
                {
                    DeleteDirectory(dir.FullName);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                ++deleteAttempts;
                if (deleteAttempts >= 5)
                {
                    throw;
                }
                ClearDirectory(path);
            }
            catch (IOException e)
            {
                ++deleteAttempts;
                if (deleteAttempts >= 5)
                {
                    throw;
                }
                ClearDirectory(path);
            }
            _logger.Information("Cleared directory {DirectoryClearName:l}.", path);
        }

        public bool TryClearDirectory(string path)
        {
            var result = false;
            try
            {
                ClearDirectory(path);
                return result = true;
            }
            catch (DirectoryNotFoundException e)
            {
                _logger.Information("Couldn't delete file or directory {DirectoryDeleteName:l}. Error: {DeletionError}", path, e);
                return result;
            }
            catch (ArgumentNullException e)
            {
                _logger.Information("Couldn't delete file or directory {DirectoryDeleteName:l}. Error: {DeletionError}", path, e);
                return result;
            }
            catch (ArgumentException e)
            {
                _logger.Information("Couldn't delete file or directory {DirectoryDeleteName:l}. Error: {DeletionError}", path, e);
                return result;
            }
            catch (SecurityException e)
            {
                _logger.Error("Couldn't delete file {DirectoryDeleteName:l}. Security exception. Error: {DeletionError}", path, e);
                return result;
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.Error("Couldn't delete file or directory {DirectoryDeleteName:l}. It might be used by another program. Error: {DeletionError}", path, e);
                return result;
            }
            catch (IOException e)
            {
                _logger.Error("Couldn't delete file or directory {DirectoryDeleteName:l}. It might be used by another program. Error: {DeletionError}", path, e);
                return result;
            }
        }

        public void DeleteDirectory(string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            var deleteAttempts = 0;
            try
            {
                Thread.Sleep(0);
                Directory.Delete(path, true);
            }
            catch (IOException e)
            {
                ++deleteAttempts;
                if (deleteAttempts >= 5)
                {
                    throw;
                }
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException e)
            {
                ++deleteAttempts;
                if (deleteAttempts >= 5)
                {
                    throw;
                }
                Directory.Delete(path, true);
            }

            _logger.Information("Deleted directory {DirectoryDeleteName:l}.", path);
        }

        public bool TryDeleteDirectory(string path)
        {
            var result = false;
            try
            {
                DeleteDirectory(path);
                return result = true;
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.Information(
                    "Couldn't delete directory {DirectoryDeleteName:l}. Unauthorized access. Error: {DeletionError}",
                    path, e);
                return result;
            }
            catch (PathTooLongException e)
            {
                _logger.Information("Couldn't delete directory {DirectoryDeleteName:l}. Error: {DeletionError}", path, e);
                return result;
            }
            catch (DirectoryNotFoundException e)
            {
                _logger.Information("Couldn't delete directory {DirectoryDeleteName:l}. Error: {DeletionError}", path, e);
                return result;
            }
            catch (ArgumentNullException e)
            {
                _logger.Information("Couldn't delete directory {DirectoryDeleteName:l}. Error: {DeletionError}", path, e);
                return result;
            }
            catch (ArgumentException e)
            {
                _logger.Information("Couldn't delete directory {DirectoryDeleteName:l}. Error: {DeletionError}", path, e);
                return result;
            }
            catch (IOException e)
            {
                _logger.Error(
                    "Couldn't delete directory {DirectoryDeleteName:l}. It might be used by another program. Error: {DeletionError}",
                    path, e);
                return result;
            }
        }

        #endregion [  common methods  ]

        #region [  music directories methods  ]

        //TODO: Refactor or delete
        /// <summary>
        /// Moves only music files and subdirectories, containing images or text files.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        public void MoveMusicDirectory(string sourcePath, string destPath)
        {
            var dirInfo = new DirectoryInfo(sourcePath);
            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();
            //add more precise file check
            if (files.Any(arg => arg.Length > MaxFileSize))
            {
                return;
            }
            var directoriesToMove = new List<DirectoryInfo>();
            foreach (var dir in directories)
            {
                if (IsImageDirectory(dir))
                {
                    directoriesToMove.Add(dir);
                }
            }
            CreateDirectory(destPath);

            foreach (var file in files)
            {
                var destFilePath = Path.Combine(destPath, file.Name);
                File.Move(file.FullName, destFilePath);
            }

            foreach (var dirToMove in directoriesToMove)
            {
                var destDirName = Path.Combine(destPath, dirToMove.Name);
                MoveDirectory(dirToMove.FullName, destDirName);
            }
        }

        public void MoveMusicDirectory(IMusicDirInfo mDirInfo, string albumStorageDir)
        {
            if (mDirInfo == null) throw new ArgumentNullException(nameof(mDirInfo));
            if (string.IsNullOrEmpty(albumStorageDir)) throw new ArgumentNullException(nameof(albumStorageDir));

            //перемещаю аудио файлы
            foreach (var trackInfo in mDirInfo.TrackList)
            {
                MoveFile(trackInfo, albumStorageDir);
            }
            //перемещаю отдельные изображения
            var imgDirPath = Path.Combine(albumStorageDir, DefaultAlbumImagesDirectory);
            CreateDirectory(imgDirPath);
            foreach (var imageFile in mDirInfo.ImageFiles)
            {
                MoveFile(imageFile, imgDirPath);
            }
            //перемещаю файлы из папок с изображениями
            foreach (var imageDirectory in mDirInfo.ImageDirectories)
            {
                foreach (var imageFile in imageDirectory.Children)
                {
                    MoveFile(imageFile, imgDirPath);
                }
            }
            //перемещаю текстовые файлы
            if (mDirInfo.TextFiles.Any())
            {
                var docDirPath = Path.Combine(albumStorageDir, DefaultAlbumDocsDirectory);
                CreateDirectory(docDirPath);
                foreach (var textFile in mDirInfo.TextFiles)
                {
                    MoveFile(textFile, docDirPath);
                }
            }
            //пермещаю неизвестные файлы
            if (mDirInfo.UnknownFiles.Any())
            {
                var unknownDirPath = Path.Combine(albumStorageDir, DefaultAlbumUnknownFilesDirectory);
                CreateDirectory(unknownDirPath);
                foreach (var unknownFile in mDirInfo.UnknownFiles)
                {
                    MoveFile(unknownFile, unknownDirPath);
                }
            }
            //удаляю исходную папку
            DeleteSourceMusicDirectory(mDirInfo.Path);
        }

        public void MoveFile(ISimpleFileInfo sFileInfo, string destDirectory)
        {
            var destPath = Path.Combine(destDirectory, sFileInfo.Name);
            File.Move(sFileInfo.Path, destPath);
        }

        /// <summary>
        /// Deletes source music directory if no files left in it. Otherwise deletes all empty subdirectories.
        /// </summary>
        /// <param name="path"></param>
        public bool DeleteSourceMusicDirectory(string path)
        {
            var musicFolderEmpty = true;
            var dirInfo = new DirectoryInfo(path);
            var subDirs = dirInfo.GetDirectories();
            foreach (var subDir in subDirs)
            {
                var fi = subDir.GetFiles("*", SearchOption.AllDirectories);
                if (fi.Any())
                {
                    musicFolderEmpty = false;
                    continue;
                }
                subDir.Delete();
            }

            if (musicFolderEmpty)
            {
                dirInfo.Delete();
                _logger.Information("Deleted source music directory {DeleteMusicDirectory}.", path);
            }
            else
            {
                _logger.Information(
                    "Couldn't delete source music directory {DeleteMusicDirectory}. Some unknown files left.", path);
            }

            return musicFolderEmpty;
        }

        public string CreateArtistStorageDirectory(string musicCollectionPath, string artistName)
        {
            if (string.IsNullOrEmpty(musicCollectionPath))
                throw new ArgumentException("Value cannot be null or empty.", nameof(musicCollectionPath));
            if (string.IsNullOrEmpty(artistName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(artistName));

            var path = Path.Combine(musicCollectionPath, artistName);
            var imgPath = Path.Combine(path, DefaultArtistPhotosDirectory);
            CreateDirectory(imgPath);
            return path;
        }

        public string CreateAlbumStorageDirectory(string artistDirectoryPath, string albumDirectoryName)
        {
            if (string.IsNullOrEmpty(artistDirectoryPath))
                throw new ArgumentException("Value cannot be null or empty.", nameof(artistDirectoryPath));
            if (string.IsNullOrEmpty(albumDirectoryName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(albumDirectoryName));
            var path = Path.Combine(artistDirectoryPath, albumDirectoryName);
            CreateDirectory(path);
            return path;
        }

        #endregion [  music directories methods  ]

        //TODO: Add possibility to pass an array of certain file types.
        public List<DirectoryInfo> ScanDirectory(string path, string fileExtension = "*.mp3")
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var musicDirectories = new List<DirectoryInfo>();
            try
            {
                var coreDirectory = new DirectoryInfo(path);
                ScanSingleDirectory(coreDirectory, musicDirectories, fileExtension);
                return musicDirectories;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while scanning directory: {ScanDirectory}", path);
                return musicDirectories;
            }
        }

        #endregion [  public methods  ]

        #region [  private methods  ]

        private void MoveDirectoryWithSubdirectories(string sourcePath, string destPath)
        {
            Directory.Move(sourcePath, destPath);
            _logger.Information("Moved directory with subdirectories from {DirectoryMoveSource:l} to {DirectoryMoveDestination:l}.", sourcePath, destPath);
        }

        //TODO: Add  check of file quantity and directory name
        private bool IsImageDirectory(DirectoryInfo dirInfo)
        {
            var imageFiles = dirInfo.GetFiles("*.jpg");
            var totalFileQuantity = dirInfo.GetFiles().Length;
            return imageFiles.Length == totalFileQuantity;
        }

        //TODO: Write proper path validation.
        private void CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Value cannot be null or empty.", nameof(path));
        }

        private void ScanSingleDirectory(DirectoryInfo dirInfo, in List<DirectoryInfo> musicDirectories, string fileExtension = "*.mp3")
        {
            if (musicDirectories == null) throw new ArgumentNullException(nameof(musicDirectories));
            if (fileExtension == null) throw new ArgumentNullException(nameof(fileExtension));
            var files = dirInfo.GetFiles(fileExtension);
            if (files.Any())
            {
                musicDirectories.Add(dirInfo);
                _logger.Information("Found music directory with {MusicFileExtension} files: {PossibleMusicDirectoryName:l}. ", fileExtension, dirInfo.FullName);
            }

            var dirs = dirInfo.GetDirectories();
            foreach (var dir in dirs)
            {
                ScanSingleDirectory(dir, musicDirectories, fileExtension);
            }
        }

        #endregion [  private methods  ]
    }
}