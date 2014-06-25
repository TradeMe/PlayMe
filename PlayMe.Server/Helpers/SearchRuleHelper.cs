using System.Collections.Generic;
using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.SearchHelperRules.Interfaces;

namespace PlayMe.Server.Helpers
{
    public class SearchRuleHelper : ISearchRuleHelper
    {
        private readonly IEnumerable<ISearchTrackRule> searchTrackRules;
        private readonly IEnumerable<ISearchAlbumRule> searchAlbumRules;
        private readonly IEnumerable<ISearchArtistRule> searchArtistRules;
        public SearchRuleHelper(IEnumerable<ISearchTrackRule> searchTrackRules, IEnumerable<ISearchAlbumRule> searchAlbumRules, IEnumerable<ISearchArtistRule> searchArtistRules)
        {
            this.searchTrackRules = searchTrackRules;
            this.searchAlbumRules = searchAlbumRules;
            this.searchArtistRules = searchArtistRules;
        }

        public TrackPagedList FilterTracks(TrackPagedList pagedTracks)
        {
            IList<Track> filteredTracks = pagedTracks.Tracks.Where(track => !searchTrackRules.Any(s => s.IsTrackRestricted(track))).ToList();

            pagedTracks.Tracks = filteredTracks;
            pagedTracks.Total = pagedTracks.Tracks.Count();
            return pagedTracks;
        }

        public ArtistPagedList FilterArtists(ArtistPagedList pagedArtists)
        {
            IList<Artist> filteredArtists = pagedArtists.Artists.Where(artist => !searchArtistRules.Any(s => s.IsArtistRestricted(artist))).ToList();

            pagedArtists.Artists = filteredArtists;
            pagedArtists.Total = pagedArtists.Artists.Count();
            return pagedArtists;
        }

        public AlbumPagedList FilterAlbums(AlbumPagedList pagedAlbums)
        {
            IList<Album> filteredAlbums = pagedAlbums.Albums.Where(album => !searchAlbumRules.Any(s => s.IsAlbumRestricted(album))).ToList();

            pagedAlbums.Albums = filteredAlbums;
            pagedAlbums.Total = pagedAlbums.Albums.Count();
            return pagedAlbums;
        }
    }
}