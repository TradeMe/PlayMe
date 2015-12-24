using System;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using PlayMe.Web.Code;
using PlayMe.Web.MusicServiceReference;

namespace PlayMe.Web.Hubs
{
    public class QueueHub : Hub
    {
        public void GetCurrentTrack()
        {
            try
            {
                using (var client = new MusicServiceClient())
                {
                    var track=client.GetPlayingTrack();
                    Clients.Caller.updateCurrentTrack(track);
                }
            }
            catch
            {
                //Todo:Log error
            }
        }
        public void GetPlayingSoon()
        {
            try
            {
                using (var client = new MusicServiceClient())
                {
                    Clients.Caller.updatePlayingSoon(client.GetQueue().Take(5));
                }
            }
            catch
            {
                //Todo:Log error
            }
        }
        public void GetRecentlyPlayed()
        {
            try
            {
                using (var client = new MusicServiceClient())
                {
                    Clients.Caller.updateRecentlyPlayed(client.GetTrackHistory(1, 5, null));
                }
            }
            catch
            {
                //Todo:Log error
            }
        }

        public void VetoTrack(string id)
        {
            using (var client = new MusicServiceClient())
            {
                var idGuid = Guid.Empty;
                if (!Guid.TryParse(id, out idGuid)) return;
                var identityHelper = new IdentityHelper();
                client.VetoTrack(idGuid, identityHelper.GetCurrentIdentityName());
            }
        }
        
        public void LikeTrack(string id)
        {
            try
            {
                using (var client = new MusicServiceClient())
                {
                    var idGuid = Guid.Empty;
                    if (!Guid.TryParse(id, out idGuid)) return;
                    var identityHelper = new IdentityHelper();
                    client.LikeTrack(idGuid, identityHelper.GetCurrentIdentityName());
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void NextTrack(string id)
        {
            using (var client = new MusicServiceClient())
            {
                var idGuid = Guid.Empty;
                if (!Guid.TryParse(id, out idGuid)) return;
                var identityHelper = new IdentityHelper();
                client.SkipTrack(idGuid, identityHelper.GetCurrentIdentityName());
            }
        }

        public void ForgetTrack(string id)
        {
            using (var client = new MusicServiceClient())
            {
                var idGuid = Guid.Empty;
                if (!Guid.TryParse(id, out idGuid)) return;
                var identityHelper = new IdentityHelper();
                client.ForgetTrack(idGuid, identityHelper.GetCurrentIdentityName(), false);
            }
        }

        public void PauseTrack()
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();                
                client.PauseTrack(identityHelper.GetCurrentIdentityName());
                Clients.All.updateCurrentTrack(client.GetPlayingTrack());
            }
        }

        public void ResumeTrack()
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();
                client.ResumeTrack(identityHelper.GetCurrentIdentityName());
                Clients.All.updateCurrentTrack(client.GetPlayingTrack());
            }
        }

        public void GetCurrentVolume()
        {
            using (var client = new MusicServiceClient())
            {
                Clients.All.updateCurrentVolume(client.GetCurrentVolume());
            }
        }

        public void IncreaseVolume()
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();
                client.IncreaseVolume(identityHelper.GetCurrentIdentityName());
                Clients.All.updateCurrentVolume(client.GetCurrentVolume());
            }
        }

        public void DecreaseVolume()
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();
                client.DecreaseVolume(identityHelper.GetCurrentIdentityName());
                Clients.All.updateCurrentVolume(client.GetCurrentVolume());
            }
        }
    }
}