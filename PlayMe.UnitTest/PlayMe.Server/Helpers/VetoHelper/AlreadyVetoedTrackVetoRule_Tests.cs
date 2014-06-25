using System.Collections.Generic;
using NUnit.Framework;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.VetoHelperRules;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.VetoHelper
{
    [TestFixture]
    public class AlreadyVetoedTrackVetoRule_Tests : TestBase<AlreadyVetoedTrackVetoRule>
    {
        [Test]
        public void if_user_has_already_vetoed_track_CantVetoTrack_returns_true()
        {
            // Arrange

            // Act
            var result = ClassUnderTest.CantVetoTrack("a user", new QueuedTrack { Vetoes = new List<Veto> { new Veto{ByUser="a user"} } });

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void if_track_has_not_been_vetoed_by_user_CantVetoTrack_returns_false()
        {
            // Arrange

            // Act
            var result = ClassUnderTest.CantVetoTrack("a user", new QueuedTrack { Vetoes = new List<Veto> { new Veto { ByUser = "another user" } } });

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void if_track_has_no_vetoes_CantVetoTrack_returns_false()
        {
            // Arrange

            // Act
            var result = ClassUnderTest.CantVetoTrack("a user", new QueuedTrack { Vetoes = new List<Veto> () });

            // Assert
            Assert.That(result, Is.False);
        }
    }
}