using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.Interfaces
{
    public interface ISkipRule
    {
        int GetRequiredVetoCount(QueuedTrack track);
    }
}