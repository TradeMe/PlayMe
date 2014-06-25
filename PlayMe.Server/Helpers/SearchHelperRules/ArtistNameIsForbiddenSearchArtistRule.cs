using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.SearchHelperRules.Interfaces;

namespace PlayMe.Server.Helpers.SearchHelperRules
{
    public class ArtistNameIsForbiddenSearchArtistRule : ISearchArtistRule
    {
        private readonly ISearchRuleSettings searchRuleSettings;
        public ArtistNameIsForbiddenSearchArtistRule(ISearchRuleSettings searchRuleSettings)
        {
            this.searchRuleSettings = searchRuleSettings;
        }

        public bool IsArtistRestricted(Artist artist)
        {
            if (artist == null || artist.Name == null) return true;
            char[] comma = { ',' };
            var names = searchRuleSettings.ForbiddenArtistNames.Split(comma);

            if (!names.Any() || (names.Count() == 1 && names.FirstOrDefault() == string.Empty)) return false;

            return searchRuleSettings.ForbiddenTrackNames != string.Empty &&
                   names.Any(s => artist.Name.ToUpper().Contains(s.ToUpper()));
        }
    }
}
