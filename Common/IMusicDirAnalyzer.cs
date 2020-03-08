using Common;
using Discogs.Entity;

namespace Common
{
    public interface IMusicDirAnalyzer
    {
        IMusicDirInfo AnalyzeMusicDirectory(ISimpleFileInfo dirSFi);
        bool CompareAlbums(IMusicDirInfo mDirInfo, DiscogsRelease dRelease);
    }
}