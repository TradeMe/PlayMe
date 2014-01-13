using System.Web.Optimization;

namespace PlayMe.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/Styles/Desktop")
                .Include("~/Content/styles/bootstrap.css")
                .Include("~/Content/Desktop/Main.css")
                .Include("~/Content/styles/Common.css")
                );

            bundles.Add(new StyleBundle("~/Content/Styles/Mobile")
                .Include("~/Content/styles/bootstrap.css")
                .Include("~/Content/styles/Common.css")
                .Include("~/Content/Mobile/Main.css")
                );
            
            bundles.Add(
                new ScriptBundle("~/Scripts/core")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/notify.js")
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/jquery.signalR-{version}.js")
                .Include("~/Scripts/knockout-{version}.js")
                .Include("~/Scripts/knockout.mapping-latest.js")
                .Include("~/Scripts/sugar.js")
                );

            bundles.Add(
                new ScriptBundle("~/Scripts/app-core")
                .Include("~/Scripts/bindingHandlers.js")                
                );

            bundles.Add(
                new ScriptBundle("~/Scripts/app-main")
                .Include("~/Scripts/app/main-built.js")
                );
    
            bundles.Add(
                new ScriptBundle("~/Scripts/app-main.mobile")
                .Include("~/Scripts/app/main.mobile-built.js")
                );
        }
    }
}