using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.Interfaces
{
    public interface IAlreadyQueuedHelper
    {
        QueuedTrack ResetAlreadyQueued(QueuedTrack queuedTrack, string user);
        Track ResetAlreadyQueued(Track track, string user);
    }
}