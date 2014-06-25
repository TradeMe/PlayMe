using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Broadcast
{
    public interface IBroadcastMessageRuleResolver
    {
        IMessage GetMessage(QueuedTrack queuedTrack);
    }
}