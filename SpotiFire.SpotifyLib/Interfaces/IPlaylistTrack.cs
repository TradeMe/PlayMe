using System;

namespace SpotiFire.SpotifyLib
{
    public interface IPlaylistTrack : ITrack
    {
        DateTime CreateTime { get; }
        //IUser Creator { get; }
        bool Seen { get; }
    }
}
