using System.IO;

namespace Common
{
    public interface IFileAnalyzer
    {
        string GetArtistNameFromFileTags(string path);

        string GetArtistNameFromDirectory(string path);

        string GetArtistNameFromDirectory(DirectoryInfo dirInfo);

        string GetAlbumTitleFromFileTags(string path);

        string GetAlbumTitleFromDirectory(string path);

        string GetAlbumTitleFromDirectory(DirectoryInfo dirInfo);

        string GetTrackNameFromFileTags(string path);

        IBasicAlbumInfo GetBasicAlbumInfoFromDirectory(DirectoryInfo dirInfo);

        IBasicAlbumInfo GetBasicAlbumInfoFromDirectory(string path);
    }
}