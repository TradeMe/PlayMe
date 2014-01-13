using System;

namespace SpotiFire.SpotifyLib
{
    public class TracksEventArgs : EventArgs
    {
        int[] trackIndices;
        public TracksEventArgs(int[] trackIndices)
        {
            this.trackIndices = trackIndices;
        }

        public int[] TrackIndices
        {
            get { return this.trackIndices; }
        }
    }
}
