using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class Artist : PlayMeObject
    {
        public enum LoadedState
        {
            Basic,
            FullNoTracks,
            FullWithTracks
        }

        public virtual ArtistProfile Profile { get; set; }
        public virtual string PortraitId { get; set; }
        public virtual string PortraitUrlLarge { get; set; }
        public virtual string PortraitUrlMedium { get; set; }
        public virtual string PortraitUrlSmall { get; set; }
        public virtual IEnumerable<Album> Albums { get; set; }
        public virtual IEnumerable<Track> Tracks { get; set; }
        public virtual LoadedState LoadStatus { get; set; }
    }
}