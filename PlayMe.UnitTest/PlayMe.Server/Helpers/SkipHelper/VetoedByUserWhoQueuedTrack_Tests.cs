using System.Collections.Generic;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers.SkipHelperRules;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.SkipHelper
{
    [TestFixture]
    public class VetoedByUserWhoQueuedTrack_Tests : TestBase<VetoedByUserWhoQueuedTrackSkipRule>
    {
        [Test]
        public void if_track_vetoed_by_same_user_that_queued_it_GetRequiredVetoCount_returns_1()
        {
            // Arrange

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(new QueuedTrack{User="a user", Vetoes = new List<Veto>{new Veto{ByUser="a user"}}});

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void if_track_vetoed_by_different_user_that_queued_it_GetRequiredVetoCount_returns_intmax()
        {
            // Arrange
            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(new QueuedTrack { User = "a user", Vetoes = new List<Veto> { new Veto { ByUser = "another user" } } });

            // Assert
            Assert.That(result, Is.EqualTo(int.MaxValue));
        }
    }
}