using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.VetoHelperRules;
using PlayMe.Server.Interfaces;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.VetoHelper
{
    [TestFixture]
    public class ExceededDailyLimitVetoRule_Tests : TestBase<ExceededDailyLimitVetoRule>
    {
        private Mock<IVetoHelperSettings> settingsMock;
        private Mock<INowHelper> nowHelperMock;

        [SetUp]
        public void Setup()
        {
            settingsMock = GetMock<IVetoHelperSettings>();
            settingsMock.SetupGet(m => m.DailyVetoLimit).Returns(2);
            
            nowHelperMock = GetMock<INowHelper>();
            nowHelperMock.SetupGet(m => m.Now).Returns(new DateTime(2013,10,2,10,0,0));

        }

        [Test]
        public void if_user_has_already_vetoed_up_to_the_limit_CantVetoTrack_returns_true()
        {
            // Arrange
            var dsMock = GetMock <IDataService<QueuedTrack>>();
            dsMock.Setup(m => m.GetAll())
                  .Returns(new List<QueuedTrack>{
                                    new QueuedTrack
                                       {
                                           StartedPlayingDateTime = new DateTime(2013, 10, 2, 9, 0, 0),
                                           Vetoes = new List<Veto> {new Veto {ByUser = "a user"}}
                                       },
                                    new QueuedTrack
                                       {
                                           StartedPlayingDateTime = new DateTime(2013, 10, 2, 8, 0, 0),
                                           Vetoes = new List<Veto> {new Veto {ByUser = "a user"}}
                                       }

                               }.AsQueryable());

            // Act
            var result = ClassUnderTest.CantVetoTrack("a user", null);

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void if_user_has_vetoed_one_less_than_the_limit_CantVetoTrack_returns_false()
        {
            // Arrange
            var dsMock = GetMock<IDataService<QueuedTrack>>();
            dsMock.Setup(m => m.GetAll())
                  .Returns(new List<QueuedTrack>{
                                    new QueuedTrack
                                       {
                                           StartedPlayingDateTime = new DateTime(2013, 10, 2, 9, 0, 0),
                                           Vetoes = new List<Veto> {new Veto {ByUser = "a user"}}
                                       }                                       
                               }.AsQueryable());            
            // Act
            var result = ClassUnderTest.CantVetoTrack("a user", null);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void if_user_has_already_vetoed_up_to_the_limit_on_a_previous_day_but_has_no_vetoes_for_current_day_CantVetoTrack_returns_false()
        {
            // Arrange
            var dsMock = GetMock<IDataService<QueuedTrack>>();
            dsMock.Setup(m => m.GetAll())
                  .Returns(new List<QueuedTrack>{
                                    new QueuedTrack
                                       {
                                           StartedPlayingDateTime = new DateTime(2013, 10, 1, 9, 0, 0),
                                           Vetoes = new List<Veto> {new Veto {ByUser = "a user"}}
                                       },
                                    new QueuedTrack
                                       {
                                           StartedPlayingDateTime = new DateTime(2013, 10, 1, 8, 0, 0),
                                           Vetoes = new List<Veto> {new Veto {ByUser = "a user"}}
                                       }

                               }.AsQueryable());
            // Act
            var result = ClassUnderTest.CantVetoTrack("a user", null);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void if_user_has_not_vetoed_anything_CantVetoTrack_returns_false()
        {
            // Arrange
            var dsMock = GetMock<IDataService<QueuedTrack>>();
            dsMock.Setup(m => m.GetAll())
                  .Returns(new List<QueuedTrack>{
                                    new QueuedTrack
                                       {
                                           StartedPlayingDateTime = new DateTime(2013, 10, 2, 9, 0, 0),
                                           Vetoes = new List<Veto>{new Veto {ByUser = "another user"}}
                                       },
                                       new QueuedTrack
                                       {
                                           StartedPlayingDateTime = new DateTime(2013, 10, 2, 9, 0, 0),
                                           Vetoes = new List<Veto>{new Veto {ByUser = "another user"}}
                                       }
                               }.AsQueryable());
            // Act
            var result = ClassUnderTest.CantVetoTrack("a user", new QueuedTrack { Vetoes = new List<Veto> () });

            // Assert
            Assert.That(result, Is.False);
        }
    }
}