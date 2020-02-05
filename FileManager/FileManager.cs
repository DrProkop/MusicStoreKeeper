using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Common;
using Serilog;

namespace FileManager
{
    public class FileManager:IFileManager
    {
        private ILogger _logger;
        public string DefaultMusicDirectory { get; }
        public FileManager(ILoggerManager loggerManager)
        {
            _logger = loggerManager.GetLogger(this);
            DefaultMusicDirectory = ConfigurationManager.AppSettings.Get("MusicStorage");
        }

        #region [constants]

        private const long MaxFileSize = 500000000;

        #endregion

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

        #endregion

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
            if (files.Any(arg=>arg.Length> MaxFileSize))
            {
                return;
            }
            var directoriesToMove=new List<DirectoryInfo>();
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
                var destDirName= Path.Combine(destPath, dirToMove.Name);
                MoveDirectory(dirToMove.FullName, destDirName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        public void DeleteMusicDirectory(string path)
        {

        }

        #endregion


        //TODO: Add possibility to pass an array of certain file types.
        public List<DirectoryInfo> ScanDirectory(string path, string fileExtension="*.mp3")
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
                _logger.Error(e,"Error while scanning directory: {ScanDirectory}", path);
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

        #endregion

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
            if (files.Length>0)
            {
                musicDirectories.Add(dirInfo);
                _logger.Information("Found directory with {FileExtension} files: {PossibleDirectoryName:l}. ", fileExtension, dirInfo.FullName);
            }

            var dirs=dirInfo.GetDirectories();
            foreach (var dir in dirs)
            {
                ScanSingleDirectory(dir,musicDirectories,fileExtension);
            }
        }

        #endregion
    }
}
