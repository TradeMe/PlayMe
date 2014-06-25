using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Broadcast.MessageRules
{
    public class FizzBungBroadcastMessageRule : IBroadcastMessageRule
    {
        private const int FizzBungCount = 4;
        private const string FizzBung = "The office is fizzing at the bung for \"{0}\" right now!";
        public IMessage GetMessage(QueuedTrack queuedTrack)
        {
            IMessage message = null;
            if(queuedTrack.LikeCount == FizzBungCount)
            {
                message = new Message(FizzBung,queuedTrack.Track.Name);
            }
            return message;
        }

        public int Priority
        {
            get { return FizzBungCount; }
        }
    }
}
