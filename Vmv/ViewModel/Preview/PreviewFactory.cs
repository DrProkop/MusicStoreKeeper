using Common;
using MusicStoreKeeper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class PreviewFactory
    {
        public PreviewFactory(IMusicFileAnalyzer musicFileAnalyzer, IFileManager fileManager)
        {
            _musicFileAnalyzer = musicFileAnalyzer;
            _fileManager = fileManager;
        }

        #region [  fields  ]

        //get rid of this field
        private readonly IMusicFileAnalyzer _musicFileAnalyzer;

        private readonly IFileManager _fileManager;

        #endregion [  fields  ]

        #region [  public methods  ]

        public IPreviewVm CreatePreviewVm(object arg)
        {
            if (arg == null) return null;
            if (arg is ISimpleFileInfo file)
            {
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
            if (artist == null) throw new ArgumentNullException(nameof(artist));
            var artistPreviewVm = new ArtistPreviewVm(artist);
            if (string.IsNullOrEmpty(artist.StoragePath)) return artistPreviewVm;
            var imgDirPath = Path.Combine(artist.StoragePath, _fileManager.DefaultArtistPhotosDirectory);
            HandlePreviewImages(artistPreviewVm, imgDirPath);
            return artistPreviewVm;
        }

        public AlbumPreviewVm CreateAlbumPreviewVm(Album album)
        {
            if (album == null) throw new ArgumentNullException(nameof(album));
            var albumPreviewVm = new AlbumPreviewVm(album);
            if (string.IsNullOrEmpty(album.StoragePath)) return albumPreviewVm;
            var imgDirPath = Path.Combine(album.StoragePath, _fileManager.DefaultAlbumImagesDirectory);
            HandlePreviewImages(albumPreviewVm, imgDirPath);
            return albumPreviewVm;
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

        private void HandlePreviewImages(ModelEntityPreviewVmBase modelEntityPreviewVmBase, string imageDirectoryPath)
        {
            var images = LoadImages(imageDirectoryPath);
            modelEntityPreviewVmBase.ImageCollection = images;
            modelEntityPreviewVmBase.SelectedImage = modelEntityPreviewVmBase.ImageCollection.FirstOrDefault();
        }

        //TODO: Rework LoadImages
        private List<ImageSource> LoadImages(string imgDirPath)
        {
            var di = new DirectoryInfo(imgDirPath);
            if (!di.Exists) return new List<ImageSource>();
            var imgFileInfos = di.GetFiles("*.jpg");
            var images = new List<ImageSource>();
            foreach (var fi in imgFileInfos)
            {
                var bi = new BitmapImage();

                bi.BeginInit();
                bi.UriSource = new Uri(fi.FullName);
                bi.EndInit();

                images.Add(bi);
            }

            return images;
        }

        #endregion [  private methods  ]
    }
}