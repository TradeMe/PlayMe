using PlayMe.Common.Model;
namespace PlayMe.Server.Helpers.SearchHelperRules.Interfaces
{
    public interface ISearchTrackRule
    {
        bool IsTrackRestricted(Track track);
    }
}
