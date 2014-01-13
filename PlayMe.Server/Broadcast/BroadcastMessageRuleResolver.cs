using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Server.Broadcast.MessageRules;

namespace PlayMe.Server.Broadcast
{
    public class BroadcastMessageRuleResolver : IBroadcastMessageRuleResolver
    {
        private readonly IEnumerable<IBroadcastMessageRule> broadcastMessageRules;

        public BroadcastMessageRuleResolver(IEnumerable<IBroadcastMessageRule> broadcastMessageRules)
        {
            this.broadcastMessageRules = broadcastMessageRules;
        }

        public IMessage GetMessage(QueuedTrack queuedTrack)
        {
            return broadcastMessageRules.OrderByDescending(r => r.Priority)
                         .Select(r => r.GetMessage(queuedTrack))
                         .FirstOrDefault(m => m != null);            
        }
    }
}
