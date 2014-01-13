using System;
using System.Collections.Generic;
using System.Configuration;

namespace PlayMe.Server
{
    public class Settings : ISettings
    {

        public string AdminUsers
        {
            get { return ConfigurationManager.AppSettings["AdminUsers"]; }
		}

        public int MinAutoplayableTracks
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["MinAutoplayableTracks"]); }
        }

        public int MaxAutoplayableTracks
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["MaxAutoplayableTracks"]); }
        }

        public int VetoCount
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["VetoCount"]); }
        }

        public int Randomizer
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["Randomizer"]); }
        }

        public int RandomizerRatio
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["RandomizerRatio"]); }
        }

        public decimal RandomWeighting {
            get
            {
                decimal parsed;
                string unparsed = ConfigurationManager.AppSettings["RandomWeighting"]; 
                return decimal.TryParse(unparsed,out parsed) ? parsed : 1;
            }
        }
        public int DontRepeatTrackForHours
        {
            get
            {
                int parsed;
                string unparsed = ConfigurationManager.AppSettings["DontRepeatTrackForHours"];
                return int.TryParse(unparsed, out parsed) ? parsed : 24;
            }
        }

        public bool AutoStart
        {
            get
            {
                Boolean parsed;
                string unparsed = ConfigurationManager.AppSettings["MusicService.AutoStart"];
                return Boolean.TryParse(unparsed, out parsed) && parsed;
            }
        }

    }
}
