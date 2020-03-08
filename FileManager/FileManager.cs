using Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace FileManager
{
    public class FileManager : IFileManager
    {
        private ILogger _logger;
        
        public FileManager(ILoggerManager loggerManager)
        {
            _logger = loggerManager.GetLogger(this);
            
        }

        #region [constants]

        private const long MaxFileSize = 500000000;
        public  string DefaultArtistPhotosDirectory => "artist photos";
        public string DefaultAlbumImagesDirectory => "images";
        public  string DefaultAlbumDocsDirectory => "doc";
        public  string DefaultAlbumUnknownFilesDirectory => "other";

        #endregion [constants]

        #region [  public methods  ]

        #region [  common methods  ]

        public void CreateDirectory(string path)
        {
            var dI = Directory.CreateDirectory(path);
            _logger.Information("Created directory {DirectoryName:l} at {DirectoryPath:l}.", dI.Name, dI.FullName);
        }

        public void MoveDirectory(string sourcePath, string destPath)
        {
            Directory.Move(sourcePath, destPath);
            _logger.Information("Moved directory from {DirectorySource:l} to {DirectoryDestination:l}.", sourcePath, destPath);
        }

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
            _logger.Information("Deleted directory {DirectoryName:l} at {DirectoryParent:l}.", Path.GetFileName(path), Path.GetDirectoryName(path));
        }

        #endregion [  common methods  ]

        #region [  music directories methods  ]

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
            DeleteSourceMusicDirectoryIfEmpty(mDirInfo.Path);
        }

        public void MoveFile(ISimpleFileInfo sFileInfo, string destDirectory)
        {
            var destPath = Path.Combine(destDirectory, sFileInfo.Name);
            File.Move(sFileInfo.Path, destPath);
        }

        /// <summary>
        /// Deletes source music directory 
        /// </summary>
        /// <param name="path"></param>
        public void DeleteSourceMusicDirectoryIfEmpty(string path)
        {
            var dirInfo=new DirectoryInfo(path);
            var fi = dirInfo.GetFileSystemInfos();
            if (fi.Length==0)
            {
                DeleteDirectory(path);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sourcePath"></param>
        public void DeleteMusicDirectory(string path)
        {
            throw new NotImplementedException();
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

        //TODO: Acdd  check of file quantity and directory name
        private bool IsImageDirectory(DirectoryInfo dirInfo)
        {
            var imageFiles = dirInfo.GetFiles("*.jpg");
            var totalFileQuantity = dirInfo.GetFiles().Length;
            return imageFiles.Length == totalFileQuantity;
        }

        #endregion [  public methods  ]

        #region [  private methods  ]

        //TODO: Write proper path validation.
        private void CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Value cannot be null or empty.", nameof(path));
        }

        private void ScanSingleDirectory(DirectoryInfo dirInfo, List<DirectoryInfo> musicDirectories, string fileExtension = "*.mp3")
        {
            if (dirInfo == null) throw new ArgumentNullException(nameof(dirInfo));
            if (musicDirectories == null) throw new ArgumentNullException(nameof(musicDirectories));
            if (fileExtension == null) throw new ArgumentNullException(nameof(fileExtension));
            var files = dirInfo.GetFiles(fileExtension);
            if (files.Length > 0)
            {
                musicDirectories.Add(dirInfo);
                _logger.Information("Found directory with {FileExtension} files: {PossibleDirectoryName:l}. ", fileExtension, dirInfo.FullName);
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