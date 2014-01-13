using PlayMe.Common.Model;

namespace PlayMe.Data.Mongo
{
    public class SoundBoardInfoMongoDataService : MongoDataService<SoundBoardInfo>
    {
        protected override string DataCollectionName
        {
            get { return "SoundBoardInfo"; }
        }     
    }
}
