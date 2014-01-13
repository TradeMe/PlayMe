
namespace SpotiFire.SpotifyLib
{
    public interface IContainerPlaylist : IPlaylist
    {
        sp_playlist_type Type { get; }
    }
}
