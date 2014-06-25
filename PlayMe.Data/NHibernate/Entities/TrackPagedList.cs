using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class TrackPagedList
    {
        public int Total { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
    }
}
