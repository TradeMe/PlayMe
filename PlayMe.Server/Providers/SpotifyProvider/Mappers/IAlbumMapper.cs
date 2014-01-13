using PlayMe.Common.Model;
using SpotiFire.SpotifyLib;

namespace PlayMe.Server.Providers.SpotifyProvider.Mappers
{
    public interface IAlbumMapper
    {
        Album Map(IAlbum album, SpotifyMusicProvider musicProvider, bool mapArtist = false);
    }
}