using System;

namespace PlayMe.Server.Player
{
    public interface IBufferedPlayer:IPlayer
    {
        int EnqueueSamples(int channels, int rate, byte[] samples, int frames);

        event EventHandler PlaybackEnded;
    }
}
