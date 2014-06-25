using System;

namespace PlayMe.Common.Model
{
    public class PlayMeObject: DataObject
    {
        private string musicProviderIdentifier;
        public string Link { get; set; }
        public string Name { get; set; }
        public Uri ExternalLink { get; set; }

        public string MusicProviderIdentifier
        {
            get
            {
                //For tracks that were saved before we introduced Music Providers
                return musicProviderIdentifier ??  "sp";
            }
            set 
            {
                musicProviderIdentifier = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Link);
        }
    }
}
