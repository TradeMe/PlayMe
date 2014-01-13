using System;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server
{
    public class NowHelper : INowHelper
    {
        public DateTime Now {
            get { return DateTime.Now; }
        }
    }
}