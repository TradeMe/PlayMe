using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class Playlist:PlayMeObject
    {
        public string Description { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
    }
}
