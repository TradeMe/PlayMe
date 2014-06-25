using System.Collections.Generic;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Queries;

namespace PlayMe.Server
{
    public class SearchSuggestionService : ISearchSuggestionService
    {
        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly INowHelper nowHelper;
        private const int maxResultSize = 10;

        public SearchSuggestionService(IRepository<SearchTerm> _searchTermRepository, INowHelper nowHelper)
        {
            this.nowHelper = nowHelper;
            this._searchTermRepository = _searchTermRepository;
        }

        public IEnumerable<string> GetSearchSuggestions(string partialSearchTerm)
        {
            return _searchTermRepository.GetAll()
                .GetMatchingSearchTerms(partialSearchTerm, maxResultSize);
        }

        public void UpdateSearchTermRecords(string searchTerm)
        {
            var searchTermObject = _searchTermRepository.GetAll().GetSearchTerm(searchTerm);
            if (searchTermObject != null) // if term exists, update date and count
            {
                searchTermObject.SearchCount++;
                searchTermObject.LastDateSearched = nowHelper.Now;
                _searchTermRepository.Update(searchTermObject);
            }
            else  // if the term doesn't exist, create and add new one
            {
                searchTermObject = new SearchTerm(searchTerm);
                _searchTermRepository.Insert(searchTermObject);
            }
        }
    }
}

