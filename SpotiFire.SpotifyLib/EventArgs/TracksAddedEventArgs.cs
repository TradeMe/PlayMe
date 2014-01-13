
namespace SpotiFire.SpotifyLib
{
    public class TracksAddedEventArgs : TracksEventArgs
    {
        private ITrack[] tracks;
        public TracksAddedEventArgs(int[] trackIndices, ITrack[] tracks)
            : base(trackIndices)
        {
            this.tracks = tracks;
        }

        public ITrack[] Tracks
        {
            get { return this.tracks; }
        }
    }
}
