namespace SpotiFire.SpotifyLib
{
    public interface IPlaylistTrackList : IEditableArray<IPlaylistTrack>
    {
        void Add(ITrack track, int index);
    }
}
