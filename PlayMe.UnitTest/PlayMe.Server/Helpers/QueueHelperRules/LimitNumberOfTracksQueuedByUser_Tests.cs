using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.QueueHelperRules;
using PlayMe.Server.Queue.Interfaces;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.QueueHelperRules
{
    [TestFixture]
    public class LimitNumberOfTracksQueuedByUser_Tests
    {
        private QueuedTrack queuedTrack;
        private QueuedTrack queuedTrack2;
        private QueuedTrack queuedTrack3;
        private QueuedTrack queuedTrack4;
        private QueuedTrack queuedTrack5;
        private DateTime playedTime = DateTime.Now;
        private string user = "jimmi";
        [SetUp]
        public void SetUp()
        {
            queuedTrack = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = user,
                Track = new Track
                {
                    Name = "Test Track name",
                    Album = new Album { Name = "Test Album name" }
                }

            };

            queuedTrack2 = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = user,
                Track = new Track
                {
                    Name = "Test Track name2",
                    Album = new Album { Name = "Test Album name2" }
                }

            };

            queuedTrack3 = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = user,
                Track = new Track
                {
                    Name = "Test Track name3",
                    Album = new Album { Name = "Test Album name3" }
                }

            };

            queuedTrack4 = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = user,
                Track = new Track
                {
                    Name = "Test Track name4",
                    Album = new Album { Name = "Test Album name4" }
                }

            };

            queuedTrack5 = new QueuedTrack
            {
                StartedPlayingDateTime = playedTime,
                User = user,
                Track = new Track
                {
                    Name = "Test Track name5",
                    Album = new Album { Name = "Test Album name5" }
                }

            };
        }

        [Test]
        public void Check_that_we_can_queue_with_one_song_currently_queued()
        {
            // Arrange
            var queueRuleSettings = new Mock<IQueueRuleSettings>();
            queueRuleSettings.Setup(q => q.QueueCount).Returns(5);

            var list = new List<QueuedTrack> { queuedTrack };
            var queueManager = new Mock<IQueueManager>();
            queueManager.Setup(q => q.GetAll()).Returns(list);

            // Act
            var classUnderTest = new LimitNumberOfTracksQueuedByUserQueueRule(queueManager.Object, queueRuleSettings.Object);
            var result = classUnderTest.CannotQueue(queuedTrack.Track, user);

            // Assert
            Assert.That(result == string.Empty);
        }

        [Test]
        public void Check_that_we_can_queue_with_four_song_currently_queued()
        {
            // Arrange
            var queueRuleSettings = new Mock<IQueueRuleSettings>();
            queueRuleSettings.Setup(q => q.QueueCount).Returns(5);

            var list = new List<QueuedTrack> { queuedTrack, queuedTrack2, queuedTrack3, queuedTrack4 };
            var queueManager = new Mock<IQueueManager>();
            queueManager.Setup(q => q.GetAll()).Returns(list);

            // Act
            var classUnderTest = new LimitNumberOfTracksQueuedByUserQueueRule(queueManager.Object, queueRuleSettings.Object);
            var result = classUnderTest.CannotQueue(queuedTrack.Track, user);

            // Assert
            Assert.That(result == string.Empty);
        }

        [Test]
        public void Check_that_we_CANNOT_queue_with_five_song_currently_queued()
        {
            // Arrange
            var queueRuleSettings = new Mock<IQueueRuleSettings>();
            queueRuleSettings.Setup(q => q.QueueCount).Returns(5);

            var list = new List<QueuedTrack> { queuedTrack, queuedTrack2, queuedTrack3, queuedTrack4, queuedTrack5 };
            var queueManager = new Mock<IQueueManager>();
            queueManager.Setup(q => q.GetAll()).Returns(list);

            // Act
            var classUnderTest = new LimitNumberOfTracksQueuedByUserQueueRule(queueManager.Object, queueRuleSettings.Object);
            var result = classUnderTest.CannotQueue(queuedTrack.Track, user);

            // Assert
            Assert.That(result != string.Empty);
        }
    }
}
