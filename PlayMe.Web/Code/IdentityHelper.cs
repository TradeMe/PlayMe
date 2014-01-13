using System.Web;

namespace PlayMe.Web.Code
{
    public class IdentityHelper : IIdentityHelper
    {
        public string GetCurrentIdentityName()
        {
            return HttpContext.Current.User.Identity.Name;
        }
    }
}