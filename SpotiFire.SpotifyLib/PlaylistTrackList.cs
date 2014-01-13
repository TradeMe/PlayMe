using System;

namespace SpotiFire.SpotifyLib
{
    internal sealed class PlaylistTrackList : DelegateList<IPlaylistTrack>, IPlaylistTrackList
    {
        private Action<ITrack, int> addTrack;
        public PlaylistTrackList(Func<int> getLength, Func<int, IPlaylistTrack> getIndex, Action<ITrack, int> addFunc, Action<int> removeFunc, Func<bool> readonlyFunc)
            : base(getLength, getIndex, addFunc, removeFunc, readonlyFunc)
        {
            this.addTrack = addFunc;
        }

        public void Add(ITrack track, int index)
        {
            addTrack(track, index);
        }
    }
}
