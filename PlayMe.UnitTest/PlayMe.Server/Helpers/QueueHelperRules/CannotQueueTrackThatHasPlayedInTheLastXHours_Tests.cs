using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.QueueHelperRules;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.QueueHelperRules
{
    [TestFixture]
    public class CannotQueueTrackThatHasPlayedInTheLastXHours_Tests
    {
        private QueuedTrack queuedTrack;
        private string user = "jimmi";
        [SetUp]
        public void SetUp()
        {
            queuedTrack = new QueuedTrack
            {
                Track = new Track
                {
                    Name = "Test Track name",
                    Album = new Album { Name = "Test Album name" }
                },
                User = user
                
            };
        }

        [Test]
        public void Check_a_track_played_4_hours_and_one_minute_can_be_played_again()
        {
            // Arrange
            var queueRuleSettings = new Mock<IQueueRuleSettings>();
            queueRuleSettings.Setup(q => q.LastXHours).Returns(4);

            var fourHoursAgoPlusOneMinute = DateTime.Now.AddHours(-4).AddMinutes(-1);
            queuedTrack.StartedPlayingDateTime = fourHoursAgoPlusOneMinute;

            var list = new List<QueuedTrack> { queuedTrack };
            var queuedTrackDataService = new Mock<IRepository<QueuedTrack>>();
            queuedTrackDataService.Setup(q => q.GetAll()).Returns(list.AsQueryable());

            // Act
            var classUnderTest = new CannotQueueTrackThatHasPlayedInTheLastXHoursQueueRule(queuedTrackDataService.Object, queueRuleSettings.Object);
            var result = classUnderTest.CannotQueue(queuedTrack.Track, user);

            // Assert
            Assert.That(result == string.Empty);
        }

        [Test]
        public void Check_a_track_played_3_hours_and_59_minutes_CANNOT_be_played_again()
        {
            // Arrange
            var queueRuleSettings = new Mock<IQueueRuleSettings>();
            queueRuleSettings.Setup(q => q.LastXHours).Returns(4);

            var threeHoursAndfiftyMinutesAgo = DateTime.Now.AddHours(-3).AddMinutes(-59);
            queuedTrack.StartedPlayingDateTime = threeHoursAndfiftyMinutesAgo;

            var list = new List<QueuedTrack> { queuedTrack };
            var queuedTrackDataService = new Mock<IRepository<QueuedTrack>>();
            queuedTrackDataService.Setup(q => q.GetAll()).Returns(list.AsQueryable());

            // Act
            var classUnderTest = new CannotQueueTrackThatHasPlayedInTheLastXHoursQueueRule(queuedTrackDataService.Object, queueRuleSettings.Object);
            var result = classUnderTest.CannotQueue(queuedTrack.Track, user);

            // Assert
            Assert.That(result.Any());
        }
    }
}
