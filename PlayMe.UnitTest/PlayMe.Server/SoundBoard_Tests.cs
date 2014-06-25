using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Player;
using PlayMe.Server.SoundBoard;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server
{
    [TestFixture]
    public class SoundBoard_Tests : TestBase<SoundBoardService>
    {
        [Test]
        public void if_not_enabled_PlayVetoSound_plays_nothing()
        {
            //arrange
            var settingsMock = GetMock<ISoundBoardSettings>();
            var playerMock = GetMock<IStreamedPlayer>();
            settingsMock.Setup(m => m.IsEnabled).Returns(false);

            //act
            ClassUnderTest.PlayVetoSound();

            //assert
            playerMock.Verify(m => m.PlayFromFile(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void if_first_skip_of_the_day_PlayVetoSound_plays_first_blood()
        {
            //arrange
            var settingsMock = GetMock<ISoundBoardSettings>();
            
            settingsMock.Setup(m => m.IsEnabled).Returns(true);
            settingsMock.Setup(m => m.SecondsBetweenSkipThreshold).Returns(10);

            var pathBuilderMock = GetMock<IPathBuilder>();
            pathBuilderMock.Setup(m => m.BuildFilePath("firstblood.wav")).Returns("\\\\apath\\sounds\\firstbloo.wav");

            var nowMock = GetMock<INowHelper>();
            nowMock.Setup(m => m.Now).Returns(new DateTime(2013, 1, 2));

            var dataServiceMock = GetMock<IRepository<SoundBoardInfo>>();
            dataServiceMock.Setup(m => m.GetAll())
                           .Returns(new List<SoundBoardInfo>(){new SoundBoardInfo {lastSkippedSongTime = new DateTime(2013, 1, 1).ToUniversalTime()}}.AsQueryable());

            var playerMock = GetMock<IStreamedPlayer>();

            //act
            ClassUnderTest.PlayVetoSound();

            //assert
            playerMock.Verify(m => m.PlayFromFile("\\\\apath\\sounds\\firstbloo.wav"), Times.Once());
        }

        [Test]
        public void if_not_first_skip_of_the_day_PlayVetoSound_doesnt_play_first_blood()
        {
            //arrange
            var settingsMock = GetMock<ISoundBoardSettings>();

            settingsMock.Setup(m => m.IsEnabled).Returns(true);

            var pathBuilderMock = GetMock<IPathBuilder>();
            pathBuilderMock.Setup(m => m.BuildFilePath("firstblood.wav")).Returns("\\\\apath\\sounds\\firstbloo.wav");

            var nowMock = GetMock<INowHelper>();
            nowMock.Setup(m => m.Now).Returns(new DateTime(2013, 1, 1));

            var dataServiceMock = GetMock<IRepository<SoundBoardInfo>>();
            dataServiceMock.Setup(m => m.GetAll())
                           .Returns(new List<SoundBoardInfo>() { new SoundBoardInfo { lastSkippedSongTime = new DateTime(2013, 1, 1).ToUniversalTime() } }.AsQueryable());

            var playerMock = GetMock<IStreamedPlayer>();

            //act
            ClassUnderTest.PlayVetoSound();

            //assert
            playerMock.Verify(m => m.PlayFromFile("\\\\apath\\sounds\\firstbloo.wav"), Times.Never());
        }

        [TestCase(1, "doublekill.wav")]
        [TestCase(4, "ultrakill.wav")]
        public void if_skipped_PlayVetoSound_increments_skip_count_and_plays_correct_sound(int lastSkipCount, string fileName)
        {
            //arrange
            var settingsMock = GetMock<ISoundBoardSettings>();

            settingsMock.Setup(m => m.IsEnabled).Returns(true);
            settingsMock.Setup(m => m.SecondsBetweenSkipThreshold).Returns(10);
            var pathBuilderMock = GetMock<IPathBuilder>();
            pathBuilderMock.Setup(m => m.BuildFilePath(fileName)).Returns("\\\\apath\\sounds\\" + fileName);

            var nowMock = GetMock<INowHelper>();
            nowMock.Setup(m => m.Now).Returns(new DateTime(2013, 1, 2, 10, 0, 5));

            var dataServiceMock = GetMock<IRepository<SoundBoardInfo>>();
            dataServiceMock.Setup(m => m.GetAll())
                           .Returns(new List<SoundBoardInfo>() { new SoundBoardInfo { lastSkippedSongTime = new DateTime(2013, 1, 2 , 10, 0, 0).ToUniversalTime(), skippedSongsCount = lastSkipCount} }.AsQueryable());

            var playerMock = GetMock<IStreamedPlayer>();

            //act
            ClassUnderTest.PlayVetoSound();

            //assert
            dataServiceMock.Verify(m => m.InsertOrUpdate(It.Is<SoundBoardInfo>(c => c.skippedSongsCount == lastSkipCount + 1)), Times.Once());
            playerMock.Verify(m => m.PlayFromFile("\\\\apath\\sounds\\" + fileName), Times.Once());
            playerMock.Verify(m => m.PlayFromFile("\\\\apath\\sounds\\firstbloo.wav"), Times.Never());
        }
        
        [Test]
        public void if_skipped_maximum_times_PlayVetoSound_plays_no_sound()
        {
            //arrange
            var settingsMock = GetMock<ISoundBoardSettings>();

            settingsMock.Setup(m => m.IsEnabled).Returns(true);
            settingsMock.Setup(m => m.SecondsBetweenSkipThreshold).Returns(10);

            var pathBuilderMock = GetMock<IPathBuilder>();
            pathBuilderMock.Setup(m => m.BuildFilePath(It.IsAny<string>())).Returns("\\\\apath\\sounds\\filename.wav");


            var nowMock = GetMock<INowHelper>();
            nowMock.Setup(m => m.Now).Returns(new DateTime(2013, 1, 2, 10, 0, 5));

            var dataServiceMock = GetMock<IRepository<SoundBoardInfo>>();
            dataServiceMock.Setup(m => m.GetAll())
                           .Returns(new List<SoundBoardInfo>() { new SoundBoardInfo { lastSkippedSongTime = new DateTime(2013, 1, 2, 10, 0, 0).ToUniversalTime(), skippedSongsCount = 11 } }.AsQueryable());

            var playerMock = GetMock<IStreamedPlayer>();

            //act
            ClassUnderTest.PlayVetoSound();

            //assert
            playerMock.Verify(m => m.PlayFromFile(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void if_skipped_outside_seconds_between_skip_threshold_PlayVetoSound_resets_skipCount_and_plays_no_sound()
        {
            //arrange
            var settingsMock = GetMock<ISoundBoardSettings>();

            settingsMock.Setup(m => m.IsEnabled).Returns(true);
            settingsMock.Setup(m => m.SecondsBetweenSkipThreshold).Returns(10);

            var pathBuilderMock = GetMock<IPathBuilder>();
            pathBuilderMock.Setup(m => m.BuildFilePath(It.IsAny<string>())).Returns("\\\\apath\\sounds\\filename.wav");


            var nowMock = GetMock<INowHelper>();
            nowMock.Setup(m => m.Now).Returns(new DateTime(2013, 1, 2, 10, 0, 15)); //5 seconds outside of threshold

            var soundInfo = new SoundBoardInfo
                                {
                                    lastSkippedSongTime = new DateTime(2013, 1, 2, 10, 0, 0).ToUniversalTime(),
                                    skippedSongsCount = 3
                                };
            var dataServiceMock = GetMock<IRepository<SoundBoardInfo>>();
            dataServiceMock.Setup(m => m.GetAll())
                           .Returns(new List<SoundBoardInfo>() { soundInfo }.AsQueryable());
            
            var playerMock = GetMock<IStreamedPlayer>();
            
            //act
            ClassUnderTest.PlayVetoSound();
            soundInfo.skippedSongsCount = 1;
            //assert
            dataServiceMock.Verify(m => m.InsertOrUpdate(It.Is<SoundBoardInfo>(c=>c.skippedSongsCount == 1)), Times.Once());
            playerMock.Verify(m => m.PlayFromFile(It.IsAny<string>()), Times.Never());
        }
    }
}
