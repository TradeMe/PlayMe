using System.Web.Http;
using PlayMe.Common.Model;
using PlayMe.Web.MusicServiceReference;
using PlayMe.Web.Code;

namespace PlayMe.Web.Controllers
{
    public class BrowseController : ApiController
    {

        [HttpGet]
        public Artist Artist(string id, string provider)
        {
            using (var client = new MusicServiceClient())
            {
                return client.BrowseArtist(id,provider);
            }

        }
        [HttpGet]
        public Album Album(string id, string provider)
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();
                return client.BrowseAlbum(id, provider, identityHelper.GetCurrentIdentityName());
            }
        }


    }
}
