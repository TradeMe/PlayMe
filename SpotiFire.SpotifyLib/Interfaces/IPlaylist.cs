
namespace SpotiFire.SpotifyLib
{
    public interface IPlaylist : ISpotifyObject, IAsyncLoaded
    {
        string Name { get; set; }
        IPlaylistTrackList Tracks { get; }
        string ImageId { get; }
        bool IsColaberativ { get; set; }
        void AutoLinkTracks(bool autoLink);
        string Description { get; }
        bool PendingChanges { get; }

        event PlaylistEventHandler<TracksAddedEventArgs> TracksAdded;
        event PlaylistEventHandler<TracksEventArgs> TracksRemoved;
        event PlaylistEventHandler<TracksMovedEventArgs> TracksMoved;
        event PlaylistEventHandler Renamed;
        event PlaylistEventHandler StateChanged;
        event PlaylistEventHandler<PlaylistUpdateEventArgs> UpdateInProgress;
        event PlaylistEventHandler MetadataUpdated;
        event PlaylistEventHandler<TrackCreatedChangedEventArgs> TrackCreatedChanged;
        event PlaylistEventHandler<TrackSeenEventArgs> TrackSeenChanged;
        event PlaylistEventHandler<DescriptionEventArgs> DescriptionChanged;
        event PlaylistEventHandler<ImageEventArgs> ImageChanged;
    }
}
