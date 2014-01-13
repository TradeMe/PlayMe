using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace SpotiFire.SpotifyLib
{
    public delegate void PlaylistEventHandler(IPlaylist playlist, EventArgs args);
    public delegate void PlaylistEventHandler<TEventArgs>(IPlaylist playlist, TEventArgs args) where TEventArgs : EventArgs;



    internal class Playlist : CountedDisposeableSpotifyObject, IPlaylist
    {
        #region Wrapper
        internal protected class PlaylistWrapper : DisposeableSpotifyObject, IPlaylist
        {
            internal Playlist playlist;
            public PlaylistWrapper(Playlist playlist)
            {
                this.playlist = playlist;
                playlist.TracksAdded += new PlaylistEventHandler<TracksAddedEventArgs>(playlist_TracksAdded);
                playlist.TracksRemoved += new PlaylistEventHandler<TracksEventArgs>(playlist_TracksRemoved);
                playlist.TracksMoved += new PlaylistEventHandler<TracksMovedEventArgs>(playlist_TracksMoved);
                playlist.Renamed += new PlaylistEventHandler(playlist_Renamed);
                playlist.StateChanged += new PlaylistEventHandler(playlist_StateChanged);
                playlist.UpdateInProgress += new PlaylistEventHandler<PlaylistUpdateEventArgs>(playlist_UpdateInProgress);
                playlist.MetadataUpdated += new PlaylistEventHandler(playlist_MetadataUpdated);
                playlist.TrackCreatedChanged += new PlaylistEventHandler<TrackCreatedChangedEventArgs>(playlist_TrackCreatedChanged);
                playlist.TrackSeenChanged += new PlaylistEventHandler<TrackSeenEventArgs>(playlist_TrackSeenChanged);
                playlist.DescriptionChanged += new PlaylistEventHandler<DescriptionEventArgs>(playlist_DescriptionChanged);
                playlist.ImageChanged += new PlaylistEventHandler<ImageEventArgs>(playlist_ImageChanged);
            }

            void playlist_ImageChanged(IPlaylist playlist, ImageEventArgs args)
            {
                if (ImageChanged != null)
                    ImageChanged(this, args);
            }

            void playlist_DescriptionChanged(IPlaylist playlist, DescriptionEventArgs args)
            {
                if (DescriptionChanged != null)
                    DescriptionChanged(this, args);
            }

            void playlist_TrackSeenChanged(IPlaylist playlist, TrackSeenEventArgs args)
            {
                if (TrackSeenChanged != null)
                    TrackSeenChanged(this, args);
            }

            void playlist_TrackCreatedChanged(IPlaylist playlist, TrackCreatedChangedEventArgs args)
            {
                if (TrackCreatedChanged != null)
                    TrackCreatedChanged(this, args);
            }

            void playlist_MetadataUpdated(IPlaylist playlist, EventArgs args)
            {
                if (MetadataUpdated != null)
                    MetadataUpdated(this, args);
            }

            void playlist_UpdateInProgress(IPlaylist playlist, PlaylistUpdateEventArgs args)
            {
                if (UpdateInProgress != null)
                    UpdateInProgress(this, args);
            }

            void playlist_StateChanged(IPlaylist playlist, EventArgs args)
            {
                if (StateChanged != null)
                    StateChanged(this, args);
            }

            void playlist_Renamed(IPlaylist playlist, EventArgs args)
            {
                if (Renamed != null)
                    Renamed(this, args);
            }

            void playlist_TracksMoved(IPlaylist playlist, TracksMovedEventArgs args)
            {
                if (TracksMoved != null)
                    TracksMoved(this, args);
            }

            void playlist_TracksRemoved(IPlaylist playlist, TracksEventArgs args)
            {
                if (TracksRemoved != null)
                    TracksRemoved(this, args);
            }

            void playlist_TracksAdded(IPlaylist playlist, TracksAddedEventArgs args)
            {
                if (TracksAdded != null)
                    TracksAdded(this, args);
            }

            protected override void OnDispose()
            {
                playlist.TracksAdded -= new PlaylistEventHandler<TracksAddedEventArgs>(playlist_TracksAdded);
                playlist.TracksRemoved -= new PlaylistEventHandler<TracksEventArgs>(playlist_TracksRemoved);
                playlist.TracksMoved -= new PlaylistEventHandler<TracksMovedEventArgs>(playlist_TracksMoved);
                playlist.Renamed -= new PlaylistEventHandler(playlist_Renamed);
                playlist.StateChanged -= new PlaylistEventHandler(playlist_StateChanged);
                playlist.UpdateInProgress -= new PlaylistEventHandler<PlaylistUpdateEventArgs>(playlist_UpdateInProgress);
                playlist.MetadataUpdated -= new PlaylistEventHandler(playlist_MetadataUpdated);
                playlist.TrackCreatedChanged -= new PlaylistEventHandler<TrackCreatedChangedEventArgs>(playlist_TrackCreatedChanged);
                playlist.TrackSeenChanged -= new PlaylistEventHandler<TrackSeenEventArgs>(playlist_TrackSeenChanged);
                playlist.DescriptionChanged -= new PlaylistEventHandler<DescriptionEventArgs>(playlist_DescriptionChanged);
                playlist.ImageChanged -= new PlaylistEventHandler<ImageEventArgs>(playlist_ImageChanged);
                if (this.GetType() == typeof(PlaylistWrapper))
                    Playlist.Delete(playlist.playlistPtr);
                playlist = null;
            }

            protected override int IntPtrHashCode()
            {
                return IsAlive() ? playlist.IntPtrHashCode() : 0;
            }

            public IPlaylistTrackList Tracks
            {
                get { IsAlive(true); return playlist.Tracks; }
            }

            public bool IsLoaded
            {
                get { IsAlive(true); return playlist.IsLoaded; }
            }

            public string Name
            {
                get { IsAlive(true); return playlist.Name; }
                set { IsAlive(true); playlist.Name = value; }
            }

            public ISession Session
            {
                get { IsAlive(true); return playlist.Session; }
            }

            public event PlaylistEventHandler<TracksAddedEventArgs> TracksAdded;

            public event PlaylistEventHandler<TracksEventArgs> TracksRemoved;

            public event PlaylistEventHandler<TracksMovedEventArgs> TracksMoved;

            public event PlaylistEventHandler Renamed;

            public event PlaylistEventHandler StateChanged;

            public event PlaylistEventHandler<PlaylistUpdateEventArgs> UpdateInProgress;

            public event PlaylistEventHandler MetadataUpdated;

            public event PlaylistEventHandler<TrackCreatedChangedEventArgs> TrackCreatedChanged;

            public event PlaylistEventHandler<TrackSeenEventArgs> TrackSeenChanged;

            public event PlaylistEventHandler<DescriptionEventArgs> DescriptionChanged;

            public event PlaylistEventHandler<ImageEventArgs> ImageChanged;


            public string ImageId
            {
                get { IsAlive(true); return playlist.ImageId; }
            }


            public bool IsColaberativ
            {
                get { IsAlive(true); return playlist.IsColaberativ; }
                set { IsAlive(true); playlist.IsColaberativ = value; }
            }

            public void AutoLinkTracks(bool autoLink)
            {
                IsAlive(true);
                playlist.AutoLinkTracks(autoLink);
            }

            public string Description
            {
                get { IsAlive(true); return playlist.Description; }
            }

            public bool PendingChanges
            {
                get { IsAlive(true); return playlist.PendingChanges; }
            }
        }
        internal static IntPtr GetPointer(IPlaylist playlist)
        {
            if (playlist.GetType() == typeof(PlaylistWrapper))
                return ((PlaylistWrapper)playlist).playlist.playlistPtr;
            throw new ArgumentException("Invalid playlist");
        }
        #endregion
        #region Counter
        private static Dictionary<IntPtr, Playlist> playlists = new Dictionary<IntPtr, Playlist>();
        private static readonly object playlistsLock = new object();

        internal static IPlaylist Get(Session session, IntPtr playlistPtr)
        {
            Playlist playlist;
            lock (playlistsLock)
            {
                if (!playlists.ContainsKey(playlistPtr))
                {
                    playlists.Add(playlistPtr, new Playlist(session, playlistPtr));
                }
                playlist = playlists[playlistPtr];
                playlist.AddRef();
            }
            return new PlaylistWrapper(playlist);
        }

        internal static void Delete(IntPtr playlistPtr)
        {
            lock (playlistsLock)
            {
                Playlist playlist = playlists[playlistPtr];
                int count = playlist.RemRef();
                if (count == 0)
                    playlists.Remove(playlistPtr);
            }
        }
        #endregion

        #region Spotify Delegates
        #region Struct
        private struct sp_playlist_callbacks
        {
            internal IntPtr tracks_added;
            internal IntPtr tracks_removed;
            internal IntPtr tracks_moved;
            internal IntPtr playlist_renamed;
            internal IntPtr playlist_state_changed;
            internal IntPtr playlist_update_in_progress;
            internal IntPtr playlist_metadata_updated;
            internal IntPtr track_created_changed;
            internal IntPtr track_seen_changed;
            internal IntPtr description_changed;
            internal IntPtr image_changed;
            internal IntPtr track_message_changed; //TODO: Implement
            internal IntPtr subscribers_changed; // TODO: Implement
        }
        #endregion
        private delegate void tracks_added_cb(IntPtr playlistPtr, IntPtr tracksPtr, int num_tracks, int position, IntPtr userdataPtr);
        private delegate void tracks_removed_cb(IntPtr playlistPtr, IntPtr trackIndicesPtr, int num_tracks, IntPtr userdataPtr);
        private delegate void tracks_moved_cd(IntPtr playlistPtr, IntPtr trackIndicesPtr, int num_tracks, int new_position, IntPtr userdataPtr);
        private delegate void playlist_renamed_cb(IntPtr playlistPtr, IntPtr userdataPtr);
        private delegate void playlist_state_changed_cb(IntPtr playlistPtr, IntPtr userdataPtr);
        private delegate void playlist_update_in_progress_cb(IntPtr playlistPtr, bool done, IntPtr userdataPtr);
        private delegate void playlist_metadata_updated_cb(IntPtr playlistPtr, IntPtr userdataPtr);
        private delegate void track_created_changed_cb(IntPtr playlistPtr, int position, IntPtr userPtr, int when, IntPtr userdataPtr);
        private delegate void track_seen_changed_cb(IntPtr playlistPtr, int position, bool seen, IntPtr userdataPtr);
        private delegate void description_changed_cb(IntPtr playlistPtr, IntPtr descPtr, IntPtr userdataPtr);
        private delegate void image_changed_cb(IntPtr playlistPtr, IntPtr imgIdPtr, IntPtr userdataPtr);
        #endregion

        #region Spotify Callbacks
        private tracks_added_cb tracks_added;
        private tracks_removed_cb tracks_removed;
        private tracks_moved_cd tracks_moved;
        private playlist_renamed_cb playlist_renamed;
        private playlist_state_changed_cb playlist_state_changed;
        private playlist_update_in_progress_cb playlist_update_in_progress;
        private playlist_metadata_updated_cb playlist_metadata_updated;
        private track_created_changed_cb track_created_changed;
        private track_seen_changed_cb track_seen_changed;
        private description_changed_cb description_changed;
        private image_changed_cb image_changed;
        #endregion

        #region Spotify Callback Implementations
        private void TracksAddedCallback(IntPtr playlistPtr, IntPtr tracksPtr, int num_tracks, int position, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                int[] trackIndices = new int[num_tracks];
                ITrack[] tracks = new ITrack[num_tracks];
                IntPtr[] trackPtrs = new IntPtr[num_tracks];
                Marshal.Copy(tracksPtr, trackPtrs, 0, num_tracks);

                for (int i = 0; i < num_tracks; i++)
                {
                    trackIndices[i] = position + i;
                    tracks[i] = Track.Get(session, trackPtrs[i]);
                }

                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TracksAddedEventArgs>(p => p.OnTracksAdded, this), new TracksAddedEventArgs(trackIndices, tracks)));
            }
        }

        private void TracksRemovedCallback(IntPtr playlistPtr, IntPtr trackIndicesPtr, int num_tracks, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                int[] trackIndices = new int[num_tracks];
                Marshal.Copy(trackIndicesPtr, trackIndices, 0, num_tracks);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TracksEventArgs>(p => p.OnTracksRemoved, this), new TracksEventArgs(trackIndices)));
            }
        }

        private void TracksMovedCallback(IntPtr playlistPtr, IntPtr trackIndicesPtr, int num_tracks, int new_position, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                int[] trackIndices = new int[num_tracks];
                Marshal.Copy(trackIndicesPtr, trackIndices, 0, num_tracks);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TracksMovedEventArgs>(p => p.OnTracksMoved, this), new TracksMovedEventArgs(trackIndices, new_position)));
            }
        }

        private void RenamedCallback(IntPtr playlistPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(p => p.OnRenamed, this), new EventArgs()));
            }
        }

        private void StateChangedCallback(IntPtr playlistPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(p => p.OnStateChanged, this), new EventArgs()));
            }
        }

        private void UpdateInProgressCallback(IntPtr playlistPtr, bool complete, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<PlaylistUpdateEventArgs>(p => p.OnUpdateInProgress, this), new PlaylistUpdateEventArgs(complete)));
            }
        }

        private void MetadataUpdatedCallback(IntPtr playlistPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<EventArgs>(p => p.OnMetadataUpdated, this), new EventArgs()));
            }
        }

        private void TrackCreatedChangedCallback(IntPtr playlistPtr, int position, IntPtr userPtr, int when, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                ITrack track = Track.Get(session, libspotify.sp_playlist_track(playlistPtr, position));
                //IUser user = User.Get(session, userPtr);
                DateTime dtWhen = new DateTime(TimeSpan.FromSeconds(when).Ticks, DateTimeKind.Utc);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TrackCreatedChangedEventArgs>(p => p.OnTrackCreatedChanged, this), new TrackCreatedChangedEventArgs(track, dtWhen)));
            }
        }

        private void TrackSeenChangedCallback(IntPtr playlistPtr, int position, bool seen, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                ITrack track = Track.Get(session, libspotify.sp_playlist_track(playlistPtr, position));
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<TrackSeenEventArgs>(p => p.OnTrackSeenChanged, this), new TrackSeenEventArgs(track, seen)));
            }
        }

        private void DescriptionChangedCallback(IntPtr playlistPtr, IntPtr descriptionPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                string description = libspotify.GetString(descriptionPtr, String.Empty);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<DescriptionEventArgs>(p => p.OnDescriptionChanged, this), new DescriptionEventArgs(description)));
            }
        }

        private void ImageChangedCallback(IntPtr playlistPtr, IntPtr imageIdPtr, IntPtr userdataPtr)
        {
            if (playlistPtr == this.playlistPtr)
            {
                string imgId = libspotify.ImageIdToString(imageIdPtr);
                session.EnqueueEventWorkItem(new EventWorkItem(CreateDelegate<ImageEventArgs>(p => p.OnImageChanged, this), new ImageEventArgs(imgId)));
            }
        }
        #endregion

        #region Event Functions
        protected virtual void OnTracksAdded(TracksAddedEventArgs args)
        {
            PlaylistTrack.Update(this, aditions: args.TrackIndices);
            if (TracksAdded != null)
                TracksAdded(this, args);
        }
        protected virtual void OnTracksRemoved(TracksEventArgs args)
        {
            PlaylistTrack.Update(this, removals: args.TrackIndices);
            if (TracksRemoved != null)
                TracksRemoved(this, args);
        }
        protected virtual void OnTracksMoved(TracksMovedEventArgs args)
        {
            PlaylistTrack.Update(this, moves: args.TrackIndices, movedTo: args.NewPosition);
            if (TracksMoved != null)
                TracksMoved(this, args);
        }
        protected virtual void OnRenamed(EventArgs args)
        {
            if (Renamed != null)
                Renamed(this, args);
        }
        protected virtual void OnStateChanged(EventArgs args)
        {
            if (StateChanged != null)
                StateChanged(this, args);
        }
        protected virtual void OnUpdateInProgress(PlaylistUpdateEventArgs args)
        {
            if (UpdateInProgress != null)
                UpdateInProgress(this, args);
        }
        protected virtual void OnMetadataUpdated(EventArgs args)
        {
            if (MetadataUpdated != null)
                MetadataUpdated(this, args);
        }
        protected virtual void OnTrackCreatedChanged(TrackCreatedChangedEventArgs args)
        {
            if (TrackCreatedChanged != null)
                TrackCreatedChanged(this, args);
        }
        protected virtual void OnTrackSeenChanged(TrackSeenEventArgs args)
        {
            if (TrackSeenChanged != null)
                TrackSeenChanged(this, args);
        }
        protected virtual void OnDescriptionChanged(DescriptionEventArgs args)
        {
            if (DescriptionChanged != null)
                DescriptionChanged(this, args);
        }
        protected virtual void OnImageChanged(ImageEventArgs args)
        {
            if (ImageChanged != null)
                ImageChanged(this, args);
        }
        #endregion

        #region Events
        public event PlaylistEventHandler<TracksAddedEventArgs> TracksAdded;
        public event PlaylistEventHandler<TracksEventArgs> TracksRemoved;
        public event PlaylistEventHandler<TracksMovedEventArgs> TracksMoved;
        public event PlaylistEventHandler Renamed;
        public event PlaylistEventHandler StateChanged;
        public event PlaylistEventHandler<PlaylistUpdateEventArgs> UpdateInProgress;
        public event PlaylistEventHandler MetadataUpdated;
        public event PlaylistEventHandler<TrackCreatedChangedEventArgs> TrackCreatedChanged;
        public event PlaylistEventHandler<TrackSeenEventArgs> TrackSeenChanged;
        public event PlaylistEventHandler<DescriptionEventArgs> DescriptionChanged;
        public event PlaylistEventHandler<ImageEventArgs> ImageChanged;
        #endregion

        #region Declarations
        internal IntPtr playlistPtr = IntPtr.Zero;
        private Session session;
        private IntPtr callbacksPtr = IntPtr.Zero;
        private bool registerCallbacks;

        private IPlaylistTrackList tracks;
        #endregion

        #region Constructor
        protected Playlist(Session session, IntPtr playlistPtr, bool registerCallbacks = true)
        {
            if (playlistPtr == IntPtr.Zero)
                throw new ArgumentException("playlistPtr can't be zero.");

            if (session == null)
                throw new ArgumentNullException("Session can't be null.");

            this.session = session;
            this.playlistPtr = playlistPtr;
            this.registerCallbacks = registerCallbacks;

            if (registerCallbacks)
            {
                tracks_added = new tracks_added_cb(TracksAddedCallback);
                tracks_removed = new tracks_removed_cb(TracksRemovedCallback);
                tracks_moved = new tracks_moved_cd(TracksMovedCallback);
                playlist_renamed = new playlist_renamed_cb(RenamedCallback);
                playlist_state_changed = new playlist_state_changed_cb(StateChangedCallback);
                playlist_update_in_progress = new playlist_update_in_progress_cb(UpdateInProgressCallback);
                playlist_metadata_updated = new playlist_metadata_updated_cb(MetadataUpdatedCallback);
                track_created_changed = new track_created_changed_cb(TrackCreatedChangedCallback);
                track_seen_changed = new track_seen_changed_cb(TrackSeenChangedCallback);
                description_changed = new description_changed_cb(DescriptionChangedCallback);
                image_changed = new image_changed_cb(ImageChangedCallback);
                sp_playlist_callbacks callbacks = new sp_playlist_callbacks
                {
                    tracks_added = Marshal.GetFunctionPointerForDelegate(tracks_added),
                    tracks_removed = Marshal.GetFunctionPointerForDelegate(tracks_removed),
                    tracks_moved = Marshal.GetFunctionPointerForDelegate(tracks_moved),
                    playlist_renamed = Marshal.GetFunctionPointerForDelegate(playlist_renamed),
                    playlist_state_changed = Marshal.GetFunctionPointerForDelegate(playlist_state_changed),
                    playlist_update_in_progress = Marshal.GetFunctionPointerForDelegate(playlist_update_in_progress),
                    playlist_metadata_updated = Marshal.GetFunctionPointerForDelegate(playlist_metadata_updated),
                    track_created_changed = Marshal.GetFunctionPointerForDelegate(track_created_changed),
                    track_seen_changed = Marshal.GetFunctionPointerForDelegate(track_seen_changed),
                    description_changed = Marshal.GetFunctionPointerForDelegate(description_changed),
                    image_changed = Marshal.GetFunctionPointerForDelegate(image_changed)
                };

                callbacksPtr = Marshal.AllocHGlobal(Marshal.SizeOf(callbacks));
                Marshal.StructureToPtr(callbacks, callbacksPtr, true);
            }

            lock (libspotify.Mutex)
            {
                libspotify.sp_playlist_add_ref(playlistPtr);
                if (registerCallbacks)
                    libspotify.sp_playlist_add_callbacks(playlistPtr, callbacksPtr, IntPtr.Zero);
            }

            session.DisposeAll += new SessionEventHandler(session_DisposeAll);

            tracks = new PlaylistTrackList(
                () =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return libspotify.sp_playlist_num_tracks(playlistPtr);
                },
                (index) =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        return PlaylistTrack.Get(session, this, libspotify.sp_playlist_track(playlistPtr, index), index);
                },
                (track, index) =>
                {
                    IsAlive(true);
                    IntPtr trackArrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
                    IntPtr[] ptrArray = new[] { Track.GetPointer(track) };

                    try
                    {
                        Marshal.Copy(ptrArray, 0, trackArrayPtr, 1);

                        lock (libspotify.Mutex)
                            libspotify.sp_playlist_add_tracks(playlistPtr, trackArrayPtr, 1, index, session.sessionPtr);
                    }
                    finally
                    {
                        try { Marshal.FreeHGlobal(trackArrayPtr); }
                        catch
                        { }
                    }


                },
                (index) =>
                {
                    IsAlive(true);
                    lock (libspotify.Mutex)
                        libspotify.sp_playlist_remove_tracks(playlistPtr, new int[] { index }, 1);
                },
                () => false
            );
            PlaylistTrack.RegisterPlaylist(this);
        }

        void session_DisposeAll(ISession sender, SessionEventArgs e)
        {
            Dispose();
        }
        #endregion

        #region Private Methods
        private static Delegate CreateDelegate<T>(Expression<Func<Playlist, Action<T>>> expr, Playlist p) where T : EventArgs
        {
            return expr.Compile().Invoke(p);
        }
        #endregion

        #region Internal Track Methods
        internal DateTime GetTrackCreationTime(PlaylistTrack track)
        {
            lock (libspotify.Mutex)
                return new DateTime(TimeSpan.FromSeconds(libspotify.sp_playlist_track_create_time(playlistPtr, track.position)).Ticks);
        }

        internal bool GetTrackSeen(PlaylistTrack track)
        {
            lock (libspotify.Mutex)
                return libspotify.sp_playlist_track_seen(playlistPtr, track.position);
        }
        #endregion

        #region Public Functions
        public void AutoLinkTracks(bool autoLink)
        {
            IsAlive(true);
            lock (libspotify.Mutex)
                libspotify.sp_playlist_set_autolink_tracks(playlistPtr, autoLink);
        }
        #endregion

        #region Properties
        public bool IsLoaded
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_playlist_is_loaded(playlistPtr);
            }
        }

        public virtual string Name
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.GetString(libspotify.sp_playlist_name(playlistPtr), String.Empty);
            }
            set
            {
                if (value.Length > 255)
                    throw new ArgumentException("value can't be longer than 255 chars.");
                IsAlive(true);
                lock (libspotify.Mutex)
                    libspotify.sp_playlist_rename(playlistPtr, value);
            }
        }

        public IPlaylistTrackList Tracks
        {
            get
            {
                IsAlive(true);
                return tracks;
            }
        }

        public string ImageId
        {
            get
            {
                IsAlive(true);
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * 20);
                bool ans = false;
                lock (libspotify.Mutex)
                    ans = libspotify.sp_playlist_get_image(playlistPtr, ptr);
                try
                {
                    if (ans)
                    {
                        return libspotify.ImageIdToString(ptr);
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        public bool IsColaberativ
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_playlist_is_collaborative(playlistPtr);
            }
            set
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    libspotify.sp_playlist_set_collaborative(playlistPtr, value);
            }
        }

        public string Description
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.GetString(libspotify.sp_playlist_get_description(playlistPtr), String.Empty);
            }
        }

        public bool PendingChanges
        {
            get
            {
                IsAlive(true);
                lock (libspotify.Mutex)
                    return libspotify.sp_playlist_has_pending_changes(playlistPtr);
            }
        }

        public ISession Session
        {
            get { IsAlive(true); return session; }
        }
        #endregion

        #region Cleanup
        protected override void OnDispose()
        {
            session.DisposeAll -= new SessionEventHandler(session_DisposeAll);
            PlaylistTrack.UnregisterPlaylist(this);
            if (!session.ProcExit)
                lock (libspotify.Mutex)
                {
                    if (registerCallbacks)
                    {
                        try { libspotify.sp_playlist_remove_callbacks(playlistPtr, callbacksPtr, IntPtr.Zero); }
                        finally { Marshal.FreeHGlobal(callbacksPtr); }
                    }
                    libspotify.sp_playlist_release(playlistPtr);
                }

            playlistPtr = IntPtr.Zero;
            callbacksPtr = IntPtr.Zero;
        }
        #endregion

        protected override int IntPtrHashCode()
        {
            return playlistPtr.GetHashCode();
        }
    }
}
