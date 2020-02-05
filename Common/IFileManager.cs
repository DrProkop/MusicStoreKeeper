using System.Collections.Generic;
using System.IO;

namespace Common
{
    public interface IFileManager
    {
        void CreateDirectory(string path);
        void MoveDirectory(string sourcePath, string destPath);
        void DeleteDirectory(string path);
        List<DirectoryInfo> ScanDirectory(string path, string fileExtension);
        //music directories related methods
        void MoveMusicDirectory(string sourcePath, string destPath);
    }
}
