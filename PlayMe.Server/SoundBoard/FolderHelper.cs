using System;

namespace PlayMe.Server.SoundBoard
{
    public class FolderHelper : IFolderHelper
    {
        public string ApplicationFolder
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }
    }
}
