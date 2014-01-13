
namespace PlayMe.Server.Broadcast
{
    public interface IMessage
    {
        string GetMessage(int truncateAt);
        string GetMessage();
    }
}
