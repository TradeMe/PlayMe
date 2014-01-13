using System;

namespace PlayMe.Server.Player
{
    public interface IStreamedPlayer: IPlayer
    {
        void PlayFromUrl(Uri url);
        void PlayFromFile(string fileName);

        event EventHandler PlaybackEnded;
    }
}
