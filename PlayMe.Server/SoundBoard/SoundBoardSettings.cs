using System;
using System.Configuration;

namespace PlayMe.Server.SoundBoard
{
    public class SoundBoardSettings : ISoundBoardSettings
    {
        public bool IsEnabled
        {
            get
            {
                bool parsed;
                string unparsed = ConfigurationManager.AppSettings["SoundBoard.IsEnabled"];
                return bool.TryParse(unparsed, out parsed) && parsed;
            }
        }
        
        public int SecondsBetweenSkipThreshold
        {
            get
            {
                int parsed;
                string unparsed = ConfigurationManager.AppSettings["SoundBoard.SecondsBetweenSkipThreshold"];
                if (int.TryParse(unparsed, out parsed)) return parsed;

                return 10;
            }
        }
    }
}
