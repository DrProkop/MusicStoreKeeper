using System;
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

        void DeleteFile(string path);

        void DeleteFile(ISimpleFileInfo simpleFi);

        //file name operations

        Tuple<string, int> IncrementFileName(string fileName);

        Tuple<string, int> GenerateUniqueName(ICollection<string> fileNames, string fileNameToCheck);

        Tuple<string, int> GenerateNameForDownloadedImage(List<string> imageNames, string imageNamePattern, int number);

        int GetNumberAtTheEndOfAFileName(string fileName, out int numberIndex);

        int GetNumberAtTheEndOfAString(string fileName, out int numberIndex);

        List<string> GetFileNamesFromDirectory(string directoryPath);

        List<FileInfo> GetImageFileInfosFromDirectory(string directoryPath);

        List<string> GetImageNamesFromDirectory(string directoryPath);

        List<ISimpleFileInfo> GetImageSimpleFileInfosFromDirectory(string directoryPath);
        //music directories methods

        List<DirectoryInfo> ScanDirectory(string path, string fileExtension);

        bool CopyMusicDirectory(IMusicDirInfo mDirInfo, string albumStorageDir);

        string CreateArtistStorageDirectory(string musicCollectionPath, string artistName);

        string CreateAlbumStorageDirectory(string artistDirectoryPath, string albumDirectoryName);

        bool DeleteSourceMusicDirectoryFiles(IMusicDirInfo mDirInfo);

        bool DeleteSourceMusicDirectory(string path);

        //default directory names
        string DefaultArtistPhotosDirectory { get; }
        string DefaultAlbumImagesDirectory { get; }
        string DefaultAlbumDocsDirectory { get; }
        string DefaultAlbumUnknownFilesDirectory { get; }
    }
}