namespace PlayMe.Data.NHibernate.Entities
{
    public class MusicProviderDescriptor :  DataObject
    {
        public virtual string Identifier { get; set; }
        public virtual string Name { get; set; }
    }
}
