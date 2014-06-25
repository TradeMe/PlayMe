using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class ArtistPagedList
    {
        public int Total {get; set; }
        public IEnumerable<Artist> Artists {get; set; }
    }
}