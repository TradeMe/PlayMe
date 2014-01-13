using PlayMe.Common.Model;

namespace PlayMe.Data.Mongo
{
    public class QueuedTrackMongoDataService: MongoDataService<QueuedTrack>, ISharedCollection<QueuedTrack>
    {       
        protected override string DataCollectionName
        {
            get { return "TrackHistory"; }
        }     
    }
}
