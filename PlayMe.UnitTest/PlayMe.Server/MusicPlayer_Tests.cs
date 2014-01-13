using System;
using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Player;
using PlayMe.Server.Providers;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server
{
    [TestFixture]
    public class MusicPlayer_Tests : TestBase<MusicPlayer>
    {
        [Test]
        public void if_playing_track_is_paused_then_resumed_CurrentPlayTrack_shows_correct_paused_time()
        {
            //Arrange
            var playingTrack = new QueuedTrack { Track = new Track { } };
            var providerFactoryMock = GetMock<IMusicProviderFactory>();
            var providerMock = GetMock<IMusicProvider>();
            var playerMock = GetMock<IPlayer>();
            providerFactoryMock.Setup(m => m.GetMusicProviderByIdentifier(It.IsAny<string>()))
                               .Returns(providerMock.Object);
            providerMock.Setup(m => m.Player).Returns(playerMock.Object);
            var nowMock = GetMock<INowHelper>();
            var startTime = new DateTime(2013, 01, 01, 1, 0, 0);
            var firstPause = new DateTime(2013, 01, 01, 1, 0, 1);
            var firstResume = new DateTime(2013, 01, 01, 1, 0, 3);
            var secondPause = new DateTime(2013, 01, 01, 1, 0, 5);
            var secondResume = new DateTime(2013, 01, 01, 1, 0, 8);
            nowMock.Setup(m => m.Now).ReturnsInOrder(startTime,firstPause,firstResume,secondPause,secondResume);
            //Act
            var classUnderTest = ClassUnderTest;
            classUnderTest.PlayTrack(playingTrack);
            classUnderTest.PauseTrack("");
            classUnderTest.ResumeTrack("");
            classUnderTest.PauseTrack("");
            classUnderTest.ResumeTrack("");
            //Assert            
            Assert.That(classUnderTest.CurrentlyPlayingTrack.PausedDurationAsMilliseconds, Is.EqualTo(5000));
        }

        [Test]
        public void if_playing_track_paused_CurrentPlayTrack_shows_correct_paused_time()
        {
            //Arrange
            var playingTrack = new QueuedTrack { Track = new Track { } };
            var providerFactoryMock = GetMock<IMusicProviderFactory>();
            var providerMock = GetMock<IMusicProvider>();
            var playerMock = GetMock<IPlayer>();
            providerFactoryMock.Setup(m => m.GetMusicProviderByIdentifier(It.IsAny<string>()))
                               .Returns(providerMock.Object);
            providerMock.Setup(m => m.Player).Returns(playerMock.Object);
            var nowMock = GetMock<INowHelper>();
            var startTime = new DateTime(2013, 01, 01, 1, 0, 0);
            var firstPause = new DateTime(2013, 01, 01, 1, 0, 1);
            var firstResume = new DateTime(2013, 01, 01, 1, 0, 3);
            var secondPause = new DateTime(2013, 01, 01, 1, 0, 5);
            var secondResume = new DateTime(2013, 01, 01, 1, 0, 8);
            nowMock.Setup(m => m.Now).ReturnsInOrder(startTime, firstPause, firstResume, secondPause, secondResume);
            //Act
            var classUnderTest = ClassUnderTest;
            classUnderTest.PlayTrack(playingTrack);
            classUnderTest.PauseTrack("");
            classUnderTest.ResumeTrack("");
            classUnderTest.PauseTrack("");
            classUnderTest.ResumeTrack("");
            //Assert            
            Assert.That(classUnderTest.CurrentlyPlayingTrack.PausedDurationAsMilliseconds, Is.EqualTo(5000));
        }
    }
}
