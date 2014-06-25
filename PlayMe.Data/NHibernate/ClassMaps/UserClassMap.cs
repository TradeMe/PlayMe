using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class UserClassMap : BaseClassMap<User>
    {
        public UserClassMap()
        {
            Id(x => x.Id);
            Map(x => x.Username);
            Map(x => x.IsAdmin);
        }
    }
}
