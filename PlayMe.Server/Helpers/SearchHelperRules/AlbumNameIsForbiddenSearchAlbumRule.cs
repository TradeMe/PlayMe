using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.SearchHelperRules.Interfaces;

namespace PlayMe.Server.Helpers.SearchHelperRules
{
    public class AlbumNameIsForbiddenSearchAlbumRule : ISearchAlbumRule
    {
        private readonly ISearchRuleSettings searchRuleSettings;
        public AlbumNameIsForbiddenSearchAlbumRule(ISearchRuleSettings searchRuleSettings)
        {
            this.searchRuleSettings = searchRuleSettings;
        }

        public bool IsAlbumRestricted(Album album)
        {
            if (album == null || album.Name == null) return true;

            char[] comma = { ',' };
            var names = searchRuleSettings.ForbiddenAlbumNames.Split(comma);

            if (!names.Any() || (names.Count() ==1 && names.FirstOrDefault() == string.Empty)) return false;

            return searchRuleSettings.ForbiddenTrackNames != string.Empty &&
                   names.Any(s => album.Name.ToUpper().Contains(s.ToUpper()));
        }
    }
}
