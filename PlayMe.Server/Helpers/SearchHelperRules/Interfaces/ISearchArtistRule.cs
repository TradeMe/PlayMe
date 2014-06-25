using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.SearchHelperRules.Interfaces
{
    public interface ISearchArtistRule
    {
        bool IsArtistRestricted(Artist artist);
    }
}
