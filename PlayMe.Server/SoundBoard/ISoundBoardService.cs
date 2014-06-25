
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.SoundBoard
{
    public interface ISoundBoardService
    {
        void PlayVetoSound();

        void PlayFinishHim(int requiredVetos, QueuedTrack foundTrack);
    }
}
