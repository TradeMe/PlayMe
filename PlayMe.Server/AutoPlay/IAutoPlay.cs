using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.AutoPlay
{
    public interface IAutoPlay
    {
        QueuedTrack FindTrack();
    }
}
