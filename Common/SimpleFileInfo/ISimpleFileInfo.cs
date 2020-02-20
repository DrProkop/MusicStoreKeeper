using System.Collections.Generic;
using System.IO;

namespace Common
{
    public interface ISimpleFileInfo
    {
        FileInfo Info { get; }
        SfiType Type { get; }
        string Name { get; }
        string Extension { get; }
        string Path { get; }
        List<ISimpleFileInfo> Children { get; set; }
    }
}