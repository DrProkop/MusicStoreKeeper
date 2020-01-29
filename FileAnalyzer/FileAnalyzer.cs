using System;
using System.IO;
using Common;
using File = TagLib.File;

namespace FileAnalyzer
{
    /// <summary>
    /// Serves for getting information about artist name, album titles, track names etc. using music files tags.
    /// </summary>
    public class FileAnalyzer : IFileAnalyzer
    {
        public FileAnalyzer(string fileExtension="*.mp3")
        {
            FileExtension = fileExtension;
        }

        #region [  fields  ]

        public readonly string FileExtension;

        #endregion [  fields  ]

        #region [  public methods  ]

        #region [  artist  ]

        public string GetArtistNameFromFileTags(string path)
        {
            if (!CheckFilePathAndExtension(path)) return string.Empty;

            using (var file = File.Create(path))
            {
                var firstArtist = file.Tag.FirstPerformer;
                return firstArtist;
            }
        }

        public string GetArtistNameFromDirectory(string path)
        {
            var di = GetDirInfo(path);
            return GetArtistNameFromDirectory(di);
        }

        public string GetArtistNameFromDirectory(DirectoryInfo dirInfo)
        {
            var fInfos = dirInfo.GetFiles(FileExtension);
            if (fInfos.Length <= 0) return string.Empty;
            return GetArtistNameFromFileTags(fInfos[0].FullName);
        }

        #endregion [  artist  ]

        #region [  album  ]

        public string GetAlbumTitleFromFileTags(string path)
        {
            if (!CheckFilePathAndExtension(path)) return string.Empty;
            using (var file = File.Create(path))
            {
                var album = file.Tag.Album;
                return album;
            }
        }

        public string GetAlbumTitleFromDirectory(string path)
        {
            var di = GetDirInfo(path);
            return GetAlbumTitleFromDirectory(di);
        }

        public string GetAlbumTitleFromDirectory(DirectoryInfo dirInfo)
        {
            var fInfos = dirInfo.GetFiles(FileExtension);
            if (fInfos.Length <= 0) return string.Empty;
            return GetAlbumTitleFromFileTags(fInfos[0].FullName);
        }

        #endregion [  album  ]

        #region [  track  ]

        public string GetTrackNameFromFileTags(string path)
        {
            if (!CheckFilePathAndExtension(path)) return string.Empty;
            using (var file = File.Create(path))
            {
                var track = file.Tag.Title;
                return track;
            }
        }

        #endregion [  track  ]

        public IBasicAlbumInfo GetBasicAlbumInfoFromDirectory(DirectoryInfo dirInfo)
        {
            var bai = new BasicAlbumInfo();
            var fInfos = dirInfo.GetFiles(FileExtension);
            if (fInfos.Length <= 0) return bai;
            using (var file = File.Create(fInfos[0].FullName))
            {
                bai.ArtistName = file.Tag.FirstPerformer;
                bai.AlbumTitle = file.Tag.Album;
                bai.TrackName = file.Tag.Title;
                bai.Year = file.Tag.Year;
                return bai;
            }
        }

        public IBasicAlbumInfo GetBasicAlbumInfoFromDirectory(string path)
        {
            var di = GetDirInfo(path);
            return GetBasicAlbumInfoFromDirectory(di);
        }

        #endregion [  public methods  ]

        #region [  private methods  ]

        private bool CheckFilePathAndExtension(string path)
        {
            if (Path.HasExtension(path))
            {
                return Path.GetExtension(path).Equals(FileExtension, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        private DirectoryInfo GetDirInfo(string path)
        {
            var dirPath = Path.GetDirectoryName(path);
            return new DirectoryInfo(dirPath);
        }

        #endregion [  private methods  ]
    }
}