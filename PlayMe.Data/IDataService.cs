using System;
using System.Linq;

namespace PlayMe.Data
{
    public interface IDataService<T>
    {
        IQueryable<T> GetAll();
        T Get(Guid id);
        void Insert(T entity);
        void Update(T entity);
        void InsertOrUpdate(T entity);
        void Delete(T entity);
    }
}
