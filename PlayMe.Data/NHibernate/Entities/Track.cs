using System;
using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class Track : PlayMeObject
    {
        public virtual IEnumerable<Artist> Artists { get; set; }
        public virtual Album Album { get; set; }
        public virtual TimeSpan Duration { get; set; }
        public virtual long DurationMilliseconds { get; set; }
        public virtual bool IsAlreadyQueued { get; set; }
        public virtual bool IsAvailable { get; set; }
        public virtual int Popularity { get; set; }
        public virtual string TrackArtworkUrl { get; set; }
        public virtual string Reason { get; set; }
    }
}