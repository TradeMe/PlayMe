using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.Mongo
{
    public class RickRollMongoRepository: MongoRepository<RickRoll>
    {
        protected override string DataCollectionName
        {
            get { return "RickRolls"; }
        }     
    }
}
