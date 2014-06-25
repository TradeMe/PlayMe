using System.Collections.Generic;
using System.Linq;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUserSettings userSettings;

        public UserService(IRepository<User> _userRepository, IUserSettings userSettings)
        {
            this.userSettings = userSettings;
            this._userRepository = _userRepository;
        }

        public bool IsUserAdmin(string user)
        {            
            bool databaseAdmin = _userRepository.GetAll().Any(a => a.Username.ToLower() == user.ToLower());

            return IsUserSuperAdmin(user) || databaseAdmin;
        }


        public bool IsUserSuperAdmin(string user)
        {
            return userSettings.AdminUsers.Any(a => a.ToLower() == user.ToLower());
        }

        public IEnumerable<User> GetAdminUsers()
        {
            return _userRepository.GetAll().ToArray();
        }

        public string GetDomain()
        {
            return userSettings.Domain;
        }
    }
}
