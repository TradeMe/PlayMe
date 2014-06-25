using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.Mongo
{
    public class SoundBoardInfoMongoRepository : MongoRepository<SoundBoardInfo>
    {
        protected override string DataCollectionName
        {
            get { return "SoundBoardInfo"; }
        }     
    }
}
