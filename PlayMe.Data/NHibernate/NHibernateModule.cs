using System.ServiceModel;
using NHibernate;
using Ninject;
using Ninject.Modules;

namespace PlayMe.Data.NHibernate
{
    public class NHibernateModule : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IRepository<>)).To(typeof(Repository<>));
            Bind<ISessionFactory>().ToConstant(SessionFactoryBuilder.CreateSessionFactory()).InSingletonScope();
            Bind<ISession>().ToMethod(c => c.Kernel.Get<ISessionFactory>()
                .OpenSession()).InScope(c => OperationContext.Current);
        }
    }
}
