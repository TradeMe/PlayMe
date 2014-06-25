using System;
using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class ArtistProfile
    {
        public string Biography { get; set; }
        public IEnumerable<Guid> SimilarArtistsIds { get; set; }
    }
}