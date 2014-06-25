using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class PlayMeObjectClassMap<T> : BaseClassMap<T> where T : PlayMeObject
    {
        public PlayMeObjectClassMap()
        {
            Map(x => x.Link)
                .Column("link")
                .Not.Nullable();
            Map(x => x.Name)
                .Column("name")
                .Length(200)
                .Not.Nullable();
        }
    }
}
