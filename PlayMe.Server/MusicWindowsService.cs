using System.ServiceProcess;
using PlayMe.Server.ServiceModel;

namespace PlayMe.Server
{
    partial class MusicWindowsService : ServiceBase
    {
        private readonly NinjectServiceHost<MusicService> serviceHost;
        
        public MusicWindowsService(NinjectServiceHost<MusicService> serviceHost)
        {
            InitializeComponent();
            this.serviceHost = serviceHost;
        }

        protected override void OnStart(string[] args)
        {
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            serviceHost.Close();
        }

        internal void Start(string[] args)
        {
            OnStart(args);
        }
        
    }
}
