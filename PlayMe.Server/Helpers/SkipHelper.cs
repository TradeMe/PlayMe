using System.Collections.Generic;
using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.Server.Helpers
{
    public class SkipHelper : ISkipHelper
    {
        private readonly IEnumerable<ISkipRule> skipRules;

        public SkipHelper(IEnumerable<ISkipRule> skipRules)
        {
            this.skipRules = skipRules;
        }

        public int RequiredVetoCount(QueuedTrack track)
        {
            return skipRules.Min(rule => rule.GetRequiredVetoCount(track));
        }
    }
}