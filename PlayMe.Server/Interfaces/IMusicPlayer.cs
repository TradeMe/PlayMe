using PlayMe.Common.Model;

namespace PlayMe.Server.Interfaces
{
    public interface IMusicPlayer
    {
        void PlayTrack(QueuedTrack trackToPlay);
        void PauseTrack(string user);
        void ResumeTrack(string user);
        QueuedTrack CurrentlyPlayingTrack { get;}
        void EndTrack();
        void IncreaseVolume();
        void DecreaseVolume();
    }
}
