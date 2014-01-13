using System;

namespace SpotiFire.SpotifyLib
{
    public class ImageEventArgs : EventArgs
    {
        string imageId;
        public ImageEventArgs(string imageId)
        {
            this.imageId = imageId;
        }
    }
}
