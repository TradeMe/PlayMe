using System.Collections.Generic;
using System.Web.Http;
using PlayMe.Common.Model;
using PlayMe.Web.MusicServiceReference;

namespace PlayMe.Web.Controllers
{
    public class SearchController : ApiController
    {
        [HttpGet]
        public SearchResults Default(string searchTerm, string provider)
        {
            using (var client = new MusicServiceClient())
            {
                return client.SearchAll(searchTerm, provider, User.Identity.Name);
            }
        }

        [HttpGet]
        public IEnumerable<string> Suggestions(string partialSearchTerm)
        {
            using (var client = new MusicServiceClient())
            {
                return client.MatchSearchTermHistory(partialSearchTerm);
            }
        
        }

        [HttpGet]
        public IEnumerable<MusicProviderDescriptor> ActiveMusicProviders()
        {
            using (var client = new MusicServiceClient())
            {
                return client.GetActiveProviders();
            }

        }
    }
}
