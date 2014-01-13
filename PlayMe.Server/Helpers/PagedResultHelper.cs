using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;

namespace PlayMe.Server.Helpers
{
    public class PagedResultHelper
    {
        public PagedResult<T> GetPagedResult<T>(int start, int total, IEnumerable<T> thisPage)
        {
            var pageData = thisPage as IList<T> ?? thisPage.ToList();
            return new PagedResult<T>
                       {
                           PageData = pageData,
                           HasMorePages = (total > start + pageData.Count()) 
                       };
        }
    }
}
