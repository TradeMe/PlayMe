
using System;
namespace SpotiFire.SpotifyLib
{
    public class TrackCreatedChangedEventArgs : TrackEventArgs
    {
        private DateTime when;
        //TODO: Implement user private User who
        public TrackCreatedChangedEventArgs(ITrack track, DateTime when/*, User who*/)
            : base(track)
        {
            this.when = when;
            //this.who = who;
        }
    }
}
