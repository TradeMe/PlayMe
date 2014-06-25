using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class Playlist:PlayMeObject
    {
        public string Description { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
    }
}
