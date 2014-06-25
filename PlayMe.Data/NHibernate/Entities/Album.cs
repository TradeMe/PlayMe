using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class Album:PlayMeObject
    {        
        public virtual Artist Artist { get; set; }
        public virtual int Year { get; set; }
        public virtual string ArtworkId { get; set; }
        public virtual IEnumerable<Track> Tracks { get; set; }
        public virtual bool IsAvailable { get; set; }
        public virtual string ArtworkUrlLarge { get; set; }
        public virtual string ArtworkUrlMedium { get; set; }
        public virtual string ArtworkUrlSmall { get; set; }
    }
}