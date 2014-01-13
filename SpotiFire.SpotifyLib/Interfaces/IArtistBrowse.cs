using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib {

    public interface IArtistBrowse : ISpotifyObject, IDisposable {
        sp_error Error { get; }
        IArtist Artist { get; }
        event ArtistBrowseEventHandler Complete;
        bool IsComplete { get; }
        IArray<string> PortraitIds { get; }
        IArray<ITrack> Tracks { get; }
        IArray<IAlbum> Albums { get; }
        IArray<IArtist> SimilarArtists { get; }
        string Biography { get; }
    }

}
