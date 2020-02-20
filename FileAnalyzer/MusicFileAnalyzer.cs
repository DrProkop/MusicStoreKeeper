using System;
using System.IO;
using Common;
using File = TagLib.File;

namespace FileAnalyzer
{
    /// <summary>
    /// Serves for getting information about artist name, album titles, track names etc. using music files tags.
    /// </summary>
    public class MusicFileAnalyzer : IMusicFileAnalyzer
    {
        public MusicFileAnalyzer(string fileExtension="*.mp3")
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

        public IBasicTrackInfo GetBasicAlbumInfoFromDirectory(DirectoryInfo dirInfo)
        {
            var bai = new BasicTrackInfo();
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

        public IBasicTrackInfo GetBasicAlbumInfoFromDirectory(string path)
        {
            var di = GetDirInfo(path);
            return GetBasicAlbumInfoFromDirectory(di);
        }

        //TODO: Add check for file type
        public IBasicTrackInfo GetBasicAlbumInfoFromAudioFile(FileInfo fileInfo)
        {
            var bai = new BasicTrackInfo();
            if (!FileExtension.Trim('*').Equals(fileInfo.Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                return bai;
            }
            using (var file = File.Create(fileInfo.FullName))
            {
                bai.ArtistName = file.Tag.FirstPerformer;
                bai.AlbumTitle = file.Tag.Album;
                bai.TrackName = file.Tag.Title;
                bai.Year = file.Tag.Year;
                return bai;
            }
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