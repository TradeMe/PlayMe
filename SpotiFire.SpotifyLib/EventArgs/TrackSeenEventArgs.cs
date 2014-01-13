
namespace SpotiFire.SpotifyLib
{
    public class TrackSeenEventArgs : TrackEventArgs
    {
        bool seen;
        public TrackSeenEventArgs(ITrack track, bool seen)
            : base(track)
        {
            this.seen = seen;
        }

        public bool Seen
        {
            get { return this.seen; }
        }

    }
}
