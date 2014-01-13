using System;

namespace SpotiFire.SpotifyLib
{
    public abstract class DisposeableSpotifyObject : IDisposable
    {
        private volatile bool disposed = false;
        private readonly object disposeObject = new object();
        protected abstract void OnDispose();
        protected abstract int IntPtrHashCode();

        public void Dispose()
        {
            lock (disposeObject)
            {
                if (disposed)
                    return;
                disposed = true;
            }
            OnDispose();
        }

        ~DisposeableSpotifyObject()
        {
            try { Dispose(); }
            catch { }
        }

        /// <summary>
        /// Returns wheater or not the current object is still alive (has not been disposed).
        /// </summary>
        /// <param name="throwOnDisposed">If throwOnDisposed is set to true, and the object is disposed an InvalidOperationException is cast.</param>
        /// <returns>True if the object is not disposed, otherwize false.</returns>
        protected bool IsAlive(bool throwOnDisposed = false)
        {
            bool ret;
            lock (disposeObject)
            {
                ret = !disposed;
            }
            if (!ret && throwOnDisposed)
                throw new InvalidOperationException("Can't perform this operation on an disposed object.");
            return ret;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return IntPtrHashCode();
        }

        static public bool operator ==(DisposeableSpotifyObject obj, object obj2)
        {
            if (!Object.ReferenceEquals(obj, obj2))
                return obj.Equals(obj2);
            return true;
        }

        static public bool operator !=(DisposeableSpotifyObject obj, object obj2)
        {
            if (!Object.ReferenceEquals(obj, obj2))
                return !obj.Equals(obj2);
            return false;
        }
    }
}
