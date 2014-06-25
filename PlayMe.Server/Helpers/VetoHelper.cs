using System.Collections.Generic;
using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.VetoHelperRules.Interfaces;

namespace PlayMe.Server.Helpers
{
    public class VetoHelper : IVetoHelper
    {
        private readonly IEnumerable<IVetoRule> vetoRules;

        public VetoHelper(IEnumerable<IVetoRule> vetoRules)
        {
            this.vetoRules = vetoRules;
        }
        
        public bool CantVetoTrack(string vetoedByUser, QueuedTrack track)
        {
            return vetoRules.Any(r => r.CantVetoTrack(vetoedByUser, track));
        }
    }
}