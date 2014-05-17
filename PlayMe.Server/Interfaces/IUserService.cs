using System.Collections.Generic;
using PlayMe.Common.Model;

namespace PlayMe.Server.Interfaces
{
    public interface IUserService
    {
        bool IsUserAdmin(string user);
        bool IsUserSuperAdmin(string user);
        IEnumerable<User> GetAdminUsers();
        string GetDomain();
    }
}