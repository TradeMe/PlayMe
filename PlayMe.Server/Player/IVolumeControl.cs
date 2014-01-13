using PlayMe.Server.Player;

namespace PlayMe.Server
{
    public interface IVolumeControl : IVolume
    { 
        float IncreaseVolume();        
        float DecreaseVolume();
    }
}
