using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

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

        public float StartUpVolume
        {
            get
            {
                int parsed;
                string unparsed = ConfigurationManager.AppSettings["StartUpVolumePercentage"];
                return int.TryParse(unparsed, out parsed) ? (parsed / 100f) : 0.5f;
            }
        }

        public int Randomizer
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["Randomizer"]); }
        }

        public List<int> Randomizers
        {
            get
            {
                return ConfigurationManager.AppSettings["Randomizers"].Split(';')
                       .Select(r => Convert.ToInt32(r)).ToList();
            }
        }

        public int RandomizerRatio
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["RandomizerRatio"]); }
        }

        public decimal RandomWeighting {
            get
            {
                decimal parsed;
                var unparsed = ConfigurationManager.AppSettings["RandomWeighting"]; 
                return decimal.TryParse(unparsed,out parsed) ? parsed : 1;
            }
        }
        public int DontRepeatTrackForHours
        {
            get
            {
                int parsed;
                var unparsed = ConfigurationManager.AppSettings["DontRepeatTrackForHours"];
                return int.TryParse(unparsed, out parsed) ? parsed : 24;
            }
        }

        public bool AutoStart
        {
            get
            {
                Boolean parsed;
                var unparsed = ConfigurationManager.AppSettings["MusicService.AutoStart"];
                return Boolean.TryParse(unparsed, out parsed) && parsed;
            }
        }

        public bool ForgetTrackThatExceedsMaxVetoes
        {
            get
            {
                Boolean parsed;
                var unparsed = ConfigurationManager.AppSettings["ForgetTrackThatExceedsMaxVetoes"];
                return Boolean.TryParse(unparsed, out parsed) && parsed;
            }
        }
    }
}
