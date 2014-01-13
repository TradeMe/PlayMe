using System;
using System.Threading;
using PlayMe.Plumbing.Diagnostics;
using TweetSharp;

namespace PlayMe.Server.Broadcast.Broadcasters.Twitter
{
    public class TwitterBroadcaster : IBroadcaster
    {
        private readonly ITwitterSettings settings;
        private readonly ILogger logger;

        public TwitterBroadcaster(ITwitterSettings settings, ILogger logger)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public bool IsEnabled
        {
            get { return settings.IsEnabled; }
        }

        public void Broadcast(IMessage message)
        {
            if (IsEnabled)
            {
                ThreadPool.QueueUserWorkItem(SendTweet, message.GetMessage(140));
            }
        }

        protected void SendTweet(object tweetMessage)
        {

            var tweet = (string)tweetMessage;
            try
            {
                var twitterApp = new TwitterService(settings.ConsumerKey, settings.ConsumerSecret);
                twitterApp.AuthenticateWith(settings.Token, settings.TokenSecret);
                twitterApp.SendTweet(new SendTweetOptions {Status = tweet});
                logger.Trace("'{0}' was tweeted",tweet);
            }
            catch (Exception ex)
            {
                logger.Warn("The tweet '{0}' failed with the exception: {1}", tweet,ex.Message);
            }
        }
    }
}
