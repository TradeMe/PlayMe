
namespace PlayMe.Server.Player
{
    public interface IPlayer
    {
        void Pause();
        void Resume();
        void IncreaseVolume();
        void DecreaseVolume();
        void Reset();
    }
}
