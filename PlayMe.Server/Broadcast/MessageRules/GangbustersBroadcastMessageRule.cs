using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Broadcast.MessageRules
{
    public class GangbustersBroadcastMessageRule : IBroadcastMessageRule
    {
        private const int GangbustersCount = 8;
        private const string Gangbusters = "\"{0}\" is going \"Gangbusterz\" in the office right now!";
        public IMessage GetMessage(QueuedTrack queuedTrack)
        {
            IMessage message = null;
            if (queuedTrack.LikeCount == GangbustersCount)
            {
                message = new Message(Gangbusters, queuedTrack.Track.Name);
            }
            return message;
        }

        public int Priority
        {
            get { return GangbustersCount; }
        }
    }
}
