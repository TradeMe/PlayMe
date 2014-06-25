using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class QueuedTrackClassMap: BaseClassMap<QueuedTrack>
    {
        public QueuedTrackClassMap()
        {
            Map(x => x.User)
                .Column("queued_by_user")
                .Length(50)
                .Not.Nullable();
            Map(x => x.StartedPlayingDateTime)
                .Column("started_playing_datetime");
            References(x => x.Track)
                .Column("track_id");
            HasMany(x => x.Likes)
                .Cascade.All();
            HasMany(x => x.Vetoes)
                .Cascade.All();
        }
    }
}
