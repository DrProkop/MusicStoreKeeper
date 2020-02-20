using System.Collections.Generic;
using System.IO;

namespace Common
{
    public class DummySimpleFileInfo : ISimpleFileInfo
    {
        public FileInfo Info { get; }
        public SfiType Type => SfiType.Unknown;
        public string Name => string.Empty;
        public string Extension => string.Empty;
        public string Path => string.Empty;
        public List<ISimpleFileInfo> Children { get; set; }
    }
}