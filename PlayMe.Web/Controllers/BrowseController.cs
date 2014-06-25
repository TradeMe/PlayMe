using System.Web.Http;
using PlayMe.Web.MusicServiceReference;

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
                return client.BrowseAlbum(id, provider, User.Identity.Name);
            }
        }


    }
}
