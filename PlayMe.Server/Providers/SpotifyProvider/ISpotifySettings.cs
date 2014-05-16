namespace PlayMe.Server.Providers.SpotifyProvider
{
    public interface ISpotifySettings
    {
        bool IsEnabled { get; }
        string UserName { get; }
        string Password { get; }
        byte[] ApplicationKey { get; }
    }
}
