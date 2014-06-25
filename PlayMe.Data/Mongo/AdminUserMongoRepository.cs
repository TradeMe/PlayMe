using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.Mongo
{
    public class AdminUserMongoRepository : MongoRepository<User>
    {
        protected override string DataCollectionName
        {
            get { return "AdminUsers"; }
        }
    }
}
