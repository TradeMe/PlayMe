using PlayMe.Common.Model;
namespace PlayMe.Server.Helpers.Interfaces
{
    public interface ISearchRuleHelper
    {
        TrackPagedList FilterTracks(TrackPagedList pagedTracks);
        ArtistPagedList FilterArtists(ArtistPagedList pagedArtists);
        AlbumPagedList FilterAlbums(AlbumPagedList pagedAlbums);
    }
}
