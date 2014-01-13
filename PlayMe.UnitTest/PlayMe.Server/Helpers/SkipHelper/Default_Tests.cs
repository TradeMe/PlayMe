using System.Collections.Generic;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server;
using PlayMe.Server.Helpers.SkipHelperRules;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.SkipHelperRules
{
    [TestFixture]
    public class Default_Tests : TestBase<DefaultSkipRule>
    {
        [Test]
        public void if_like_count_less_than_min_required_vetoes_from_settings_GetRequiredVetoCount_returns_min_required_vetoes_from_settings()
        {
            // Arrange
            var settingsMock = GetMock<ISettings>();
            settingsMock.SetupGet(m => m.VetoCount).Returns(3);
            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(new QueuedTrack{Likes = new List<Like>{new Like{}}});

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void if_like_count_more_than_min_required_vetoes_from_settings_GetRequiredVetoCount_returns_like_count()
        {
            // Arrange
            var settingsMock = GetMock<ISettings>();
            settingsMock.SetupGet(m => m.VetoCount).Returns(3);
            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(new QueuedTrack { Likes = new List<Like> { new Like { }, new Like { }, new Like { }, new Like { } } });

            // Assert
            Assert.That(result, Is.EqualTo(4));
        }
    }
}