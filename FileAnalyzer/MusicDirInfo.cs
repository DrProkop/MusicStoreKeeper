using Common;
using System.Collections.Generic;
using System.Linq;

namespace FileAnalyzer
{
    public class MusicDirInfo : IMusicDirInfo
    {
        #region [  properties  ]

        public string Path { get; set; }

        public List<IBasicTrackInfo> TrackInfos { get; set; } = new List<IBasicTrackInfo>();

        public List<ISimpleFileInfo> TrackList { get; set; } = new List<ISimpleFileInfo>();

        public int MusicFilesInDirectory => TrackInfos.Count;

        public List<ISimpleFileInfo> ImageFiles { get; set; } = new List<ISimpleFileInfo>();

        public List<ISimpleFileInfo> TextFiles { get; set; } = new List<ISimpleFileInfo>();

        public List<ISimpleFileInfo> UnknownFiles { get; set; } = new List<ISimpleFileInfo>();

        public List<ISimpleFileInfo> ImageDirectories { get; set; } = new List<ISimpleFileInfo>();

        public List<ISimpleFileInfo> SubDirectories { get; set; } = new List<ISimpleFileInfo>();

        #endregion [  properties  ]

        #region [  public methods  ]

        public bool CanBeMovedAutomatically()
        {
            return !SubDirectories.Any() && UnknownFiles.Count <= 3;
        }

        #endregion [  public methods  ]
    }
}