using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    //TODO: Implement INotifyPropertyChanged
    public interface ISimpleFileInfo
    {
        FileInfo Info { get; }
        string Name { get; }
        bool IsDirectory { get; }
        ObservableCollection<ISimpleFileInfo> Children { get; set; }
    }

    public class SimpleFileInfo : ISimpleFileInfo
    {
        private readonly FileInfo _fInfo;


        public SimpleFileInfo(FileInfo fInfo)
        {
            _fInfo = fInfo;
            
        }

        #region [  properties  ]

        public FileInfo Info => _fInfo;

        public string Name => _fInfo.Name;

        public bool IsDirectory => _fInfo.Attributes == FileAttributes.Directory;

        private ObservableCollection<ISimpleFileInfo> _children = new ObservableCollection<ISimpleFileInfo>();

        public ObservableCollection<ISimpleFileInfo> Children
        {
            get => _children;
            set { _children = value; }
        }

        #endregion


    }

    public class DummySimpleFileInfo : ISimpleFileInfo
    {
        public FileInfo Info { get; }
        public string Name => string.Empty;
        public bool IsDirectory => false;
        public ObservableCollection<ISimpleFileInfo> Children { get; set; }
    }
}
