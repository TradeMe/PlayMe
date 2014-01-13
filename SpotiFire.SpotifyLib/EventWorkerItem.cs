using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SpotiFire.SpotifyLib
{
    internal class EventWorkItem
    {	
        private Delegate handler;
        private object[] args;

        internal EventWorkItem(Delegate handler, params object[] args)
        {
            this.handler = handler;
            this.args = args;
        }
        
        internal void Execute()
        {
            if(handler != null)
            {
                handler.DynamicInvoke(this.args);	
            }
        }
    }
}
