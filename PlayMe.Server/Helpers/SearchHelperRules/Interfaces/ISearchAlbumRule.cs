using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.SearchHelperRules.Interfaces
{
    public interface ISearchAlbumRule
    {
        bool IsAlbumRestricted(Album album);
    }
}
