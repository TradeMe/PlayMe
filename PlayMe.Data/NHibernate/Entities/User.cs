
namespace PlayMe.Data.NHibernate.Entities
{
    public class User : DataObject
    {
        public virtual string Username { get; set; }
        public virtual bool IsAdmin { get; set; }
    }
}
