using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class PlaylistPagedList
    {
        public int Total { get; set; }
        public IEnumerable<Playlist> Playlists { get; set; }
    }
}
