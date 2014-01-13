namespace PlayMe.Data.Mongo
{
    public class MapReduceResult<T>
    {
        public string _id { get; set; }
        public T value{get;set;}
    }
}
