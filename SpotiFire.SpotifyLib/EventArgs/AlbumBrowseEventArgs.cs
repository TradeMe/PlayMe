using System;

namespace SpotiFire.SpotifyLib
{
    public class AlbumBrowseEventArgs : EventArgs
    {
        private IAlbumBrowse result;

        internal AlbumBrowseEventArgs(IAlbumBrowse result)
        {
            this.result = result;
        }

        public IAlbumBrowse Result
        {
            get { return result; }
        }
    }
}
