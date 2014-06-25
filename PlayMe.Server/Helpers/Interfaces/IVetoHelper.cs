using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.Interfaces
{
    public interface IVetoHelper
    {
        bool CantVetoTrack(string vetoedByUser, QueuedTrack track);
    }
}