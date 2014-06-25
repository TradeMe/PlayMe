using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class LikeClassMap: BaseClassMap<Like>
    {
        public LikeClassMap()
        {
            Map(x => x.ByUser)
                .Column("liked_by_user");
        }
    }
}
