using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SpotiFire.SpotifyLib
{

    public delegate void ArtistBrowseEventHandler(IArtistBrowse sender, ArtistBrowseEventArgs e);
    internal class ArtistBrowse : CountedDisposeableSpotifyObject, IArtistBrowse
    {

        #region Wrapper
        private sealed class ArtistBrowseWrapper : DisposeableSpotifyObject, IArtistBrowse
        {
            internal ArtistBrowse artistBrowse;
            public ArtistBrowseWrapper(ArtistBrowse artistBrowse)
            {
                this.artistBrowse = artistBrowse;
                artistBrowse.Complete += new ArtistBrowseEventHandler(artistBrowse_Complete);
            }

            void artistBrowse_Complete(IArtistBrowse sender, ArtistBrowseEventArgs e)
            {
                if (sender == artistBrowse)
                    if (Complete != null)
                        Complete(this, e);
            }

            public event ArtistBrowseEventHandler Complete;

            protected override void OnDispose()
            {
                artistBrowse.Complete -= new ArtistBrowseEventHandler(artistBrowse_Complete);
                ArtistBrowse.Delete(artistBrowse.artistBrowsePtr);
                artistBrowse = null;
            }

            public sp_error Error
            {
                get { IsAlive(true); return artistBrowse.Error; }
            }

            public IArtist Artist
            {
                get { IsAlive(true); return artistBrowse.Artist; }
            }

            public IArray<string> PortraitIds
            {
                get { IsAlive(true); return artistBrowse.PortraitIds; }
            }

            public IArray<IAlbum> Albums
            {
                get { IsAlive(true); return artistBrowse.Albums; }
            }

            public IArray<ITrack> Tracks
            {
                get { IsAlive(true); return artistBrowse.Tracks; }
            }

            public IArray<IArtist> SimilarArtists
            {
                get { IsAlive(true); return artistBrowse.SimilarArtists; }
            }

            public string Biography
            {
                get { IsAlive(true); return artistBrowse.Biography; }
            }

            public ISession Session
            {
                get { IsAlive(true); return artistBrowse.Session; }
            }

            public bool IsComplete
            {
                get { IsAlive(true); return artistBrowse.IsComplete; }
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? artistBrowse.artistBrowsePtr.GetHashCode() : 0;
            }

        }
        internal static IntPtr GetPointer(IArtistBrowse artistBrowse)
        {
            if (artistBrowse.GetType() == typeof(ArtistBrowseWrapper))
                return ((ArtistBrowseWrapper)artistBrowse).artistBrowse.artistBrowsePtr;
            throw new ArgumentException("Invalid artist browse");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, ArtistBrowse> artistBrowsers = new Dictionary<IntPtr, ArtistBrowse>();
        private static readonly object artistsBrowseLock = new object();

        internal static IArtistBrowse Get(Session session, IntPtr artistBrowsePtr)
        {
            ArtistBrowse artistBrowse;
            lock (artistsBrowseLock)
            {
                if (!artistBrowsers.ContainsKey(artistBrowsePtr))
                {
                    artistBrowsers.Add(artistBrowsePtr, new ArtistBrowse(session, artistBrowsePtr));
                }
                artistBrowse = artistBrowsers[artistBrowsePtr];
                artistBrowse.AddRef();
            }
            return new ArtistBrowseWrapper(artistBrowse);
        }

        internal static void Delete(IntPtr artistBrowsePtr)
        {
            lock (artistsBrowseLock)
            {
                ArtistBrowse artistBrowse = artistBrowsers[artistBrowsePtr];
                int count = artistBrowse.RemRef();
                if (count == 0)
                    artistBrowsers.Remove(artistBrowsePtr);
            }
        }
        #endregion

        #region Delegates
        internal delegate void artistbrowse_complete_cb(IntPtr artistBrowsePtr, IntPtr userdataPtr);
        #endregion

        #region Declarations

        internal IntPtr artistBrowsePtr = IntPtr.Zero;
        private Session session;
        private bool isComplete = false;

        internal static artistbrowse_complete_cb artistbrowse_complete = new artistbrowse_complete_cb(_ArtistBrowseCompleteCallback);

        private IArtist artist;
        private DelegateArray<string> portraitIds;
        private DelegateArray<ITrack> tracks;
        private DelegateArray<IAlbum> albums;
        private DelegateArray<IArtist> similarArtists;
        private string biography;
        #endregion

        #region Constructor and setup
        private ArtistBrowse(Session session, IntPtr artistBrowsePtr)
        {

            if (artistBrowsePtr == IntPtr.Zero)
                throw new ArgumentException("artistBrowsePtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;

            this.artistBrowsePtr = artistBrowsePtr;

            this.portraitIds = new DelegateArray<string>(() =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_artistbrowse_num_portraits(artistBrowsePtr);
            }, (index) =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.ImageIdToString(libspotify.sp_artistbrowse_portrait(artistBrowsePtr, index));
            });

            this.tracks = new DelegateArray<ITrack>(() =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_artistbrowse_num_tracks(artistBrowsePtr);
            }, (index) =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return Track.Get(session, libspotify.sp_artistbrowse_track(artistBrowsePtr, index));
            });

            this.albums = new DelegateArray<IAlbum>(() =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_artistbrowse_num_albums(artistBrowsePtr);
            }, (index) =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return Album.Get(session, libspotify.sp_artistbrowse_album(artistBrowsePtr, index));
            });

            this.similarArtists = new DelegateArray<IArtist>(() =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_artistbrowse_num_similar_artists(artistBrowsePtr);
            }, (index) =>
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return SpotifyLib.Artist.Get(session, libspotify.sp_artistbrowse_similar_artist(artistBrowsePtr, index));
            });

            _Complete += new artistbrowse_complete_cb(ArtistBrowse__Complete);
            session.DisposeAll += new SessionEventHandler(session_DisposeAll);
        }

        private void ArtistBrowse__Complete(IntPtr artistBrowsePtr, IntPtr userdataPtr)
        {
            if (artistBrowsePtr == this.artistBrowsePtr)
            {
                this.isComplete = true;
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<ArtistBrowseEventArgs>(ab => ab.OnComplete, this), new ArtistBrowseEventArgs(this)));
                _Complete -= new artistbrowse_complete_cb(ArtistBrowse__Complete);
            }
        }

        static void _ArtistBrowseCompleteCallback(IntPtr artistBrowsePtr, IntPtr userdataPtr)
        {
            if (_Complete != null)
                _Complete(artistBrowsePtr, userdataPtr);
        }
        #endregion

        #region Properties

        public sp_error Error
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_artistbrowse_error(artistBrowsePtr);
            }
        }

        public IArtist Artist
        {
            get
            {
                if (artist == null)
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        artist = SpotifyLib.Artist.Get(session, libspotify.sp_artistbrowse_artist(artistBrowsePtr));
                }
                return artist;
            }
        }

        public IArray<string> PortraitIds
        {
            get
            {
                return portraitIds;
            }
        }

        public IArray<IAlbum> Albums
        {
            get
            {
                return albums;
            }
        }

        public IArray<ITrack> Tracks
        {
            get
            {
                return tracks;
            }
        }

        public IArray<IArtist> SimilarArtists
        {
            get
            {
                return similarArtists;
            }
        }

        public string Biography
        {
            get
            {
                if (biography == null)
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        biography = libspotify.GetString(libspotify.sp_artistbrowse_biography(artistBrowsePtr), String.Empty);
                }
                return biography;
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

        public bool IsComplete
        {
            get
            {
                IsAlive(true);
                return isComplete;
            }
        }
        #endregion

        #region Public methods

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[ArtistBrowse]");
            sb.AppendLine("Error=" + Error);
            sb.AppendLine(Artist.ToString());
            sb.AppendLine("PortraitsIds.Length=" + PortraitIds.Count);
            foreach (string portraitId in PortraitIds)
                sb.AppendLine(portraitId);

            sb.AppendLine("Tracks.Length=" + Tracks.Count);
            foreach (Track t in Tracks)
                sb.AppendLine(t.ToString());
            sb.AppendLine("SimilarArtists.Length=" + SimilarArtists.Count);
            foreach (Artist a in SimilarArtists)
                sb.AppendLine(a.ToString());

            sb.AppendFormat("Biography:{0}{1}", Environment.NewLine, Biography);

            return sb.ToString();
        }


        #endregion

        #region Cleanup

        protected override void OnDispose()
        {
            _Complete -= new artistbrowse_complete_cb(ArtistBrowse__Complete);
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                    libspotify.sp_artistbrowse_release(artistBrowsePtr);

            artistBrowsePtr = IntPtr.Zero;
        }

        private void session_DisposeAll(ISession sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Private methods
        private static Delegate CreateDelegate<T>(Expression<Func<ArtistBrowse, Action<T>>> expr, ArtistBrowse ab) where T : ArtistBrowseEventArgs
        {
            return expr.Compile().Invoke(ab);
        }
        #endregion

        #region Protected Methods
        protected virtual void OnComplete(ArtistBrowseEventArgs args)
        {
            if (this.Complete != null)
                this.Complete(this, args);
        }
        #endregion

        #region Event
        public event ArtistBrowseEventHandler Complete;
        private static event artistbrowse_complete_cb _Complete;
        #endregion

        protected override int IntPtrHashCode()
        {
            throw new NotImplementedException();
        }

    }
}
