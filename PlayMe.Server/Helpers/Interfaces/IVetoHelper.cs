using PlayMe.Common.Model;

namespace PlayMe.Server.Helpers.Interfaces
{
    public interface IVetoHelper
    {
        bool CantVetoTrack(string vetoedByUser, QueuedTrack track);
    }
}