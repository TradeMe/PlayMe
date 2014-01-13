using PlayMe.Common.Model;

namespace PlayMe.Server.AutoPlay
{
    public interface IAutoPlay
    {
        QueuedTrack FindTrack();
    }
}
