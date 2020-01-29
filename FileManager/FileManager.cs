using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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



        #region [  public methods  ]

        public void CreateDirectory(string path)
        {
            var dI=Directory.CreateDirectory(path);
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

        //TODO: Add possibility to pass an array of certain file types.
        public List<DirectoryInfo> ScanDirectory(string path, string fileExtension="*.mp3")
        {
            var dirInfos=new DirectoryInfo(path).EnumerateDirectories("*", SearchOption.AllDirectories);
            var musicDirectories=new List<DirectoryInfo>();
            foreach (var di in dirInfos)
            {

                var files = di.GetFiles(fileExtension);
                if (files.Length==0)
                {
                    continue;
                }
                musicDirectories.Add(di);
                _logger.Information("Found directory with {FileExtension} files: {PossibleDirectoryName:l}. ", fileExtension, di.FullName);
                
            }

            return musicDirectories;
        }

        #endregion

        #region [  private methods  ]
        //TODO: Write proper path validation.
        private void CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            
        }

        #endregion
    }
}
