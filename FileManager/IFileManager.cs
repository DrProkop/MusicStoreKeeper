using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public interface IFileManager
    {
        void CreateDirectory(string path);
        void MoveDirectory(string sourcePath, string destPath);
        void DeleteDirectory(string path);
        List<DirectoryInfo> ScanDirectory(string path, string fileExtension);
    }
}
