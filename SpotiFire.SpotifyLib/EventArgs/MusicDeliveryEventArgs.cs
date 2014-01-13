using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotiFire.SpotifyLib
{
    public class MusicDeliveryEventArgs
    {
        private int channels;
        private int rate;
        private byte[] samples;
        private int frames;

        internal MusicDeliveryEventArgs(int channels, int rate, byte[] samples, int frames)
        {
            this.channels = channels;
            this.rate = rate;
            this.samples = samples;
            this.frames = frames;

            this.ConsumedFrames = 0;
        }

        public int ConsumedFrames
        {
            get;
            set;
        }

        public int Frames
        {
            get
            {
                return frames;
            }
        }

        public int Channels
        {
            get
            {
                return channels;
            }
        }

        public int Rate
        {
            get
            {
                return rate;
            }
        }


        public byte[] Samples
        {
            get
            {
                return samples;
            }
        }
    }
}
