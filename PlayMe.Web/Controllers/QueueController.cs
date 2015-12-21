using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using PlayMe.Common.Model;
using PlayMe.Web.Code;
using PlayMe.Web.MusicServiceReference;

namespace PlayMe.Web.Controllers
{
    public class QueueController : ApiController
    {
        [HttpPost]
        public string Enqueue(JObject json)
        {
            dynamic trackToQueue = json;
            string id = trackToQueue.id;
            string provider = trackToQueue.provider;
            string reason = trackToQueue.reason;
            var identityHelper = new IdentityHelper();
            var user = identityHelper.GetCurrentIdentityName();
            using (var client = new MusicServiceClient())
            {
                var track = client.GetTrack(id, provider, user);
                var queuedTrack = new QueuedTrack
                {
                    Track = track,
                    User = user,
                    Reason = reason
                };

                var errors = client.QueueTrack(queuedTrack);
                return errors;
            }
        }

        [HttpGet]
        public void VetoTrack(string id)
        {
            using (var client = new MusicServiceClient())
            {
                Guid idGuid;
                if (Guid.TryParse(id, out idGuid))
                {
                    var identityHelper = new IdentityHelper();
                    client.VetoTrack(idGuid, identityHelper.GetCurrentIdentityName());
                }
            }
        }

        [HttpGet]
        public void VetoCurrentTrack()
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();
                client.VetoTrack(Guid.Empty, identityHelper.GetCurrentIdentityName());
            }
        }

        [HttpGet]
        public IEnumerable<QueuedTrack> Default()
        {
            using (var client = new MusicServiceClient())
            {
                return client.GetQueue();
            }
        }

    }
}
