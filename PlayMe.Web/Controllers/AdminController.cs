using System;
using System.Collections.Generic;
using System.Web.Http;
using PlayMe.Common.Model;
using PlayMe.Web.Code;
using PlayMe.Web.MusicServiceReference;

namespace PlayMe.Web.Controllers
{
    public class AdminController : ApiController
    {        
        [HttpGet]
        public void SkipTrack(string id)
        {
            using (var client = new MusicServiceClient())
            {
                if (!IsUserSuperAdmin())
                    throw new HttpResponseException(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));

                Guid idGuid;
                if (Guid.TryParse(id, out idGuid))
                {
                    var identityHelper = new IdentityHelper();
                    client.SkipTrack(idGuid, identityHelper.GetCurrentIdentityName());
                }
            }
        }

        [HttpGet]
        public bool IsUserAdmin()
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();
                return client.IsUserAdmin(identityHelper.GetCurrentIdentityName());
            }
        }
        [HttpGet]
        public bool IsUserSuperAdmin()
        {
            using (var client = new MusicServiceClient())
            {
                var identityHelper = new IdentityHelper();
                return client.IsUserSuperAdmin(identityHelper.GetCurrentIdentityName());
            }
        }

        [HttpGet]
        public IEnumerable<User> GetAdminUsers() 
        {
            using (var client = new MusicServiceClient())
            {
                return client.GetAdminUsers();       
            }
        }

        [HttpPost]
        public User AddAdminUser(User user)
        { 
            using (var client = new MusicServiceClient())
            {
                if (!IsUserSuperAdmin()) 
                    throw new HttpResponseException(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));
   
                // fix format of Username
                var username = user.Username.Split('\\');
                user.Username = username[0] + "\\" + username[1].ToLower();

                var identityHelper = new IdentityHelper();
                return client.AddAdminUser(user, identityHelper.GetCurrentIdentityName());    
            }
        }

        [HttpPost]
        public bool RemoveAdminUser(User user)
        {
            using (var client = new MusicServiceClient())
            {
                if (!IsUserSuperAdmin())
                {
                    throw new HttpResponseException(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));
                }

                var identityHelper = new IdentityHelper();
                if (identityHelper.GetCurrentIdentityName().ToLower() != user.Username.ToLower())
                {
                    client.RemoveAdminUser(user.Username, identityHelper.GetCurrentIdentityName());
                    return true;
                }
                return false;
            }
        }

        [HttpPost]
        public RickRoll AddRickRoll(PlayMeObject playMeObject)
        {
            using (var client = new MusicServiceClient())
            {                
                if (!IsUserSuperAdmin())
                    throw new HttpResponseException(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));

                return client.AddRickRoll(playMeObject);
            }
        }

        [HttpPost]
        public void RemoveRickRoll([FromBody] Guid id)
        {
            using (var client = new MusicServiceClient())
            {
                if (!IsUserSuperAdmin())
                    throw new HttpResponseException(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));

                client.RemoveRickRoll(id);
            }
        }

        [HttpGet]
        public IEnumerable<RickRoll> GetRickRolls()
        {
            using (var client = new MusicServiceClient())
            {
                if (!IsUserAdmin())
                    throw new HttpResponseException(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));

                return client.GetAllRickRolls();
            }
        }

        [HttpGet]
        public string GetCurrentIdentityName()
        {
            var identityHelper = new IdentityHelper();
            return identityHelper.GetCurrentIdentityName();
        }

        [HttpGet]
        public PagedResult<LogEntry> GetLogEntries(string direction="Ascending", int start=0,int take=20)
        {
            using (var client = new MusicServiceClient())
            {
                if (!IsUserAdmin())
                    throw new HttpResponseException(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));

                var dir = SortDirection.Ascending;
                if (!string.IsNullOrEmpty(direction) && direction == "Descending")
                {
                    dir = SortDirection.Descending;   
                }
                return client.GetLogEntries(dir, start, take);
            }
        }
    }
}
