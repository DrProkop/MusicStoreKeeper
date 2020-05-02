using System.Collections.Generic;
using System.IO;

namespace Common
{
    public interface IFileManager
    {
        //general methods
        void CreateDirectory(string path);

        void CopyDirectory(string sourcePath, string destPath, bool copySubDirs=true);

        void MoveDirectory(string sourcePath, string destPath, bool moveSubDirs=true);

        void DeleteDirectory(string path);

        void ClearDirectory(string path);

        List<DirectoryInfo> ScanDirectory(string path, string fileExtension);

        //methods for music directories
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