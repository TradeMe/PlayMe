using System.Collections.Generic;
using System.Linq;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Queries
{
    public static class SearchTermQueries
    {

        /// <summary>
        /// Orders all matching terms first by 'search count', then by 'latest date searched', and returns the top 'maxResults' ordered alphabetically
        /// </summary>
        /// <param name="source"></param>
        /// <param name="partialSearchTerm"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetMatchingSearchTerms(this IQueryable<SearchTerm> source, string partialSearchTerm, int maxResults) 
        {
            return source.Where(t =>  t.Term.StartsWith(partialSearchTerm.ToLower()))
                .OrderByDescending(t => t.SearchCount)
                .ThenByDescending(t => t.LastDateSearched)
                .Take(maxResults)
                .Select(t => t.Term)
                .ToArray()
                .OrderBy(t => t); // not sure if there's a better way to do this bit
        }

        /// <summary>
        /// Returns the SearchTerm that matches 'searchTerm' exactly (case insensitive).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="searchTerm"></param>
        /// <returns> or null if not found</returns>
        public static SearchTerm GetSearchTerm(this IQueryable<SearchTerm> source, string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            return source.FirstOrDefault(t => t.Term == searchTerm);
        }


    }
}
