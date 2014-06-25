using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class PagedResult<T>
    {        
        public IEnumerable<T> PageData {get; set; }        
        public bool HasMorePages { get; set; }
    }
}