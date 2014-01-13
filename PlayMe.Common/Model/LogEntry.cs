using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlayMe.Common.Model
{
    public class LogEntry //: DataObject
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }
        public int sequenceID { get; set; }
        public DateTime timeStamp { get; set; }
        public string machineName { get; set; }
        public string loggerName { get; set; }
        public string message { get; set; }
        public string formattedMessage { get; set; }
        public string level { get; set; }
        public int UserStackFrameNumber { get; set; }
        public object[] parameters { get; set; }
    }
}
