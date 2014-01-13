using Ninject.Modules;
using PlayMe.Server.Providers.SoundCloud.Mappers;

namespace PlayMe.Server.Providers.SoundCloud
{
    public class SoundCloudNinjectModule: NinjectModule
    {
        public SoundCloudNinjectModule()
        {
            
        }
        public override void Load()
        {
            Bind<IMusicProvider>().To<SoundCloudMusicProvider>().InSingletonScope();
            Bind<ITrackMapper>().To<TrackMapper>();
            Bind<ISoundCloudSettings>().To<SoundCloudSettings>();
        }
    }
}
