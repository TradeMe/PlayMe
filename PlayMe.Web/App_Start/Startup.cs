using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PlayMe.Web.App_Start.Startup))]
namespace PlayMe.Web.App_Start
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}