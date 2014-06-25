using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class TrackClassMap : PlayMeObjectClassMap<Track>
    {
        public TrackClassMap()
        {            
            Map(x => x.Duration)
                .Column("duration")
                .Not.Nullable();
            Map(x => x.TrackArtworkUrl)
                .Column("artwork_url")
                .Length(200);
            References(x => x.Album)
                .Column("album_id");
            HasManyToMany(x => x.Artists)
                .Table("track_artist")
                .ParentKeyColumn("track_id")
                .ChildKeyColumn("artist_id");
            
        }
    }
}
