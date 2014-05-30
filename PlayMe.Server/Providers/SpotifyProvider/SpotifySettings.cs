using System;
using System.Configuration;

namespace PlayMe.Server.Providers.SpotifyProvider
{
    public class SpotifySettings : ISpotifySettings
    {
        public bool IsEnabled
        {
            get
            {
                Boolean parsed;
                string unparsed = ConfigurationManager.AppSettings["Spotify.IsEnabled"];
                return Boolean.TryParse(unparsed, out parsed) && parsed;
            }
        }
        public string UserName { get { return ConfigurationManager.AppSettings["Spotify.UserName"]; } }
        public string Password { get { return ConfigurationManager.AppSettings["Spotify.Password"]; } }
        public byte[] ApplicationKey { get { return Convert.FromBase64String(ConfigurationManager.AppSettings["Spotify.ApplicationKey"]); } }
    }
}
