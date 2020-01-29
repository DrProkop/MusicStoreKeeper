using NUnit.Framework;
using System.Configuration;
using System.IO;

namespace FileManager.Tests
{
    [TestFixture]
    public class FileManagerTests
    {
        private string _testDir;
        private IFileManager fM;

        [OneTimeSetUp]
        public void Init()
        {
            _testDir = ConfigurationManager.AppSettings.Get("TestFolder");
            Directory.CreateDirectory(_testDir);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            Directory.Delete(_testDir, true);
        }

        [SetUp]
        public void Setup()
        {
            fM = new FileManager();
        }

        [Test]
        public void DeleteDirectoryShouldDeleteGivenDirectory()
        {
            var deletePath = Path.Combine(_testDir, "FolderToDelete");
            Directory.CreateDirectory(deletePath);
            var deleteFilePath = Path.Combine(deletePath, "FileToDelete.txt");
            //TODO: Redo with using
            var fs = File.Create(deleteFilePath);
            fs.Close();

            fM.DeleteDirectory(deletePath);

            Assert.That(!Directory.Exists(deletePath));
        }

        [Test]
        public void CreateDirectoryShouldCreateGivenDirectory()
        {
            var createPath = Path.Combine(_testDir, "FolderToCreate");

            fM.CreateDirectory(createPath);

            Assert.That(Directory.Exists(createPath));
        }

        [Test]
        public void MoveDirectoryShouldMoveGivenDirectory()
        {
            var sourceDirPath = Path.Combine(_testDir, "SourceFolder");
            var fileName = "FileToMove.txt";
            var sourceFilePath = Path.Combine(sourceDirPath, fileName);
            var destinationDirPath = Path.Combine(_testDir, "TargetFolder");
            Directory.CreateDirectory(sourceDirPath);
            var fStream = File.Create(sourceFilePath);
            fStream.Close();

            fM.MoveDirectory(sourceDirPath, destinationDirPath);

            Assert.That(Directory.Exists(destinationDirPath));
            Assert.That(File.Exists(Path.Combine(destinationDirPath, fileName)));
        }
    }
}