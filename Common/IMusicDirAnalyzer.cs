using Common;

namespace Common
{
    public interface IMusicDirAnalyzer
    {
        IMusicDirInfo AnalyzeMusicDirectory(ISimpleFileInfo dirSFi);
    }
}