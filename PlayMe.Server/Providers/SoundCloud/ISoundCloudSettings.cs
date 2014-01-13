namespace PlayMe.Server.Providers.SoundCloud
{
    public interface ISoundCloudSettings
    {
        bool IsEnabled { get; }
        string UserName { get; }
        string Password { get; }
        string ClientId { get; }
        string ClientSecret { get; }
    }
}
