using System;
using System.Runtime.InteropServices;

namespace SpotiFire.SpotifyLib
{
    internal static class libspotify
    {
        internal static readonly object Mutex = new object();

        #region Constraints
        public const int SPOTIFY_API_VERSION = 11;
        public const int STRINGBUFFER_SIZE = 256;
        #endregion

        #region Helpers
        internal static string GetString(IntPtr ptr, string defaultValue)
        {
            if (ptr == IntPtr.Zero)
                return defaultValue;

            System.Collections.Generic.List<byte> l = new System.Collections.Generic.List<byte>();
            byte read = 0;
            do
            {
                read = Marshal.ReadByte(ptr, l.Count);
                l.Add(read);
            }
            while (read != 0);

            if (l.Count > 0)
                return System.Text.Encoding.UTF8.GetString(l.ToArray(), 0, l.Count - 1);
            else
                return string.Empty;
        }

        internal static string ImageIdToString(IntPtr idPtr)
        {
            if (idPtr == IntPtr.Zero)
                return string.Empty;

            byte[] id = new byte[20];
            Marshal.Copy(idPtr, id, 0, 20);

            return ImageIdToString(id);
        }

        private static string ImageIdToString(byte[] id)
        {
            if (id == null)
                return string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in id)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        internal static byte[] StringToImageId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 40)
                return null;
            byte[] ret = new byte[20];
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    ret[i] = byte.Parse(id.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
                return ret;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Error Handeling
        /// <summary>
        /// Convert a numeric libspotify error code to a text string.
        /// </summary>
        /// <param name="error">The error code.</param>
        /// <returns>The text-representation of the error.</returns>
        [DllImport("libspotify")]
        public static extern string sp_error_message(sp_error error);
        #endregion

        #region Session Handling
        /// <summary>
        /// Initialize a session. The session returned will be initialized, but you will need to log in before you can perform any other operation.
        /// </summary>
        /// <param name="config">The configuration to use for the session.</param>
        /// <param name="sessionPtr">If successful, a new session - otherwise null.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_session_create(ref sp_session_config config, out IntPtr sessionPtr);

        /// <summary>
        /// Release the session. This will clean up all data and connections associated with the session.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_release(IntPtr sessionPtr);

        /// <summary>
        /// Logs in the specified username/password combo. This initiates the download in the background. A callback is called when login is complete.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">The username to log in.</param>
        /// <param name="password">The password for the specified username.</param>
        /// <param name="remember">If set, the username / password will be remembered by libspotify.</param>
        /// <param name="blob">If you have received a blob in the credentials_blob_updated you can pas this here instead of password</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern void sp_session_login(IntPtr sessionPtr, string username, string password, bool remember, string blob);

        /// <summary>
        /// Fetches the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>The logged in user (or null if not logged in).</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_user(IntPtr sessionPtr);

        /// <summary>
        /// Logs out the currently logged in user.
        /// </summary>
        /// <remarks>
        /// Always call this before terminating the application and libspotify is currently logged in. Otherwise, the settings and cache may be lost.
        /// </remarks>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern void sp_session_logout(IntPtr sessionPtr);

        /// <summary>
        /// Gets the connection state of the specified session.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>The connection state.</returns>
        [DllImport("libspotify")]
        internal static extern sp_connectionstate sp_session_connectionstate(IntPtr sessionPtr);

        /// <summary>
        /// The userdata associated with the session.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>The userdata that was passed in on session creation.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_user_data(IntPtr sessionPtr);

        /// <summary>
        /// Set maximum cache size.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="size">Maximum cache size in megabytes. Setting it to 0 (the default) will let libspotify automatically resize the cache (10% of disk free space).</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_set_cache_size(IntPtr sessionPtr, uint size);

        /// <summary>
        /// Make the specified session process any pending events.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="next_timeout">Stores the time (in milliseconds) until you should call this function again.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_process_events(IntPtr sessionPtr, out int next_timeout);

        /// <summary>
        /// Loads the specified track.
        /// After successfully loading the track, you have the option of running sp_session_player_play() directly,
        /// or using sp_session_player_seek() first. When this call returns, the track will have been loaded,
        /// unless an error occurred.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="track">Track object from playlist or search.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_session_player_load(IntPtr sessionPtr, IntPtr track);

        /// <summary>
        /// Seek to position in the currently loaded track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="offset">Track position, in milliseconds.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_session_player_seek(IntPtr sessionPtr, int offset);

        /// <summary>
        /// PlayerPlay or pause the currently loaded track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="play">If set to true, playback will occur. If set to false, the playback will be paused.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_session_player_play(IntPtr sessionPtr, bool play);

        /// <summary>
        /// Stops the currently playing track.
        /// This frees some resources held by libspotify to identify the currently playing track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_player_unload(IntPtr sessionPtr);

        /// <summary>
        /// Prefetch a track.
        /// Instruct libspotify to start loading of a track into its cache. This could be done by an application just before the current track ends.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="track">The track to be prefetched.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_session_player_prefetch(IntPtr sessionPtr, IntPtr track);

        /// <summary>
        /// Returns the playlist container for the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>Playlist container object, NULL if not logged in.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_playlistcontainer(IntPtr sessionPtr);

        /// <summary>
        /// Returns an inbox playlist for the currently logged in user.
        /// <seealso cref="libspotify.sp_playlist_release"/>
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>A playlist.</returns>
        /// <remarks>You need to release the playlist when you are done with it.</remarks>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_inbox_create(IntPtr sessionPtr);

        /// <summary>
        /// Returns the starred list for the current user.
        /// <seealso cref="libspotify.sp_playlist_release"/>
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>A playlist.</returns>
        /// <remarks>You need to release the playlist when you are done with it.</remarks>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_starred_create(IntPtr sessionPtr);

        /// <summary>
        /// Returns the starred list for a user.
        /// <seealso cref="libspotify.sp_playlist_release"/>
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">Canonical username.</param>
        /// <returns>A playlist.</returns>
        /// <remarks>You need to release the playlist when you are done with it.</remarks>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_starred_for_user_create(IntPtr sessionPtr, string username);

        /// <summary>
        /// Return the published container for a given canonical_username, or the currently logged in user if canonical_username is null.
        /// The container can be released when you're done with it, using <c>sp_session_publishedcontainer_fo_user_release()</c>, or it will be released when calling <c>sp_session_logout()</c>.
        /// Subsequent requests for a published container will return the same object, unless it has been released previously.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">Canonical username.</param>
        /// <returns>Playlist container object, null if not logged in or not found. </returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_publishedcontainer_for_user(IntPtr sessionPtr, string username);

        /// <summary>
        /// Releases the playlistcontainer for canonical_username.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="username">Canonical username.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_publishedcontainer_for_user_release(IntPtr sessionPtr, string username);

        /// <summary>
        /// Set preferred bitrate for music streaming.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="bitrate">Preferred bitrate.</param>
        [DllImport("libspotify")]
        internal static extern void sp_session_preferred_bitrate(IntPtr sessionPtr, sp_bitrate bitrate);

        /// <summary>
        /// Return number of friends in the currently logged in users friends list.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>Number of users in friends. Each user can be extracted using the <c>sp_session_friend()</c>
        /// method. The number of users in the list will not be updated nor change order between calls to <c>sp_session_process_events()</c></returns>
        [DllImport("libspotify")]
        internal static extern int sp_session_num_friends(IntPtr sessionPtr);

        /// <summary>
        /// Retrun the given user from the currently logged in users list of friends.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="index">Index in the list.</param>
        /// <returns>A user. The object is owned by the session so the caller should not release it.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_session_friend(IntPtr sessionPtr, int index);
        #endregion

        #region Links (Spotify URIs)
        /// <summary>
        /// Create a Spotify link given a string.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="link">A string representation of a Spotify link.</param>
        /// <returns>A link representation of the given string representation. If the link could not be parsed, this function returns NULL.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_string(string link);

        /// <summary>
        /// Creates a link object from an artist.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="artistPtr">The artist.</param>
        /// <returns>A link object representing the artist.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_artist(IntPtr artistPtr);

        /// <summary>
        /// Creates a link object from an artist.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="artistPtr">The artist.</param>
        /// <returns>A link object representing the artist.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_album(IntPtr albumPtr);


        /// <summary>
        /// Creates a link object from an playlist.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="artistPtr">The playlist.</param>
        /// <returns>A link object representing the playlist.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_playlist(IntPtr playlistPtr);

        /// <summary>
        /// Generates a link object from a track.
        /// </summary>
        /// <remarks>You need to release the link when you are done with it.</remarks>
        /// <param name="trackPtr">The track.</param>
        /// <returns>>A link object representing the track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_create_from_track(IntPtr trackPtr,int offsett);

        /// <summary>
        /// Create a string representation of the given Spotify link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose string representation you are interested in.</param>
        /// <param name="bufferPtr">The buffer to hold the string representation of link.</param>
        /// <param name="buffer_size">The max size of the buffer that will hold the string representation.
        /// The resulting string is guaranteed to always be null terminated if buffer_size &gt; 0.</param>
        /// <returns>The number of characters in the string representation of the link.
        /// If this value is greater or equal than buffer_size, output was truncated.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_link_as_string(IntPtr linkPtr, IntPtr bufferPtr, int buffer_size);

        /// <summary>
        /// Gets the link type of the specified link.
        /// </summary>
        /// <param name="linkPtr">The link.</param>
        /// <returns>The link type of the specified link - see the sp_linktype enum for possible values.</returns>
        [DllImport("libspotify")]
        internal static extern sp_linktype sp_link_type(IntPtr linkPtr);

        /// <summary>
        /// The track representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose track you are interested in.</param>
        /// <returns>The track representation of the given track link.
        /// If the link is not of track type then NULL is returned.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_track(IntPtr linkPtr);

        /// <summary>
        /// The track and offset into track representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose track you are interested in.</param>
        /// <param name="offsetPtr">The offset into track (in seconds). If the link does not contain an offset this will be set to 0.</param>
        /// <returns>The track representation of the given track link If the link is not of track type then NULL is returned.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_track_and_offset(IntPtr linkPtr, out int offsetPtr);

        /// <summary>
        /// The album representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose album you are interested in.</param>
        /// <returns>The album representation of the given album link.
        /// If the link is not of album type then NULL is returned.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_album(IntPtr linkPtr);

        /// <summary>
        /// The artist representation for the given link.
        /// </summary>
        /// <param name="linkPtr">The Spotify link whose artist you are interested in.</param>
        /// <returns>The artist representation of the given link.
        /// If the link is not of artist type then NULL is returned.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_link_as_artist(IntPtr linkPtr);

        /// <summary>
        /// Adds a reference to the specified link.
        /// </summary>
        /// <param name="linkPtr">The link.</param>
        [DllImport("libspotify")]
        internal static extern void sp_link_add_ref(IntPtr linkPtr);

        /// <summary>
        /// Releases the specified link.
        /// </summary>
        /// <param name="linkPtr">The link.</param>
        [DllImport("libspotify")]
        internal static extern void sp_link_release(IntPtr linkPtr);
        #endregion

        #region Tracks subsytem
        /// <summary>
        /// Get load status for the specified track. If the track is not loaded yet, all other functions operating on the track return default values.
        /// </summary>
        /// <param name="albumPtr">The track whose load status you are interested in.</param>
        /// <returns>True if track is loaded, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_track_is_loaded(IntPtr trackPtr);

        /// <summary>
        /// Return an error code associated with a track. For example if it could not load.
        /// </summary>
        /// <param name="albumPtr">The track.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_track_error(IntPtr trackPtr);

        /// <summary>
        /// Return true if the track is available for playback.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="albumPtr">The track.</param>
        /// <remarks>The track must be loaded or this function will always return false.
        /// <seealso cref="libspotify.sp_track_is_loaded"/>
        /// </remarks>
        /// <returns>True if track is available for playback, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern sp_track_availability sp_track_get_availability(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Return true if the track is a local file.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="albumPtr">The track.</param>
        /// <remarks>The track must be loaded or this function will always return false.
        /// <seealso cref="libspotify.sp_track_is_loaded"/>
        /// </remarks>
        /// <returns>True if track is a local file, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_track_is_local(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Return true if the track is autolinked to another track.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="albumPtr">The track.</param>
        /// <remarks>The track must be loaded or this function will always return false.
        /// <seealso cref="libspotify.sp_track_is_loaded"/>
        /// </remarks>
        /// <returns>True if track is autolinked, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_track_is_autolinked(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Return true if the track is starred by the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="albumPtr">The track.</param>
        /// <remarks>The track must be loaded or this function will always return false.
        /// <seealso cref="libspotify.sp_track_is_loaded"/>
        /// </remarks>
        /// <returns>True if track is a starred file, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_track_is_starred(IntPtr sessionPtr, IntPtr trackPtr);

        /// <summary>
        /// Star/Unstar the specified tracks.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="trackArrayPtr">An array of pointer to tracks.</param>
        /// <param name="num_tracks">Count of <c>trackArray</c>.</param>
        /// <param name="star">Starred status of the track.</param>
        [DllImport("libspotify")]
        internal static extern void sp_track_set_starred(IntPtr sessionPtr, IntPtr trackArrayPtr, int num_tracks, bool star);

        /// <summary>
        /// The number of artists performing on the specified track.
        /// </summary>
        /// <param name="albumPtr">The track whose number of participating artists you are interested in.</param>
        /// <returns>The number of artists performing on the specified track. If no metadata is available for the track yet, this function returns 0.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_num_artists(IntPtr trackPtr);

        /// <summary>
        /// The artist matching the specified index performing on the current track.
        /// </summary>
        /// <param name="albumPtr">The track whose participating artist you are interested in.</param>
        /// <param name="index">The index for the participating artist. Should be in the interval [0, <c>sp_track_num_artists()</c> - 1]</param>
        /// <returns>The participating artist, or NULL if invalid index.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_track_artist(IntPtr trackPtr, int index);

        /// <summary>
        /// The album of the specified track.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>The album of the given track. You need to increase the refcount if you want to keep the pointer around. If no metadata is available for the track yet, this function returns 0.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_track_album(IntPtr trackPtr);

        /// <summary>
        /// The string representation of the specified track's name.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>The string representation of the specified track's name.
        /// Returned string is valid as long as the album object stays allocated and
        /// no longer than the next call to <c>sp_session_process_events()</c>.
        /// If no metadata is available for the track yet, this function returns empty string. </returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_track_name(IntPtr trackPtr);

        /// <summary>
        /// The duration, in milliseconds, of the specified track.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>The duration of the specified track, in milliseconds If no metadata is available for the track yet, this function returns 0.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_duration(IntPtr trackPtr);

        /// <summary>
        /// Returns popularity for track.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>Popularity in range 0 to 100, 0 if undefined.
        /// If no metadata is available for the track yet, this function returns 0.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_popularity(IntPtr trackPtr);

        /// <summary>
        /// Returns the disc number for a track.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>Disc index. Possible values are [1, total number of discs on album].
        /// This function returns valid data only for tracks appearing in a browse artist or browse album result (otherwise returns 0).</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_disc(IntPtr trackPtr);

        /// <summary>
        /// Returns the position of a track on its disc.
        /// </summary>
        /// <param name="albumPtr">A track object.</param>
        /// <returns>Track position, starts at 1 (relative the corresponding disc).
        /// This function returns valid data only for tracks appearing in a browse artist or browse album result (otherwise returns 0).</returns>
        [DllImport("libspotify")]
        internal static extern int sp_track_index(IntPtr trackPtr);

        /// <summary>
        /// Returns the newly created local track.
        /// </summary>
        /// <param name="artist">Name of the artist.</param>
        /// <param name="title">Song title.</param>
        /// <param name="album">Name of the album, or an empty string if not available.</param>
        /// <param name="length">Count in MS, or -1 if not available.</param>
        /// <returns>A track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_localtrack_create(string artist, string title, string album, int length);

        /// <summary>
        /// Increase the reference count of a track.
        /// </summary>
        /// <param name="albumPtr">The track object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_track_add_ref(IntPtr trackPtr);

        /// <summary>
        /// Decrease the reference count of a track.
        /// </summary>
        /// <param name="albumPtr">The track object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_track_release(IntPtr trackPtr);
        #endregion

        #region Album subsystem
        /// <summary>
        /// Check if the album object is populated with data.
        /// </summary>
        /// <param name="albumPtr">The album object.</param>
        /// <returns>True if metadata is present, false if not.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_album_is_loaded(IntPtr albumPtr);

        /// <summary>
        /// Return true if the album is available in the current region.
        /// </summary>
        /// <param name="albumPtr">The album.</param>
        /// <returns>True if album is available for playback, otherwise false.</returns>
        /// <remarks>The album must be loaded or this function will always return false.
        /// <seealso cref="libspotify.sp_album_is_loaded"/>
        /// </remarks>
        [DllImport("libspotify")]
        internal static extern bool sp_album_is_available(IntPtr albumPtr);

        /// <summary>
        /// Get the artist associated with the given album.
        /// </summary>
        /// <param name="albumPtr">Album object.</param>
        /// <returns>A reference to the artist. null if the metadata has not been loaded yet.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_album_artist(IntPtr albumPtr);

        /// <summary>
        /// Return image ID representing the album's coverart.
        /// </summary>
        /// <param name="albumPtr">Album object.</param>
        /// <returns>ID byte sequence that can be passed to <c>sp_image_create()</c>.
        /// If the album has no image or the metadata for the album is not loaded yet, this function returns null.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_album_cover(IntPtr albumPtr);

        /// <summary>
        /// Return name of album.
        /// </summary>
        /// <param name="albumPtr">Album object.</param>
        /// <returns>Name of album. Returned string is valid as long as the album object stays allocated
        /// and no longer than the next call to <c>sp_session_process_events()</c>.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_album_name(IntPtr albumPtr);

        /// <summary>
        /// Return release year of specified album.
        /// </summary>
        /// <param name="albumPtr">The album.</param>
        /// <returns>Release year.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_album_year(IntPtr albumPtr);

        /// <summary>
        /// Return type of specified album.
        /// </summary>
        /// <param name="albumPtr">Album object.</param>
        /// <returns>The album type.</returns>
        [DllImport("libspotify")]
        internal static extern sp_albumtype sp_album_type(IntPtr albumPtr);

        /// <summary>
        /// Increase the reference count of an album.
        /// </summary>
        /// <param name="albumPtr">The album.</param>
        [DllImport("libspotify")]
        internal static extern void sp_album_add_ref(IntPtr albumPtr);

        /// <summary>
        /// Decrease the reference count of an album.
        /// </summary>
        /// <param name="albumPtr">The album.</param>
        [DllImport("libspotify")]
        internal static extern void sp_album_release(IntPtr albumPtr);
        #endregion

        #region Artist subsystem
        /// <summary>
        /// Check if the artist object is populated with data.
        /// </summary>
        /// <param name="artistPtr">The artist object.</param>
        /// <returns>True if metadata is present, false if not.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_artist_is_loaded(IntPtr artistPtr);

        /// <summary>
        /// Return name of artist.
        /// </summary>
        /// <param name="artistPtr">Artist object.</param>
        /// <returns>Name of artist. Returned string is valid as long as the artist object stays allocated
        /// and no longer than the next call to <c>sp_session_process_events()</c>.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artist_name(IntPtr artistPtr);

        /// <summary>
        /// Return portrait id for artist.
        /// </summary>
        /// <param name="artistPtr">Artist object.</param>
        /// <returns>Name of artist. Returned string is valid as long as the artist object stays allocated
        /// and no longer than the next call to <c>sp_session_process_events()</c>.</returns>        
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artist_portrait(IntPtr artistPtr);	

        /// <summary>
        /// Increase the reference count of an artist.
        /// </summary>
        /// <param name="artistPtr">The artist.</param>
        [DllImport("libspotify")]
        internal static extern void sp_artist_add_ref(IntPtr artistPtr);

        /// <summary>
        /// Decrease the reference count of an artist.
        /// </summary>
        /// <param name="artistPtr">The artist.</param>
        [DllImport("libspotify")]
        internal static extern void sp_artist_release(IntPtr artistPtr);
        #endregion

        #region Image subsystem
        /// <summary>
        /// Create an image object.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="idPtr">Spotify image ID.</param>
        /// <returns>Pointer to an image object. To free the object, use <c>sp_image_release()</c>.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_image_create(IntPtr sessionPtr, IntPtr idPtr);

        /// <summary>
        /// Add a callback that will be invoked when the image is loaded.
        /// If an image is loaded, and loading fails, the image will behave like an empty image.
        /// </summary>
        /// <param name="imagePtr">The image.</param>
        /// <param name="callbackPtr">Callback that will be called when image has been fetched.</param>
        /// <param name="userdataPtr">Opaque pointer passed to <c>callback</c>.</param>
        [DllImport("libspotify")]
        internal static extern void sp_image_add_load_callback(IntPtr imagePtr, IntPtr callbackPtr, IntPtr userdataPtr);

        /// <summary>
        /// Remove an image load callback previously added with <see cref="libspotify.sp_image_add_load_callback"/>.
        /// </summary>
        /// <param name="imagePtr">The image.</param>
        /// <param name="callbackPtr">Callback that will not be called when image has been fetched.</param>
        /// <param name="userdataPtr">Opaque pointer passed to <c>callback</c></param>
        [DllImport("libspotify")]
        internal static extern void sp_image_remove_load_callback(IntPtr imagePtr, IntPtr callbackPtr, IntPtr userdataPtr);

        /// <summary>
        /// Check if an image is loaded. Before the image is loaded,
        /// the rest of the methods will behave as if the image is empty.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>True if image is loaded, false otherwise.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_image_is_loaded(IntPtr imagePtr);

        /// <summary>
        /// Check if image retrieval returned an error code.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_image_error(IntPtr imagePtr);

        /// <summary>
        /// Get image format.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>Image format as described by <see cref="SpotiFire.SpotiFireLib.sp_imageformat"/>.</returns>
        [DllImport("libspotify")]
        internal static extern sp_imageformat sp_image_format(IntPtr imagePtr);

        /// <summary>
        /// Get image data.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <param name="sizePtr">Size of raw image data.</param>
        /// <returns>Pointer to raw image data.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_image_data(IntPtr imagePtr, out IntPtr sizePtr);

        /// <summary>
        /// Get image ID.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>Image ID.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_image_image_id(IntPtr imagePtr);

        /// <summary>
        /// Increase the reference count of an image.
        /// </summary>
        /// <param name="imagePtr">The image.</param>
        [DllImport("libspotify")]
        internal static extern void sp_image_add_ref(IntPtr imagePtr);

        /// <summary>
        /// Decrease the reference count of an image.
        /// </summary>
        /// <param name="imagePtr">The image.</param>
        [DllImport("libspotify")]
        internal static extern void sp_image_release(IntPtr imagePtr);
        #endregion

        #region Search subsystem
        /// <summary>
        /// Create a search object from the given query.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="query">Query search string, e.g. 'The Rolling Stones' or 'album:"The Black Album"'</param>
        /// <param name="track_offset">The offset among the tracks of the result.</param>
        /// <param name="track_count">The number of tracks to ask for.</param>
        /// <param name="album_offset">The offset among the albums of the result.</param>
        /// <param name="album_count">The number of albums to ask for.</param>
        /// <param name="artist_offset">The offset among the artists of the result.</param>
        /// <param name="artist_count">The number of artists to ask for.</param>
        /// <param name="playlist_offset">The offset among the playlists of the result.</param>
        /// <param name="playlist_count">The number of playlists to ask for.</param>
        /// <param name="search_type">	Type of search, can be used for suggest searches.</param>
        /// <param name="callbackPtr">Callback that will be called once the search operation is complete.
        /// Pass null if you are not interested in this event.</param>
        /// <param name="userdataPtr">Opaque pointer passed to callback.</param>
        /// <returns>Pointer to a search object. To free the object, use <c>sp_search_release()</c></returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_create(IntPtr sessionPtr, string query, int track_offset,
            int track_count, int album_offset, int album_count, int artist_offset, int artist_count,
            int playlist_offset, int playlist_count, sp_search_type search_type, IntPtr callbackPtr, IntPtr userdataPtr);

        /// <summary>
        /// Create a search object from the radio channel.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="from_year">Include tracks starting from this year.</param>
        /// <param name="to_year">Include tracks up to this year.</param>
        /// <param name="genres">Bitmask of genres to include.</param>
        /// <param name="callbackPtr">Callback that will be called once the search operation is complete.
        /// Pass null if you are not interested in this event.</param>
        /// <param name="userdataPtr">Opaque pointer passed to callback.</param>
        /// <returns>Pointer to a search object. To free the object, use <c>sp_search_release()</c></returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_radio_search_create(IntPtr sessionPtr, uint from_year, uint to_year,
            sp_radio_genre genres, IntPtr callbackPtr, IntPtr userdataPtr);

        /// <summary>
        /// Get load status for the specified search. Before it is loaded, it will behave as an empty search result.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>True if search is loaded, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_search_is_loaded(IntPtr searchPtr);

        /// <summary>
        /// Check if search returned an error code.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_search_error(IntPtr searchPtr);

        /// <summary>
        /// Get the number of tracks for the specified search.
        /// </summary>
        /// <param name="searchPtr">A serach object.</param>
        /// <returns>The number of tracks for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_num_tracks(IntPtr searchPtr);

        /// <summary>
        /// Return the track at the given index in the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <param name="index">Index of the wanted track. Should be in the interval [0, <c>sp_search_num_tracks()</c> - 1]</param>
        /// <returns>The track at the given index in the given search object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_track(IntPtr searchPtr, int index);

        /// <summary>
        /// Get the number of albums for the specified search.
        /// </summary>
        /// <param name="searchPtr">A serach object.</param>
        /// <returns>The number of albums for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_num_albums(IntPtr searchPtr);

        /// <summary>
        /// Return the album at the given index in the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <param name="index">Index of the wanted album. Should be in the interval [0, <c>sp_search_num_albums()</c> - 1]</param>
        /// <returns>The album at the given index in the given search object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_album(IntPtr searchPtr, int index);

        /// <summary>
        /// Get the number of artists for the specified search.
        /// </summary>
        /// <param name="searchPtr">A serach object.</param>
        /// <returns>The number of artists for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_num_artists(IntPtr searchPtr);

        /// <summary>
        /// Get the number of playlists for the specified search.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The number of playlists for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_num_playlists(IntPtr searchPtr);

        /// <summary>
        /// Get the number of playlists for the specified search.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The number of playlists for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern string sp_search_playlist_name(IntPtr searchPtr,int index);
        
        /// <summary>
        /// Get the number of playlists for the specified search.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The number of playlists for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern string sp_search_playlist_uri(IntPtr searchPtr, int index);

        /// <summary>
        /// Return the artist at the given index in the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <param name="index">Index of the wanted artist. Should be in the interval [0, <c>sp_search_num_artists()</c> - 1]</param>
        /// <returns>The artist at the given index in the given search object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_artist(IntPtr searchPtr, int index);

        /// <summary>
        /// Return the search query for the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The search query for the given search object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_query(IntPtr searchPtr);

        /// <summary>
        /// Return the "Did you mean" query for the given search object.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The "Did you mean" query for the given search object, or the empty string if no such info is available.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_search_did_you_mean(IntPtr searchPtr);

        /// <summary>
        /// Return the total number of tracks for the search query - regardless of the interval requested at creation.
        /// If this value is larger than the interval specified at creation of the search object, more search results
        /// are available. To fetch these, create a new search object with a new interval.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The total number of tracks matching the original query.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_total_tracks(IntPtr searchPtr);

        /// <summary>
        /// Return the total number of albums for the search query - regardless of the interval requested at creation.
        /// If this value is larger than the interval specified at creation of the search object, more search results
        /// are available. To fetch these, create a new search object with a new interval.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The total number of albums matching the original query.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_total_albums(IntPtr searchPtr);

        /// <summary>
        /// Return the total number of artists for the search query - regardless of the interval requested at creation.
        /// If this value is larger than the interval specified at creation of the search object, more search results
        /// are available. To fetch these, create a new search object with a new interval.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The total number of artists matching the original query.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_total_artists(IntPtr searchPtr);

        /// <summary>
        /// Return the total number of playlists for the search query - regardless of the interval requested at creation.
        /// If this value is larger than the interval specified at creation of the search object, more search results
        /// are available. To fetch these, create a new search object with a new interval.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        /// <returns>The number of playlists for the specified search.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_search_total_playlists(IntPtr searchPtr);

        /// <summary>
        /// Increase the reference count of a search result.
        /// </summary>
        /// <param name="searchPtr">A serach object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_search_add_ref(IntPtr searchPtr);

        /// <summary>
        /// Decrease the reference count of a search result.
        /// </summary>
        /// <param name="searchPtr">A search object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_search_release(IntPtr searchPtr);
        #endregion

        #region Playlist subsystem
        #region Playlist
        /// <summary>
        /// Get load status for the specified playlist. If it's false, you have to wait until playlist_state_changed happens,
        /// and check again if is_loaded has changed.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>True if playlist is loaded, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_playlist_is_loaded(IntPtr playlistPtr);

        /// <summary>
        /// Register interest in the given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlist_callbacks.</param>
        /// <param name="userdataPtr">Userdata to be passed to callbacks.</param>
        [DllImport("libspotify")]
        internal static extern void sp_playlist_add_callbacks(IntPtr playlistPtr, IntPtr callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Unregister interest in the given playlist.
        /// The combination of (callbacks, userdata) is used to find the entry to be removed.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlist_callbacks.</param>
        /// <param name="userdataPtr">Userdata to be passed to callbacks.</param>
        [DllImport("libspotify")]
        internal static extern void sp_playlist_remove_callbacks(IntPtr playlistPtr, IntPtr callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Return number of tracks in the given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>The number of tracks in the playlist.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlist_num_tracks(IntPtr playlistPtr);

        /// <summary>
        /// Return the track at the given index in a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="index">Index into playlist container. Should be in the interval [0, sp_playlist_num_tracks() - 1].</param>
        /// <returns>The track at the given index.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_track(IntPtr playlistPtr, int index);

        /// <summary>
        /// Return when the given index was added to the playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="index">Index into playlist container. Should be in the interval [0, sp_playlist_num_tracks() - 1].</param>
        /// <returns>Time, Seconds since unix epoch.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlist_track_create_time(IntPtr playlistPtr, int index);

        /// <summary>
        /// Return user that added the given index in the playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="index">Index into playlist container. Should be in the interval [0, sp_playlist_num_tracks() - 1].</param>
        /// <returns>User object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_track_creator(IntPtr playlistPtr, int index);

        /// <summary>
        /// Return if a playlist entry is marked as seen or not.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="index">Index into playlist container. Should be in the interval [0, sp_playlist_num_tracks() - 1].</param>
        /// <returns>Seen state.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_playlist_track_seen(IntPtr playlistPtr, int index);

        /// <summary>
        /// Return name of given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>The name of the given playlist.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_name(IntPtr playlistPtr);

        /// <summary>
        /// Rename the given playlist The name must not consist of only spaces and it must be shorter than 256 characters.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="newName">New name for playlist.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_playlist_rename(IntPtr playlistPtr, string newName);

        /// <summary>
        /// Return a pointer to the user for the given playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>User object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_owner(IntPtr playlistPtr);

        /// <summary>
        /// Return collaborative status for a playlist.
        /// A playlist in collaborative state can be modifed by all users, not only the user owning the list.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>true if playlist is collaborative, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_playlist_is_collaborative(IntPtr playlistPtr);

        /// <summary>
        /// Set collaborative status for a playlist.
        /// A playlist in collaborative state can be modifed by all users, not only the user owning the list.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="collaborative">Wheater or not the playlist should be collaborative.</param>
        [DllImport("libspotify")]
        internal static extern void sp_playlist_set_collaborative(IntPtr playlistPtr, bool collaborative);

        /// <summary>
        /// Set autolinking state for a playlist.
        /// If a playlist is autolinked, unplayable tracks will be made playable by linking them to other Spotify tracks, where possible.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="link">The new value.</param>
        [DllImport("libspotify")]
        internal static extern void sp_playlist_set_autolink_tracks(IntPtr playlistPtr, bool link);

        /// <summary>
        /// Get description for a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>Description</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_get_description(IntPtr playlistPtr);

        /// <summary>
        /// Get image for a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="imageId">[out] 20 byte image id.</param>
        /// <returns>True if playlist has an image, otherwise false.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_playlist_get_image(IntPtr playlistPtr, IntPtr imageId);

        /// <summary>
        /// Check if a playlist has pending changes.
        /// Pending changes are local changes that have not yet been acknowledged by the server.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <returns>A flag representing if there are pending changes or not.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_playlist_has_pending_changes(IntPtr playlistPtr);

        /// <summary>
        /// Add tracks to a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="trackArrayPtr">Array of pointer to tracks.</param>
        /// <param name="numTracks">Count of <c>tracks</c> array.</param>
        /// <param name="position">Start position in playlist where to insert the tracks.</param>
        /// <param name="sessionPtr">Your session object.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_playlist_add_tracks(IntPtr playlistPtr, IntPtr trackArrayPtr, int numTracks, int position, IntPtr sessionPtr);

        /// <summary>
        /// Remove tracks from a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="trackIndices">Array of pointer to track indices.
        /// A certain track index should be present at most once, e.g. [0, 1, 2] is valid indata, whereas [0, 1, 1] is invalid.</param>
        /// <param name="numTracks">Count of <c>trackIndices</c> array.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_playlist_remove_tracks(IntPtr playlistPtr, int[] trackIndices, int numTracks);

        /// <summary>
        /// Move tracks in playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        /// <param name="trackIndices">Array of pointer to track indices to be moved.
        /// A certain track index should be present at most once, e.g. [0, 1, 2] is valid indata, whereas [0, 1, 1] is invalid.</param>
        /// <param name="numTracks">Count of <c>trackIndices</c> array.</param>
        /// <param name="newPosition">New position for tracks.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_playlist_reorder_tracks(IntPtr playlistPtr, int[] trackIndices, int numTracks, int newPosition);

        /// <summary>
        /// Load an already existing playlist without adding it to a playlistcontainer.
        /// </summary>
        /// <param name="playlistPtr">Session object.</param>
        /// <param name="linkPtr">Link object referring to a playlist.</param>
        /// <returns>A playlist. The reference is owned by the caller and should be released with sp_playlist_release().</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlist_create(IntPtr sessionPtr, IntPtr linkPtr);

        /// <summary>
        /// Increase the reference count of a playlist.
        /// </summary>
        /// <param name="playlistPtr">Playlist object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_playlist_add_ref(IntPtr playlistPtr);

        /// <summary>
        /// Decrease the reference count of a playlist.
        /// </summary>
        /// <param name="playlistPtr">The playlist object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_playlist_release(IntPtr playlistPtr);
        #endregion

        #region Playlist container
        /// <summary>
        /// Register interest in changes to a playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlistcontainer_callbacks.</param>
        /// <param name="userdataPtr">Opaque value passed to callbacks.</param>
        [DllImport("libspotify")]
        internal static extern void sp_playlistcontainer_add_callbacks(IntPtr pcPtr, IntPtr callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Unregister interest in changes to a playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="callbacksPtr">Callbacks, see sp_playlistcontainer_callbacks</param>
        /// <param name="userdataPtr">Opaque value passed to callbacks.</param>
        [DllImport("libspotify")]
        internal static extern void sp_playlistcontainer_remove_callbacks(IntPtr pcPtr, IntPtr callbacksPtr, IntPtr userdataPtr);

        /// <summary>
        /// Return the number of playlists in the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <returns>Number of playlists, -1 if undefined.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_playlistcontainer_num_playlists(IntPtr pcPtr);

        /// <summary>
        /// Return a pointer to the playlist at a specific index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>Number of playlists.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_playlist(IntPtr pcPtr, int index);

        /// <summary>
        /// Return the type of the playlist at a index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>Type of the playlist.</returns>
        [DllImport("libspotify")]
        internal static extern sp_playlist_type sp_playlistcontainer_playlist_type(IntPtr pcPtr, int index);

        /// <summary>
        /// Gets the name of the playlist folder.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">The playlist index.</param>
        /// <param name="buffer">Pointer to name-buffer.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_playlistcontainer_playlist_folder_name(IntPtr pcPtr, int index, IntPtr buffer, int bufferSize);

        /// <summary>
        /// Return the folder id at index.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index in playlist container. Should be in the interval [0, sp_playlistcontainer_num_playlists() - 1].</param>
        /// <returns>The group ID.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_playlist_folder_id(IntPtr pcPtr, int index);

        /// <summary>
        /// Add an empty playlist at the end of the playlist container. The name must not consist of only spaces and it must be shorter than 256 characters.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="name">Name of new playlist.</param>
        /// <returns>Pointer to the new playlist. Can be null if the operation fails.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_add_new_playlist(IntPtr pcPtr, string name);

        /// <summary>
        /// Add an existing playlist at the end of the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="linkPtr">Link object pointing to a playlist.</param>
        /// <returns>Pointer to the new playlist. Will be null if the playlist already exists.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_add_playlist(IntPtr pcPtr, IntPtr linkPtr);

        /// <summary>
        /// Remove playlist at index from the given playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index of playlist to be removed.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_playlistcontainer_remove_playlist(IntPtr pcPtr, int index);

        /// <summary>
        /// Move a playlist in the playlist container.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <param name="index">Index of playlist to be moved.</param>
        /// <param name="newPosition">New position for the playlist.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_playlistcontainer_move_playlist(IntPtr pcPtr, int index, int newPosition);

        /// <summary>
        /// Returns a pointer to the user object of the owner.
        /// </summary>
        /// <param name="pcPtr">Playlist container.</param>
        /// <returns>The user object or null if unknown or none.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_playlistcontainer_owner(IntPtr pcPtr);
        #endregion
        #endregion

        #region Artist browsing
        /// <summary>
        /// Initiate a request for browsing an artist.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="artistPtr">Artist to be browsed. The artist metadata does not have to be loaded.</param>
        /// <param name="callbackPtr">Callback to be invoked when browsing has been completed. Pass NULL if you are not interested in this event.</param>
        /// <param name="type">Type of data requested, see the sp_artistbrowse_type enum for details.</param>
        /// <param name="userDataPtr">Userdata passed to callback.</param>
        /// <returns>Artist browse object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_create(IntPtr sessionPtr, IntPtr artistPtr, sp_artistbrowse_type type, IntPtr callbackPtr, IntPtr userDataPtr);

        /// <summary>
        /// Check if an artist browse request is completed.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns>True if browsing is completed, false if not.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_artistbrowse_is_loaded(IntPtr artistBrowsePtr);

        /// <summary>
        /// Check if browsing returned an error code.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns>One of the following errors, from sp_error SP_ERROR_OK SP_ERROR_IS_LOADING SP_ERROR_OTHER_PERMANENT SP_ERROR_OTHER_TRANSIENT.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_artistbrowse_error(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return a pointer to its artist object.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns>Artist object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_artist(IntPtr artistBrowsePtr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns></returns>
        [DllImport("libspotify")]
        internal static extern int sp_artistbrowse_num_portraits(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return number of portraits available.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns>Number of portraits for given artist.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_portrait(IntPtr artistBrowsePtr, int index);

        /// <summary>
        /// Given an artist browse object, return number of tracks.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns>Number of tracks for given artist.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_artistbrowse_num_tracks(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return one of its tracks.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <param name="index">The index for the track. Should be in the interval [0, sp_artistbrowse_num_tracks() - 1].</param>
        /// <returns>A track object, or NULL if the index is out of range.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_track(IntPtr artistBrowsePtr, int index);

        /// <summary>
        /// Given an artist browse object, return number of albums.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns>Number of albums for given artist.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_artistbrowse_num_albums(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return one of its albums.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <param name="index">The index for the album. Should be in the interval [0, sp_artistbrowse_num_albums() - 1].</param>
        /// <returns>A album object, or NULL if the index is out of range.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_album(IntPtr artistBrowsePtr, int index);

        /// <summary>
        /// Given an artist browse object, return number of similar artists
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns>Number of similar artists for given artist.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_artistbrowse_num_similar_artists(IntPtr artistBrowsePtr);

        /// <summary>
        /// Given an artist browse object, return a similar artist by index.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <param name="index">The index for the artist. Should be in the interval [0, sp_artistbrowse_num_similar_artists() - 1].</param>
        /// <returns>A pointer to an artist object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_similar_artist(IntPtr artistBrowsePtr, int index);

        /// <summary>
        /// Given an artist browse object, return the artists biography. This function must be called from the same thread that did sp_session_create().
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        /// <returns>Biography string in UTF-8 format. Returned string is valid as long as the album object stays allocated and no longer than the next call to sp_session_process_events().</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_artistbrowse_biography(IntPtr artistBrowsePtr);

        /// <summary>
        /// Increase the reference count of an artist browse result.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_artistbrowse_add_ref(IntPtr artistBrowsePtr);

        /// <summary>
        /// Decrease the reference count of an artist browse result.
        /// </summary>
        /// <param name="artistBrowsePtr">Artist browse object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_artistbrowse_release(IntPtr artistBrowsePtr);
        #endregion

        #region Album browsing
        /// <summary>
        /// Initiate a request for browsing an album.
        /// </summary>
        /// <param name="sessionPtr">Session object.</param>
        /// <param name="albumPtr">Album to be browsed. The album metadata does not have to be loaded.</param>
        /// <param name="callbackPtr">Callback to be invoked when browsing has been completed. Pass NULL if you are not interested in this event.</param>
        /// <param name="userDataPtr">Userdata passed to callback.</param>
        /// <returns>Album browse object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_create(IntPtr sessionPtr, IntPtr albumPtr, IntPtr callbackPtr, IntPtr userDataPtr);

        /// <summary>
        /// Check if an album browse request is completed.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <returns>True if browsing is completed, false if not.</returns>
        [DllImport("libspotify")]
        internal static extern bool sp_albumbrowse_is_loaded(IntPtr albumBrowsePtr);

        /// <summary>
        /// Check if browsing returned an error code.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <returns>One of the following errors, from sp_error SP_ERROR_OK SP_ERROR_IS_LOADING SP_ERROR_OTHER_PERMANENT SP_ERROR_OTHER_TRANSIENT.</returns>
        [DllImport("libspotify")]
        internal static extern sp_error sp_albumbrowse_error(IntPtr albumBrowsePtr);

        /// <summary>
        /// Given an album browse object, return a pointer to its album object.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <returns>Album object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_album(IntPtr albumBrowsePtr);

        /// <summary>
        /// Given an album browse object, return a pointer to its artist object.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <returns>Artist object.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_artist(IntPtr albumBrowsePtr);

        /// <summary>
        /// Given an album browse object, return number of copyright strings.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <returns>Number of copyright strings available, 0 if unknown.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_albumbrowse_num_copyrights(IntPtr albumBrowsePtr);

        /// <summary>
        /// Given an album browse object, return one of its copyright strings.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <param name="index">The index for the copyright string. Should be in the interval [0, sp_albumbrowse_num_copyrights() - 1].</param>
        /// <returns>Copyright string in UTF-8 format, or NULL if the index is invalid. Returned string is valid as long as the album object stays allocated and no longer than the next call to sp_session_process_events().</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_copyright(IntPtr albumBrowsePtr, int index);

        /// <summary>
        /// Given an album browse object, return number of tracks.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <returns>Number of tracks on album.</returns>
        [DllImport("libspotify")]
        internal static extern int sp_albumbrowse_num_tracks(IntPtr albumBrowsePtr);

        /// <summary>
        /// Given an album browse object, return a pointer to one of its tracks.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <param name="index">The index for the track. Should be in the interval [0, sp_albumbrowse_num_tracks() - 1].</param>
        /// <returns>A track.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_track(IntPtr albumBrowsePtr, int index);

        /// <summary>
        /// Given an album browse object, return its review.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        /// <returns>Review string in UTF-8 format. Returned string is valid as long as the album object stays allocated and no longer than the next call to sp_session_process_events().</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_albumbrowse_review(IntPtr albumBrowsePtr);

        /// <summary>
        /// Increase the reference count of an album browse result.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_albumbrowse_add_ref(IntPtr albumBrowsePtr);

        /// <summary>
        /// Decrease the reference count of an album browse result.
        /// </summary>
        /// <param name="albumBrowsePtr">Album browse object.</param>
        [DllImport("libspotify")]
        internal static extern void sp_albumbrowse_release(IntPtr albumBrowsePtr);
        #endregion

        #region Structs
        internal struct sp_session_config
        {
            internal int api_version;
            internal string cache_location;
            internal string settings_location;
            internal IntPtr application_key;
            internal int application_key_size;
            internal string user_agent;
            internal IntPtr callbacks;
            internal IntPtr userdata;
            internal bool compress_playlists;
            internal bool dont_save_metadata_for_playlists;
            internal bool initially_unload_playlists;
            internal string device_id;
            internal string tracefile;
        }

        internal struct sp_session_callbacks
        {
            internal IntPtr logged_in;
            internal IntPtr logged_out;
            internal IntPtr metadata_updated;
            internal IntPtr connection_error;
            internal IntPtr message_to_user;
            internal IntPtr notify_main_thread;
            internal IntPtr music_delivery;
            internal IntPtr play_token_lost;
            internal IntPtr log_message;
            internal IntPtr end_of_track;
            internal IntPtr streaming_error;
            internal IntPtr userinfo_updated;
            internal IntPtr start_playback;
            internal IntPtr stop_playback;
            internal IntPtr get_audio_buffer_stats;
            internal IntPtr offline_status_updated;
            internal IntPtr offline_error;
            internal IntPtr credentials_blob_updated;
        }

        internal struct sp_audioformat
        {
            internal int sample_type;
            internal int sample_rate;
            internal int channels;
        }

        internal struct sp_audio_buffer_stats
        {
            int samples;
            int stutter;
        }
        #endregion
    }

    #region Enums
    public enum sp_error
    {
        OK = 0,
        BAD_API_VERSION = 1,
        API_INITIALIZATION_FAILED = 2,
        TRACK_NOT_PLAYABLE = 3,
        RESOURCE_NOT_LOADED = 4,
        APPLICATION_KEY = 5,
        BAD_USERNAME_OR_PASSWORD = 6,
        USER_BANNED = 7,
        UNABLE_TO_CONTACT_SERVER = 8,
        CLIENT_TOO_OLD = 9,
        OTHER_PERMANENT = 10,
        BAD_USER_AGENT = 11,
        MISSING_CALLBACK = 12,
        INVALID_INDATA = 13,
        INDEX_OUT_OF_RANGE = 14,
        USER_NEEDS_PREMIUM = 15,
        OTHER_TRANSIENT = 16,
        IS_LOADING = 17,
        NO_STREAM_AVAILABLE = 18,
        PERMISSION_DENIED = 19,
        INBOX_IS_FULL = 20,
        NO_CACHE = 21,
        NO_SUCH_USER = 22,
        NO_CREDENTIALS = 23,
        NETWORK_DISABLED = 24,
        INVALID_DEVICE_ID = 25,
        CANT_OPEN_TRACE_FILE = 26,
        APPLICATION_BANNED = 27,
        OFFLINE_TOO_MANY_TRACKS = 28,
        OFFLINE_DISK_CACHE = 29,
        OFFLINE_EXPIRED = 30,
        OFFLINE_NOT_ALLOWED = 31,
        OFFLINE_LICENSE_LOST = 32,
        OFFLINE_LICENSE_ERROR = 33
    }

    public enum sp_connectionstate
    {
        LOGGED_OUT = 0,
        LOGGED_IN = 1,
        DISCONNECTED = 2,
        UNDEFINED = 3,
        OFFLINE = 4
    }

    public enum sp_bitrate
    {
        BITRATE_160k = 0,
        BITRATE_320k = 1,
        BITRATE_96k = 2
    }

    public enum sp_radio_genre
    {
        ALT_POP_ROCK = 0x1,
        BLUES = 0x2,
        COUNTRY = 0x4,
        DISCO = 0x8,
        FUNK = 0x10,
        HARD_ROCK = 0x20,
        HEAVY_METAL = 0x40,
        RAP = 0x80,
        HOUSE = 0x100,
        JAZZ = 0x200,
        NEW_WAVE = 0x400,
        RNB = 0x800,
        POP = 0x1000,
        PUNK = 0x2000,
        REGGAE = 0x4000,
        POP_ROCK = 0x8000,
        SOUL = 0x10000,
        TECHNO = 0x20000
    }

    public enum sp_albumtype
    {
        SP_ALBUMTYPE_ALBUM = 0,
        SP_ALBUMTYPE_SINGLE = 1,
        SP_ALBUMTYPE_COMPILATION = 2,
        SP_ALBUMTYPE_UNKNOWN = 3
    }

    public enum sp_imageformat
    {
        SP_IMAGE_FORMAT_UNKNOWN = -1,
        SP_IMAGE_FORMAT_JPEG = 0
    }

    public enum sp_image_size
    {
        SP_IMAGE_SIZE_NORMAL = 0,
        SP_IMAGE_SIZE_SMALL = 1,
        SP_IMAGE_SIZE_LARGE = 2
    }

    /// <summary>
    /// Playlist types.
    /// </summary>
    public enum sp_playlist_type
    {
        /// <summary>
        /// A normal playlist.
        /// </summary>
        SP_PLAYLIST_TYPE_PLAYLIST = 0,

        /// <summary>
        /// Marks a folder starting point.
        /// </summary>
        SP_PLAYLIST_TYPE_START_FOLDER = 1,

        /// <summary>
        /// Marks a folder ending point.
        /// </summary>
        SP_PLAYLIST_TYPE_END_FOLDER = 2,

        /// <summary>
        /// Unknown entry.
        /// </summary>
        SP_PLAYLIST_TYPE_PLACEHOLDER = 3
    }

    public enum sp_search_type
    {
        STANDARD = 0,
        SUGGEST = 1
    }

    /// <summary>
    /// Controls the type of data that will be included in artist browse queries.
    /// </summary>
    public enum sp_artistbrowse_type
    {
        /// <summary>
        /// All information except tophit tracks This mode is deprecated and will removed in a future release.
        /// </summary>
        FULL = 0,

        /// <summary>
        /// Only albums and data about them, no tracks. In other words, sp_artistbrowse_num_tracks() will return 0.
        /// </summary>
        NO_TRACKS = 1,

        /// <summary>
        /// Only return data about the artist (artist name, similar artist biography, etc.
        /// No tracks or album will be abailable. sp_artistbrowse_num_tracks() and sp_artistbrowse_num_albums() will both return 0.
        /// </summary>
        NO_ALBUMS = 2
    }

    public enum sp_track_availability
    {
        SP_TRACK_AVAILABILITY_UNAVAILABLE = 0,
        SP_TRACK_AVAILABILITY_AVAILABLE = 1,
        SP_TRACK_AVAILABILITY_NOT_STREAMABLE = 2,
        SP_TRACK_AVAILABILITY_BANNED_BY_ARTIST 	= 3
    }
    #endregion
}
