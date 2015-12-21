using System;
using System.Collections.Generic;
using System.Web.Http;
using PlayMe.Common.Model;
using PlayMe.Web.Code;
using PlayMe.Web.MusicServiceReference;

namespace PlayMe.Web.Controllers
{
    public class LikesController : ApiController
    {
        [HttpGet]
        public void LikeTrack(string id)
        {
            using (var client = new MusicServiceClient())
            {
                Guid idGuid = Guid.Empty;
                if (Guid.TryParse(id, out idGuid))
                {
                    var identityHelper = new IdentityHelper();
                    client.LikeTrack(idGuid, identityHelper.GetCurrentIdentityName());
                }
            }
        }

        [HttpGet]
        public PagedResult<Track> MyLikes(int start=0,int take=20)
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();
                var user = identityHelper.GetCurrentIdentityName();
                
                
                return client.GetLikes(start, take, user); 
            }
        }
    }
}
