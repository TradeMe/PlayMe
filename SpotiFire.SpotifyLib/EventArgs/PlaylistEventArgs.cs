using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class PlaylistEventArgs : EventArgs
    {
        private IntPtr playlistPtr;
        private int index;

        public PlaylistEventArgs(IntPtr playlistPtr, int index)
        {
            this.playlistPtr = playlistPtr;
            this.index = index;
        }
    }
}
