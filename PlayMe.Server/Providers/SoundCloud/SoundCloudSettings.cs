using System;
using System.Configuration;

namespace PlayMe.Server.Providers.SoundCloud
{
    public class SoundCloudSettings : ISoundCloudSettings
    {
        public bool IsEnabled
        {
            get
            {
                Boolean parsed;
                string unparsed = ConfigurationManager.AppSettings["SoundCloud.IsEnabled"];
                return Boolean.TryParse(unparsed, out parsed) && parsed;
            }
        }
        public string UserName
        {
            get { return ConfigurationManager.AppSettings["SoundCloud.UserName"]; }
        }
        public string Password
        {
            get { return ConfigurationManager.AppSettings["SoundCloud.Password"]; }
        }

        public string ClientId
        {
            get { return ConfigurationManager.AppSettings["SoundCloud.ClientId"]; } 
        }
        public string ClientSecret 
        { 
            get { return ConfigurationManager.AppSettings["SoundCloud.ClientSecret"]; } 
        }
    }
}
