using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Broadcast.MessageRules
{
    public interface IBroadcastMessageRule
    {
        IMessage GetMessage(QueuedTrack queuedTrack);
        int Priority { get; }
    }
}
