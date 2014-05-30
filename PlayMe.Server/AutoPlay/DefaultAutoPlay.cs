using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Data.Mongo;
using PlayMe.Server.AutoPlay.TrackRandomizers;
using PlayMe.Server.Providers;

namespace PlayMe.Server.AutoPlay
{
    public class DefaultAutoPlay : IAutoPlay
    {
        private readonly IDataService<MapReduceResult<TrackScore>> trackScoreDataService;
        private readonly IDataService<QueuedTrack> queuedTrackDataService;
        private readonly Stack<QueuedTrack> _tracksForAutoplaying = new Stack<QueuedTrack>();
        private readonly Settings settings;
        private readonly IMusicProviderFactory musicProviderFactory;
        private readonly IRandomizerFactory randomizerFactory;

        public DefaultAutoPlay(
            IMusicProviderFactory musicProviderFactory, 
            IDataService<MapReduceResult<TrackScore>> trackScoreDataService,
            IDataService<QueuedTrack> queuedTrackDataService,
            IRandomizerFactory randomizerFactory,
            Settings settings
            )
        {
            
            this.trackScoreDataService = trackScoreDataService;
            this.queuedTrackDataService = queuedTrackDataService;
            this.musicProviderFactory = musicProviderFactory;
            this.randomizerFactory = randomizerFactory;
            this.settings = settings;
        }

        public QueuedTrack FindTrack()
        {
            if (_tracksForAutoplaying.Count <= settings.MinAutoplayableTracks)
            {
                if (_tracksForAutoplaying.Count == 0)
                {
                    //If we have no tracks, we're probably just starting the service, just get the last track played
                    FillBagWithLastTrack();
                }
                // Fill the bag as a non-blocking call
                ThreadPool.QueueUserWorkItem(FillBagWithAutoplayTracks);
            }
            QueuedTrack track = null;
            if (_tracksForAutoplaying.LongCount() > 0)
            {
                track = randomizerFactory.Randomize.Execute(_tracksForAutoplaying.Pop());
            }

            return track;
        }
        
        private void FillBagWithAutoplayTracks(object stateInfo)
        {
            var scoredTracks = PickTracks();
            if (scoredTracks == null)
                return;

            var chosenTracks = scoredTracks;
            foreach (var qt in chosenTracks)
            {
                var queuedTrack = new QueuedTrack();
                var tracksMusicProvider = musicProviderFactory.GetMusicProviderByIdentifier(qt.value.Track.MusicProvider.Identifier);
                if (!tracksMusicProvider.IsEnabled) continue;
                var t = tracksMusicProvider.GetTrack(qt._id, Constants.AutoplayUserName);
                if (t == null) continue;
                queuedTrack.Track = t;
                queuedTrack.User = Constants.AutoplayUserName;

                _tracksForAutoplaying.Push(queuedTrack);
            }
        }

        private void FillBagWithLastTrack()
        {
            var chosenTrack =queuedTrackDataService.GetAll()
                .Where( t => !t.Vetoes.Any())
                .OrderByDescending(qt => qt.StartedPlayingDateTime)
                .FirstOrDefault();
            if(chosenTrack!=null){
                _tracksForAutoplaying.Push(chosenTrack);
            }
        }

        private IEnumerable<MapReduceResult<TrackScore>> PickTracks()
        {
            var minMillisecondsSinceLastPlay = TimeSpan.FromHours(settings.DontRepeatTrackForHours).TotalMilliseconds;
            var results = PickTracks(minMillisecondsSinceLastPlay);
            //If no tracks were returned, it may be that service has only just been installed. Don't restrict tracks based on last played time
            var mapReduceResults = results as IList<MapReduceResult<TrackScore>> ?? results.ToList();
            if (!mapReduceResults.Any()) results = PickTracks(0);
            return mapReduceResults;
        }

        private IEnumerable<MapReduceResult<TrackScore>> PickTracks(double minMillisecondsSinceLastPlay)
        {
            return trackScoreDataService.GetAll()
                .Where(s => !s.value.IsExcluded && s.value.MillisecondsSinceLastPlay > minMillisecondsSinceLastPlay)                
                .OrderByDescending(s => s.value.Score)
                .Take(settings.MaxAutoplayableTracks)
                .ToList();
        }
    }
}

