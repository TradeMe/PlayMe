
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayMe.Server.Broadcast.Broadcasters.Twitter
{
    public class TwitterSettings: ITwitterSettings
    {
        public bool IsEnabled
        {
            get
            {
                bool parsed;
                string unparsed = ConfigurationManager.AppSettings["TwitterBroadcaster.IsEnabled"];
                return bool.TryParse(unparsed, out parsed) && parsed;
            }
        }
        public string ConsumerKey { get { return ConfigurationManager.AppSettings["TwitterBroadcaster.ConsumerKey"]; } }
        public string ConsumerSecret { get { return ConfigurationManager.AppSettings["TwitterBroadcaster.ConsumerSecret"]; } }
        public string Token { get { return ConfigurationManager.AppSettings["TwitterBroadcaster.Token"]; } }
        public string TokenSecret { get { return ConfigurationManager.AppSettings["TwitterBroadcaster.TokenSecret"]; } }
    }
}
