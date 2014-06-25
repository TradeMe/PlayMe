using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace PlayMe.Data.NHibernate
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ITransaction transaction;

        internal UnitOfWork(ITransaction transaction)
        {
            this.transaction = transaction;
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Rollback()
        {
            transaction.Rollback();
        }

        public void Dispose()
        {
            Commit();
        }
    }
}
