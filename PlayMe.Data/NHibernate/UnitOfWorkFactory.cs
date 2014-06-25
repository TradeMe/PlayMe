using System;
using NHibernate;

namespace PlayMe.Data.NHibernate
{
    public class UnitOfWorkFactory: IUnitOfWorkFactory
    {
        private readonly ISession session;

        public UnitOfWorkFactory(ISession session)
        {
            this.session = session;
        }
        
        public void Execute(Action<IUnitOfWork> action)
        {
            using (var transaction = session.BeginTransaction())
            {
                action(new UnitOfWork(transaction));
            }            
        }
    }
}
