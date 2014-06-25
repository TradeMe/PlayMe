using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers;
using PlayMe.Server.Helpers.QueueHelperRules.Interfaces;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers
{
    [TestFixture]
    public class QueueRuleHelper_Tests
    {
        private QueuedTrack queuedTrack;
        [SetUp]
        public void SetUp()
        {
            queuedTrack = new QueuedTrack
            {
                Track = new Track
                {
                    Name = "Test Track name",
                    Album = new Album { Name = "Test Album name" }
                }
            };
        }

        [Test]
        public void Check_that_helper_returns_false_result_from_rule()
        {
            // Arrange
            var expectedResult = string.Empty;

            var rule = new Mock<IQueueRule>();
            var user = "jimmi";
            rule.Setup(r => r.CannotQueue(queuedTrack.Track, user)).Returns(expectedResult);

            var listOfRules = new List<IQueueRule> {rule.Object};

            var classUnderTest = new QueueRuleHelper(listOfRules);
            
            // Act
            var result = classUnderTest.CannotQueueTrack(queuedTrack.Track, user);

            // Assert
            Assert.That(result.All(r => r == string.Empty));
        }

        [Test]
        public void Check_that_helper_returns_true_result_from_rule()
        {
            // Arrange
            var expectedResult = "error";

            var user = "jimmi";
            var rule = new Mock<IQueueRule>();
            rule.Setup(r => r.CannotQueue(queuedTrack.Track, user)).Returns(expectedResult);

            var listOfRules = new List<IQueueRule> { rule.Object };

            var classUnderTest = new QueueRuleHelper(listOfRules);

            // Act
            var result = classUnderTest.CannotQueueTrack(queuedTrack.Track, user);

            // Assert
            Assert.That(result.Any());
        }

        [Test]
        public void Check_that_helper_returns_true_result_even_when_one_rule_returns_false()
        {
            // Arrange
            var expectedResult = "error";

            var user = "jimmi";
            var rule1 = new Mock<IQueueRule>();
            rule1.Setup(r => r.CannotQueue(queuedTrack.Track, user)).Returns(expectedResult);

            var rule2 = new Mock<IQueueRule>();
            rule2.Setup(r => r.CannotQueue(queuedTrack.Track, user)).Returns(string.Empty);

            var listOfRules = new List<IQueueRule> { rule1.Object, rule2.Object };

            var classUnderTest = new QueueRuleHelper(listOfRules);

            // Act
            var result = classUnderTest.CannotQueueTrack(queuedTrack.Track, user);

            // Assert
            Assert.That(result.Any());
        }
    }
}
