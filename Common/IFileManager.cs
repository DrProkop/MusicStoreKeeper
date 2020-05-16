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

        //general file methods

        void CopyFile(string sourceFilePath, string targetDirectory, string newFileName = default);

        void CopyFile(ISimpleFileInfo sourceFi, string targetDirectory, string newFileName = default);

        void CopyFileWithAutomaticRenaming(string sourceFilePath, string targetDirectory);

        void CopyFileWithAutomaticRenaming(ISimpleFileInfo sourceFi, string targetDirectory);

        void MoveFile(string sourceFilePath, string targetDirectory, string newFileName = default);

        void MoveFile(ISimpleFileInfo sourceFi, string targetDirectory, string newFileName = default);

        void MoveFileWithAutomaticRenaming(string sourceFilePath, string targetDirectory);

        void MoveFileWithAutomaticRenaming(ISimpleFileInfo sourceFi, string targetDirectory);

        string IncrementFileName(string fileName);

        string GenerateUniqueName(ICollection<string> fileNames, string fileNameToCheck);

        //image directories methods

        List<FileInfo> GetImagesFileInfosFromDirectory(string directoryPath);

        List<string> GetImageNamesFromDirectory(string directoryPath);

        //void MoveImage(ISimpleFileInfo simpleFi, string targetDirectoryPath, bool deleteDuplicates = false);

        //void MoveImage(string imagePath, string targetDirectoryPath, bool deleteDuplicates = false);

        //music directories methods

        List<DirectoryInfo> ScanDirectory(string path, string fileExtension);

        bool MoveMusicDirectory(IMusicDirInfo mDirInfo, string albumStorageDir);

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