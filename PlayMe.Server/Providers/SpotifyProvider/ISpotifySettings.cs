namespace PlayMe.Server.Providers.SpotifyProvider
{
    public interface ISpotifySettings
    {
        string UserName { get; }
        string Password { get; }
        byte[] ApplicationKey { get; }
    }

}
