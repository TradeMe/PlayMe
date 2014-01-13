using NUnit.Framework;
using PlayMe.Server.SoundBoard;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server
{
    [TestFixture]
    public class PathBuilder_Tests : TestBase<PathBuilder>
    {
        [TestCase("\\\\somePath", "myFile.wav", "\\\\somePath\\SoundBoard\\Sounds\\myFile.wav")]
        [TestCase("\\\\somePath\\", "myFile.wav", "\\\\somePath\\SoundBoard\\Sounds\\myFile.wav")]
        public void BuildPath_returns_combined_path(string applicationFolder,string fileName,string expectedResult)
        {
            // Arrange
            var folderHelper = GetMock<IFolderHelper>();
            folderHelper.Setup(m => m.ApplicationFolder).Returns(applicationFolder);
            // Act
            var result = ClassUnderTest.BuildFilePath(fileName);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

    }
}
