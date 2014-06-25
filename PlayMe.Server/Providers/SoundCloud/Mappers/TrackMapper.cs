using System;
using System.Globalization;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.Server.Providers.SoundCloud.Mappers
{
    public class TrackMapper : ITrackMapper
    {
        private readonly IAlreadyQueuedHelper alreadyQueuedHelper;

        public TrackMapper(IAlreadyQueuedHelper alreadyQueuedHelper)
        {
            this.alreadyQueuedHelper = alreadyQueuedHelper;
        }

        public Track Map(Model.Track track, SoundCloudMusicProvider musicProvider, string user, bool mapArtists = false)
        {
            var trackLink = track.id;
            var trackResult = new Track
            {
                Link = trackLink.ToString(CultureInfo.InvariantCulture),
                Name = track.title,
                IsAvailable = track.streamable,
                Duration = new TimeSpan(0,0,0,0,track.duration),
                DurationMilliseconds = track.duration,   
                MusicProvider = musicProvider.Descriptor,
                TrackArtworkUrl = track.artwork_url ?? "content/styles/Images/soundcloud-icon.png",
                ExternalLink = new Uri(track.permalink_url)
            };
            alreadyQueuedHelper.ResetAlreadyQueued((trackResult),user);
            
            return trackResult;
        }
    }
}