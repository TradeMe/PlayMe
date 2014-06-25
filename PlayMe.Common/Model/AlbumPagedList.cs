using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class AlbumPagedList
    {
        public int Total{get;set;}

        public IEnumerable<Album> Albums{get;set;}
    }
}
