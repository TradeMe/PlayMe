using PlayMe.Data.NHibernate.Entities;
using SpotiFire.SpotifyLib;

namespace PlayMe.Server.Providers.SpotifyProvider.Mappers
{
    public interface ITrackMapper
    {
        Track Map(ITrack track, SpotifyMusicProvider musicProvider, string user, bool mapAlbum=false,bool mapArtists=false);
    }
}