using Common;
using Moq;
using NUnit.Framework;
using Serilog;
using System.IO;
using System.Linq;
using System.Text;

namespace FileManager.Tests
{
    [TestFixture]
    public class FileManagerTests
    {
        #region [  constants  ]

        private const string TestDirName = "TestDirectory";
        private const string TestSubDirName = "TestSubDirectory";
        private const string DestinationDirName = "TargetDirectory";
        private const string TextFileName = "SomeTextFile.txt";
        private const string MusicFileName = "SomeSong.mp3";
        private const string RandomFileName = "SomeUnknownFile.dat";

        #endregion [  constants  ]

        #region [  fileds  ]

        private string _rootTestDirPath;
        private string _testDirPath;
        private string _testSubDirPath;
        private string _destinationDirPath;
        private string _textFilePath;
        private string _musicFilePath;
        private string _randomFilePath;
        private IFileManager _sut;

        #endregion [  fileds  ]

        #region [  setup/teardown  ]

        [OneTimeSetUp]
        public void Init()
        {
            //root directory for all tests
            var testDllPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _rootTestDirPath = Path.Combine(testDllPath, "RootTestDirectory");
            Directory.CreateDirectory(_rootTestDirPath);
            //main directory to test
            _testDirPath = Path.Combine(_rootTestDirPath, TestDirName);
            //subdirectory
            _testSubDirPath = Path.Combine(_testDirPath, TestSubDirName);
            //target directory
            _destinationDirPath = Path.Combine(_rootTestDirPath, DestinationDirName);
            //text file
            _textFilePath = Path.Combine(_testDirPath, TextFileName);
            //music file
            _musicFilePath = Path.Combine(_testDirPath, MusicFileName);
            //random unknown file in child directory
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
            //text
            using (var fs = File.Create(_textFilePath))
            {
                var title = new UTF8Encoding(true).GetBytes("New Text File");
                fs.Write(title, 0, title.Length);
            }
            //music
            using (var fs = File.Create(_musicFilePath)) { }
            //random
            using (var fs = File.Create(_randomFilePath)) { }
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
            _sut.MoveDirectory(_testDirPath, _destinationDirPath);

            Assert.Multiple(() =>
            {
                Assert.That(Directory.Exists(_destinationDirPath));
                Assert.That(File.Exists(Path.Combine(_destinationDirPath, TextFileName)));
                Assert.That(File.Exists(Path.Combine(_destinationDirPath, TestSubDirName, RandomFileName)));
                Assert.That(!Directory.Exists(_testDirPath));
            });
        }

        [Test]
        public void MoveDirectoryShouldMoveOnlyFilesFromGivenDirectory()
        {
            _sut.MoveDirectory(_testDirPath, _destinationDirPath, false);

            Assert.Multiple(() =>
            {
                //destination
                Assert.That(Directory.Exists(_destinationDirPath));
                Assert.That(File.Exists(Path.Combine(_destinationDirPath, TextFileName)));
                Assert.That(!Directory.Exists(Path.Combine(_destinationDirPath, TestSubDirName)));
                //source
                Assert.That(Directory.Exists(_testDirPath));
                Assert.That(Directory.Exists(_testSubDirPath));
                Assert.That(!File.Exists(_textFilePath));

            });
        }

        [Test]
        public void MoveDirectoryShouldThrowOnInvalidSourcePath()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
                _sut.MoveDirectory(Path.Combine(_testDirPath, "DirectoryDoesntExist"), _destinationDirPath));
        }

        [Test]
        public void CopyDirectoryShouldCopyCopyGivenDirectoryWithSubdirectories()
        {
            _sut.CopyDirectory(_testDirPath, _destinationDirPath);

            Assert.Multiple(() =>
            {
                //destination
                Assert.That(Directory.Exists(_destinationDirPath));
                Assert.That(File.Exists(Path.Combine(_destinationDirPath, TextFileName)));
                Assert.That(File.Exists(Path.Combine(_destinationDirPath, TestSubDirName, RandomFileName)));

                //source
                Assert.That(Directory.Exists(_testDirPath));
                Assert.That(File.Exists(_textFilePath));
                Assert.That(File.Exists(_randomFilePath));
            });
        }

        [Test]
        public void CopyDirectoryShouldCopyCopyGivenDirectoryWithoutSubdirectories()
        {
            _sut.CopyDirectory(_testDirPath, _destinationDirPath, false);

            Assert.Multiple(() =>
            {
                //destination
                Assert.That(Directory.Exists(_destinationDirPath));
                Assert.That(File.Exists(Path.Combine(_destinationDirPath, TextFileName)));
                Assert.That(!Directory.Exists(Path.Combine(_destinationDirPath, TestSubDirName)));

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
                _sut.CopyDirectory(Path.Combine(_testDirPath, "DirectoryDoesntExist"), _destinationDirPath, true));
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
    }
}