using Common;
using MusicStoreKeeper.Model;
using System.Windows.Media;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class PreviewFactory
    {
        public PreviewFactory(IMusicFileAnalyzer musicFileAnalyzer)
        {
            _musicFileAnalyzer = musicFileAnalyzer;
        }

        #region [  fields  ]

        private readonly IMusicFileAnalyzer _musicFileAnalyzer;

        #endregion [  fields  ]

        #region [  public methods  ]

        public IPreviewVm CreatePreviewVm(object arg)
        {
            if (arg == null) return null;
            if (arg is ISimpleFileInfo file)
            {
                //TODO: Add enum PreviewableFiles to ISimpleFileInfo
                switch (file.Type)
                {
                    case SfiType.AudioFile:
                        return CreateAudioFilePreviewVm(file);

                    case SfiType.ImageFile:
                        return CreateImageFilePreviewVm(file);

                    case SfiType.TextFile:
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

        public ArtistPreviewVm CreateArtistPreviewVm(Artist artist)
        {
            return new ArtistPreviewVm(artist);
        }

        public AlbumPreviewVm CreateAlbumPreviewVm(Album album)
        {
            return new AlbumPreviewVm(album);
        }

        #endregion [  public methods  ]

        #region [  private methods  ]

        private AudioFilePreviewVm CreateAudioFilePreviewVm(ISimpleFileInfo file)
        {
            var basicTrackInfo = _musicFileAnalyzer.GetBasicAlbumInfoFromAudioFile(file.Info);
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

        #endregion [  private methods  ]
    }
}