using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.Mongo
{
    public class QueuedTrackMongoRepository: MongoRepository<QueuedTrack>, ISharedCollection<QueuedTrack>
    {       
        protected override string DataCollectionName
        {
            get { return "TrackHistory"; }
        }     
    }
}
