using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class SpotifyException : Exception
    {
        private sp_error res;

        public SpotifyException(sp_error res)
        {
            // TODO: Complete member initialization
            this.res = res;
        }

        public sp_error SpotifyError
        {
            get { return res; }
        }
    }
}
