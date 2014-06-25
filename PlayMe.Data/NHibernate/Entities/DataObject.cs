using System;

namespace PlayMe.Data.NHibernate.Entities
{
    public class DataObject
    {
        public static Guid GenerateId()
        {
            return Guid.NewGuid();
        }

        public virtual Guid Id { get;  set; }
        public virtual bool IsDeleted { get; set; }
    }
}
