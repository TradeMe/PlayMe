namespace PlayMe.Common.Model
{
    public class SearchResults
    {
        public string DidYouMean { get; set; }
        public ArtistPagedList PagedArtists { get; set; }
        public AlbumPagedList PagedAlbums { get; set; }
        public TrackPagedList PagedTracks { get; set; }
    }
}