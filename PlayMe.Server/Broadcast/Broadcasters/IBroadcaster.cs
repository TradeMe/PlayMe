namespace PlayMe.Server.Broadcast.Broadcasters
{
    public interface IBroadcaster
    {
        bool IsEnabled { get; }
        void Broadcast(IMessage message);
    }
}
