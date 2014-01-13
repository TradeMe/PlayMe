using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PlayMe.Common.Model;

namespace PlayMe.Data.Mongo
{
    public class LogEntryMongoDataService //: MongoDataService<LogEntry>
    {
        public LogEntryMongoDataService()
        {
            lock (this)
            {
                if (!BsonClassMap.IsClassMapRegistered(typeof(LogEntry)))
                {
                    var noIdConventions = new ConventionProfile();
                    noIdConventions.SetIdMemberConvention(new NamedIdMemberConvention()); // no names
                    BsonClassMap.RegisterConventions(noIdConventions, t => t == typeof(LogEntry));
                    BsonClassMap.RegisterClassMap<LogEntry>(cm => cm.AutoMap());
                }
            }
        }

        protected /*override*/ string DataCollectionName
        {
            get { return "Logging"; }
        }

        public IQueryable<LogEntry> Get()
        {
            var settings = new MongoSettings();
            var server = new MongoClient(settings.DatabaseConnectionString).GetServer();
            var db = server.GetDatabase(settings.DatabaseName, SafeMode.True);
            
            return db.GetCollection<LogEntry>(DataCollectionName).AsQueryable();
        }
    }
}
