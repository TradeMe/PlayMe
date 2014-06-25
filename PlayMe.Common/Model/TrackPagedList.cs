using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class TrackPagedList
    {
        public int Total { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
    }
}
