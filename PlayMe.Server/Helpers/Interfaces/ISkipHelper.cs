using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.Interfaces
{
    public interface ISkipHelper
    {
        int RequiredVetoCount(QueuedTrack track);
    }
}