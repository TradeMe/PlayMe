using FluentNHibernate.Mapping;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class BaseClassMap<T>: ClassMap<T> where T:DataObject
    {
        public BaseClassMap()
        {
            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.GuidComb();
        }
    }
}
