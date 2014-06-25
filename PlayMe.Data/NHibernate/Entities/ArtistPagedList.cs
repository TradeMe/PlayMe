using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class ArtistPagedList
    {
        public int Total {get; set; }
        public IEnumerable<Artist> Artists {get; set; }
    }
}