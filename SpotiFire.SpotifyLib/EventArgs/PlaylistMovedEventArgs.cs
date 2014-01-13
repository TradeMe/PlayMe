using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class PlaylistMovedEventArgs : PlaylistEventArgs
    {
        private int newIndex;
        public PlaylistMovedEventArgs(IntPtr playlistPtr, int index, int newIndex)
            : base(playlistPtr, index)
        {
            this.newIndex = newIndex;
        }
    }
}
