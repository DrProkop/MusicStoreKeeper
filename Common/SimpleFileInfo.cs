using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    //TODO: Implement INotifyPropertyChanged
    public interface ISimpleFileInfo
    {
        string Icon { get; }
        FileInfo Info { get; }
        string Name { get; }
        string Extension { get; }
        bool IsDirectory { get; }
        bool IsImage { get; }
        bool IsTextDocument { get; }
        bool IsAudioFile { get; }
        ObservableCollection<ISimpleFileInfo> Children { get; set; }
    }

    public class SimpleFileInfo : ISimpleFileInfo
    {
        //TODO: Move file type definition to separate class
        private static readonly string[] ImageExtensions = {".JPG", ".BMP", ".PNG", ".GIF"};
        private static readonly string[] TextExtensions = { ".TXT", ".LOG", ".MD"};
        private static readonly string[] AudioExtensions = { ".MP3", ".FLAC" };
        private readonly FileInfo _fInfo;


        public SimpleFileInfo(FileInfo fInfo)
        {
            _fInfo = fInfo;
        }

        #region [  properties  ]

        
        public FileInfo Info => _fInfo;

        public string Name => _fInfo.Name;

        public string Extension => _fInfo.Extension.ToUpper();

        public bool IsDirectory => _fInfo.Attributes == FileAttributes.Directory;

        public bool IsImage=>ImageExtensions.Contains(Extension);
        
        public bool IsTextDocument=>TextExtensions.Contains(Extension);

        public bool IsAudioFile => AudioExtensions.Contains(Extension);
    

        public string Icon { get; }

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
        public string Extension => string.Empty;
        public bool IsDirectory => false;
        public bool IsImage => false;
        public bool IsTextDocument => false;
        public bool IsAudioFile => false;
        public string Icon { get; }
        public ObservableCollection<ISimpleFileInfo> Children { get; set; }
    }
}
