using System.IO;
using System.Net;
using System.Web.Mvc;

namespace PlayMe.Web.Controllers
{
    public class SpotifyController : Controller
    {
        public ContentResult PassThrough(string url)
        {
            var request = WebRequest.Create("http://ws.spotify.com/" + url + "?" + Request.QueryString.ToString());
            request.ContentType = Request.ContentType;
            using (var response = request.GetResponse())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                return Content(streamReader.ReadToEnd(), response.ContentType);
            }            
        }      
    }
}
