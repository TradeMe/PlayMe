using System;

namespace SpotiFire.SpotifyLib
{
    public class SearchEventArgs : EventArgs
    {
        private ISearch result;

        internal SearchEventArgs(ISearch result)
        {
            this.result = result;
        }

        public ISearch Result
        {
            get
            {
                return result;
            }
        }
    }
}
