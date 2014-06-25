using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class PagedResult<T>
    {        
        public IEnumerable<T> PageData {get; set; }        
        public bool HasMorePages { get; set; }
    }
}