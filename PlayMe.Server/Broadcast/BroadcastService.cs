using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Server.Broadcast.Broadcasters;

namespace PlayMe.Server.Broadcast
{
    public class BroadcastService : IBroadcastService
    {
        private readonly IEnumerable<IBroadcaster> broadcasters;
        private readonly IBroadcastMessageRuleResolver broadcastMessageRuleResolver;

        public BroadcastService(IEnumerable<IBroadcaster> broadcasters, IBroadcastMessageRuleResolver broadcastMessageRuleResolver)
        {
            this.broadcastMessageRuleResolver = broadcastMessageRuleResolver;
            this.broadcasters = broadcasters;
        }

        public void Broadcast(QueuedTrack queuedTrack)
        {
            var message = broadcastMessageRuleResolver.GetMessage(queuedTrack);
            if (message == null) return;

            foreach (var broadcastService in broadcasters.Where(b => b.IsEnabled))
            {
                broadcastService.Broadcast(message);
            }
        }
    }
}
