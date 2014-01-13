using System.Collections.Generic;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Queries;

namespace PlayMe.Server
{
    public class SearchSuggestionService : ISearchSuggestionService
    {
        private readonly IDataService<SearchTerm> searchTermDataService;
        private readonly INowHelper nowHelper;
        private const int maxResultSize = 10;

        public SearchSuggestionService(IDataService<SearchTerm> searchTermDataService, INowHelper nowHelper)
        {
            this.nowHelper = nowHelper;
            this.searchTermDataService = searchTermDataService;
        }

        public IEnumerable<string> GetSearchSuggestions(string partialSearchTerm)
        {
            return searchTermDataService.GetAll()
                .GetMatchingSearchTerms(partialSearchTerm, maxResultSize);
        }

        public void UpdateSearchTermRecords(string searchTerm)
        {
            var searchTermObject = searchTermDataService.GetAll().GetSearchTerm(searchTerm);
            if (searchTermObject != null) // if term exists, update date and count
            {
                searchTermObject.SearchCount++;
                searchTermObject.LastDateSearched = nowHelper.Now;
                searchTermDataService.Update(searchTermObject);
            }
            else  // if the term doesn't exist, create and add new one
            {
                searchTermObject = new SearchTerm(searchTerm);
                searchTermDataService.Insert(searchTermObject);
            }
        }
    }
}

