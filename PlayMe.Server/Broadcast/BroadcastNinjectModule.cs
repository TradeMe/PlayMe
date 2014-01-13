using Ninject.Modules;
using PlayMe.Server.Broadcast.Broadcasters;
using PlayMe.Server.Broadcast.Broadcasters.Twitter;
using PlayMe.Server.Broadcast.MessageRules;

namespace PlayMe.Server.Broadcast
{
    public class BroadcastNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBroadcastService>().To<BroadcastService>();
            Bind<IBroadcastMessageRuleResolver>().To<BroadcastMessageRuleResolver>();

            //Broadcasters
            Bind<IBroadcaster>().To<TwitterBroadcaster>();
            Bind<ITwitterSettings>().To<TwitterSettings>();

            //Rules
            Bind<IBroadcastMessageRule>().To<FizzBungBroadcastMessageRule>();
            Bind<IBroadcastMessageRule>().To<GangbustersBroadcastMessageRule>();
        }
    }
}
