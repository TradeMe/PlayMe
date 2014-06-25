using System;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Player;

namespace PlayMe.Server.Providers
{
    public interface IMusicProvider
    {
        MusicProviderDescriptor Descriptor { get; }
        bool IsEnabled { get; }
        int Priority { get;  }

        #region Searching
        SearchResults SearchAll(string searchTerm, string user);
        #endregion
        #region Browsing
        Artist BrowseArtist(string link, bool mapTracksNotAlbums);

        Album BrowseAlbum(string link, string user);

        Track GetTrack(string link, string user);

        #endregion

        #region Player
        void PlayTrack(Track trackToPlay);

        void EndTrack();

        IPlayer Player { get; }

        event EventHandler TrackEnded;

        #endregion
    }
}
