using System;

namespace PlayMe.Common.Model
{
    public class DataObject
    {
        public static Guid GenerateId()
        {
            return Guid.NewGuid();
        }

        public virtual Guid Id { get;  set; }
        public bool IsDeleted { get; set; }
    }
}
