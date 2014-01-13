using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpotiFire.SpotifyLib
{

    internal class Artist : CountedDisposeableSpotifyObject, IArtist
    {
        #region Wrapper
        private class ArtistWrapper : DisposeableSpotifyObject, IArtist
        {
            internal Artist artist;
            internal ArtistWrapper(Artist artist)
            {
                this.artist = artist;
            }

            protected override void OnDispose()
            {
                Artist.Delete(artist.artistPtr);
                artist = null;
            }

            public bool IsLoaded
            {
                get { IsAlive(true); return artist.IsLoaded; }
            }

            public string Name
            {
                get { IsAlive(true); return artist.Name; }
            }

            public ISession Session
            {
                get { IsAlive(true); return artist.Session; }
            }

            public IArtistBrowse Browse(sp_artistbrowse_type type = sp_artistbrowse_type.FULL)
            {
                return IsAlive() ? artist.Browse(type) : null;
            }

            public string PortraitId
            {
                get { IsAlive(true); return artist.PortraitId; }
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? artist.artistPtr.GetHashCode() : 0;
            }
        }
        internal static IntPtr GetPointer(IArtist artist)
        {
            if (artist.GetType() == typeof(ArtistWrapper))
                return ((ArtistWrapper)artist).artist.artistPtr;
            throw new ArgumentException("Invalid artist");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, Artist> artists = new Dictionary<IntPtr, Artist>();
        private static readonly object artistsLock = new object();

        internal static IArtist Get(Session session, IntPtr artistPtr)
        {
            Artist artist;
            lock (artistsLock)
            {
                if (!artists.ContainsKey(artistPtr))
                {
                    artists.Add(artistPtr, new Artist(session, artistPtr));
                }
                artist = artists[artistPtr];
                artist.AddRef();
            }
            return new ArtistWrapper(artist);
        }

        internal static void Delete(IntPtr artistPtr)
        {
            lock (artistsLock)
            {
                Artist artist = artists[artistPtr];
                int count = artist.RemRef();
                if (count == 0)
                {
                    artists.Remove(artistPtr);
                    //artist.Dispose();
                }
            }
        }
        #endregion

        #region Declarations
        internal IntPtr artistPtr = IntPtr.Zero;
        private Session session;
        #endregion

        #region Constructor
        private Artist(Session session, IntPtr artistPtr)
        {
            if (artistPtr == IntPtr.Zero)
                throw new ArgumentException("artistPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.artistPtr = artistPtr;
            lock (libspotify.Mutex)
                libspotify.sp_artist_add_ref(artistPtr);

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);
        }

        void session_DisposeAll(ISession sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Properties
        public bool IsLoaded
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_artist_is_loaded(artistPtr);
            }
        }

        public string Name
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.GetString(libspotify.sp_artist_name(artistPtr), String.Empty);
            }
        }

        public string PortraitId
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.ImageIdToString(libspotify.sp_artist_portrait(artistPtr));
            }
        }

        public ISession Session
        {
            get
            {
                IsAlive(true);
                return session;
            }
        }
        #endregion

        #region Public methods

        public IArtistBrowse Browse(sp_artistbrowse_type type = sp_artistbrowse_type.FULL)
        {
            lock (libspotify.Mutex)
            {
                IntPtr artistBrowsePtr = libspotify.sp_artistbrowse_create(session.sessionPtr, artistPtr, type,
                    Marshal.GetFunctionPointerForDelegate(SpotifyLib.ArtistBrowse.artistbrowse_complete), IntPtr.Zero);
                return artistBrowsePtr != IntPtr.Zero ? SpotifyLib.ArtistBrowse.Get(session, artistBrowsePtr) : null;
            }
        }
               
        #endregion

        #region Cleanup
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                    libspotify.sp_artist_release(artistPtr);

            artistPtr = IntPtr.Zero;
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
