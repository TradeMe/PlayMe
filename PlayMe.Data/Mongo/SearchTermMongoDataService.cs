using PlayMe.Common.Model;

namespace PlayMe.Data.Mongo
{
    public class SearchTermMongoDataService : MongoDataService<SearchTerm>
    {
        protected override string DataCollectionName
        {
            get { return "SearchHistory"; }
        }
    }
}
