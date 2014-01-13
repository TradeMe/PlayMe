using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace PlayMe.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{resource}.svc/{*pathInfo}"); 

            routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{action}/{provider}/{id}",
                new { action = "Default", id = RouteParameter.Optional, provider = RouteParameter.Optional}
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}/{provider}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional, provider = RouteParameter.Optional}
            );

            routes.MapRoute(
                "SpotifyApi",
                "spotify/api/{*url}",
                new { controller = "Spotify", action = "PassThrough" }
            );
        }
    }
}