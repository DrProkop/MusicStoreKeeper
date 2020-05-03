using System.Collections.Generic;
using System.IO;

namespace Common
{
    public interface IFileManager
    {
        //general methods
        void CreateDirectory(string path);

        void CopyDirectory(string sourcePath, string destPath, bool copySubDirs = true);

        void MoveDirectory(string sourcePath, string destPath, bool moveSubDirs = true);

        void DeleteDirectory(string path);

        bool TryDeleteDirectory(string path);

        void ClearDirectory(string path);

        bool TryClearDirectory(string path);

        List<DirectoryInfo> ScanDirectory(string path, string fileExtension);

        void MoveMusicDirectory(IMusicDirInfo mDirInfo, string albumStorageDir);

        string CreateArtistStorageDirectory(string musicCollectionPath, string artistName);

        string CreateAlbumStorageDirectory(string artistDirectoryPath, string albumDirectoryName);

        bool DeleteSourceMusicDirectory(string path);

        //default directory names
        string DefaultArtistPhotosDirectory { get; }
        string DefaultAlbumImagesDirectory { get; }
        string DefaultAlbumDocsDirectory { get; }
        string DefaultAlbumUnknownFilesDirectory { get; }
    }
}