using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class PlaylistPagedList
    {
        public int Total { get; set; }
        public IEnumerable<Playlist> Playlists { get; set; }
    }
}
