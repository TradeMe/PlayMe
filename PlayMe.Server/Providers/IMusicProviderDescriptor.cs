namespace PlayMe.Server.Providers
{
    public interface IMusicProviderDescriptor
    {
        string Name { get;}
        string Identifier { get; }
        bool IsEnabled { get; }
    }
}
