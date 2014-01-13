using Ninject.Modules;
using PlayMe.Server.Providers.SpotifyProvider.Mappers;

namespace PlayMe.Server.Providers.SpotifyProvider
{
    public class SpotifyNinjectModule : NinjectModule
    {        
        public override void Load()
        {
            Bind<IMusicProvider>().To<SpotifyMusicProvider>().InSingletonScope();
            Bind<ITrackMapper>().To<TrackMapper>();
            Bind<IAlbumMapper>().To<AlbumMapper>();
            Bind<IArtistMapper>().To<ArtistMapper>();
            Bind<ISpotifySettings>().To<SpotifySettings>();
        }
    }
}
