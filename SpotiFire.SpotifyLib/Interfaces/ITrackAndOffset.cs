using System;

namespace SpotiFire.SpotifyLib
{
    public interface ITrackAndOffset
    {
        ITrack Track { get; }
        TimeSpan Offset { get; }
    }
}
