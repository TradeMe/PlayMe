using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers;
using PlayMe.Server.Helpers.SearchHelperRules.Interfaces;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers
{
    [TestFixture]
    public class SearchRuleHelper_Tests
    {
        private Track track;

        [SetUp]
        public void SetUp()
        {
            IList<Artist> artists = new List<Artist>();
            var artist = new Artist {Name = "test name"};
            artists.Add(artist);
            track = new Track
                        {
                            Name = "Test Track name",
                            Album = new Album {Name = "Test Album name",Artist = artist},
                            Artists = artists,
                            Duration = new TimeSpan(0,0,5,0),
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
        public void Check_that_we_get_a_track_back_when_track_is_not_restricted()
        {
            // Arrange
            var trackPagedList = new TrackPagedList { Tracks = new List<Track> { track },Total = 1};

            var rule = new Mock<ISearchTrackRule>();
            rule.Setup(r => r.IsTrackRestricted(track)).Returns(false);

            var listOfTrackRules = new List<ISearchTrackRule> { rule.Object };
            var listOfAlbumRules = new Mock<IEnumerable<ISearchAlbumRule>>();
            var listOfArtistRules = new Mock<IEnumerable<ISearchArtistRule>>();

            var classUnderTest = new SearchRuleHelper(listOfTrackRules, listOfAlbumRules.Object, listOfArtistRules.Object);

            // Act
            var result = classUnderTest.FilterTracks(trackPagedList);

            // Assert
            Assert.That(result.Tracks.FirstOrDefault() == track);
            Assert.That(result.Total == 1);
        }

        [Test]
        public void Check_that_we_dont_get_a_track_back_when_track_is_restricted()
        {
            // Arrange
            var trackPagedList = new TrackPagedList { Tracks = new List<Track> { track }, Total = 1 };

            var rule = new Mock<ISearchTrackRule>();
            rule.Setup(r => r.IsTrackRestricted(track)).Returns(true);

            var listOfTrackRules = new List<ISearchTrackRule> { rule.Object };
            var listOfAlbumRules = new Mock<IEnumerable<ISearchAlbumRule>>();
            var listOfArtistRules = new Mock<IEnumerable<ISearchArtistRule>>();

            var classUnderTest = new SearchRuleHelper(listOfTrackRules, listOfAlbumRules.Object, listOfArtistRules.Object);

            // Act
            var result = classUnderTest.FilterTracks(trackPagedList);

            // Assert
            Assert.That(result.Tracks.FirstOrDefault() == null);
            Assert.That(result.Total == 0);
        }
    }
}
