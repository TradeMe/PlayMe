using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib {

    public interface IAlbumBrowse : ISpotifyObject, IDisposable {
        sp_error Error { get; }
        IAlbum Album { get; }
        IArtist Artist { get; }
        event AlbumBrowseEventHandler Complete;
        bool IsComplete { get; }
        IArray<string> Copyrights { get; }
        IArray<ITrack> Tracks { get; }
        string Review { get; }
    }

}
