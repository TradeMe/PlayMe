using PlayMe.Data.NHibernate.Entities;
using SpotiFire.SpotifyLib;

namespace PlayMe.Server.Providers.SpotifyProvider.Mappers
{
    public interface IAlbumMapper
    {
        Album Map(IAlbum album, SpotifyMusicProvider musicProvider, bool mapArtist = false);
    }
}