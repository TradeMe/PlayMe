using System;

namespace PlayMe.Common.Model
{
    public class PlayMeObject
    {

        private MusicProviderDescriptor musicProvider;

        public string Link { get; set; }
        public string Name { get; set; }
        public Uri ExternalLink { get; set; }

        public MusicProviderDescriptor MusicProvider
        {
            get
            {
                //For tracks that were saved before we introduced Music Providers
                return musicProvider ?? new MusicProviderDescriptor { Name = "Spotify", Identifier = "sp" };
            }

            set { musicProvider = value; }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Link);
        }
        
    }
}
