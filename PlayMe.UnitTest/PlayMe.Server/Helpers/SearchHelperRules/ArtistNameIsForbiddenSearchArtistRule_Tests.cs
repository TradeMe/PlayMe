using Moq;
using NUnit.Framework;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.SearchHelperRules;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.SearchHelperRules
{
    [TestFixture]
    public class ArtistNameIsForbiddenSearchArtistRule_Tests
    {
        private Artist artist;
        [SetUp]
        public void SetUp()
        {
            artist = new Artist { Name = "test name" };
        }

        [Test]
        public void Check_that_if_settings_is_empty_return_false()
        {
            // Arrange
            var names = string.Empty;
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenArtistNames).Returns(names);

            // Act
            var classUnderTest = new ArtistNameIsForbiddenSearchArtistRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsArtistRestricted(artist);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_true_is_returned_when_artist_name_is_not_restricted()
        {
            // Arrange
            var names = "Cyrus";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenArtistNames).Returns(names);

            // Act
            var classUnderTest = new ArtistNameIsForbiddenSearchArtistRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsArtistRestricted(artist);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_false_is_returned_when_artist_name_is_restricted()
        {
            // Arrange
            var names = "test";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenArtistNames).Returns(names);

            // Act
            var classUnderTest = new ArtistNameIsForbiddenSearchArtistRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsArtistRestricted(artist);

            // Assert
            Assert.That(result == true);
        }

        [Test]
        public void Check_that_true_is_returned_when_artist_name_is_not_one_of_the_restricted_names()
        {
            // Arrange
            var names = "Cyrus,Brittney";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenArtistNames).Returns(names);

            // Act
            var classUnderTest = new ArtistNameIsForbiddenSearchArtistRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsArtistRestricted(artist);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_false_is_returned_when_artist_name_is_one_of_the_restricted_names()
        {
            // Arrange
            var names = "Cyrus,name";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenArtistNames).Returns(names);

            // Act
            var classUnderTest = new ArtistNameIsForbiddenSearchArtistRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsArtistRestricted(artist);

            // Assert
            Assert.That(result == true);
        }
    }
}
