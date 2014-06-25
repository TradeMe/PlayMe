using System;
using System.Linq;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.Extensions;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Providers;
using PlayMe.Server.Queries;

namespace PlayMe.Server
{
    public class MusicPlayer : IMusicPlayer
    {
        private readonly ILogger logger;
        private readonly IMusicProviderFactory musicProviderFactory;
        private IMusicProvider currentProvider;
        private readonly IRickRollService rickRollService;

        private DateTime? lastPaused;
        private long totalPausedDuration;
        private QueuedTrack currentlyPlayingTrack;
        private readonly IRepository<QueuedTrack> _queuedTrackRepository;
        private readonly IAlreadyQueuedHelper alreadyQueuedHelper;
        private readonly ICallbackClient callbackClient;
        private readonly INowHelper nowHelper;

        public MusicPlayer(
            ILogger logger, 
            IMusicProviderFactory musicProviderFactory, 
            IRickRollService rickRollService, 
            IRepository<QueuedTrack> _queuedTrackRepository,
            IAlreadyQueuedHelper alreadyQueuedHelper, 
            ICallbackClient callbackClient,
            INowHelper nowHelper)
        {
            this.nowHelper = nowHelper;
            this.callbackClient = callbackClient;
            this.alreadyQueuedHelper = alreadyQueuedHelper;
            this._queuedTrackRepository = _queuedTrackRepository;
            this.rickRollService = rickRollService;
            this.musicProviderFactory = musicProviderFactory;
            this.logger = logger;
            this.callbackClient = callbackClient;
        }

        public QueuedTrack CurrentlyPlayingTrack
        {
            get
            {
                if (currentlyPlayingTrack != null)
                {
                    currentlyPlayingTrack.PausedDurationAsMilliseconds = totalPausedDuration + GetLastPauseDuration();
                }
                return currentlyPlayingTrack;
            }
        }

        #region Public Methods

        public void PlayTrack(QueuedTrack trackToPlay)
        {
            logger.Trace("MusicService.PlayTrack");
            logger.Debug("Attempting to play track {0}", trackToPlay.ToLoggerFriendlyTrackName());

            //Reset the paused data
            lastPaused = null;
            totalPausedDuration = 0;

            currentProvider = musicProviderFactory.GetMusicProviderByIdentifier(trackToPlay.Track.MusicProvider.Identifier);

            //Either play the original queued track or the Big Rick if tthe track is rickrolled
            var rickRollTrack = rickRollService.RickRoll(trackToPlay.Track, trackToPlay.User);

            currentProvider.PlayTrack(rickRollTrack);
            trackToPlay.StartedPlayingDateTime = nowHelper.Now;
            _queuedTrackRepository.InsertOrUpdate(trackToPlay);
            int total;
            var recentlyPlayed = _queuedTrackRepository.GetAll()
                .GetQueuedTracksByUser(null, 1, 5, out total)
                .Select(r => alreadyQueuedHelper.ResetAlreadyQueued(r, trackToPlay.User)).ToList();

            callbackClient.TrackHistoryChanged(new PagedResult<QueuedTrack> { HasMorePages = false, PageData = recentlyPlayed });

            currentlyPlayingTrack = trackToPlay;
            callbackClient.PlayingTrackChanged(CurrentlyPlayingTrack);
            logger.Debug("Playing track {0} queued by {1}", trackToPlay.ToLoggerFriendlyTrackName(), trackToPlay.User);
        }
        
        public void PauseTrack(string user)
        {
            if (currentlyPlayingTrack.IsPaused) return;

            currentProvider.Player.Pause();
            lastPaused = nowHelper.Now;
            currentlyPlayingTrack.IsPaused = true;
            logger.Info("{0} paused the current track {1}", user, currentlyPlayingTrack.ToLoggerFriendlyTrackName());
        }

        public void EndTrack()
        {
            currentProvider.EndTrack();
        }

        public void ResumeTrack(string user)
        {
            if (!currentlyPlayingTrack.IsPaused) return;

            currentProvider.Player.Resume();
            currentlyPlayingTrack.IsPaused = false;
            totalPausedDuration += GetLastPauseDuration();
            lastPaused = null;

            logger.Info("{0} resumed the current track {1}", user, currentlyPlayingTrack.ToLoggerFriendlyTrackName());
        }

        public void IncreaseVolume()
        {
            currentProvider.Player.IncreaseVolume();
        }

        public void DecreaseVolume()
        {
            currentProvider.Player.DecreaseVolume();
        }

        #endregion 

        private long GetLastPauseDuration()
        {
            if (lastPaused.HasValue)
            {
                return (long) nowHelper.Now.Subtract(lastPaused.Value).TotalMilliseconds;
            }

            return 0;
        }
    }
}
