using AutoMapper;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server
{
    public static class MappingDefinitions
    {
        public static void SetUpMappings()
        {
            Mapper.CreateMap<User, Common.Model.User>();
        }
    }
}
