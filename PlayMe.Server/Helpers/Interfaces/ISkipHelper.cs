using PlayMe.Common.Model;

namespace PlayMe.Server.Helpers.Interfaces
{
    public interface ISkipHelper
    {
        int RequiredVetoCount(QueuedTrack track);
    }
}