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
            var artistPreviewVm = new ArtistPreviewVm(artist);
            var imgDirPath = Path.Combine(artist.StoragePath, "photos");
            var images = LoadImages(imgDirPath);
            if (!images.Any()) return artistPreviewVm;
            artistPreviewVm.ImageCollection = images;
            artistPreviewVm.SelectedImage = artistPreviewVm.ImageCollection.First();
            return artistPreviewVm;
        }

        public AlbumPreviewVm CreateAlbumPreviewVm(Album album)
        {
            var albumPreviewVm = new AlbumPreviewVm(album);
            var imgDirPath = Path.Combine(album.StoragePath, "images");
            var images = LoadImages(imgDirPath);
            if (!images.Any()) return albumPreviewVm;
            albumPreviewVm.ImageCollection = images;
            albumPreviewVm.SelectedImage = albumPreviewVm.ImageCollection.First();
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

        //TODO: Rework LoadImages
        private List<ImageSource> LoadImages(string imgDirPath)
        {
            var di = new DirectoryInfo(imgDirPath);
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