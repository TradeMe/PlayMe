using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SpotiFire.SpotifyLib
{
    #region Enums
    public enum sp_linktype
    {
        SP_LINKTYPE_INVALID = 0,
        SP_LINKTYPE_TRACK = 1,
        SP_LINKTYPE_ALBUM = 2,
        SP_LINKTYPE_ARTIST = 3,
        SP_LINKTYPE_SEARCH = 4,
        SP_LINKTYPE_PLAYLIST = 5,
        SP_LINKTYPE_PROFILE = 6,
        SP_LINKTYPE_STARRED = 7,
        SP_LINKTYPE_LOCALTRACK = 8
    }
    #endregion

    internal abstract class Link : CountedDisposeableSpotifyObject, ILink
    {
        #region Wrapper
        private class LinkWrapper<T> : DisposeableSpotifyObject, ILink<T>
        {
            internal Link link;
            internal LinkWrapper(Link link)
            {
                this.link = link;
            }

            public T Object
            {
                get
                {
                    IsAlive(true);
                    return ((ILink<T>)link).Object;
                }
            }

            public sp_linktype Type
            {
                get
                {
                    IsAlive(true);
                    return link.Type;
                }
            }

            object ILink.Object
            {
                get { return Object; }
            }

            public ISession Session
            {
                get
                {
                    IsAlive(true);
                    return link.Session;
                }
            }

            protected override void OnDispose()
            {
                Link.Delete(link.linkPtr);
                link = null;
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? link.IntPtrHashCode() : 0;
            }

            public override string ToString()
            {
                return IsAlive() ? link.ToString() : String.Empty;
            }
        }
        internal static IntPtr GetPointer(ILink link)
        {
            if (typeof(LinkWrapper<>).IsInstanceOfType(link))
                return ((Link)typeof(LinkWrapper<>).GetField("link", BindingFlags.NonPublic).GetValue(link)).linkPtr;
            throw new ArgumentException("Invalid link");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, Link> links = new Dictionary<IntPtr, Link>();
        private static readonly object linksLock = new object();

        internal static ILink Get(Session session, IntPtr linkPtr)
        {
            Link link;
            lock (linksLock)
            {
                if (!links.ContainsKey(linkPtr))
                {
                    links.Add(linkPtr, Link.Create(session, linkPtr));
                }
                link = links[linkPtr];
                link.AddRef();
            }
            return Link.CreateWrapper(link);
        }

        internal static void Delete(IntPtr linkPtr)
        {
            lock (linksLock)
            {
                Link link = links[linkPtr];
                int count = link.RemRef();
                if (count == 0)
                    links.Remove(linkPtr);
            }
        }

        private static Link Create(Session session, IntPtr linkPtr)
        {
            sp_linktype type;
            lock (libspotify.Mutex)
                type = libspotify.sp_link_type(linkPtr);

            switch (type)
            {
                case sp_linktype.SP_LINKTYPE_TRACK:
                    return new TrackLink(session, linkPtr);
                case sp_linktype.SP_LINKTYPE_ALBUM:
                    return new AlbumLink(session, linkPtr);
                case sp_linktype.SP_LINKTYPE_ARTIST:
                    return new ArtistLink(session, linkPtr);
                case sp_linktype.SP_LINKTYPE_SEARCH:
                    throw new ArgumentException("Search not supported.");
                case sp_linktype.SP_LINKTYPE_PLAYLIST:
                    return new PlaylistLink(session, linkPtr);
                case sp_linktype.SP_LINKTYPE_PROFILE:
                    //x return new ProfileLink(session, linkPtr);
                    throw new NotImplementedException("User not implemented");
                case sp_linktype.SP_LINKTYPE_STARRED:
                    return new PlaylistLink(session, linkPtr);
                case sp_linktype.SP_LINKTYPE_LOCALTRACK:
                    return new TrackLink(session, linkPtr);
                default:
                    throw new ArgumentException("Invalid link.");
            }
        }

        private static ILink CreateWrapper(Link link)
        {
            var type = link.Type;
            switch (type)
            {
                case sp_linktype.SP_LINKTYPE_TRACK:
                    return new LinkWrapper<ITrackAndOffset>(link);
                case sp_linktype.SP_LINKTYPE_ALBUM:
                    return new LinkWrapper<IAlbum>(link);
                case sp_linktype.SP_LINKTYPE_ARTIST:
                    return new LinkWrapper<IArtist>(link);
                case sp_linktype.SP_LINKTYPE_SEARCH:
                    throw new ArgumentException("Search not supported.");
                case sp_linktype.SP_LINKTYPE_PLAYLIST:
                    return new LinkWrapper<IPlaylist>(link);
                case sp_linktype.SP_LINKTYPE_PROFILE:
                    //x return new LinkWrapper<IUser>(link);
                    throw new NotImplementedException("User not implemented");
                case sp_linktype.SP_LINKTYPE_STARRED:
                    return new LinkWrapper<IPlaylist>(link);
                case sp_linktype.SP_LINKTYPE_LOCALTRACK:
                    return new LinkWrapper<ITrackAndOffset>(link);
                default:
                    throw new ArgumentException("Invalid link.");
            }
        }
        #endregion

        #region Declarations
        internal IntPtr linkPtr = IntPtr.Zero;
        protected Session session;
        #endregion

        #region Constructor
        private Link(Session session, IntPtr linkPtr)
        {
            if (linkPtr == IntPtr.Zero)
                throw new ArgumentException("linkPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");
            this.session = session;
            this.linkPtr = linkPtr;

            lock (libspotify.Mutex)
                libspotify.sp_link_add_ref(linkPtr);

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);
        }

        void session_DisposeAll(ISession sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Properties
        public sp_linktype Type
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_link_type(linkPtr);
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

        public abstract object Object { get; }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            if (!IsAlive())
                return "";
            IntPtr bufferPtr = IntPtr.Zero;
            try
            {
                int size = libspotify.STRINGBUFFER_SIZE;
                bufferPtr = Marshal.AllocHGlobal(size);
                lock (libspotify.Mutex)
                    libspotify.sp_link_as_string(linkPtr, bufferPtr, size);

                return libspotify.GetString(bufferPtr, String.Empty);
            }
            finally
            {
                try { Marshal.FreeHGlobal(bufferPtr); }
                catch { }
            }
        }
        #endregion

        #region IDisposeable
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);

            if (!session.ProcExit)
                lock (libspotify.Mutex)
                    libspotify.sp_link_release(linkPtr);

            linkPtr = IntPtr.Zero;
        }
        #endregion

        #region LinkTypes
        private class ArtistLink : Link, ILink<IArtist>
        {
            public override object Object
            {
                get
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return Artist.Get(session, libspotify.sp_link_as_artist(linkPtr));
                }
            }

            internal ArtistLink(Session session, IntPtr linkPtr)
                : base(session, linkPtr) { }

            IArtist ILink<IArtist>.Object
            {
                get { return (IArtist)Object; }
            }

        }

        private class TrackLink : Link, ILink<ITrackAndOffset>
        {
            private class LinkAndOffset : ITrackAndOffset
            {
                private ITrack track;
                private TimeSpan offset;

                public LinkAndOffset(Session session, IntPtr linkPtr, int offset)
                {
                    this.offset = TimeSpan.FromSeconds(offset);
                    this.track = SpotifyLib.Track.Get(session, linkPtr);
                }

                public ITrack Track
                {
                    get { return track; }
                }

                public TimeSpan Offset
                {
                    get { return offset; }
                }
            }

            public override object Object
            {
                get
                {
                    IsAlive(true);
                    int offset;
                    IntPtr trackPtr;
                    lock (libspotify.Mutex)
                        trackPtr = libspotify.sp_link_as_track_and_offset(linkPtr, out offset);
                    return new LinkAndOffset(session, trackPtr, offset);
                }
            }

            ITrackAndOffset ILink<ITrackAndOffset>.Object
            {
                get { return (ITrackAndOffset)Object; }
            }

            internal TrackLink(Session session, IntPtr linkPtr)
                : base(session, linkPtr) { }
        }

        private class AlbumLink : Link, ILink<IAlbum>
        {
            public override object Object
            {
                get
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return Album.Get(session, libspotify.sp_link_as_album(linkPtr));
                }
            }

            IAlbum ILink<IAlbum>.Object
            {
                get { return (IAlbum)Object; }
            }

            internal AlbumLink(Session session, IntPtr linkPtr)
                : base(session, linkPtr) { }
        }

        private class PlaylistLink : Link, ILink<IPlaylist>
        {
            public override object Object
            {
                get
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return Playlist.Get(session, libspotify.sp_playlist_create(session.sessionPtr, linkPtr));
                }
            }

            IPlaylist ILink<IPlaylist>.Object
            {
                get { return (IPlaylist)Object; }
            }

            internal PlaylistLink(Session session, IntPtr linkPtr)
                : base(session, linkPtr) { }
        }
        //TODO: User link
        #endregion

        protected override int IntPtrHashCode()
        {
            return linkPtr.GetHashCode();
        }
    }
}
