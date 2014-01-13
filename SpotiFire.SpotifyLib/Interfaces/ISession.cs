using System;
namespace SpotiFire.SpotifyLib
{
    public interface ISession : IDisposable
    {
        event SessionEventHandler ConnectionError;
        sp_connectionstate ConnectionState { get; }
        event SessionEventHandler EndOfTrack;
        event SessionEventHandler Exception;
        void Login(string username, string password, bool remember);
        event SessionEventHandler LoginComplete;
        event SessionEventHandler LogMessage;
        void Logout();
        event SessionEventHandler LogoutComplete;
        event SessionEventHandler MessageToUser;
        event SessionEventHandler MetadataUpdated;
        event MusicDeliveryEventHandler MusicDeliver;
        sp_error PlayerLoad(ITrack track);
        sp_error PlayerPause();
        sp_error PlayerPlay();
        sp_error PlayerSeek(int offset);
        sp_error PlayerSeek(TimeSpan offset);
        void PlayerUnload();
        IPlaylistContainer PlaylistContainer { get; }
        IPlaylist Starred { get; }
        event SessionEventHandler PlayTokenLost;
        ISearch Search(string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, int playlistOffset, int playlistCount, sp_search_type type);
        void SetPrefferedBitrate(sp_bitrate bitrate);
        event SessionEventHandler StartPlayback;
        event SessionEventHandler StopPlayback;
        event SessionEventHandler StreamingError;
        event SessionEventHandler UserinfoUpdated;
    }
}
