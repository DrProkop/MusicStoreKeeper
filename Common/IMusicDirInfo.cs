using System.Collections.Generic;

namespace Common
{
    public interface IMusicDirInfo
    {
        string Path { get; set; }
        List<IBasicTrackInfo> TrackInfos { get; set; }
        List<ISimpleFileInfo> TrackList { get; set; }
        int MusicFilesInDirectory { get; }
        List<ISimpleFileInfo> ImageFiles { get; set; }
        List<ISimpleFileInfo> TextFiles { get; set; }
        List<ISimpleFileInfo> UnknownFiles { get; set; }
        List<ISimpleFileInfo> ImageDirectories { get; set; }
        List<ISimpleFileInfo> SubDirectories { get; set; }
        bool CanBeMovedAutomatically();
    }
}