using System;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.Player;
using PlayMe.Server.Providers.SpotifyProvider.Mappers;
using SpotiFire.SpotifyLib;
using Album = PlayMe.Common.Model.Album;
using Artist = PlayMe.Common.Model.Artist;
using Track = PlayMe.Common.Model.Track;

namespace PlayMe.Server.Providers.SpotifyProvider
{
    public sealed class SpotifyMusicProvider : IMusicProvider, IDisposable
    { 
        private readonly ISession session;
        private readonly ILogger logger;
        private readonly IBufferedPlayer player;
        private readonly ITrackMapper trackMapper;
        private readonly IAlbumMapper albumMapper;
        private readonly ISpotifySettings spotifySettings;

        public SpotifyMusicProvider(ILogger logger,IBufferedPlayer player, ITrackMapper trackMapper, IAlbumMapper albumMapper, ISpotifySettings spotifySettings)
        {
            this.spotifySettings = spotifySettings;
            this.albumMapper = albumMapper;
            this.trackMapper = trackMapper;
            this.logger = logger;
            this.player = player;

            logger.Debug("Creating Spotify session");
            session = Spotify.CreateSession(spotifySettings.ApplicationKey, "c:\\temp", "c:\\temp", "Spotifire");
            //session.VolumeNormalization = true;              
            session.MusicDeliver += session_MusicDelivered;
            session.EndOfTrack += session_EndOfTrack;
            session.ConnectionError += session_ConnectionError;
            Login();
        }

        #region Spotify session events

        void session_ConnectionError(ISession sender, SessionEventArgs e)
        {
            if (e == null) throw new ArgumentNullException("e");
            logger.Error("session_connectionError {0}", e.Message);
            Login();
        }

        void session_EndOfTrack(ISession sender, SessionEventArgs e)
        {
            if (TrackEnded != null)
            {
                TrackEnded(this, new EventArgs());
            }
        }

        void session_MusicDelivered(ISession sender, MusicDeliveryEventArgs e)
        {
            e.ConsumedFrames = e.Samples.Length > 0 ? player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames) : 0;
        }
     
        #endregion

        public event EventHandler TrackEnded;

        public Track GetTrack(string link, string user)
        {
            var track = GetSpotifireTrack(link);
            return trackMapper.Map(track, this, user, true, true);
        }

        private ITrack GetSpotifireTrack(string link)
        {
            const string prefix = "spotify:track:";
            using (var l = session.ParseLink(prefix + link))
            {
                var track = l.As<ITrack>();
                var trackTask = track.Load();
                trackTask.Wait();
                return track;
            }
        }

        private void Login()
        {
            if (session.ConnectionState == sp_connectionstate.LOGGED_IN || session.ConnectionState == sp_connectionstate.OFFLINE) return;            
            logger.Debug("Logging into Spotify with username '{0}'", spotifySettings.UserName);
            session.Login(spotifySettings.UserName, spotifySettings.Password, false);
            var result = session.WaitForLoginComplete();
            if (result != sp_error.OK) logger.Error("Spotify Login failed");        
        }

        #region Browse and Search

        public SearchResults SearchAll(string searchTerm, string user)
        {
            Login();
            using (var search = session.Search(searchTerm, 0, 30, 0, 15, 0, 15, 0, 0, sp_search_type.STANDARD))
            {
                search.WaitForCompletion();
                if (!search.IsComplete)
                {
                    logger.Error("Search for {0} timed out", searchTerm);
                    return null;
                }
                var results = new SearchResults();

                //set up artists
                var pagedArtists = new ArtistPagedList
                {
                    Total = search.TotalArtists,
                    Artists = search.Artists.Select(a => new ArtistMapper().Map(a, this)).ToArray()
                };

                //set up albums
                var pagedAlbums = new AlbumPagedList
                {
                    Total = search.TotalAlbums,
                    Albums = search.Albums.Where(a => a.IsAvailable)
                        .Select(a => albumMapper.Map(a, this, true))
                        .ToArray()
                };

                ////set up tracks
                var pagedTracks = new TrackPagedList
                {
                    Total = search.TotalTracks,
                    Tracks = search.Tracks
                        .Select(t => trackMapper.Map(t, this, user, true, true))
                        .Where(t => t.IsAvailable)
                        .ToArray()
                };

                results.PagedArtists = pagedArtists;
                results.PagedAlbums = pagedAlbums;
                results.PagedTracks = pagedTracks;

                return results;
            }
        }

        public Artist BrowseArtist(string link, bool mapTracks)
        {
            Login();
            logger.Debug("Artist Browse started for link {0}",link);
            const string prefix = "spotify:artist:";

            var browseTypeEnum = mapTracks ? sp_artistbrowse_type.FULL : sp_artistbrowse_type.NO_TRACKS;
            using (var artist = session.ParseLink(prefix + link).As<IArtist>())
            using (var browse = artist.Browse(browseTypeEnum))
            {
                browse.WaitForCompletion();
                if (!browse.IsComplete) logger.Error("Artist Browse timed out");

                var albums = browse.Albums
                    .Where(a => a.IsAvailable)
                    .Select(a => albumMapper.Map(a, this))
                    .ToArray();
                var artistResult = new ArtistMapper().Map(artist, this);
                artistResult.Profile = new ArtistProfile
                {
                    Biography = browse.Biography,
                    SimilarArtists = browse.SimilarArtists
                    .Select(a => new ArtistMapper().Map(a, this))
                    .ToArray()
                };

                artistResult.Albums = albums;

                logger.Debug("Artist Browse completed for link {0}", link);
                return artistResult;
            }
        }

        public Album BrowseAlbum(string link, string user)
        {
            Login();
            logger.Debug("Album Browse started for link {0}", link);
            const string prefix = "spotify:album:";
            using (var album = session.ParseLink(prefix + link).As<IAlbum>())
            {
                using (var browse = album.Browse())
                {
                    browse.WaitForCompletion();
                    if (!browse.IsComplete)
                    {
                        logger.Error("Album Browse timed out");
                        return null;
                    }

                    var albumResult = albumMapper.Map(album, this, true);
                    var tracks = browse.Tracks
                                       .Select(t => trackMapper.Map(t, this, user, false, true))
                                       .ToArray();

                    albumResult.Tracks = tracks;
                    logger.Debug("Album Browse completed for link {0}", link);
                    return albumResult;
                }
            }
        }
    
        #endregion
        
        public void Dispose()
        {
            session.Dispose();
        }

        public void PlayTrack(Track track)
        {
            var spotifireTrack = GetSpotifireTrack(track.Link);          
            session.PlayerLoad(spotifireTrack);
            session.PlayerPlay();           
        }
        
        public void EndTrack()
        {
            player.Resume();            
            player.Reset();
            session.PlayerUnload();
        }

        public bool IsEnabled { get { return spotifySettings.IsEnabled; } }
        
        public IPlayer Player
        {
            get { return player; }
        }

        public MusicProviderDescriptor Descriptor
        {
            get
            {
                return new MusicProviderDescriptor
                {
                    Identifier = "sp",
                    Name = "Spotify"
                };
            }
        }

        public Uri ExternalLinkBaseUrl
        {
            get { return new Uri("http://open.spotify.com/"); }
        }


        public int Priority
        {
            get { return 0; }
        }
    }
}
