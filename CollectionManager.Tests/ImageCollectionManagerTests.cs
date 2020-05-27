using System.Collections.Generic;
using System.IO;
using Common;
using Discogs;
using NUnit.Framework;
using Moq;
using MusicStoreKeeper.CollectionManager;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Model;
using Serilog;

namespace CollectionManager.Tests
{
    [TestFixture]
    public class ImageCollectionManagerTests
    {

        #region [  constants  ]

        #endregion

        #region [  fields  ]

        private IImageCollectionManager _sut;
        private Mock<IFileManager> fileManager;


        #endregion

        #region [  setup/teardown  ]

        [OneTimeSetUp]
        public void Init()
        {
            // mock and sut initialize
            //DiscogsClient
            var client = new DiscogsClient();
            //IFileManager
            fileManager = new Mock<IFileManager>();
            //IRepository
            var repository=new Mock<IRepository>();
            //IImageService
            var imageService =new Mock<IImageService>();
            //ILoggerManager
            var loggerManager = new Mock<ILoggerManager>();
            var iLoggerSerilog = new Mock<ILogger>();
            loggerManager.Setup(log => log.GetLogger(It.IsAny<object>())).Returns(iLoggerSerilog.Object);
            //sut
            _sut=new ImageCollectionManager(client,fileManager.Object,repository.Object,imageService.Object,loggerManager.Object);
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

        #endregion

        #region [  tests  ]

        [Test]
        public void RefreshImageDirectoryShouldReturnFalseIfImageDirectoryHasntChanged()
        {
            fileManager.Setup(f => f.GetImageNamesFromDirectory(It.IsAny<string>())).Returns(new List<string>()
                {"Image1.jpg", "Image2.jpg", "Image3.jpg"});
            var imageDataList=new List<ImageData>()
            {
                new ImageData(){Name = "Image1.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image2.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image3.jpg", Status = ImageStatus.InCollection}
            };

            var result = _sut.RefreshImageDirectory(imageDataList, 1, "directoryPath");

            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void RefreshImageDirectoryShouldAddNewImagesFoundInImageDirectory()
        {
            //new images are "Image4.jpg", "cover.jpg".
            fileManager.Setup(f => f.GetImageNamesFromDirectory(It.IsAny<string>())).Returns(new List<string>()
                {"Image1.jpg", "Image2.jpg", "Image3.jpg", "Image4.jpg", "cover.jpg" });

            var imageDataList = new List<ImageData>()
            {
                new ImageData(){Name = "Image1.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image2.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "Image3.jpg", Status = ImageStatus.InCollection},
                new ImageData(){Name = "SomeDeletedImage.jpg", Status = ImageStatus.Deleted}

            };

            var result = _sut.RefreshImageDirectory(imageDataList, 1, "directoryPath");
            Assert.That(result, Is.EqualTo(false));
            Assert.That(imageDataList, Has.Count.EqualTo(6) );
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i=>i.Name.Equals("Image4.jpg")));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("cover.jpg")));
        }

        [Test]
        public void RefreshImageDirectoryShouldRemoveImageDataOfDeletedImagesWithEmptySource()
        {
            //deleted images are "Image4.jpg", "cover.jpg". "Image4.jpg" had some source
            fileManager.Setup(f => f.GetImageNamesFromDirectory(It.IsAny<string>())).Returns(new List<string>()
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
            Assert.That(result, Is.EqualTo(false));
            Assert.That(imageDataList, Has.Count.EqualTo(4));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("Image4.jpg")));
            Assert.That(imageDataList, Has.None.Matches<ImageData>(i => i.Name.Equals("cover.jpg")));
        }

        [Test]
        public void RefreshImageDirectoryShouldRemoveAndAddNewImageDataSimultaneously()
        {
            //new images are "newImage1.jpg", "newImage2.jpg".
            //deleted images are "Image4.jpg", "cover.jpg". "Image4.jpg" had some source
            fileManager.Setup(f => f.GetImageNamesFromDirectory(It.IsAny<string>())).Returns(new List<string>()
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
            Assert.That(result, Is.EqualTo(false));
            Assert.That(imageDataList, Has.Count.EqualTo(6));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("newImage1.jpg")));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("newImage2.jpg")));
            Assert.That(imageDataList, Has.Exactly(1).Matches<ImageData>(i => i.Name.Equals("Image4.jpg")));
            Assert.That(imageDataList, Has.None.Matches<ImageData>(i => i.Name.Equals("cover.jpg")));
        }

        #endregion

        #region [  private methods  ]

        #endregion

    }
}