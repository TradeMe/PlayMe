using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server
{
    public class UserService : IUserService
    {
        private readonly IDataService<User> userDataService;
        private readonly IUserSettings userSettings;

        public UserService(IDataService<User> userDataService, IUserSettings userSettings)
        {
            this.userSettings = userSettings;
            this.userDataService = userDataService;
        }

        public bool IsUserAdmin(string user)
        {            
            bool databaseAdmin = userDataService.GetAll().Any(a => a.Username.ToLower() == user.ToLower());

            return IsUserSuperAdmin(user) || databaseAdmin;
        }


        public bool IsUserSuperAdmin(string user)
        {
            return userSettings.AdminUsers.Any(a => a.ToLower() == user.ToLower());
        }

        public IEnumerable<User> GetAdminUsers()
        {
            return userDataService.GetAll().ToArray();
        }

        public string GetDomain()
        {
            return userSettings.Domain;
        }
    }
}
