using System;
namespace SpotiFire.SpotifyLib
{
    public interface ITrack : ISpotifyObject, IAsyncLoaded, IDisposable
    {
        IAlbum Album { get; }
        IArray<IArtist> Artists { get; }
        int Disc { get; }
        TimeSpan Duration { get; }
        sp_error Error { get; }
        int Index { get; }
        bool IsAvailable { get; }
        bool IsStarred { get; set; }
        string Name { get; }
        int Popularity { get; }
    }
}
