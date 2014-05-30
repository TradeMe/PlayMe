
using PlayMe.Common.Model;

namespace PlayMe.Server.SoundBoard
{
    public interface ISoundBoardService
    {
        void PlayVetoSound();

        void PlayFinishHim(int requiredVetos, QueuedTrack foundTrack);
    }
}
