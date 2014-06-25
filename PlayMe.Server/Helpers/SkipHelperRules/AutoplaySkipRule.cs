using System;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;

namespace PlayMe.Server.Helpers.SkipHelperRules
{
    public class AutoplaySkipRule : ISkipRule
    {
        public int GetRequiredVetoCount(QueuedTrack track)
        {
            const int minVetoCount = 2;
            return (string.Compare(track.User, Constants.AutoplayUserName, StringComparison.CurrentCultureIgnoreCase) == 0) ? minVetoCount : int.MaxValue;            
        }

    }
}