using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace PlayMe.Web.Code
{
    public static class AppScripts
    {
        public static IHtmlString Render(string virtualPath, string requirePath, string mainPath)
        {
            var output = Scripts.Render(virtualPath);
            if (output.ToString().Contains(virtualPath.Replace("~", string.Empty)))
            {
                return output;
            }                

            var scriptTagBuilder = new TagBuilder("script");
            scriptTagBuilder.Attributes.Add("src", requirePath);
            scriptTagBuilder.Attributes.Add("data-main", mainPath);
            return new HtmlString(scriptTagBuilder.ToString());            
        }
    }
}