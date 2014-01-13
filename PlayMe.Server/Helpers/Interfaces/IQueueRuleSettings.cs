
namespace PlayMe.Server.Helpers.Interfaces
{
    public interface IQueueRuleSettings
    {
        int QueueCount { get; }
        int LastXHours { get; }
    }
}
