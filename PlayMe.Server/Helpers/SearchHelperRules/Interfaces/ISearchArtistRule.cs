using PlayMe.Common.Model;

namespace PlayMe.Server.Helpers.SearchHelperRules.Interfaces
{
    public interface ISearchArtistRule
    {
        bool IsArtistRestricted(Artist artist);
    }
}
