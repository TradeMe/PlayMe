using System.Configuration;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.Server.Helpers
{
    public class SearchRuleSettings : ISearchRuleSettings
    {
        public string ForbiddenTrackNames
        {
            get { return ConfigurationManager.AppSettings["SearchRuleSetting.ForbiddenTrackNames"]; }
        }

        public string ForbiddenAlbumNames
        {
            get { return ConfigurationManager.AppSettings["SearchRuleSetting.ForbiddenAlbumNames"]; }
        }

        public string ForbiddenArtistNames
        {
            get { return ConfigurationManager.AppSettings["SearchRuleSetting.ForbiddenArtistNames"]; }
        }
    }
}
