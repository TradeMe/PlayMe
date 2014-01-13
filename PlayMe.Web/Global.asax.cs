using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;
using PlayMe.Web.App_Start;

namespace PlayMe.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Override the default json serializer settings
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            var jSettings = new Newtonsoft.Json.JsonSerializerSettings()
                                {
                                    DefaultValueHandling = DefaultValueHandling.Include
                                };
            jsonFormatter.SerializerSettings = jSettings;
            
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);            
        }
    }
}