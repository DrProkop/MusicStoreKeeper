using System;
using System.IO;
using System.Linq;

namespace Common
{
    public static class SimpleFileInfoFactory
    {
        private static readonly string[] ImageExtensions = { ".JPG", ".BMP", ".PNG", ".GIF" };
        private static readonly string[] TextExtensions = { ".TXT", ".LOG", ".MD", ".NFO" };
        private static readonly string[] AudioExtensions = { ".MP3", ".FLAC" };

        public static ISimpleFileInfo Create(FileInfo fileInfo)
        {
            var type = GetFileType(fileInfo);
            return new SimpleFileInfo(fileInfo, type);
        }

        public static DummySimpleFileInfo CreateDummy()
        {
            return new DummySimpleFileInfo();
        }

        private static SfiType GetFileType(FileInfo fileInfo)
        {
            var extension = fileInfo.Extension;
            if (fileInfo.Attributes == FileAttributes.Directory)
            {
                return SfiType.Directory;
            }
            if (AudioExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
            {
                return SfiType.AudioFile;
            }
            if (ImageExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
            {
                return SfiType.ImageFile;
            }
            if (TextExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase))
            {
                return SfiType.TextFile;
            }

            return SfiType.Unknown;
        }
    }
}