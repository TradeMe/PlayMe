using System;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers.Interfaces;
using S = SpotiFire.SpotifyLib;

namespace PlayMe.Server.Providers.SpotifyProvider.Mappers
{
    public class TrackMapper : ITrackMapper
    {
        private readonly IAlreadyQueuedHelper alreadyQueuedHelper;
        private readonly IAlbumMapper albumMapper;
        private readonly IArtistMapper artistMapper;

        public TrackMapper(IAlbumMapper albumMapper, IArtistMapper artistMapper, IAlreadyQueuedHelper alreadyQueuedHelper)
        {
            this.artistMapper = artistMapper;
            this.albumMapper = albumMapper;
            this.alreadyQueuedHelper = alreadyQueuedHelper;
        }

        public Track Map(S.ITrack track, SpotifyMusicProvider musicProvider, string user, bool mapAlbum=false,bool mapArtists=false)
        {
            string trackLink = track.GetLinkString();
            var trackResult = new Track
            {
                Link = trackLink,
                Name = track.Name,
                IsAvailable = track.IsAvailable,
                Duration = track.Duration,
                DurationMilliseconds = (long)track.Duration.TotalMilliseconds,
                MusicProvider = musicProvider.Descriptor,
                ExternalLink = new Uri(musicProvider.ExternalLinkBaseUrl, "/track/" + trackLink)
            };

            if (mapAlbum && track.Album != null)
            {
                trackResult.Album = albumMapper.Map(track.Album, musicProvider);
            }
            if (mapArtists && track.Artists != null)
            {
                trackResult.Artists = track.Artists.Select(t => artistMapper.Map(t,musicProvider)).ToArray();
            }

            //We want to set whether the track is already queued 
            if(alreadyQueuedHelper!=null)
            {
                trackResult = alreadyQueuedHelper.ResetAlreadyQueued((trackResult), user);
            }
            return trackResult;
        }
    }
}