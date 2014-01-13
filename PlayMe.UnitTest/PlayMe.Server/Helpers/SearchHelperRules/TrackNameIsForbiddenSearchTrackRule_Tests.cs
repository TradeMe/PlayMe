using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.SearchHelperRules;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.SearchHelperRules
{
    [TestFixture]
    public class TrackNameIsForbiddenSearchTrackRule_Tests
    {
        private Track track;
        [SetUp]
        public void SetUp()
        {
            IList<Artist> artists = new List<Artist>();
            var artist = new Artist { Name = "test name" };
            artists.Add(artist);
            track = new Track
            {
                Name = "Test Track name",
                Album = new Album { Name = "Test Album name", Artist = artist },
                Artists = artists,
                Duration = new TimeSpan(0, 0, 5, 0),
                DurationMilliseconds = 5,
                IsAlreadyQueued = false,
                IsAvailable = true,
                Link = "/test",
                MusicProvider = new MusicProviderDescriptor
                {
                    Name = "Spotify",
                    Identifier = "sp"                    
                },
                Popularity = 0,
                TrackArtworkUrl = "/test"
            };
        }

        [Test]
        public void Check_that_if_settings_is_empty_return_false()
        {
            // Arrange
            var names = string.Empty;
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenTrackNames).Returns(names);

            // Act
            var classUnderTest = new TrackNameIsForbiddenSearchTrackRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsTrackRestricted(track);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_false_is_Returned_when_track_does_not_match_with_restricted_name()
        {
            // Arrange
            var names = "Ball";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenTrackNames).Returns(names);

            // Act
            var classUnderTest = new TrackNameIsForbiddenSearchTrackRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsTrackRestricted(track);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_false_is_Returned_when_track_does_not_match_with_restricted_names()
        {
            // Arrange
            var names = "Ball,Towers";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenTrackNames).Returns(names);

            // Act
            var classUnderTest = new TrackNameIsForbiddenSearchTrackRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsTrackRestricted(track);

            // Assert
            Assert.That(result == false);
        }

        [Test]
        public void Check_that_true_is_Returned_when_track_does_match_with_restricted_name()
        {
            // Arrange
            var names = "test";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenTrackNames).Returns(names);

            // Act
            var classUnderTest = new TrackNameIsForbiddenSearchTrackRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsTrackRestricted(track);

            // Assert
            Assert.That(result == true);
        }

        [Test]
        public void Check_that_true_is_Returned_when_track_does_match_with_restricted_names()
        {
            // Arrange
            var names = "test,ball";
            var iSearchRuleSettings = new Mock<ISearchRuleSettings>();
            iSearchRuleSettings.Setup(s => s.ForbiddenTrackNames).Returns(names);

            // Act
            var classUnderTest = new TrackNameIsForbiddenSearchTrackRule(iSearchRuleSettings.Object);
            var result = classUnderTest.IsTrackRestricted(track);

            // Assert
            Assert.That(result == true);
        }
    }
}
