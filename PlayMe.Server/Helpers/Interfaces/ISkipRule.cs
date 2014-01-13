using PlayMe.Common.Model;

namespace PlayMe.Server.Helpers.Interfaces
{
    public interface ISkipRule
    {
        int GetRequiredVetoCount(QueuedTrack track);
    }
}