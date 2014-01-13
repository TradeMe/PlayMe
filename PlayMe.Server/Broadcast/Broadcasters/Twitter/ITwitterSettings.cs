using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayMe.Server.Broadcast.Broadcasters.Twitter
{
    public interface ITwitterSettings
    {
        bool IsEnabled { get; }
        string ConsumerKey { get; }
        string ConsumerSecret { get; }
        string Token { get; }
        string TokenSecret { get; }
    }
}
