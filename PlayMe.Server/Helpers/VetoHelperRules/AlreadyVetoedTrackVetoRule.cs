using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.VetoHelperRules.Interfaces;

namespace PlayMe.Server.Helpers.VetoHelperRules
{
    public class AlreadyVetoedTrackVetoRule : IVetoRule
    {
        public bool CantVetoTrack(string vetoedByUser, QueuedTrack track)
        {
            return track.Vetoes.Any(v => v.ByUser == vetoedByUser);
        }
    }
}
