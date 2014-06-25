using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.Mongo
{
    public class SearchTermMongoRepository : MongoRepository<SearchTerm>
    {
        protected override string DataCollectionName
        {
            get { return "SearchHistory"; }
        }
    }
}
