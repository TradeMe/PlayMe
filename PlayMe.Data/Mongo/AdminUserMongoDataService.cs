using PlayMe.Common.Model;

namespace PlayMe.Data.Mongo
{
    public class AdminUserMongoDataService : MongoDataService<User>
    {
        protected override string DataCollectionName
        {
            get { return "AdminUsers"; }
        }
    }
}
