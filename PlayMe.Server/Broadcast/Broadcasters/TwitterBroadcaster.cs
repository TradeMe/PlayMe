using System.Threading;
using TweetSharp;

namespace PlayMe.Server.Broadcast.Broadcasters
{
    public class TwitterBroadcaster : IBroadcaster
    {
        private const string AppConsumerKey = "op4RKn8PvKytdw2Klow";
        private const string AppConsumerSecret = "EN7chkf7QfW0RcDySnwn4MtEiSRypsCHQ2y8AVcrJOY";
        private const string oAuthToken = "1096826214-u3mWQVzsgdgHx5INYgk0qZ7TP4uiiC4RZPSA5Hk";
        private const string oAuthTokenSecret = "djtUtrWlYlzTX9KQ5VVtJ0B5u709cEheNMiKugtQA";

        public void Broadcast(IMessage message)
        {
            ThreadPool.QueueUserWorkItem(SendTweet, message.GetMessage(140));
        }

        protected void SendTweet(object tweetMessage)
        {

            string tweet = (string)tweetMessage;

            var twitterApp = new TwitterService(AppConsumerKey, AppConsumerSecret);
            twitterApp.AuthenticateWith(oAuthToken, oAuthTokenSecret);
            
            //Broken by TweetSharp package update
            //twitterApp.SendTweet(tweet);
        }
    }
}
