using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Providers.SoundCloud.Mappers
{
    public interface ITrackMapper
    {
        Track Map(Model.Track track, SoundCloudMusicProvider musicProvider, string user, bool mapArtists = false);
    }
}