using System;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.AutoPlay.TrackRandomizers.Interfaces;
using PlayMe.Server.Extensions;
using PlayMe.Server.Providers;

namespace PlayMe.Server.AutoPlay.TrackRandomizers
{
    public class SimilarArtistRandomizer : ITrackRandomizer
    {
        private IMusicProvider musicProvider;
        private readonly IMusicProviderFactory musicProviderFactory;
        private readonly Settings settings;
        private readonly ILogger logger;

        public SimilarArtistRandomizer(IMusicProviderFactory musicProviderFactory, ILogger logger)
        {
            this.settings = new Settings();
            this.logger = logger;
            this.musicProviderFactory = musicProviderFactory;
        }

        public int Version
        {
            get { return 1; }
        }

        public QueuedTrack Execute(QueuedTrack trackToQueue)
        {
            if (DateTime.Now.Second%settings.RandomizerRatio == 0)
            {
                return trackToQueue;
            }

            if (trackToQueue.Track.MusicProvider == null || trackToQueue.Track.MusicProvider.Identifier != "sp") return trackToQueue;

            musicProvider = musicProviderFactory.GetMusicProviderByIdentifier(trackToQueue.Track.MusicProvider.Identifier);

            var browse = musicProvider.BrowseArtist(trackToQueue.Track.Artists.First().Link, false);
            var artist = browse.Profile.SimilarArtists.Random().FirstOrDefault();

            if (artist == null) return trackToQueue;

            var randomAlbum = musicProvider.BrowseArtist(artist.Link, false).Albums.Random().Take(1).ToList();

            if (randomAlbum.Any())
            {
                var albumLink = randomAlbum.Random().First();
                var tracks = musicProvider.BrowseAlbum(albumLink.Link, trackToQueue.User);

                if (tracks.Tracks != null && tracks.Tracks.Any())
                {
                    var track = tracks.Tracks.Random().FirstOrDefault();

                    if (track != null)
                    {
                        logger.Debug("Similar Artist Randomizer, converting track {0}", trackToQueue.ToLoggerFriendlyTrackName());

                        trackToQueue.Track = track;
                        trackToQueue.Track.Album = albumLink;
                        trackToQueue.User = "AutoPlay-SimilarArtist";
                        trackToQueue.Id = DataObject.GenerateId(); //if you don't do this then the autoplayed song will update over the user requested song
                    }
                }
            }

            return trackToQueue;
        }
    }
}