using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.AutoPlay.TrackRandomizers.Interfaces
{
    public interface ITrackRandomizer
    {
        int Version { get; }
        QueuedTrack Execute(QueuedTrack trackToQueue);
    }
}
