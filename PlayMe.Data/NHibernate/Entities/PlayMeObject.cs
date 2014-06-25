using System;

namespace PlayMe.Data.NHibernate.Entities
{
    public class PlayMeObject: DataObject
    {
        private MusicProviderDescriptor musicProvider;
        public virtual string Link { get; set; }
        public virtual string Name { get; set; }
        public virtual Uri ExternalLink { get; set; }

        public virtual MusicProviderDescriptor MusicProvider
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
