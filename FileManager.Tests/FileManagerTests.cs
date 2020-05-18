using System;
using Common;
using Moq;
using NUnit.Framework;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileManager.Tests
{
    [TestFixture]
    public class FileManagerTests
    {
        #region [  constants  ]

        private const string TestDirName = "TestDirectory";
        private const string TestImageSubDirName = "TestImageSubDirectory";
        private const string TestSubDirName = "TestSubDirectory";
        private const string TargetDirName = "TargetDirectory";
        private const string TextFileName = "TestDocument1.txt";
        private const string Image1FileName = "TestImage1.jpg";
        private const string Image2FileName = "TestImage2.jpg";
        private const string Image3FileName = "TestImage3.jpg";
        private const string MusicFileName = "TestTrack1.mp3";
        private const string RandomFileName = "RandomFile.bin";
        private const string UnknownTypeFileName = "DataFile.dat";

        #endregion [  constants  ]

        #region [  fileds  ]

        private string _rootTestDirPath;
        private string _testDirPath;
        private string _testImageSubDirPath;
        private string _testSubDirPath;
        private string _targetDirPath;
        private string _musicFilePath;
        private string _textFilePath;
        private string _image1FilePath;
        private string _image2FilePath;
        private string _image3FilePath;
        private string _randomFilePath;
        private string _unknownTypeFilePath;
        private IFileManager _sut;
        private string[] _resourceNames;
        private Assembly _assembly;

        #endregion [  fileds  ]

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
            //main directory to test
            _testDirPath = Path.Combine(_rootTestDirPath, TestDirName);
            //subdirectory
            _testSubDirPath = Path.Combine(_testDirPath, TestSubDirName);
            //image subdirectory
            _testImageSubDirPath = Path.Combine(_testDirPath, TestImageSubDirName);
            //target directory
            _targetDirPath = Path.Combine(_rootTestDirPath, TargetDirName);
            //music file
            _musicFilePath = Path.Combine(_testDirPath, MusicFileName);
            //text file
            _textFilePath = Path.Combine(_testDirPath, TextFileName);
            //image files
            _image1FilePath = Path.Combine(_testDirPath, Image1FileName);
            _image2FilePath = Path.Combine(_testImageSubDirPath, Image2FileName);
            _image3FilePath = Path.Combine(_testImageSubDirPath, Image3FileName);
            //random file in test directory
            _unknownTypeFilePath = Path.Combine(_testDirPath, UnknownTypeFileName);
            //random file in sub directory
            _randomFilePath = Path.Combine(_testSubDirPath, RandomFileName);
            //mock and sut initialize
            var loggerManager = new Mock<ILoggerManager>();
            var iLoggerSerilog = new Mock<ILogger>();
            loggerManager.Setup(log => log.GetLogger(It.IsAny<object>())).Returns(iLoggerSerilog.Object);
            _sut = new FileManager(loggerManager.Object);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            Directory.Delete(_rootTestDirPath, true);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            //main test directory
            Directory.CreateDirectory(_testDirPath);
            //test subdirectory
            Directory.CreateDirectory(_testSubDirPath);
            //test image subdirectory
            Directory.CreateDirectory(_testImageSubDirPath);
            //music
            ExtractResource(MusicFileName, _musicFilePath);
            //text
            ExtractResource(TextFileName, _textFilePath);
            //images
            ExtractResource(Image1FileName, _image1FilePath);
            ExtractResource(Image2FileName, _image2FilePath);
            ExtractResource(Image3FileName, _image3FilePath);
            //unknown file
            ExtractResource(UnknownTypeFileName, _unknownTypeFilePath);
            //random file
            ExtractResource(RandomFileName, _randomFilePath);
        }

        private Mock<IMusicDirInfo> MusicDirectoryInfoMockSetup()
        {
            //mock data
            var trackList = new List<ISimpleFileInfo>() { SimpleFileInfoFactory.Create(_musicFilePath) };
            var imageFiles = new List<ISimpleFileInfo>() { SimpleFileInfoFactory.Create(_image1FilePath) };
            var imageDirectories = new List<ISimpleFileInfo>() { SimpleFileInfoFactory.Create(_testImageSubDirPath) };
            imageDirectories.First().Children = new List<ISimpleFileInfo>() { SimpleFileInfoFactory.Create(_image2FilePath), SimpleFileInfoFactory.Create(_image3FilePath) };
            var textFiles = new List<ISimpleFileInfo>() { SimpleFileInfoFactory.Create(_textFilePath) };
            var unknownFiles = new List<ISimpleFileInfo>() { SimpleFileInfoFactory.Create(_unknownTypeFilePath) };
            var mDirInfoMock = new Mock<IMusicDirInfo>();
            //mock setup
            mDirInfoMock.SetupGet(m => m.Path).Returns(_testDirPath);
            mDirInfoMock.SetupGet(m => m.MusicFilesInDirectory).Returns(1);
            mDirInfoMock.SetupGet(m => m.TrackList).Returns(trackList);
            mDirInfoMock.SetupGet(m => m.ImageFiles).Returns(imageFiles);
            mDirInfoMock.SetupGet(m => m.ImageDirectories).Returns(imageDirectories);
            mDirInfoMock.SetupGet(m => m.ImageDirectories).Returns(imageDirectories);
            mDirInfoMock.SetupGet(m => m.TextFiles).Returns(textFiles);
            mDirInfoMock.SetupGet(m => m.UnknownFiles).Returns(unknownFiles);
            return mDirInfoMock;
        }

        [TearDown]
        public void AfterEachTest()
        {
            DirectoryInfo di = new DirectoryInfo(_rootTestDirPath);
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        #endregion [  setup/teardown  ]

        #region [  common methods  ]

        [Test]
        public void CreateDirectoryShouldCreateGivenDirectory()
        {
            var createPath = Path.Combine(_rootTestDirPath, TestDirName);

            _sut.CreateDirectory(createPath);

            Assert.That(Directory.Exists(createPath));
        }

        [Test]
        public void MoveDirectoryShouldMoveGivenDirectoryAndAllSubdirectories()
        {
            _sut.MoveDirectory(_testDirPath, _targetDirPath);

            //target
            Assert.That(Directory.Exists(_targetDirPath));
            Assert.That(File.Exists(Path.Combine(_targetDirPath, TextFileName)));
            Assert.That(File.Exists(Path.Combine(_targetDirPath, TestSubDirName, RandomFileName)));
            Assert.That(File.Exists(Path.Combine(_targetDirPath, TestImageSubDirName, Image2FileName)));

            //source
            Assert.That(!Directory.Exists(_testDirPath));
        }

        [Test]
        public void MoveDirectoryShouldMoveOnlyFilesFromGivenDirectory()
        {
            _sut.MoveDirectory(_testDirPath, _targetDirPath, false);

            //destination
            Assert.That(Directory.Exists(_targetDirPath));
            Assert.That(File.Exists(Path.Combine(_targetDirPath, TextFileName)));
            Assert.That(!Directory.Exists(Path.Combine(_targetDirPath, TestSubDirName)));

            //source
            Assert.That(Directory.Exists(_testDirPath));
            Assert.That(Directory.Exists(_testSubDirPath));
            Assert.That(!File.Exists(_textFilePath));
        }

        [Test]
        public void MoveDirectoryShouldThrowOnInvalidSourcePath()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
                _sut.MoveDirectory(Path.Combine(_testDirPath, "DirectoryDoesntExist"), _targetDirPath));
        }

        [Test]
        public void CopyDirectoryShouldCopyCopyGivenDirectoryWithSubdirectories()
        {
            _sut.CopyDirectory(_testDirPath, _targetDirPath);

            //target
            Assert.That(Directory.Exists(_targetDirPath));
            Assert.That(File.Exists(Path.Combine(_targetDirPath, TextFileName)));
            Assert.That(File.Exists(Path.Combine(_targetDirPath, TestSubDirName, RandomFileName)));

            //source
            Assert.That(Directory.Exists(_testDirPath));
            Assert.That(File.Exists(_textFilePath));
            Assert.That(File.Exists(_randomFilePath));
        }

        [Test]
        public void CopyDirectoryShouldCopyCopyGivenDirectoryWithoutSubdirectories()
        {
            _sut.CopyDirectory(_testDirPath, _targetDirPath, false);

            Assert.Multiple(() =>
            {
                //destination
                Assert.That(Directory.Exists(_targetDirPath));
                Assert.That(File.Exists(Path.Combine(_targetDirPath, TextFileName)));
                Assert.That(!Directory.Exists(Path.Combine(_targetDirPath, TestSubDirName)));

                //source
                Assert.That(Directory.Exists(_testDirPath));
                Assert.That(File.Exists(_textFilePath));
                Assert.That(File.Exists(_randomFilePath));
            });
        }

        [Test]
        public void CopyDirectoryShouldThrowOnInvalidSourcePath()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
                _sut.CopyDirectory(Path.Combine(_testDirPath, "DirectoryDoesntExist"), _targetDirPath, true));
        }

        [Test]
        public void ClearDirectoryShouldClearGivenDirectory()
        {
            _sut.ClearDirectory(_testDirPath);
            var di = new DirectoryInfo(_testDirPath);
            var files = di.GetFiles();
            var subDirs = di.GetDirectories();

            Assert.Multiple(() =>
            {
                Assert.That(!files.Any());
                Assert.That(!subDirs.Any());
            });
        }

        [Test]
        public void ClearDirectoryShouldThrowOnInvalidPath()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _sut.ClearDirectory(Path.Combine(_testDirPath, "DirectoryDoesntExist")));
        }

        [Test]
        public void DeleteDirectoryShouldDeleteGivenDirectory()
        {
            _sut.DeleteDirectory(_testDirPath);

            Assert.That(!Directory.Exists(_testDirPath));
        }

        [Test]
        [TestCase("fileName.exe", "fileName_1.exe", 1)]
        [TestCase("FileName33", "FileName34", 34)]
        [TestCase("FileName111_a.dat", "FileName111_a_1.dat", 1)]
        [TestCase("123.jpg", "124.jpg",124)]
        public void IncrementFileNameShouldIncrementGivenNameByOneOrAddOneAtTheEnd(string duplicateFileName, string newFileName, int numberAtTheAnd)
        {
            var result = _sut.IncrementFileName(duplicateFileName);
            Assert.That(result.Item1, Is.EqualTo(newFileName));
            Assert.That(result.Item2, Is.EqualTo(numberAtTheAnd));
        }

        [Test]
        public void GenerateUniqueNameShouldReturnMinusOneIfNoNumberAtTheEndOfAFileName()
        {
            var resultNameAndNumber = _sut.GenerateUniqueName(new List<string>() { "fileName_1", "fileName_2" }, "fileName");

            Assert.That(resultNameAndNumber.Item1, Is.EqualTo("fileName"));
            Assert.That(resultNameAndNumber.Item2, Is.EqualTo(-1));
        }

        [Test]
        public void GenerateUniqueNameShouldReturnSameNameIfNoMatchesWereFound()
        {
            var resultNameAndNumber = _sut.GenerateUniqueName(new List<string>() { "fileName_1", "fileName_2" }, "fileName_3");

            Assert.That(resultNameAndNumber.Item1, Is.EqualTo("fileName_3"));
            Assert.That(resultNameAndNumber.Item2, Is.EqualTo(3));
        }

        [Test]
        public void GenerateUniqueNameShouldReturnValidNewNameIfMatchesWereFound()
        {
            var resultNameAndNumber = _sut.GenerateUniqueName(new List<string>() { "fileName_1", "fileName_2" }, "fileName_1");

            Assert.That(resultNameAndNumber.Item1, Is.EqualTo("fileName_3"));
            Assert.That(resultNameAndNumber.Item2, Is.EqualTo(3));
        }

        [Test]
        public void GenerateNameForDownloadedImageShouldReturnValidNewNameAndIncrementedNumberAtTheEndIfMatchesWereFound()
        {
            var numberAtTheEnd = 1;
            var resultNameAndNumber = _sut.GenerateNameForDownloadedImage(new List<string>() { "someRandomImageName","fileName_photo_1.jpg", "fileName_photo_2.jpg" }, "fileName", numberAtTheEnd);

            Assert.That(resultNameAndNumber.Item1, Is.EqualTo("fileName_photo_3.jpg"));
            Assert.That(resultNameAndNumber.Item2, Is.EqualTo(3));
        }

        #endregion [  common methods  ]

        #region [  music directories methods  ]

        [Test]
        public void TryDeleteSourceMusicDirectoryShouldDeleteGivenDirectoryAndSubdirectoriesIfNoFilesLeft()
        {
            _sut.ClearDirectory(_testDirPath);
            Directory.CreateDirectory(_testImageSubDirPath);
            Directory.CreateDirectory(_testSubDirPath);
            var result = _sut.DeleteSourceMusicDirectory(_testDirPath);

            Assert.Multiple(() =>
            {
                Assert.That(!Directory.Exists(_testDirPath));
                Assert.That(result);
            });
        }

        [Test]
        public void TryDeleteSourceMusicDirectoryShouldNotDeleteGivenDirectoryAndSubdirectoriesIfAnyFilesLeft()
        {
            //leave _randomFilePath
            File.Delete(_textFilePath);
            File.Delete(_musicFilePath);
            File.Delete(_image2FilePath);
            File.Delete(_image3FilePath);
            var result = _sut.DeleteSourceMusicDirectory(_testDirPath);

            Assert.Multiple(() =>
            {
                Assert.That(Directory.Exists(_testDirPath));
                Assert.That(Directory.Exists(_testSubDirPath));
                Assert.That(!Directory.Exists(_testImageSubDirPath));
                Assert.That(!result);
            });
        }
        
        [Test]
        public void DeleteSourceMusicDirectoryFilesShouldDeleteOnlyFilesProvidedInMusicDirInfo()
        {
            var mDirInfoMock = MusicDirectoryInfoMockSetup();

            _sut.DeleteSourceMusicDirectoryFiles(mDirInfoMock.Object);

            Assert.That(!File.Exists(_musicFilePath));
            Assert.That(!File.Exists(_image1FilePath));
            Assert.That(!File.Exists(_image2FilePath));
            Assert.That(!File.Exists(_unknownTypeFilePath));
            Assert.That(!File.Exists(_textFilePath));
            Assert.That(File.Exists(_randomFilePath));
            Assert.That(Directory.Exists(_testDirPath));
            Assert.That(!Directory.Exists(_testImageSubDirPath));
        }

        [Test]
        public void CopyMusicDirectoryShouldCopyGivenMusicDirectory()
        {
            //mock setup
            var mDirInfoMock = MusicDirectoryInfoMockSetup();
            //target paths
            var targetTextSubDir = Path.Combine(_targetDirPath, _sut.DefaultAlbumDocsDirectory);
            var targetImageSubDir = Path.Combine(_targetDirPath, _sut.DefaultAlbumImagesDirectory);
            var targetUnknownFilesSubDir = Path.Combine(_targetDirPath, _sut.DefaultAlbumUnknownFilesDirectory);

            Directory.CreateDirectory(_targetDirPath);

            _sut.CopyMusicDirectory(mDirInfoMock.Object, _targetDirPath);

            //target directories
            Assert.That(Directory.Exists(targetTextSubDir));
            Assert.That(Directory.Exists(targetUnknownFilesSubDir));
            Assert.That(Directory.Exists(targetImageSubDir));

            //target files
            Assert.That(File.Exists(Path.Combine(_targetDirPath, MusicFileName)));
            Assert.That(File.Exists(Path.Combine(targetTextSubDir, TextFileName)));
            Assert.That(File.Exists(Path.Combine(targetImageSubDir, Image1FileName)));
            Assert.That(File.Exists(Path.Combine(targetImageSubDir, Image3FileName)));
            Assert.That(File.Exists(Path.Combine(targetUnknownFilesSubDir, UnknownTypeFileName)));
        }

        #endregion [  music directories methods  ]

        private void ExtractResource(string resourceName, string resourceSavePath)
        {
            var resourcePath = _resourceNames.Single(str => str.EndsWith(resourceName));
            using (var fs = File.Create(resourceSavePath))
            using (var rs = _assembly.GetManifestResourceStream(resourcePath))
            {
                rs.CopyTo(fs);
            }
        }
    }
}