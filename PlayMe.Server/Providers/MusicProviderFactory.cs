using System.Collections.Generic;
using System.Linq;
namespace PlayMe.Server.Providers
{
    public class MusicProviderFactory : IMusicProviderFactory
    {
        private readonly IEnumerable<IMusicProvider> musicProviders;

        public MusicProviderFactory(IEnumerable<IMusicProvider> musicProviders)
        {
            this.musicProviders = musicProviders;
        }

        public IMusicProvider GetMusicProviderByIdentifier(string identifier)
        {
            var provider = musicProviders.FirstOrDefault(m => m.Descriptor.Identifier==identifier) ??
                           musicProviders.FirstOrDefault(m => m.Descriptor.Identifier == "sp");
            return provider;
        }

        public IEnumerable<IMusicProvider> GetAllMusicProviders()
        {
            return musicProviders.Where(p => p.IsEnabled);
        }
    }
}
