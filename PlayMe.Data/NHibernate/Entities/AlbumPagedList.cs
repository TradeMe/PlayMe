using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class AlbumPagedList
    {
        public int Total{get;set;}

        public IEnumerable<Album> Albums{get;set;}
    }
}
