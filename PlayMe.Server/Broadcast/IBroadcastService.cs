using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Broadcast
{

    public interface IBroadcastService
    {
        void Broadcast(QueuedTrack queuedTrack);
    }
}
