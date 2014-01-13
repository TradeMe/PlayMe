using System.Configuration;

namespace PlayMe.Data.Mongo
{
    public class MongoSettings
    {
        public string DatabaseConnectionString
        {
            get { return ConfigurationManager.AppSettings["DatabaseConnectionString"]; }
        }

        public string DatabaseName
        {
            get { return ConfigurationManager.AppSettings["DatabaseName"]; }
        }
    }
}
