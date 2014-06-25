using FluentNHibernate.Mapping;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class AlbumClassMap: ClassMap<Album>
    {
        public AlbumClassMap()
        {
            Id(x => x.Id);
            Map(x => x.Year)
                .Column("year");
        }
    }
}
