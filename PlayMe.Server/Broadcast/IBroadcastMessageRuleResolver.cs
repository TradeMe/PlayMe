using PlayMe.Common.Model;

namespace PlayMe.Server.Broadcast
{
    public interface IBroadcastMessageRuleResolver
    {
        IMessage GetMessage(QueuedTrack queuedTrack);
    }
}