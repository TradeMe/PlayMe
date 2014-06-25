using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class ArtistProfile
    {
        public string Biography { get; set; }
        public IEnumerable<Artist> SimilarArtists { get; set; }
    }
}