using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Server;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Queue.Interfaces;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server
{
    [TestFixture]
    public class MusicService_Tests : TestBase<MusicService>
    {
        [Test]
        public void if_current_track_is_vetoed_VetoTrack_adds_veto_but_doesnt_skip()
        {
            // Arrange
            var vetoHelper = GetMock<IVetoHelper>();
            var skipHelper = GetMock<ISkipHelper>();            
            var musicPlayer = GetMock<IMusicPlayer>();
            var track = new QueuedTrack ();
            musicPlayer.SetupGet(m=> m.CurrentlyPlayingTrack).Returns(track);

            skipHelper.Setup(m => m.RequiredVetoCount(track)).Returns(3);
            
            var ds = GetMock<IDataService<QueuedTrack>>();            
            // Act
            ClassUnderTest.VetoTrack(new Guid(), "a user");
            // Assert
            vetoHelper.Verify( m => m.CantVetoTrack("a user",track),Times.Once());
            ds.Verify(m => m.Update(track));            
            musicPlayer.Verify(m => m.EndTrack(), Times.Never());
        }

        [Test]
        public void if_current_track_has_reached_required_number_of_vetoes_VetoTrack_adds_veto_and_skips()
        {
            // Arrange
            var vetoHelper = GetMock<IVetoHelper>();
            var skipHelper = GetMock<ISkipHelper>();
            var musicPlayer = GetMock<IMusicPlayer>();
            var track = new QueuedTrack ();
            musicPlayer.SetupGet(m => m.CurrentlyPlayingTrack).Returns(track);

            skipHelper.Setup(m => m.RequiredVetoCount(track)).Returns(1);

            var ds = GetMock<IDataService<QueuedTrack>>();
            // Act
            ClassUnderTest.VetoTrack(new Guid(), "a user");
            // Assert
            vetoHelper.Verify(m => m.CantVetoTrack("a user", track), Times.Once());
            ds.Verify(m => m.Update(track));
            musicPlayer.Verify(m => m.EndTrack(), Times.Once());
        }
        
        [Test]
        public void if_upcoming_track_is_vetoed_VetoTrack_adds_veto_but_doesnt_skip()
        {
            // Arrange
            var trackId = Guid.NewGuid();
            var queueManager = GetMock<IQueueManager>();
            var skipHelper = GetMock<ISkipHelper>();
            var track = new QueuedTrack ();
            queueManager.Setup(m => m.Contains(trackId)).Returns(true);
            queueManager.Setup(m => m.Get(trackId)).Returns(track);
            skipHelper.Setup(m => m.RequiredVetoCount(track)).Returns(3);
            // Act
            ClassUnderTest.VetoTrack(trackId, "a user");
            // Assert
            Assert.That(track.Vetoes.Count, Is.EqualTo(1));
            Assert.That(track.IsSkipped, Is.EqualTo(false));
        }


        [Test]
        public void if_upcoming_track_has_reached_required_number_of_vetoes_VetoTrack_adds_veto_and_will_be_skipped()
        {
            // Arrange
            var trackId = Guid.NewGuid();
            var queueManager = GetMock<IQueueManager>();
            
            var vetoHelper = GetMock<IVetoHelper>();
            var skipHelper = GetMock<ISkipHelper>();
            var track = new QueuedTrack { Vetoes = new List<Veto>() };
            queueManager.Setup(m => m.Contains(trackId)).Returns(true);
            queueManager.Setup(m => m.Get(trackId)).Returns(track);
            skipHelper.Setup(m => m.RequiredVetoCount(track)).Returns(1);
            // Act
            ClassUnderTest.VetoTrack(trackId, "a user");
            // Assert
            Assert.That(track.Vetoes.Count == 1);
            Assert.That(track.IsSkipped);
        }
    }
}