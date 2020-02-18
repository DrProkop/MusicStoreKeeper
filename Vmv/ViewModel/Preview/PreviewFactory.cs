using Common;
using MusicStoreKeeper.Model;
using System.Windows.Media;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class PreviewFactory
    {
        public PreviewFactory(IFileAnalyzer fileAnalyzer)
        {
            _fileAnalyzer = fileAnalyzer;
        }

        #region [  fields  ]

        private readonly IFileAnalyzer _fileAnalyzer;

        #endregion [  fields  ]

        #region [  public methods  ]

        public IPreviewVm CreatePreviewVm(object arg)
        {
            if (arg == null) return null;
            if (arg is ISimpleFileInfo file)
            {
                //TODO: Add enum PreviewableFiles to ISimpleFileInfo
                if (file.IsAudioFile)
                {
                    return CreateAudioFilePreviewVm(file);
                }
                if (file.IsImage)
                {
                    return CreateImageFilePreviewVm(file);
                }

                if (file.IsTextDocument)
                {
                    return CreateTextFilePreviewVm(file);
                }
            }

            if (arg is Artist artist)
            {
                return CreateArtistPreviewVm(artist);
            }

            if (arg is Album album)
            {
                return CreateAlbumPreviewVm(album);
            }

            return null;
        }

        #endregion [  public methods  ]

        #region [  private methods  ]

        private AudioFilePreviewVm CreateAudioFilePreviewVm(ISimpleFileInfo file)
        {
            var basicTrackInfo = _fileAnalyzer.GetBasicAlbumInfoFromAudioFile(file.Info);
            return new AudioFilePreviewVm(file, basicTrackInfo);
        }

        private ImageFilePreviewVm CreateImageFilePreviewVm(ISimpleFileInfo file)
        {
            var imageSource = new ImageSourceConverter().ConvertFromString(file.Info.FullName) as ImageSource;
            var imagePreview = new ImageFilePreviewVm(file, imageSource);
            return imagePreview;
        }

        private TextFilePreviewVm CreateTextFilePreviewVm(ISimpleFileInfo file)
        {
            return new TextFilePreviewVm(file);
        }

        private ArtistPreviewVm CreateArtistPreviewVm(Artist artist)
        {
            return new ArtistPreviewVm(artist);
        }

        private AlbumPreviewVm CreateAlbumPreviewVm(Album album)
        {
            return new AlbumPreviewVm(album);
        }

        #endregion [  private methods  ]
    }
}