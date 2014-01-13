using System;
using System.Collections.Generic;

namespace SpotiFire.SpotifyLib
{
    internal class ContainerPlaylist : Playlist, IContainerPlaylist
    {
        #region KeyGen
        private class KeyGen : Tuple<IntPtr, IntPtr, sp_playlist_type>
        {
            public KeyGen(IntPtr playlistPtr, IntPtr folderId, sp_playlist_type type)
                : base(playlistPtr, folderId, type)
            {
            }
        }
        #endregion
        #region Wrapper
        internal protected class ContainerPlaylistWrapper : PlaylistWrapper, IContainerPlaylist
        {
            public ContainerPlaylistWrapper(ContainerPlaylist playlist)
                : base(playlist)
            {

            }

            protected override void OnDispose()
            {
                base.OnDispose();
                ContainerPlaylist.Delete(playlist.playlistPtr, ((ContainerPlaylist)playlist).folderId, ((ContainerPlaylist)playlist).type);
            }

            public sp_playlist_type Type
            {
                get { IsAlive(true); return ((ContainerPlaylist)playlist).Type; }
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj.GetType().IsSubclassOf(typeof(Playlist)))
                    return this.playlist.Equals(obj);
                if (obj.GetType().IsSubclassOf(typeof(PlaylistWrapper)))
                    return this.playlist.Equals(((PlaylistWrapper)obj).playlist);
                return false;
            }
        }
        #endregion
        #region Counter
        private static Dictionary<KeyGen, ContainerPlaylist> playlists = new Dictionary<KeyGen, ContainerPlaylist>();
        private static readonly object playlistsLock = new object();

        internal static IContainerPlaylist Get(Session session, PlaylistContainer container, IntPtr playlistPtr, IntPtr folderId, sp_playlist_type type)
        {
            KeyGen key = new KeyGen(playlistPtr, folderId, type);
            ContainerPlaylist playlist;
            lock (playlistsLock)
            {
                if (!playlists.ContainsKey(key))
                {
                    playlists.Add(key, new ContainerPlaylist(session, container, playlistPtr, folderId, type));
                }
                playlist = playlists[key];
                playlist.AddRef();
            }
            return new ContainerPlaylistWrapper(playlist);
        }

        internal static void Delete(IntPtr playlistPtr, IntPtr folderId, sp_playlist_type type)
        {
            lock (playlistsLock)
            {
                KeyGen key = new KeyGen(playlistPtr, folderId, type);
                ContainerPlaylist playlist = playlists[key];
                int count = playlist.RemRef();
                if (count == 0)
                    playlists.Remove(key);
            }
        }
        #endregion

        #region Fields
        private IntPtr folderId = IntPtr.Zero;
        private PlaylistContainer container;
        private sp_playlist_type type;
        #endregion

        #region Constructor
        protected ContainerPlaylist(Session session, PlaylistContainer container, IntPtr playlistPtr, IntPtr folderId, sp_playlist_type type)
            : base(session, playlistPtr, true)
        {
            this.folderId = folderId;
            this.container = container;
            this.type = type;
        }
        #endregion

        #region Properties
        public sp_playlist_type Type
        {
            get
            {
                IsAlive(true);
                return type;
            }
        }

        public override string Name
        {
            get
            {
                if (Type == sp_playlist_type.SP_PLAYLIST_TYPE_PLAYLIST)
                    return base.Name;
                if (Type == sp_playlist_type.SP_PLAYLIST_TYPE_START_FOLDER)
                    return container.GetFolderName(this);
                return null;
            }
            set
            {
                if (Type == sp_playlist_type.SP_PLAYLIST_TYPE_PLAYLIST)
                    base.Name = value;
                throw new InvalidOperationException("Can't set the name of folders.");
            }
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() == typeof(Playlist))
                return base.Equals(obj);
            if (obj.GetType() != typeof(ContainerPlaylist))
                return false;
            ContainerPlaylist cp = (ContainerPlaylist)obj;
            return cp.playlistPtr == this.playlistPtr && cp.folderId == this.folderId && this.type == cp.type;
        }
        #endregion
    }
}
