using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media;

namespace Common
{
    public interface IPreviewVm
    {
        string FileName { get; }
    }



    public abstract class PreviewVmBase : NotifyPropertyChangedBase, IPreviewVm
    {
        protected PreviewVmBase(ISimpleFileInfo fileInfo)
        {
            FileName = fileInfo.Name;
        }

        #region [  properties  ]

        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                if (_fileName == value) return;
                _fileName = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }

    public class TextFilePreviewVm : PreviewVmBase
    {
        public TextFilePreviewVm(ISimpleFileInfo fileInfo) : base(fileInfo)
        {
            SetPreviewText(fileInfo);
        }

        private  string _fileText;

        public string FileText
        {
            get => _fileText;
            set { _fileText = value;OnPropertyChanged(); }
        }

        private void  SetPreviewText(ISimpleFileInfo fileInfo)
        {
            const int maxTextSize = 1000;

            using (var sr = new StreamReader(fileInfo.Info.FullName))
            {
                if (fileInfo.Info.Length > maxTextSize)
                {
                    FileText = "File too big to preview...";
                }
                FileText =  sr.ReadToEnd();
            }
        }
    }

    public class AudioFilePreviewVm : PreviewVmBase
    {
        public AudioFilePreviewVm(ISimpleFileInfo fileInfo, IBasicAlbumInfo trackInfo) : base(fileInfo)
        {
            ArtistName = trackInfo.ArtistName;
            AlbumTitle = trackInfo.AlbumTitle;
            Year = trackInfo.Year;
            TrackName = trackInfo.TrackName;
        }

        #region [  properties  ]

        private string _artistName;

        public string ArtistName
        {
            get => _artistName;
            set
            {
                if (_artistName == value) return;
                _artistName = value;
                OnPropertyChanged();
            }
        }

        private string _albumTitle;

        public string AlbumTitle
        {
            get => _albumTitle;
            set
            {
                if (_albumTitle == value) return;
                _albumTitle = value;
                OnPropertyChanged();
            }
        }

        private string _trackName;

        public string TrackName
        {
            get => _trackName;
            set
            {
                if (_trackName == value) return;
                _trackName = value;
                OnPropertyChanged();
            }
        }

        private uint _year;

        public uint Year
        {
            get => _year;
            set
            {
                if (_year == value) return;
                _year = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }

    public class ImagePreviewVm : PreviewVmBase
    {
        public ImagePreviewVm(ISimpleFileInfo fileInfo, ImageSource imageSource) : base(fileInfo)
        {
            Image = imageSource;
        }

        #region [  properties  ]

        private ImageSource _image;

        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]
    }


}