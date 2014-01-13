using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SpotiFire.SpotifyLib
{
    internal class Track : CountedDisposeableSpotifyObject, ITrack
    {
        #region Wrapper
        protected class TrackWrapper : DisposeableSpotifyObject, ITrack
        {
            internal Track track;
            internal TrackWrapper(Track track)
            {
                this.track = track;
            }

            protected override void OnDispose()
            {
                Track.Delete(track.trackPtr);
                track = null;
            }

            public IAlbum Album
            {
                get { IsAlive(true); return track.Album; }
            }

            public IArray<IArtist> Artists
            {
                get { IsAlive(true); return track.Artists; }
            }

            public int Disc
            {
                get { IsAlive(true); return track.Disc; }
            }

            public TimeSpan Duration
            {
                get { IsAlive(true); return track.Duration; }
            }

            public sp_error Error
            {
                get { IsAlive(true); return track.Error; }
            }

            public int Index
            {
                get { IsAlive(true); return track.Index; }
            }

            public bool IsAvailable
            {
                get { IsAlive(true); return track.IsAvailable; }
            }

            public bool IsLoaded
            {
                get { IsAlive(true); return track.IsLoaded; }
            }

            public string Name
            {
                get { IsAlive(true); return track.Name; }
            }

            public int Popularity
            {
                get { IsAlive(true); return track.Popularity; }
            }

            public bool IsStarred
            {
                get { IsAlive(true); return track.IsStarred; }
                set { IsAlive(true); track.IsStarred = value; }
            }

            public ISession Session
            {
                get { IsAlive(true); return track.Session; }
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? track.trackPtr.GetHashCode() : 0;
            }
        }
        internal static IntPtr GetPointer(ITrack track)
        {
            if (track is TrackWrapper)
                return ((TrackWrapper)track).track.trackPtr;
            throw new ArgumentException("Invalid track");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, Track> tracks = new Dictionary<IntPtr, Track>();
        private static readonly object tracksLock = new object();

        internal static ITrack Get(Session session, IntPtr trackPtr)
        {
            Track track;
            lock (tracksLock)
            {
                if (!tracks.ContainsKey(trackPtr))
                {
                    tracks.Add(trackPtr, new Track(session, trackPtr));
                }
                track = tracks[trackPtr];
                track.AddRef();
            }
            return new TrackWrapper(track);
        }

        internal static void Delete(IntPtr trackPtr)
        {
            lock (tracksLock)
            {
                Track track = tracks[trackPtr];
                int count = track.RemRef();
                if (count == 0)
                    tracks.Remove(trackPtr);
            }
        }
        #endregion

        #region Declarations
        internal IntPtr trackPtr = IntPtr.Zero;
        private Session session;

        private IAlbum album = null;
        private DelegateArray<IArtist> artists = null;
        #endregion

        #region Constructor
        internal protected Track(Session session, IntPtr trackPtr)
        {
            if (trackPtr == IntPtr.Zero)
                throw new ArgumentException("trackPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.trackPtr = trackPtr;
            lock (libspotify.Mutex)
                libspotify.sp_track_add_ref(trackPtr);

            this.artists = new DelegateArray<IArtist>(() =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_track_num_artists(trackPtr);
            }, (index) =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return Artist.Get(session, libspotify.sp_track_artist(trackPtr, index));
            });

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
                    return libspotify.sp_track_is_loaded(trackPtr);
            }
        }

        public bool IsAvailable
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_track_get_availability(session.sessionPtr, trackPtr)==sp_track_availability.SP_TRACK_AVAILABILITY_AVAILABLE;
            }
        }

        public sp_error Error
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_track_error(trackPtr);
            }
        }

        public IAlbum Album
        {
            get
            {
                IsAlive(true);
                if (album == null)
                    lock (libspotify.Mutex)
                        album = SpotifyLib.Album.Get(session, libspotify.sp_track_album(trackPtr));

                return album;
            }
        }

        public IArray<IArtist> Artists
        {
            get
            {
                IsAlive(true);
                return artists;
            }
        }

        public string Name
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.GetString(libspotify.sp_track_name(trackPtr), String.Empty);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return TimeSpan.FromMilliseconds(libspotify.sp_track_duration(trackPtr));
            }
        }

        public int Popularity
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_track_popularity(trackPtr);
            }
        }

        public bool IsStarred
        {
            get
            {
                IsAlive(true);
                //x lock (libspotify.Mutex)
                //x     return libspotify.sp_track_is_starred(session.sessionPtr, trackPtr);

                return session.Starred.Tracks.Any(t => Track.GetPointer(t) == this.trackPtr);
            }
            set
            {
                IsAlive(true);
                IntPtr arrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
                IntPtr[] ptrArray = new IntPtr[] { trackPtr };
                try
                {
                    Marshal.Copy(ptrArray, 0, arrayPtr, 1);
                    lock (libspotify.Mutex)
                        libspotify.sp_track_set_starred(session.sessionPtr, arrayPtr, 1, value);
                }
                finally
                {
                    try { Marshal.FreeHGlobal(arrayPtr); }
                    catch { }
                }
            }
        }

        public int Disc
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_track_disc(trackPtr);
            }
        }

        public int Index
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_track_index(trackPtr);
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

        #region IDisposeable
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                    libspotify.sp_track_release(trackPtr);

            trackPtr = IntPtr.Zero;
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            return trackPtr.GetHashCode();
        }
    }
}
