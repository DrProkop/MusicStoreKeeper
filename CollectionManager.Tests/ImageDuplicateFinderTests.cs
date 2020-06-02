using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Moq;
using MusicStoreKeeper.ImageCollectionManager;

namespace CollectionManager.Tests
{
    [TestFixture]
    public class ImageDuplicateFinderTests
    {

        #region [  constants  ]
        private const string Image1FileName = "TestImage1.jpg";
        private const string Image2FileName = "TestImageDuplicateHighRes.jpg";
        private const string Image3FileName = "TestImageDuplicateLowRes.jpg";
        private const string Image4FileName = "TestImageDuplicateLowResMinorDifference.jpg";
        #endregion

        #region [  fields  ]

        private Assembly _assembly;
        private string[] _resourceNames;
        private string _rootTestDirPath;
        private string _image1FilePath;
        private string _image2FilePath;
        private string _image3FilePath;
        private string _image4FilePath;
        private IImageDuplicateFinder _sut;
        #endregion

        #region [  setup/teardown  ]

        [OneTimeSetUp]
        public void Init()
        {
            //root directory for all tests
            var testDllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _assembly = Assembly.GetExecutingAssembly();
            _resourceNames = _assembly.GetManifestResourceNames();
            _rootTestDirPath = Path.Combine(testDllPath, "RootTestDirectory");
            if (Directory.Exists(_rootTestDirPath))
            {
                Directory.Delete(_rootTestDirPath, true);
            }
            Directory.CreateDirectory(_rootTestDirPath);
            // test images paths
            _image1FilePath = Path.Combine(_rootTestDirPath, Image1FileName);
            _image2FilePath = Path.Combine(_rootTestDirPath, Image2FileName);
            _image3FilePath = Path.Combine(_rootTestDirPath, Image3FileName);
            _image4FilePath = Path.Combine(_rootTestDirPath, Image4FileName);
            

            // sut
            _sut =new ImageDuplicateFinder();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            Directory.Delete(_rootTestDirPath, true);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            ExtractResource(Image1FileName, _image1FilePath);
            ExtractResource(Image2FileName, _image2FilePath);
            ExtractResource(Image3FileName, _image3FilePath);
            ExtractResource(Image4FileName, _image4FilePath);
        }

        [TearDown]
        public void AfterEachTest()
        {
            var di = new DirectoryInfo(_rootTestDirPath);
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        #endregion

        #region [  tests  ]

        [Test]
        public void GetDuplicateImagePaths_ShouldReturnListOFDuplicateImagePaths()
        {
            var imagePathList=new List<string>(){ _image1FilePath, _image3FilePath, _image2FilePath, _image4FilePath};
            var imageSizeComparer = new ImageSizeComparer();

            var duplicates = _sut.GetDuplicateImagePaths(imagePathList, imageSizeComparer, 3, 0.1f).ToList();

            Assert.That(duplicates, Has.Count.EqualTo(2));
            Assert.That(duplicates, Contains.Item(_image3FilePath).And.Contains(_image4FilePath));

        }

        #endregion

        #region [  private methods  ]

        private void ExtractResource(string resourceName, string resourceSavePath)
        {
            var resourcePath = _resourceNames.Single(str => str.EndsWith(resourceName));
            using (var fs = File.Create(resourceSavePath))
            using (var rs = _assembly.GetManifestResourceStream(resourcePath))
            {
                rs.CopyTo(fs);
            }
        }

        #endregion

    }
}