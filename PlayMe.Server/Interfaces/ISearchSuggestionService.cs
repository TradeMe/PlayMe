using System.Collections.Generic;

namespace PlayMe.Server.Interfaces
{

    public interface ISearchSuggestionService
    {

        IEnumerable<string> GetSearchSuggestions(string partialSearchTerm);

        void UpdateSearchTermRecords(string searchTerm);

    }
}
