using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.Server.Helpers.SkipHelperRules
{
    public class VetoedByUserWhoQueuedTrackSkipRule : ISkipRule
    {
        public int GetRequiredVetoCount(QueuedTrack track)
        {
            return (track.Vetoes.Any(v => v.ByUser == track.User)) ? 1 : int.MaxValue;
        }
    }
}
