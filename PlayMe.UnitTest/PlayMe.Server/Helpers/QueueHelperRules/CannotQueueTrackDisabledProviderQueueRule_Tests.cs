using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers.QueueHelperRules;
using PlayMe.Server.Interfaces;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.QueueHelperRules
{
    public class CannotQueueTrackDisabledProviderQueueRule_Tests
    {
        [Test]
        public void Check_can_queue_when_track_is_not_null()
        {
            // Arrange
            var trackToQueue = new Track
                {
                    Name = "Test Track name",
                    Album = new Album { Name = "Test Album name" }
                };
            var musicPlayer = new Mock<IMusicPlayer>();

            // Act
            var classUnderTest = new CannotQueueTrackDisabledProviderQueueRule(musicPlayer.Object);
            var result = classUnderTest.CannotQueue(trackToQueue, "Tester Test");

            // Assert
            Assert.That(result == string.Empty);
        }

        [Test]
        public void Check_cannot_queue_when_track_is_null()
        {
            // Arrange
            Track trackToQueue = null;
            var musicPlayer = new Mock<IMusicPlayer>();

            // Act
            var classUnderTest = new CannotQueueTrackDisabledProviderQueueRule(musicPlayer.Object);
            var result = classUnderTest.CannotQueue(trackToQueue, "Tester Test");

            // Assert
            Assert.That(result != string.Empty);
        }
    }
}
