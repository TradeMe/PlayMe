using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.QueueHelperRules.Interfaces;
using PlayMe.Server.Queue.Interfaces;

namespace PlayMe.Server.Helpers.QueueHelperRules
{
    public class LimitNumberOfTracksQueuedByUserQueueRule : IQueueRule
    {
        private readonly IQueueManager queueManager;
        private readonly IQueueRuleSettings queueRuleSettings;

        public LimitNumberOfTracksQueuedByUserQueueRule(IQueueManager queueManager, IQueueRuleSettings queueRuleSettings)
        {
            this.queueManager = queueManager;
            this.queueRuleSettings = queueRuleSettings;
        }

        public string CannotQueue(Track track, string user)
        {
            return queueManager.GetAll().Count(q => q.User == user) >= queueRuleSettings.QueueCount
                ? string.Format("You can only queue {0} tracks at a time.", queueRuleSettings.QueueCount) : string.Empty;
        }
    }
}
