using System.Collections.Generic;
using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.QueueHelperRules.Interfaces;

namespace PlayMe.Server.Helpers
{
    public class QueueRuleHelper : IQueueRuleHelper
    {
        private readonly IEnumerable<IQueueRule> queueRules;

        public QueueRuleHelper(IEnumerable<IQueueRule> queueRules)
        {
            this.queueRules = queueRules;
        }

        public IEnumerable<string> CannotQueueTrack(Track track, string user)
        {
            var errors = queueRules.Select(q => q.CannotQueue(track, user));
            return errors;
        }
    }
}
