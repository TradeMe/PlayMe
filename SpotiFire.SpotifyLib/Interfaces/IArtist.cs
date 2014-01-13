using System;
namespace SpotiFire.SpotifyLib
{
    public interface IArtist : ISpotifyObject, IDisposable
    {

        bool IsLoaded { get; }
        string Name { get; }
        string PortraitId { get; }
        IArtistBrowse Browse(sp_artistbrowse_type type = sp_artistbrowse_type.FULL);
        
    }
}
