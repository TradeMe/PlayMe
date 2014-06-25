using System;
using Moq;
using NUnit.Framework;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.QueueHelperRules;
using PlayMe.Server.Interfaces;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.QueueHelperRules
{
    [TestFixture]
    public class CannotQueueTrackAlreadyPlaying_Tests
    {
        private QueuedTrack queuedTrack;
        private QueuedTrack queuedTrack2;
        private QueuedTrack queuedTrack3;
        private QueuedTrack queuedTrack4;
        private DateTime playedTime = DateTime.Now;

        [SetUp]
        public void SetUp()
        {
            queuedTrack = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = "Test User",
                Track = new Track
                {
                    Name = "Test Track name",
                    Album = new Album { Name = "Test Album name" }
                }

            };

            queuedTrack2 = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = "Test User",
                Track = new Track
                {
                    Name = "Test Track name2",
                    Album = new Album { Name = "Test Album name2" }
                }

            };

            queuedTrack3 = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = "Test User",
                Track = new Track
                {
                    Name = "Test Track name2",
                    Album = new Album { Name = "Test Album name" }
                }

            };

            queuedTrack4 = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = "Test User",
                Track = new Track
                {
                    Name = "Test Track name",
                    Album = new Album { Name = "Test Album name2" }
                }

            };
        }

        [Test]
        public void Check_can_queue_when_current_track_is_different_from_track_to_queue()
        {
            // Arrange
            var user = "jimmi";
            var musicPlayer = new Mock<IMusicPlayer>();
            musicPlayer.Setup(m => m.CurrentlyPlayingTrack).Returns(queuedTrack2);

            // Act
            var classUnderTest = new CannotQueueTrackAlreadyPlayingQueueRule(musicPlayer.Object);
            var result = classUnderTest.CannotQueue(queuedTrack.Track, user);

            // Assert
            Assert.That(result == string.Empty);
        }

        [Test]
        public void Check_CANNOT_queue_when_current_track_is_the_same_as_track_to_queue()
        {
            // Arrange
            var user = "jimmi";
            var musicPlayer = new Mock<IMusicPlayer>();
            musicPlayer.Setup(m => m.CurrentlyPlayingTrack).Returns(queuedTrack);

            // Act
            var classUnderTest = new CannotQueueTrackAlreadyPlayingQueueRule(musicPlayer.Object);
            var result = classUnderTest.CannotQueue(queuedTrack.Track, user);

            // Assert
            Assert.That(result != string.Empty);
        }

        [Test]
        public void Check_can_queue_when_current_track_is_different_from_track_to_queue_but_same_Album()
        {
            // Arrange
            var user = "jimmi";
            var musicPlayer = new Mock<IMusicPlayer>();
            musicPlayer.Setup(m => m.CurrentlyPlayingTrack).Returns(queuedTrack3);

            // Act
            var classUnderTest = new CannotQueueTrackAlreadyPlayingQueueRule(musicPlayer.Object);
            var result = classUnderTest.CannotQueue(queuedTrack.Track, user);

            // Assert
            Assert.That(result == string.Empty);
        }
    }
}
