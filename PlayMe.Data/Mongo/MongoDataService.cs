using System;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PlayMe.Common.Model;
namespace PlayMe.Data.Mongo
{
    public abstract class MongoDataService<T> : IDataService<T>
    {
        protected MongoDataService()
        {
            lock (this)
            {
                if (!BsonClassMap.IsClassMapRegistered(typeof (DataObject)))
                {
                    BsonClassMap.RegisterClassMap<DataObject>(t => t.MapIdMember(m => m.Id));
                }

                if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                {
                    BsonClassMap.RegisterClassMap<T>(t =>
                        {
                            t.AutoMap();
                            
                            t.SetIgnoreExtraElements(true);
                    });
                }
            }
        }
        
        protected abstract string DataCollectionName { get; }

        public virtual MongoCollection<T> DataCollection{        
            get
            {                
                return Database.GetCollection<T>(DataCollectionName);
            }
        }

        protected MongoDatabase Database
        {
            get
            {
                var settings = new MongoSettings();
                var server = MongoServer.Create(settings.DatabaseConnectionString);
                return server.GetDatabase(settings.DatabaseName, SafeMode.True);
            }
        }

        public virtual IQueryable<T> GetAll()
        {
            return DataCollection.AsQueryable();
        }

        public virtual T Get(Guid id)
        {
            return DataCollection.FindOneByIdAs<T>(id);
        }

        public virtual void Insert(T entity)
        {
            var dataObject = entity as DataObject;
            if(dataObject==null) throw new ArgumentException("Entity must inherit DataObject");            
            
            dataObject.Id = Guid.NewGuid();            
            DataCollection.Insert(entity);            
        }

        public virtual void Update(T entity)
        {
            var dataObject = entity as DataObject;
            if (dataObject == null) throw new ArgumentException("Entity must inherit DataObject");
            //As save will do an insert if there's no value
            if (dataObject.Id==Guid.Empty)
            {
                throw new ArgumentException("Id must contain a value in order to do an update");
            }
            DataCollection.Save(entity);
        }

        public virtual void InsertOrUpdate(T entity)
        {
            var dataObject = entity as DataObject;
            if (dataObject == null) throw new ArgumentException("Entity must inherit DataObject");

            if (dataObject.Id == Guid.Empty)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }

        public virtual void Delete(T entity)
        {
            var dataObject = entity as DataObject;
            if (dataObject == null) throw new ArgumentException("Entity must inherit DataObject");

            var id = dataObject.Id;

            var query = MongoDB.Driver.Builders.Query.EQ("_id", id);
            DataCollection.Remove(query);
            
        }
    }
}
