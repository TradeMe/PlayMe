using System;
namespace SpotiFire.SpotifyLib
{
    public interface IImage : ISpotifyObject, IDisposable
    {
        byte[] Data { get; }
        sp_error Error { get; }
        sp_imageformat Format { get; }
        string ImageId { get; }
        bool IsLoaded { get; }
        event ImageEventHandler Loaded;
    }
}
