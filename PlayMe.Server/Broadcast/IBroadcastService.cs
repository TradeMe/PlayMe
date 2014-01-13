using PlayMe.Common.Model;

namespace PlayMe.Server.Broadcast
{

    public interface IBroadcastService
    {
        void Broadcast(QueuedTrack queuedTrack);
    }
}
