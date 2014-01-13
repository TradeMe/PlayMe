using PlayMe.Common.Model;

namespace PlayMe.Server.Helpers.VetoHelperRules.Interfaces
{
    public interface IVetoRule
    {
        bool CantVetoTrack(string vetoedByUser,QueuedTrack track);
    }
}
