using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class VetoClassMap : BaseClassMap<Veto>
    {
        public VetoClassMap()
        {            
            Map(x => x.ByUser)
                .Column("vetoed_by_user");
        }
    }
}
