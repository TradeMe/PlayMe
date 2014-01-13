
namespace SpotiFire.SpotifyLib
{
    public class TracksMovedEventArgs : TracksEventArgs
    {
        int newPosition;
        public TracksMovedEventArgs(int[] trackIndices, int newPosition)
            : base(trackIndices)
        {
            this.newPosition = newPosition;
        }

        public int NewPosition
        {
            get { return this.newPosition; }
        }
    }
}
