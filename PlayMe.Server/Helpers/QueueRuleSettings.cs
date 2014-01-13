using System.Configuration;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.Server.Helpers
{
    public class QueueRuleSettings : IQueueRuleSettings
    {
        public int QueueCount
        {
            get
            {
                int queueCount = 0;
                return !int.TryParse(
                    ConfigurationManager.AppSettings["QueueRuleSetting.QueueCount"], out queueCount) ? 5 : queueCount;
            }
        }


        public int LastXHours
        {
            get
            {
                int hours = 0;
                return !int.TryParse(
                    ConfigurationManager.AppSettings["QueueRuleSetting.LastXHours"], out hours) ? 4 : hours;
            }
        }
    }
}
