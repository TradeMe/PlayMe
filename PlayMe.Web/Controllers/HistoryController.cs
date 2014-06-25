using System.Collections.Generic;
using System.Web.Http;
using PlayMe.Web.Code;
using PlayMe.Web.MusicServiceReference;

namespace PlayMe.Web.Controllers
{
    public class HistoryController : ApiController
    {
        [HttpGet]
        public PagedResult<QueuedTrack> Default(string filter,int start=0,int take=20)
        {
            using (var client = new MusicServiceClient())
            {
                string user = null;

                //currently can only filter to users own tracks
                if(filter!=null)
                {
                    switch (filter.ToLower())
                    {
                        case "mine":                        
                            var identityHelper = new IdentityHelper();
                            user = identityHelper.GetCurrentIdentityName();
                            break;
                        case "requested":
                            //we find any track queued by a network user
                            user = client.GetDomain();
                            break;
                    }
                }
                var history = client.GetTrackHistory(start, take, user);
                return history;
            }
        }
    }
}
