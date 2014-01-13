using System.Collections.Generic;
using System.Configuration;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server
{
    public class UserSettings : IUserSettings
    {
        public IEnumerable<string> AdminUsers
        {
            get
            {
                return ConfigurationManager.AppSettings["AdminUsers"].Split(';');
            }
        }

        public string Domain
        {
            get
            {
                return ConfigurationManager.AppSettings["Domain"];
            }
        }
    }
}
