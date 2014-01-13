using System;

namespace SpotiFire.SpotifyLib
{
    public class ArtistBrowseEventArgs : EventArgs
    {
        private IArtistBrowse result;

        internal ArtistBrowseEventArgs(IArtistBrowse result)
        {
            this.result = result;
        }

        public IArtistBrowse Result
        {
            get { return result; }
        }
    }
}
