using System.Collections.Generic;
using System.IO;

namespace Common
{
    public class SimpleFileInfo : ISimpleFileInfo
    {
        internal SimpleFileInfo(FileInfo fInfo, SfiType type = SfiType.Unknown)
        {
            Info = fInfo;
            Type = type;
        }

        #region [  properties  ]

        public FileInfo Info { get; }

        public SfiType Type { get; }

        public string Name => Info.Name;

        public string Extension => Info.Extension.ToUpper();

        public string Path => Info.FullName;

        public List<ISimpleFileInfo> Children { get; set; } = new List<ISimpleFileInfo>();

        #endregion [  properties  ]
    }
}