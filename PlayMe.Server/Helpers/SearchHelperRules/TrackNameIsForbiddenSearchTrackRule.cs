using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.SearchHelperRules.Interfaces;

namespace PlayMe.Server.Helpers.SearchHelperRules
{
    public class TrackNameIsForbiddenSearchTrackRule : ISearchTrackRule
    {
        private readonly ISearchRuleSettings searchRuleSettings;

        public TrackNameIsForbiddenSearchTrackRule(ISearchRuleSettings searchRuleSettings)
        {
            this.searchRuleSettings = searchRuleSettings;
        }

        public bool IsTrackRestricted(Track track)
        {
            if (track == null || track.Name == null) return true;
            char[] comma = {','};

            var names = searchRuleSettings.ForbiddenTrackNames.Split(comma);

            if (!names.Any() || (names.Count() == 1 && names.FirstOrDefault() == string.Empty)) return false;

            return searchRuleSettings.ForbiddenTrackNames != string.Empty &&
                   names.Any(s => track.Name.ToUpper().Contains(s.ToUpper()));
        }
    }
}
