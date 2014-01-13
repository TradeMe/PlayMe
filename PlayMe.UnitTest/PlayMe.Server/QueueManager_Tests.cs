using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Queue;
using PlayMe.Server.Queue.Interfaces;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server
{
    [TestFixture]
    public class QueueManager_Tests: TestBase<QueueManager>
    {

        [Test]        
        public void If_a_dequeued_track_is_skipped_Dequeue_logs_it_and_dequeues_next_track()
        {
            // Arrange
            var logger = GetMock<ILogger>();

            var vetoHelperMock = GetMock<ISkipHelper>();
            vetoHelperMock.Setup(v => v.RequiredVetoCount(It.IsAny<QueuedTrack>())).Returns(2); // One veto needed

            var skippedTrack = new QueuedTrack { Track = new Track{Name ="TrackA"}, IsSkipped = true}; // Two vetoes            
            var anotherSkippedTrack = new QueuedTrack { Track = new Track { Name = "TrackB" }, IsSkipped = true }; // Two vetoes
            var goodTrack = new QueuedTrack { Track = new Track { Name = "TrackC" }}; // not skipped
            IConcurrentQueueOfQueuedTrack queue = new ConcurrentQueueOfQueuedTrack();
            Register(queue);
            queue.Enqueue(skippedTrack);
            queue.Enqueue(anotherSkippedTrack);
            queue.Enqueue(goodTrack);
            // Act
            var result = ClassUnderTest.Dequeue();

            // Assert            
            logger.Verify(x => x.Debug(It.IsAny<string>(), It.IsAny<object[]>()));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Track.Name, Is.EqualTo("TrackC"));

        }
    }
}
