using System.Collections.Generic;

namespace PlayMe.Server.Providers
{
    public interface IMusicProviderFactory
    {
        IEnumerable<IMusicProvider> GetAllMusicProviders();
        IMusicProvider GetMusicProviderByIdentifier(string provider);
    }
}

