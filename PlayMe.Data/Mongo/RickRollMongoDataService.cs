using PlayMe.Common.Model;

namespace PlayMe.Data.Mongo
{
    public class RickRollMongoDataService: MongoDataService<RickRoll>
    {
        protected override string DataCollectionName
        {
            get { return "RickRolls"; }
        }     
    }
}
