using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.SearchHelperRules.Interfaces
{
    public interface ISearchTrackRule
    {
        bool IsTrackRestricted(Track track);
    }
}
