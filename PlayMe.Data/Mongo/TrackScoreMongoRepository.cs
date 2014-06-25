using System;
using System.Linq;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.Mongo
{
    public class TrackScoreMongoRepository : MongoRepository<MapReduceResult<TrackScore>>
    {
        public TrackScoreMongoRepository(ISharedCollection<QueuedTrack> queuedTrackDataService )
        {
            this.queuedTrackDataService = queuedTrackDataService;
        }

        private readonly ISharedCollection<QueuedTrack> queuedTrackDataService;

        protected override string DataCollectionName
        {
            get { return "TrackScores"; }
        }

        public override IQueryable<MapReduceResult<TrackScore>> GetAll()
        {
            MapReduce();
            return DataCollection.AsQueryable();
        }

        private void MapReduce()
        {
            var map = string.Format(@"
            function() {{
                var queuedTrack = this;
                var requestCount = (queuedTrack.User.toLowerCase().indexOf('{0}') == 0) ? 0 : 1;
                var autoPlayCount = (requestCount==1) ? 0 : 1;                               
                emit(queuedTrack.Track.Link, {{ requestCount: requestCount, vetoCount: queuedTrack.Vetoes.length, likeCount: queuedTrack.Likes.length, autoPlayCount: autoPlayCount,lastPlayed: Date.parse(queuedTrack.StartedPlayingDateTime),excluded: queuedTrack.Excluded, track: queuedTrack.Track}});
            }}", "autoplay");

            const string reduce = @"        
            function(key, values) {
                var result = {requestCount: 0, vetoCount: 0, likeCount: 0, autoPlayCount: 0, lastPlayed: 0, excluded: false, track: null };

                values.forEach(function(value){               
                    result.requestCount += value.requestCount;
                    result.vetoCount += value.vetoCount;
                    result.likeCount += value.likeCount;
                    result.autoPlayCount += value.autoPlayCount;                    
                    if(result.excluded == false) result.excluded = value.excluded; //waiting until we find at least one play 
                    if (value.lastPlayed > result.lastPlayed) result.lastPlayed = value.lastPlayed;
                    if (!result.track) result.track = value.track;
                });

                return result;
            }";
            string finalize = string.Format(@"
            function(key, value){{                
                var score =  (value.likeCount - value.vetoCount)+1;
                if (score <= 0) score = 0.001;
                score = Math.round((score * (Math.random()/{0}))*1000);                 
                return {{IsExcluded: value.excluded, Score: score, MillisecondsSinceLastPlay: Date.now()-value.lastPlayed, Track: value.track}};
            }}", 3);

            var options = new MapReduceOptionsBuilder();
            options.SetFinalize(finalize);            
            options.SetOutput(MapReduceOutput.Replace(DataCollectionName));
            if (queuedTrackDataService.DataCollection.Exists())
            {
                queuedTrackDataService.DataCollection.MapReduce(map, reduce, options).GetResults();
            }
        }
        
        public override void Update(MapReduceResult<TrackScore> entity)
        {
            throw new NotImplementedException();
        }

        public override void Insert(MapReduceResult<TrackScore> entity)
        {            
            throw new NotImplementedException();
        }
    }
}
