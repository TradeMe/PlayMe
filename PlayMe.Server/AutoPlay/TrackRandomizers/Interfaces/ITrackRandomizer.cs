using PlayMe.Common.Model;

namespace PlayMe.Server.AutoPlay.TrackRandomizers.Interfaces
{
    public interface ITrackRandomizer
    {
        int Version { get; }
        QueuedTrack Execute(QueuedTrack trackToQueue);
    }
}
