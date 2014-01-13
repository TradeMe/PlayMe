using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SpotiFire.SpotifyLib
{
    internal class Album : CountedDisposeableSpotifyObject, IAlbum
    {
        #region Wrapper
        private class AlbumWrapper : DisposeableSpotifyObject, IAlbum
        {
            internal Album album;
            internal AlbumWrapper(Album album)
            {
                this.album = album;
            }

            protected override void OnDispose()
            {
                Album.Delete(album.albumPtr);
                album = null;
            }

            public IArtist Artist
            {
                get { IsAlive(true); return album.Artist; }
            }

            public string CoverId
            {
                get { IsAlive(true); return album.CoverId; }
            }

            public bool IsAvailable
            {
                get { IsAlive(true); return album.IsAvailable; }
            }

            public bool IsLoaded
            {
                get { IsAlive(true); return album.IsLoaded; }
            }

            public string Name
            {
                get { IsAlive(true); return album.Name; }
            }

            public sp_albumtype Type
            {
                get { IsAlive(true); return album.Type; }
            }

            public int Year
            {
                get { IsAlive(true); return album.Year; }
            }

            public ISession Session
            {
                get { IsAlive(true); return album.Session; }
            }

            public IAlbumBrowse Browse()
            {
                return IsAlive() ? album.Browse() : null;
            }

            protected override int IntPtrHashCode()
            {
                if (!IsAlive()) return 0;
                return album.albumPtr.GetHashCode();
            }
        }
        internal static IntPtr GetPointer(IAlbum album)
        {
            if (album.GetType() == typeof(AlbumWrapper))
                return ((AlbumWrapper)album).album.albumPtr;
            throw new ArgumentException("Invalid album");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, Album> albums = new Dictionary<IntPtr, Album>();
        private static readonly object albumsLock = new object();

        internal static IAlbum Get(Session session, IntPtr albumPtr)
        {
            Album album;
            lock (albumsLock)
            {
                if (!albums.ContainsKey(albumPtr))
                {
                    albums.Add(albumPtr, new Album(session, albumPtr));
                }
                album = albums[albumPtr];
                album.AddRef();
            }
            return new AlbumWrapper(album);
        }

        internal static void Delete(IntPtr albumPtr)
        {
            lock (albumsLock)
            {
                Album album = albums[albumPtr];
                int count = album.RemRef();
                if (count == 0)
                {
                    albums.Remove(albumPtr);
                    //album.Dispose();
                }
            }
        }
        #endregion

        #region Declarations
        internal IntPtr albumPtr = IntPtr.Zero;
        private Session session;

        private IArtist artist = null;
        #endregion

        #region Constructor
        private Album(Session session, IntPtr albumPtr)
        {
            if (albumPtr == IntPtr.Zero)
                throw new ArgumentException("albumPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.albumPtr = albumPtr;
            lock (libspotify.Mutex)
                libspotify.sp_album_add_ref(albumPtr);

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
                    return libspotify.sp_album_is_loaded(albumPtr);
            }
        }

        public bool IsAvailable
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_album_is_available(albumPtr);
            }
        }

        public IArtist Artist
        {
            get
            {
                if (artist == null)
                    lock (libspotify.Mutex)
                        artist = SpotifyLib.Artist.Get(session, libspotify.sp_album_artist(albumPtr));

                return artist;
            }
        }

        public string Name
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.GetString(libspotify.sp_album_name(albumPtr), String.Empty);
            }
        }

        public string CoverId
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.ImageIdToString(libspotify.sp_album_cover(albumPtr));         
            }
        }

        public int Year
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_album_year(albumPtr);
            }
        }

        public sp_albumtype Type
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_album_type(albumPtr);
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

        public IAlbumBrowse Browse() {
            lock (libspotify.Mutex) {
                IntPtr albumBrowsePtr = libspotify.sp_albumbrowse_create(session.sessionPtr, albumPtr,
                    System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(SpotifyLib.AlbumBrowse.albumbrowse_complete), IntPtr.Zero);
                return albumBrowsePtr != IntPtr.Zero ? SpotifyLib.AlbumBrowse.Get(session, albumBrowsePtr) : null;
            }
        }

        #endregion

        #region Cleanup
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);
            if(!session.ProcExit)
                lock (libspotify.Mutex)
                    libspotify.sp_album_release(albumPtr);

            albumPtr = IntPtr.Zero;
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            return albumPtr.GetHashCode();
        }
    }
}
