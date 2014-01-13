
using System;
namespace SpotiFire.SpotifyLib
{
    class PlaylistList : DelegateList<IContainerPlaylist>, IPlaylistList
    {
        private Func<string, IContainerPlaylist> addNewFunc;
        public PlaylistList(Func<int> getLength, Func<int, IContainerPlaylist> getIndex, Action<IContainerPlaylist, int> addFunc, Action<int> removeFunc, Func<bool> readonlyFunc, Func<string, IContainerPlaylist> addNewFunc)
            : base(getLength, getIndex, addFunc, removeFunc, readonlyFunc)
        {

        }

        public IContainerPlaylist Add(string name)
        {
            return addNewFunc(name);
        }
    }
}
