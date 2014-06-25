using System;
using Moq;
using NUnit.Framework;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.SkipHelperRules;
using PlayMe.Server.Interfaces;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server.Helpers.SkipHelperRules
{
    [TestFixture]
    public class OutOfHours_Tests : TestBase<OutOfHoursSkipRule>
    {
        [Test]
        public void If_the_time_is_after_6_pm_GetRequiredVetoCount_returns_1()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 4, 10, 18, 0, 1));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void If_the_time_is_exactly_6_pm_GetRequiredVetoCount_returns_1()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 4, 10, 18, 0, 0));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }


        [Test]
        public void If_the_time_is_before_8_am_GetRequiredVetoCount_returns_1()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 4, 10, 7, 59, 59));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void If_the_time_is_midnight_GetRequiredVetoCount_returns_1()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 4, 10, 00, 00, 00));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }


        [Test]
        public void If_the_time_is_after_8am_GetRequiredVetoCount_returns_intmax()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 4, 10, 8, 0, 1));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(int.MaxValue));
        }


        [Test]
        public void If_the_time_is_exactly_8am_GetRequiredVetoCount_returns_intmax()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 4, 10, 8, 0, 0));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(int.MaxValue));
        }

        public void If_the_time_is_noon_GetRequiredVetoCount_returns_intMax()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 4, 12, 0, 0, 0));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(int.MaxValue));
        }

        public void If_the_day_is_saturday_GetRequiredVetoCount_returns_1()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 12, 7, 12, 0, 0));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        public void If_the_day_is_sunday_GetRequiredVetoCount_returns_1()
        {
            // Arrange
            var now = GetMock<INowHelper>();
            now.Setup(mock => mock.Now).Returns(new DateTime(2013, 12, 8, 12, 0, 0));

            // Act
            var result = ClassUnderTest.GetRequiredVetoCount(It.IsAny<QueuedTrack>());

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }
    }
}