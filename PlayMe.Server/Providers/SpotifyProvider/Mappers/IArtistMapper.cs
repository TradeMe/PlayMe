using PlayMe.Data.NHibernate.Entities;
using SpotiFire.SpotifyLib;

namespace PlayMe.Server.Providers.SpotifyProvider.Mappers
{
    public interface IArtistMapper
    {
        Artist Map(IArtist artist, SpotifyMusicProvider musicProvider);
    }
}