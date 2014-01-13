using PlayMe.Common.Model;
namespace PlayMe.Server.Helpers.SearchHelperRules.Interfaces
{
    public interface ISearchAlbumRule
    {
        bool IsAlbumRestricted(Album album);
    }
}
