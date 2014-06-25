using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Data.NHibernate
{
    public class Repository<T> : IRepository<T> where T : DataObject
    {
        private readonly ISession session;

        public Repository(ISession session)
        {
            this.session = session;
        }


        public IQueryable<T> GetAll()
        {
            return session.Query<T>();
        }

        public T Get(Guid id)
        {
            return session.Get<T>(id);
        }

        public void Insert(T entity)
        {
            session.Save(entity);
        }

        public void Update(T entity)
        {
            session.Update(entity);
        }

        public void InsertOrUpdate(T entity)
        {
            session.SaveOrUpdate(entity);
        }

        public void Delete(T entity)
        {
            session.Delete(entity);
        }

    }
}
