using System.IO;
using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class TextFilePreviewVm : FilePreviewVmBase
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
}