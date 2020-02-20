using System.IO;

namespace Common
{
    public interface IMusicFileAnalyzer
    {
        string GetArtistNameFromFileTags(string path);

        string GetArtistNameFromDirectory(string path);

        string GetArtistNameFromDirectory(DirectoryInfo dirInfo);

        string GetAlbumTitleFromFileTags(string path);

        string GetAlbumTitleFromDirectory(string path);

        string GetAlbumTitleFromDirectory(DirectoryInfo dirInfo);

        string GetTrackNameFromFileTags(string path);

        IBasicTrackInfo GetBasicAlbumInfoFromDirectory(DirectoryInfo dirInfo);

        IBasicTrackInfo GetBasicAlbumInfoFromDirectory(string path);

        IBasicTrackInfo GetBasicAlbumInfoFromAudioFile(FileInfo fileInfo);
    }
}