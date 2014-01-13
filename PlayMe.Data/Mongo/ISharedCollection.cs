using MongoDB.Driver;

namespace PlayMe.Data.Mongo
{
    public interface ISharedCollection<T>
    {
        MongoCollection<T> DataCollection { get; }
    }
}
