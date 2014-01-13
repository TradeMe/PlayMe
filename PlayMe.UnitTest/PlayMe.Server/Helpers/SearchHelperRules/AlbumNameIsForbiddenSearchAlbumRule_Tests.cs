using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.SearchHelperRules;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.SearchHelperRules
{
    public class AlbumNameIsForbiddenSearchAlbumRule_Tests
    {
        private Album album;
        [SetUp]
        public void SetUp()
        {
            album = new Album {Name = "Test Album"};
        }

        [Test]
        public void Check_that_if_settings_is_empty_return_false()
        {
            // Arrange
            var names = string.Empty;
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenAlbumNames).Returns(names);

            // Act
            var classUnderTest = new AlbumNameIsForbiddenSearchAlbumRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsAlbumRestricted(album);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_false_is_Returned_when_album_name_does_not_match_with_restricted_name()
        {
            // Arrange
            var names = "Karaoke";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenAlbumNames).Returns(names);

            // Act
            var classUnderTest = new AlbumNameIsForbiddenSearchAlbumRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsAlbumRestricted(album);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_true_is_Returned_when_album_name_does_match_with_restricted_name()
        {
            // Arrange
            var names = "test";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenAlbumNames).Returns(names);

            // Act
            var classUnderTest = new AlbumNameIsForbiddenSearchAlbumRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsAlbumRestricted(album);

            // Assert
            Assert.That(result == true);
        }

        [Test]
        public void Check_that_false_is_Returned_when_album_name_does_not_match_with_restricted_names()
        {
            // Arrange
            var names = "Karaoke, tribute";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenAlbumNames).Returns(names);

            // Act
            var classUnderTest = new AlbumNameIsForbiddenSearchAlbumRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsAlbumRestricted(album);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_ture_is_Returned_when_album_name_does_match_with_restricted_names()
        {
            // Arrange
            var names = "album,tribute";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenAlbumNames).Returns(names);

            // Act
            var classUnderTest = new AlbumNameIsForbiddenSearchAlbumRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsAlbumRestricted(album);

            // Assert
            Assert.That(result == true);
        }
    }
}
