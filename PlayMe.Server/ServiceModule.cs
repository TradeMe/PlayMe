using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Ninject.Parameters;
using NLog;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Data.Mongo;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.AutoPlay;
using PlayMe.Server.AutoPlay.TrackRandomizers;
using PlayMe.Server.Helpers;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.QueueHelperRules.Interfaces;
using PlayMe.Server.Helpers.SearchHelperRules.Interfaces;
using PlayMe.Server.Helpers.VetoHelperRules.Interfaces;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Player;
using PlayMe.Server.Providers;
using PlayMe.Server.Queue;
using PlayMe.Server.Queue.Interfaces;
using PlayMe.Server.ServiceModel;
using PlayMe.Server.SoundBoard;
using Logger = PlayMe.Plumbing.Diagnostics.Logger;
namespace PlayMe.Server
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MusicWindowsService>().ToSelf();

            Bind<IServiceBehavior>().To<NinjectServiceBehavior>();
            Bind<IDispatchMessageInspector>().To<WcfRequestScopeCleanup>()
                .WithConstructorArgument("releaseScopeAtRequestEnd", ctx => ctx.Kernel.Settings.Get("ReleaseScopeAtRequestEnd", true));

            Bind<Func<Type, IInstanceProvider>>()
                .ToMethod(ctx => serviceType => ctx.Kernel.Get<NinjectInstanceProvider>(new ConstructorArgument("serviceType", serviceType)));

            Bind<IMusicProviderFactory>().To<MusicProviderFactory>().InSingletonScope();
            
            Bind<IAutoPlay>().To<DefaultAutoPlay>();
            Bind<ISearchSuggestionService>().To<SearchSuggestionService>();
            Bind<IRickRollService>().To<RickRollService>();
            Bind<IRandomizerFactory>().To<RandomizerFactory>();

            Bind<IBufferedPlayer>().To<BassBufferedPlayer>();
            Bind<IStreamedPlayer>().To<BassStreamedPlayer>();
            Bind<IVolumeControl, IVolume>().To<VolumeControl>().InSingletonScope();                        

            Bind<ISoundBoardService>().To<SoundBoardService>();
            Bind<ISoundBoardSettings>().To<SoundBoardSettings>();
            Bind<IPathBuilder>().To<PathBuilder>();
            Bind<IFolderHelper>().To<FolderHelper>();
            
            //Bind data services
            
            Bind<IDataService<QueuedTrack>>().To<QueuedTrackMongoDataService>();
            Bind<IDataService<SearchTerm>>().To<SearchTermMongoDataService>();
            Bind<IDataService<RickRoll>>().To<RickRollMongoDataService>();            
            Bind<ISharedCollection<QueuedTrack>>().To<QueuedTrackMongoDataService>();
            Bind<IDataService<MapReduceResult<TrackScore>>>().To<TrackScoreMongoDataService>();
            Bind<IDataService<User>>().To<AdminUserMongoDataService>();
            Bind<IDataService<SoundBoardInfo>>().To<SoundBoardInfoMongoDataService>();               
            Bind<ILogger>().ToMethod(x =>
            {
                var scope = x.Request.ParentRequest.Service.FullName;
                var log = (ILogger)LogManager.GetLogger(scope, typeof(Logger));
                return log;
            });

            //Bind Skip rules
            Bind<ISkipHelper>().To<SkipHelper>();

            Kernel.Bind(x => x .FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<ISkipRule>()
                .BindDefaultInterfaces()
                .Configure((c, s) => c.InSingletonScope()));


            //Bind Veto rules
            Bind<IVetoHelper>().To<VetoHelper>();
            Bind<IVetoHelperSettings>().To<VetoHelperSettings>();
            Kernel.Bind(x => x.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<IVetoRule>()
                .BindDefaultInterfaces()
                .Configure((c, s) => c.InSingletonScope()));


            //Bind Queue rules
            Bind<IQueueRuleHelper>().To<QueueRuleHelper>();
            Bind<IQueueRuleSettings>().To<QueueRuleSettings>();
            Kernel.Bind(x => x.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<IQueueRule>()
                .BindDefaultInterfaces()
                .Configure((c, s) => c.InSingletonScope()));

            //Bind Search rules
            Bind<ISearchRuleHelper>().To<SearchRuleHelper>();
            Bind<ISearchRuleSettings>().To<SearchRuleSettings>();
            Kernel.Bind(x => x.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<ISearchTrackRule>()
                .BindDefaultInterfaces()
                .Configure((c, s) => c.InSingletonScope()));

            Kernel.Bind(x => x.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<ISearchArtistRule>()
                .BindDefaultInterfaces()
                .Configure((c, s) => c.InSingletonScope()));

            Kernel.Bind(x => x.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<ISearchAlbumRule>()
                .BindDefaultInterfaces()
                .Configure((c, s) => c.InSingletonScope()));
            
            Bind<INowHelper>().To<NowHelper>();

            Bind<ISettings>().To<Settings>();
            
            Bind<IQueueManager>().To<QueueManager>();
            Bind<IConcurrentQueueOfQueuedTrack>().To<ConcurrentQueueOfQueuedTrack>().InSingletonScope();
            Bind<IAlreadyQueuedHelper>().To<AlreadyQueuedHelper>();
            Bind<IMusicPlayer>().To<MusicPlayer>().InSingletonScope();
            Bind<IUserService>().To<UserService>();
            Bind<IUserSettings>().To<UserSettings>();
            Bind<ICallbackClient>().To<CallbackClient>();
        }
    }
}
