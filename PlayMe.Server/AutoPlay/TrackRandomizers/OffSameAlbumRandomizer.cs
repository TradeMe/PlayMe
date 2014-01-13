using System;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.AutoPlay.TrackRandomizers.Interfaces;
using PlayMe.Server.Extensions;
using PlayMe.Server.Providers;

namespace PlayMe.Server.AutoPlay.TrackRandomizers
{
    public class OffSameAlbumRandomizer : ITrackRandomizer
    {
        private IMusicProvider musicProvider;
        private readonly IMusicProviderFactory musicProviderFactory;
        private readonly Settings settings;
        private readonly ILogger logger;

        public OffSameAlbumRandomizer(IMusicProviderFactory musicProviderFactory, ILogger logger)
        {
            this.settings = new Settings();
            this.musicProviderFactory = musicProviderFactory;
            this.logger = logger;
        }

        public int Version
        {
            get { return 3; }
        }

        public QueuedTrack Execute(QueuedTrack trackToQueue)
        {
            if (trackToQueue.Track.MusicProvider == null || trackToQueue.Track.MusicProvider.Identifier != "sp") return trackToQueue;

            if (DateTime.Now.Second%settings.RandomizerRatio == 0)
            {
                return trackToQueue;
            }

            musicProvider = musicProviderFactory.GetMusicProviderByIdentifier(trackToQueue.Track.MusicProvider.Identifier);

            var album = trackToQueue.Track.Album;

            if (album == null) return trackToQueue;

            var tracks = musicProvider.BrowseAlbum(album.Link, trackToQueue.User);

            if (tracks.Tracks != null && tracks.Tracks.Any())
            {
                var track = tracks.Tracks.Random().FirstOrDefault();

                if (track != null)
                {
                    logger.Debug("Off Same Album Randomizer, converting track {0}", trackToQueue.ToLoggerFriendlyTrackName());

                    trackToQueue.Track = track;
                    trackToQueue.Track.Album = album;
                    trackToQueue.User = "AutoPlay-OffSameAlbum";
                    trackToQueue.Id = DataObject.GenerateId(); //if you don't do this then the autoplayed song will update over the user requested song
                }
            }


            return trackToQueue;
        }
    }
}