using System.Collections.Generic;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers;
using PlayMe.Server.Queue.Interfaces;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers
{
    [TestFixture]
    public class AlreadyQueuedHelper_tests:TestBase<AlreadyQueuedHelper>
    {
        private string user = "Vickers";

        [Test]
        public void if_queue_contains_track_ResetAlreadyQueued_returns_track_with_IsAlreadyQueued_on()
        {
            //Arrange
            var track = new Track
                            {
                                Link = "MyTrack",
                                IsAlreadyQueued = false
                            };

            var queueManagerMock = GetMock<IQueueManager>();
            queueManagerMock.Setup(m => m.GetAll())
                            .Returns(new List<QueuedTrack>()
                                         {
                                             new QueuedTrack {Track = new Track {Link = "ATrack"}},
                                             new QueuedTrack {Track = track} //This is the Track
                                         });
            //Act
            ClassUnderTest.ResetAlreadyQueued(track, user);

            //Assert
            Assert.That(track.IsAlreadyQueued,Is.EqualTo(true));
        }
        [Test]
        public void if_queue_doesnt_contains_track_ResetAlreadyQueued_returns_track_with_IsAlreadyQueued_off()
        {
            //Arrange
            var track = new Track
            {
                Link = "MyTrack",
                IsAlreadyQueued = true //This should be reset
            };

            var queueManagerMock = GetMock<IQueueManager>();
            queueManagerMock.Setup(m => m.GetAll())
                            .Returns(new List<QueuedTrack>()
                                         {
                                             new QueuedTrack {Track = new Track {Link = "ATrack"}}
                                         });
            //Act
            ClassUnderTest.ResetAlreadyQueued(track, user);

            //Assert
            Assert.That(track.IsAlreadyQueued, Is.EqualTo(false));
        }
    }
}
