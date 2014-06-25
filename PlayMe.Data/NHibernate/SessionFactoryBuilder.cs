using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace PlayMe.Data.NHibernate
{
    public class SessionFactoryBuilder
    {
        public static ISessionFactory CreateSessionFactory()
        {
            var config = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.Dialect<MsSql2012Dialect>().ShowSql()
                .ConnectionString("Data Source=(LocalDb)\\v11.0;Initial Catalog=PlayMe;")) //TODO: Get connectionstring
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<SessionFactoryBuilder>())
                .ExposeConfiguration(BuildSchema)
            .BuildConfiguration();
            return config.BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            var validator = new SchemaValidator(config);
            try
            {
                validator.Validate();
            }
            catch (HibernateException)
            {
                // not valid, try to update
                var update = new SchemaUpdate(config);
                update.Execute(false, true);
                
                InsertLookupData();
            }
        }

        private static void InsertLookupData()
        {
            
        }
    }
}
