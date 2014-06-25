using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.VetoHelperRules.Interfaces
{
    public interface IVetoRule
    {
        bool CantVetoTrack(string vetoedByUser,QueuedTrack track);
    }
}
