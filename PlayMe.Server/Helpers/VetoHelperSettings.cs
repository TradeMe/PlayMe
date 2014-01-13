using System.Configuration;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.Server.Helpers
{
    public class VetoHelperSettings : IVetoHelperSettings
    {
        public int DailyVetoLimit
        {
            get
            {
                int parsed;
                string unparsed = ConfigurationManager.AppSettings["VetoHelper.ExceededDailyLimitVetoRule.DailyVetoLimit"];
                return int.TryParse(unparsed, out parsed) ? parsed : 20;
            }
        }
    }
}
