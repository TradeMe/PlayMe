
namespace SpotiFire.SpotifyLib
{
    internal abstract class CountedDisposeableSpotifyObject : DisposeableSpotifyObject
    {
        private volatile int _ref = 0;
        private readonly object refLock = new object();
        internal protected void AddRef()
        {
            lock (refLock)
                _ref++;
        }
        internal protected int RemRef()
        {
            lock (refLock)
                return --_ref;
        }
    }
}
