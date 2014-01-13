using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class ArtistProfile
    {
        public string Biography { get; set; }
        public IEnumerable<Artist> SimilarArtists { get; set; }
    }
}