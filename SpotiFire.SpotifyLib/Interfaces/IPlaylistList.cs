
namespace SpotiFire.SpotifyLib
{
    public interface IPlaylistList : IEditableArray<IContainerPlaylist>
    {
        IContainerPlaylist Add(string name);
    }
}
