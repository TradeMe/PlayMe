using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Helpers.QueueHelperRules.Interfaces
{
    public interface IQueueRule
    {
        string CannotQueue(Track track, string user);
    }
}
