using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate.ClassMaps
{
    public class MusicProviderDescriptorClassMap : BaseClassMap<MusicProviderDescriptor>
    {
        public MusicProviderDescriptorClassMap()
        {
            Map(x => x.Identifier);
            Map(x => x.Name);
        }
    }
}
