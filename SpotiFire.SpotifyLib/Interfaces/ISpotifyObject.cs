using System;

namespace SpotiFire.SpotifyLib
{
    public interface ISpotifyObject : IDisposable
    {
        ISession Session { get; }
    }
}
