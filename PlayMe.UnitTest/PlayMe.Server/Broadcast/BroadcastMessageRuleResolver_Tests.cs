using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PlayMe.Common.Model;
using PlayMe.Server.Broadcast;
using PlayMe.Server.Broadcast.MessageRules;
using PlayMe.UnitTest.Plumbing;

namespace PlayMe.UnitTest.PlayMe.Server.Broadcast
{
    [TestFixture]
    public class Broadcast_Tests: TestBase<BroadcastMessageRuleResolver>
    {
        [Test]       
        public void if_only_rule_with_lowest_priority_applies_GetMessage_returns_message_from_that_rule()
        {
            //Arrange
            var ruleMock1 = new Mock<IBroadcastMessageRule>();
            var msg1 = new Message("Rule1", "Rule1");
            ruleMock1.Setup(m => m.GetMessage(It.IsAny<QueuedTrack>())).Returns(msg1);
            ruleMock1.SetupGet(m => m.Priority).Returns(1);
            var ruleMock2 = new Mock<IBroadcastMessageRule>();            
            ruleMock2.Setup(m => m.GetMessage(It.IsAny<QueuedTrack>())).Returns<IMessage>(null);
            ruleMock2.SetupGet(m => m.Priority).Returns(2);

            var classUnderTest = new BroadcastMessageRuleResolver(new List<IBroadcastMessageRule> {ruleMock1.Object, ruleMock2.Object});

            //Act
            
            var actualMessage = classUnderTest.GetMessage(new QueuedTrack());
            
            //Assert
            Assert.AreEqual(msg1,actualMessage);
        }
        [Test]
        public void if_all_rules_apply_GetMessage_returns_message_from_that_rule_with_highest_priority()
        {
            //Arrange
            var ruleMock1 = new Mock<IBroadcastMessageRule>();
            var msg1 = new Message("Rule1", "Rule1");
            ruleMock1.Setup(m => m.GetMessage(It.IsAny<QueuedTrack>())).Returns(msg1);
            ruleMock1.SetupGet(m => m.Priority).Returns(1);

            var msg2 = new Message("Rule2", "Rule2");
            var ruleMock2 = new Mock<IBroadcastMessageRule>();
            ruleMock2.Setup(m => m.GetMessage(It.IsAny<QueuedTrack>())).Returns(msg2);
            ruleMock2.SetupGet(m => m.Priority).Returns(3);

            var msg3 = new Message("Rule3", "Rule3");
            var ruleMock3 = new Mock<IBroadcastMessageRule>();
            ruleMock3.Setup(m => m.GetMessage(It.IsAny<QueuedTrack>())).Returns(msg3);
            ruleMock3.SetupGet(m => m.Priority).Returns(2);


            var classUnderTest = new BroadcastMessageRuleResolver(new List<IBroadcastMessageRule> { ruleMock1.Object, ruleMock2.Object, ruleMock3.Object });

            //Act

            var actualMessage = classUnderTest.GetMessage(new QueuedTrack());

            //Assert
            Assert.AreEqual(msg2, actualMessage);
        }

        [Test]
        public void if_no_rules_apply_GetMessage_returns_null()
        {
            //Arrange
            var ruleMock1 = new Mock<IBroadcastMessageRule>();
            ruleMock1.Setup(m => m.GetMessage(It.IsAny<QueuedTrack>())).Returns<IMessage>(null);
            ruleMock1.SetupGet(m => m.Priority).Returns(1);

            var ruleMock2 = new Mock<IBroadcastMessageRule>();
            ruleMock2.Setup(m => m.GetMessage(It.IsAny<QueuedTrack>())).Returns<IMessage>(null);
            ruleMock2.SetupGet(m => m.Priority).Returns(3);


            var classUnderTest = new BroadcastMessageRuleResolver(new List<IBroadcastMessageRule> { ruleMock1.Object, ruleMock2.Object });

            //Act

            var actualMessage = classUnderTest.GetMessage(new QueuedTrack());

            //Assert
            Assert.AreEqual(null, actualMessage);
        }
    }
}
