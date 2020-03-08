using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Common
{
    public interface IFileManager
    {
        void CreateDirectory(string path);

        void MoveDirectory(string sourcePath, string destPath);

        void DeleteDirectory(string path);

        List<DirectoryInfo> ScanDirectory(string path, string fileExtension);

        //music directories
        void MoveMusicDirectory(string sourcePath, string destPath);

        void MoveMusicDirectory(IMusicDirInfo mDirInfo, string albumStorageDir);

        string CreateArtistStorageDirectory(string musicCollectionPath, string artistName);

        string CreateAlbumStorageDirectory(string artistDirectoryPath, string albumDirectoryName);

        //default directory names
        string DefaultArtistPhotosDirectory { get; }
        string DefaultAlbumImagesDirectory { get; }
        string DefaultAlbumDocsDirectory { get; }
        string DefaultAlbumUnknownFilesDirectory { get; }
    }
}