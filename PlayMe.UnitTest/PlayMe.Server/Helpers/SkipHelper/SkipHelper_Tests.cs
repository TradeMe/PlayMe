using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.SkipHelper
{
    [TestFixture]
    public class SkipHelper_Tests
    {
        [Test]
        public void The_lowest_value_returned_by_an_applicable_ISkipRule_is_returned()
        {
            // Arrange
            var rule1 = new Mock<ISkipRule>();
            rule1.Setup(mock => mock.GetRequiredVetoCount(It.IsAny<QueuedTrack>())).Returns(2);

            var rule2 = new Mock<ISkipRule>();
            rule2.Setup(mock => mock.GetRequiredVetoCount(It.IsAny<QueuedTrack>())).Returns(3);

            var rule3 = new Mock<ISkipRule>();
            rule3.Setup(mock => mock.GetRequiredVetoCount(It.IsAny<QueuedTrack>())).Returns(int.MaxValue);            

            var rule4 = new Mock<ISkipRule>();            
            rule4.Setup(mock => mock.GetRequiredVetoCount(It.IsAny<QueuedTrack>())).Returns(1);            

            var rules = new List<ISkipRule> {rule1.Object, rule2.Object, rule3.Object, rule4.Object};

            var classUnderTest = new global::PlayMe.Server.Helpers.SkipHelper(rules);

            // Act
            var result = classUnderTest.RequiredVetoCount(new QueuedTrack());

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }
    }
}