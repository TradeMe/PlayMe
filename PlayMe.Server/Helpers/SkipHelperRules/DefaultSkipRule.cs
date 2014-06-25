using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.Server.Helpers.SkipHelperRules
{
    public class DefaultSkipRule : ISkipRule
    {
        private readonly ISettings settings;

        public DefaultSkipRule(ISettings settings)
        {
            this.settings = settings;
        }
        
        public int GetRequiredVetoCount(QueuedTrack track)
        {
            int minimumVetoCount = track.LikeCount;
            return (settings.VetoCount > minimumVetoCount) ? settings.VetoCount : minimumVetoCount;
            
        }
    }
}