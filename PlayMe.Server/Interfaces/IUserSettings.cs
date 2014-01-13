using System.Collections.Generic;

namespace PlayMe.Server.Interfaces
{
    public interface IUserSettings
    {
        IEnumerable<string> AdminUsers { get; }

        string Domain { get; }
    }
}
