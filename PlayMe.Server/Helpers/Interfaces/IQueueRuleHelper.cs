using System.Collections.Generic;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.Interfaces
{
    public interface IQueueRuleHelper
    {
        IEnumerable<string> CannotQueueTrack(Track track, string user);
    }
}
