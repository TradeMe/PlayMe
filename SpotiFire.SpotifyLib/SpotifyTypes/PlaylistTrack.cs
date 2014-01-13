using System;
using System.Collections.Generic;
using System.Linq;
namespace SpotiFire.SpotifyLib
{
    internal class PlaylistTrack : Track, IPlaylistTrack
    {
        #region KeyGen
        private class KeyGen : Tuple<Playlist, int>
        {
            public KeyGen(Playlist playlist, int position)
                : base(playlist, position)
            {

            }
        }
        #endregion
        #region Wrapper
        private class PlaylistTrackWrapper : TrackWrapper, IPlaylistTrack
        {
            public PlaylistTrackWrapper(PlaylistTrack track)
                : base(track)
            {
            }

            protected override void OnDispose() {
                PlaylistTrack.Delete(((PlaylistTrack)track).Playlist, ((PlaylistTrack)track).Position);
                track = null;
            }

            public DateTime CreateTime
            {
                get { IsAlive(true); return ((PlaylistTrack)track).CreateTime; }
            }

            public bool Seen
            {
                get { IsAlive(true); return ((PlaylistTrack)track).Seen; }
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj.GetType().IsSubclassOf(typeof(TrackWrapper)))
                    return this.track.Equals(((TrackWrapper)obj).track);
                if (obj.GetType().IsSubclassOf(typeof(Track)))
                    return this.track.Equals(obj);
                return false;
            }
        }
        #endregion
        #region Counter
        private static Dictionary<KeyGen, PlaylistTrack> tracks = new Dictionary<KeyGen, PlaylistTrack>();
        private static Dictionary<Playlist, int> playlistLengths = new Dictionary<Playlist, int>();
        private static readonly object tracksLock = new object();

        internal static IPlaylistTrack Get(Session session, Playlist playlist, IntPtr trackPtr, int position)
        {
            KeyGen key = new KeyGen(playlist, position);
            PlaylistTrack track;
            lock (tracksLock)
            {
                if (!tracks.ContainsKey(key))
                {
                    tracks.Add(key, new PlaylistTrack(session, playlist, trackPtr, position));
                }
                track = tracks[key];
                track.AddRef();
            }
            return new PlaylistTrackWrapper(track);
        }

        internal static void Delete(Playlist playlist, int position)
        {
            KeyGen key = new KeyGen(playlist, position);
            lock (tracksLock)
            {
                PlaylistTrack track = tracks[key];
                int count = track.RemRef();
                if (count == 0)
                    tracks.Remove(key);
            }
        }

        internal static void UnregisterPlaylist(Playlist playlist)
        {
            playlistLengths.Remove(playlist);
        }

        internal static void RegisterPlaylist(Playlist playlist)
        {
            playlistLengths[playlist] = playlist.Tracks.Count;
        }

        internal static void Update(Playlist playlist, IEnumerable<int> moves = null, int movedTo = -1, IEnumerable<int> aditions = null, IEnumerable<int> removals = null)
        {
            Dictionary<int, int> moved = new Dictionary<int, int>();
            List<int> newTracks = new List<int>();
            List<int> toDel = new List<int>();
            int oldCount = playlistLengths[playlist];
            int count;
            int deleted;
            if (aditions != null)
            {
                count = aditions.Count();
                for (int i = aditions.First(); i < oldCount; i++)
                    moved.Add(i, i + count);
                foreach (int index in aditions)
                    newTracks.Add(index);
            }
            else if (removals != null)
            {
                deleted = 0;
                for (int i = 0; i < oldCount; i++)
                {
                    if (removals.Contains(i))
                    {
                        deleted++;
                        continue;
                    }
                    if (deleted > 0)
                    {
                        moved.Add(i, i - deleted);
                    }
                }
            }
            else if (moves != null && movedTo != -1)
            {
                deleted = 0;
                for (int i = 0; i < oldCount; i++)
                {
                    if (moves.Contains(i))
                    {
                        deleted++;
                        continue;
                    }
                    if (deleted > 0)
                    {
                        moved.Add(i, i - deleted);
                    }
                }
                count = moves.Count();
                for (int i = movedTo; i < oldCount; i++)
                {
                    if (moved.ContainsKey(i))
                        moved[i] += count;
                    else
                        moved.Add(i, i + count);
                }
                int i1 = 0;
                foreach (int index in moves)
                    if (moved.ContainsKey(index))
                        moved[index] = movedTo + (i1++);
                    else
                        moved.Add(index, movedTo + (i1++));
            }

            foreach (var itm in moved)
            {
                KeyGen kg = new KeyGen(playlist, itm.Key);
                if (tracks.ContainsKey(kg))
                {
                    PlaylistTrack track = tracks[kg];
                    tracks.Remove(kg);
                    track.position = itm.Value;
                    kg = new KeyGen(playlist, itm.Value);
                    tracks.Add(kg, track);
                }
            }
            foreach (var itm in toDel)
            {
                KeyGen kg = new KeyGen(playlist, itm);
                if (tracks.ContainsKey(kg))
                {
                    PlaylistTrack track = tracks[kg];
                    track.Dispose();
                    tracks.Remove(kg);
                }
            }
        }
        #endregion

        #region Fields
        internal Playlist playlist;
        internal int position;
        #endregion

        #region Constructor
        public PlaylistTrack(Session session, Playlist playlist, IntPtr trackPtr, int position)
            : base(session, trackPtr)
        {
            this.playlist = playlist;
            this.position = position;
        }
        #endregion

        #region Properties
        public DateTime CreateTime
        {
            get { IsAlive(true); return playlist.GetTrackCreationTime(this); }
        }

        public bool Seen
        {
            get { IsAlive(true); return playlist.GetTrackSeen(this); }
        }

        public Playlist Playlist
        {
            get { IsAlive(true); return playlist; }
        }

        public int Position {
            get { IsAlive(true); return position; }
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() == typeof(Track))
                return base.Equals(obj);
            if (obj.GetType() != typeof(PlaylistTrack))
                return false;
            PlaylistTrack pt = (PlaylistTrack)obj;
            return pt.playlist == this.playlist && pt.position == this.position;
        }
        #endregion
    }
}
