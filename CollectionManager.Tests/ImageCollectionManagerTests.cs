using Common;
using Discogs;
using Moq;
using MusicStoreKeeper.CollectionManager;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using NUnit.Framework;
using Serilog;
using System.Collections.Generic;
using System.Drawing;
using MusicStoreKeeper.ImageCollectionManager;

namespace CollectionManager.Tests
{
    [TestFixture]
    public class ImageCollectionManagerTests
    {
        #region [  fields  ]

        private IImageCollectionManager _sut;
        private Mock<IFileManager> _fileManager;
        private Mock<IImageDuplicateFinder> _imageDuplicateFinder;
        #endregion [  fields  ]

        #region [  setup/teardown  ]

        [OneTimeSetUp]
        public void Init()
        {
            // mock and sut initialize
            //DiscogsClient
            var client = new DiscogsClient();
            //IFileManager
            _fileManager = new Mock<IFileManager>();
            //IRepository
            var repository = new Mock<IRepository>();
            //ImageDuplicateFinder
            _imageDuplicateFinder=new Mock<IImageDuplicateFinder>();
           //ILoggerManager
            var loggerManager = new Mock<ILoggerManager>();
            var iLoggerSerilog = new Mock<ILogger>();
            loggerManager.Setup(log => log.GetLogger(It.IsAny<object>())).Returns(iLoggerSerilog.Object);
            //sut
            _sut = new ImageCollectionManager(client, _fileManager.Object, repository.Object, _imageDuplicateFinder.Object, loggerManager.Object);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
        }

        [SetUp]
        public void BeforeEachTest()
        {
        }

        [TearDown]
        public void AfterEachTest()
        {
        }

        #endregion [  setup/teardown  ]

        #region [  tests  ]

        #region [  RefreshImageDirectory  ]

        [Test]
        public void RefreshImageDirectory_ShouldReturnFalseIfImageDirectoryHasntChanged()
        {
            _fileManager.Setup(f => f.GetImageNamesFromDirectory(It.IsAny<string>())).Returns(new List<string>()
                {"Image1.jpg", "Image2.jpg", "Image3.jpg"});
            var imageDataList = new List<ImageData>()
            {
                new ImageData(){Name = "Image1.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image2.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image3.jpg", Status = ImageStatus.InCollection}
            };

            var result = _sut.RefreshImageDirectory(imageDataList, 1, "directoryPath");

            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void RefreshImageDirectory_ShouldAddNewImagesFoundInImageDirectory()
        {
            //new images are "Image4.jpg", "cover.jpg".
            _fileManager.Setup(f => f.GetImageNamesFromDirectory(It.IsAny<string>())).Returns(new List<string>()
                {"Image1.jpg", "Image2.jpg", "Image3.jpg", "Image4.jpg", "cover.jpg" });

            var imageDataList = new List<ImageData>()
            {
                new ImageData(){Name = "Image1.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image2.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image3.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "SomeDeletedImage.jpg", Status = ImageStatus.Deleted}
            };

            var result = _sut.RefreshImageDirectory(imageDataList, 1, "directoryPath");
            Assert.That(result, Is.EqualTo(true));
            Assert.That(imageDataList, Has.Count.EqualTo(6));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("Image4.jpg")));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("cover.jpg")));
        }

        [Test]
        public void RefreshImageDirectory_ShouldRemoveImageDataOfDeletedImagesWithEmptySource()
        {
            //deleted images are "Image4.jpg", "cover.jpg". "Image4.jpg" had some source
            _fileManager.Setup(f => f.GetImageNamesFromDirectory(It.IsAny<string>())).Returns(new List<string>()
                {"Image1.jpg", "Image2.jpg", "Image3.jpg"});

            var imageDataList = new List<ImageData>()
            {
                new ImageData(){Name = "Image1.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image2.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image3.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image4.jpg", Status = ImageStatus.InCollection, Source = "someUrl"},
                new ImageData(){Name = "cover.jpg", Status = ImageStatus.InCollection }
            };

            var result = _sut.RefreshImageDirectory(imageDataList, 1, "directoryPath");
            Assert.That(result, Is.EqualTo(true));
            Assert.That(imageDataList, Has.Count.EqualTo(4));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("Image4.jpg")));
            Assert.That(imageDataList, Has.None.Matches<ImageData>(i => i.Name.Equals("cover.jpg")));
        }

        [Test]
        public void RefreshImageDirectory_ShouldRemoveAndAddNewImageDataSimultaneously()
        {
            //new images are "newImage1.jpg", "newImage2.jpg".
            //deleted images are "Image4.jpg", "cover.jpg". "Image4.jpg" had some source
            _fileManager.Setup(f => f.GetImageNamesFromDirectory(It.IsAny<string>())).Returns(new List<string>()
                {"Image1.jpg", "Image2.jpg", "Image3.jpg", "newImage1.jpg", "newImage2.jpg" });

            var imageDataList = new List<ImageData>()
            {
                new ImageData(){Name = "Image1.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image2.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image3.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image4.jpg", Status = ImageStatus.InCollection, Source = "someUrl"},
                new ImageData(){Name = "cover.jpg", Status = ImageStatus.InCollection }
            };

            var result = _sut.RefreshImageDirectory(imageDataList, 1, "directoryPath");
            Assert.That(result, Is.EqualTo(true));
            Assert.That(imageDataList, Has.Count.EqualTo(6));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("newImage1.jpg")));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("newImage2.jpg")));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("Image4.jpg")));
            Assert.That(imageDataList, Has.None.Matches<ImageData>(i => i.Name.Equals("cover.jpg")));
        }

        #endregion [  RefreshImageDirectory  ]

        #region [  DeleteDuplicateImagesFromDirectoryAndDb  ]

        [Test]
        public void DeleteDuplicateImagesFromDirectoryAndDb_ShouldDeleteDuplicateImageDataFromCollection()
        {
            //mock for ISimpleFileInfo
            var simpleFileInfo1 = new Mock<ISimpleFileInfo>();
            simpleFileInfo1.SetupGet(sfi => sfi.Path).Returns("c://directoryPath//image1.jpg");
            //fileManager setup
            _fileManager.Setup(f => f.GetImageSimpleFileInfosFromDirectory(It.IsAny<string>()))
                .Returns(new List<ISimpleFileInfo>() { simpleFileInfo1.Object });
            //imageDuplicateFinder  setup
            _imageDuplicateFinder.Setup(a => a.GetDuplicateImagePaths(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IComparer<Image>>(),
                    It.IsAny<byte>(),
                    It.IsAny<float>()))
                .Returns(new List<string>(){ "c://directoryPath//duplicateImageToDelete1.jpg", "c://directoryPath//duplicateImageToDelete2.jpg" });
            //imageData collection setup
            var imageDataList = new List<ImageData>()
            {
                new ImageData(){Name = "Image1.jpg"},
                new ImageData(){Name = "Image2.jpg"},
                new ImageData(){Name = "duplicateImageToKeep.jpg"},
                new ImageData(){Name = "duplicateImageToDelete1.jpg", Source = "someUrl"},
                new ImageData(){Name = "duplicateImageToDelete2.jpg" }
            };
            
            _sut.DeleteDuplicateImagesFromDirectoryAndDb(imageDataList, 1, "directoryPath");

            Assert.That(imageDataList, Has.Count.EqualTo(4));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("duplicateImageToDelete1.jpg")));
            Assert.That(imageDataList, Has.None.Matches<ImageData>(i => i.Name.Equals("duplicateImageToDelete2.jpg")));
        }

        #endregion [  DeleteDuplicateImagesFromDirectoryAndDb  ]

        #region [  GetDuplicateImagePaths  ]



        #endregion

        #endregion [  tests  ]
    }
}