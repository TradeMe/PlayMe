using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class SessionEventArgs : EventArgs
    {
        sp_error error = sp_error.OK;
        string message = string.Empty;

        internal SessionEventArgs()
        {

        }

        internal SessionEventArgs(sp_error error)
        {
            this.error = error;
        }

        internal SessionEventArgs(string message)
        {
            this.message = message;
        }

        public sp_error Status
        {
            get
            {
                return this.error;
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
        }

        public override string ToString()
        {
            return String.Format("SessionEvent:\n\tStatus: {0}\n\tMessage: {1}", Status, Message);
        }
    }
}
