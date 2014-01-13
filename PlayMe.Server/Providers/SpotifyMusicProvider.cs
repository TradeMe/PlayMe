using System;
using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.Mappers;
using SpotiFire.SpotifyLib;
using Album = PlayMe.Common.Model.Album;
using Artist = PlayMe.Common.Model.Artist;
using ArtistProfile = PlayMe.Common.Model.ArtistProfile;
using Track = PlayMe.Common.Model.Track;

namespace PlayMe.Server.Providers
{

    public sealed class SpotifyMusicProvider : IMusicProvider, IDisposable
    {
        private readonly ISession session;
        private readonly BASSPlayer player = new BASSPlayer();

        private const float VolumeStep = 0.05f;

        readonly byte[] ApplicationKey = new Byte[]
            {
                0x01, 0x5B, 0x78, 0xC2, 0x6F, 0x20, 0x99, 0x9D, 0x06, 0x6C, 0x71, 0xA2, 0x54, 0xE9, 0xB4, 0xE2,
	            0xE5, 0x71, 0x83, 0xBC, 0x52, 0xAE, 0xC0, 0x9C, 0x1C, 0x39, 0xEB, 0xE8, 0xF8, 0x3A, 0x1B, 0xA4,
	            0xDF, 0x12, 0x82, 0x8A, 0x2E, 0x69, 0x45, 0x85, 0x39, 0xDC, 0xEA, 0x47, 0x98, 0x9F, 0x00, 0x29,
	            0x95, 0xD2, 0x53, 0x87, 0x9A, 0x52, 0x50, 0x1A, 0x91, 0x84, 0x94, 0xF9, 0x9B, 0x7E, 0x54, 0x0B,
	            0x32, 0xA4, 0x22, 0xA3, 0x07, 0xD2, 0x7C, 0x70, 0x6A, 0x4C, 0x0B, 0x99, 0x79, 0x67, 0x88, 0x4E,
	            0x29, 0x49, 0x13, 0x00, 0x6E, 0x2A, 0xC0, 0xD3, 0xD4, 0xC5, 0x2E, 0xAF, 0xD0, 0x12, 0x2B, 0x80,
	            0x31, 0xB2, 0xE5, 0x56, 0x5E, 0x31, 0x70, 0x74, 0xD5, 0x15, 0x37, 0x00, 0xD1, 0xE6, 0xE1, 0xE8,
	            0xC4, 0x9D, 0xAC, 0x88, 0x74, 0xA0, 0x8F, 0xCE, 0x1A, 0x5B, 0x5B, 0xD4, 0x90, 0x5D, 0xCF, 0xD4,
	            0x01, 0xC4, 0xDF, 0x2E, 0x39, 0xD7, 0xA2, 0x16, 0xEB, 0xBA, 0x82, 0xD1, 0xA1, 0x1C, 0xE0, 0x17,
	            0x0E, 0x2E, 0xB3, 0xCB, 0x02, 0x76, 0x84, 0x16, 0x9E, 0x1A, 0xE6, 0xDF, 0x82, 0x11, 0xD3, 0xD9,
	            0x7D, 0x17, 0x5C, 0x8E, 0x00, 0x6C, 0xDA, 0xFE, 0x34, 0x41, 0xD6, 0x7B, 0xCE, 0x65, 0xAE, 0xB0,
	            0x7C, 0x6B, 0x49, 0xA3, 0xE3, 0xAE, 0x61, 0x11, 0xCC, 0x62, 0xAE, 0xF9, 0xF5, 0xC2, 0x82, 0xC1,
	            0xC1, 0x9B, 0x67, 0x7E, 0x68, 0xDA, 0x98, 0xDF, 0x50, 0xB3, 0x6C, 0x1B, 0x5E, 0x12, 0x76, 0xD1,
	            0xC5, 0x35, 0xD6, 0xAA, 0x9C, 0x51, 0xEE, 0xD8, 0x4D, 0x4C, 0x3A, 0x59, 0xDC, 0xCF, 0x80, 0x74,
	            0xC0, 0x82, 0x2A, 0x46, 0x6C, 0x24, 0x3E, 0xF5, 0x51, 0x32, 0xDE, 0x18, 0xF2, 0xC4, 0xA1, 0x76,
	            0x00, 0x64, 0x53, 0x9A, 0x78, 0x90, 0xCB, 0xE3, 0x17, 0x6A, 0x2E, 0xB2, 0xF1, 0xEE, 0x63, 0x82,
	            0x12, 0xF4, 0xB8, 0x6A, 0x07, 0xFE, 0x37, 0x58, 0x08, 0xFA, 0x8B, 0x1F, 0xCE, 0xE6, 0x3D, 0xD7,
	            0x57, 0x6F, 0xFA, 0x88, 0xEE, 0xC8, 0x7B, 0x6D, 0xBA, 0x67, 0x79, 0x8E, 0xE2, 0x0A, 0x44, 0xC0,
	            0xC0, 0x74, 0x29, 0xB7, 0xF4, 0x8B, 0x82, 0x3F, 0xED, 0x74, 0x69, 0x60, 0xA7, 0xF0, 0xD2, 0x91,
	            0x87, 0x53, 0xB7, 0x56, 0x7B, 0xDA, 0xC1, 0x8D, 0x75, 0xAF, 0xAC, 0x7A, 0x5D, 0x4F, 0x1E, 0xF6,
	            0xE4
            };

        private readonly ILogger logger;
        public SpotifyMusicProvider(ILogger logger)
        {
            this.logger = logger;
            logger.Info("Creating Spotify session");
            session = Spotify.CreateSession(ApplicationKey, "c:\\temp", "c:\\temp", "Spotifire");
            session.Exception += session_Exception;
            session.MusicDeliver += session_MusicDeliver;
            session.EndOfTrack += session_EndOfTrack;
            session.ConnectionError += session_ConnectionError;

            Login();
        }
        #region spotify session events

        void session_ConnectionError(ISession sender, SessionEventArgs e)
        {
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

        void session_MusicDeliver(ISession sender, MusicDeliveryEventArgs e)
        {
            e.ConsumedFrames = e.Samples.Length > 0 ? player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames) : 0;
        }

        void session_Exception(ISession sender, SessionEventArgs e)
        {
            logger.Info("session_Exception occurred {0}", e.Message);
        }
        #endregion

        public event EventHandler TrackEnded;

        public Track GetTrack(string link)
        {
            logger.Trace("GetTrack");
            Login();
            const string prefix = "spotify:track:";
            using (var l = session.ParseLink(prefix + link))
            using (var track = l.As<ITrack>())
            {
                {
                    var trackTask = track.Load();
                    trackTask.Wait();
                    return new TrackMapper().Map(trackTask.Result, true, true);
                }
            }
        }

        private void Login()
        {
            logger.Trace("Login");
            if (session.ConnectionState == sp_connectionstate.LOGGED_IN || session.ConnectionState == sp_connectionstate.OFFLINE) return;
            var settings = new Settings();
            logger.Info("Logging into Spotify with username '{0}'", settings.SpotifyUsername);
            session.Login(settings.SpotifyUsername, settings.SpotifyPassword, false);
            var result = session.WaitForLoginComplete();
            if (result != sp_error.OK) logger.Error("Spotify Login failed");
        }

        #region Browse and Search

        public SearchResults SearchAll(string searchTerm)
        {
            Login();
            logger.Debug("Search started for term {0}", searchTerm);
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
                        Artists = search.Artists.Select(a => new ArtistMapper().Map(a)).ToArray()
                    };

                //set up albums
                var pagedAlbums = new AlbumPagedList
                    {
                        Total = search.TotalAlbums,
                        Albums = search.Albums.Where(a => a.IsAvailable)
                            .Select(a => new AlbumMapper().Map(a, true))
                            .ToArray()
                    };

                ////set up tracks
                var pagedTracks = new TrackPagedList
                    {
                        Total = search.TotalTracks,
                        Tracks = search.Tracks
                            .Select(t => new TrackMapper().Map(t, true, true))
                            .ToArray()
                    };

                results.PagedArtists = pagedArtists;
                results.PagedAlbums = pagedAlbums;
                results.PagedTracks = pagedTracks;

                logger.Info("Search completed for term {0}", searchTerm);
                return results;
            }
        }

        public Artist BrowseArtist(string link, bool mapTracks)
        {
            Login();
            logger.Debug("Artist Browse started for link {0}", link);
            const string prefix = "spotify:artist:";

            var browseTypeEnum = mapTracks ? sp_artistbrowse_type.FULL : sp_artistbrowse_type.NO_TRACKS;
            using (var artist = session.ParseLink(prefix + link).As<IArtist>())
            using (var browse = artist.Browse(browseTypeEnum))
            {
                browse.WaitForCompletion();
                if (!browse.IsComplete) logger.Error("Artist Browse timed out");

                var artistResult = new ArtistMapper().Map(artist);
                artistResult.LoadStatus = mapTracks? Artist.LoadedState.FullWithTracks: Artist.LoadedState.FullNoTracks;
                artistResult.Profile = new ArtistProfile
                {
                    Biography = browse.Biography,
                    SimilarArtists = browse.SimilarArtists
                                           .Select(a => new ArtistMapper().Map(a))
                                           .ToArray()
                };

                artistResult.Albums = browse.Albums
                                       .Where(a => a.IsAvailable)
                                       .Select(a => new AlbumMapper().Map(a))
                                       .ToArray();

                if (mapTracks)
                {
                    artistResult.Tracks = browse.Tracks
                                       .Where(a => a.IsAvailable)
                                       .Select(a => new TrackMapper().Map(a, true))
                                       .ToArray();
                }                                 

                logger.Info("Artist Browse completed for link {0}", link);
                return artistResult;
            }
        }

        public Album BrowseAlbum(string link)
        {
            Login();
            logger.Debug("Album Browse started for link {0}", link);
            const string prefix = "spotify:album:";
            using (var album = session.ParseLink(prefix + link).As<IAlbum>())
            using (var browse = album.Browse())
            {
                browse.WaitForCompletion();
                if (!browse.IsComplete)
                {
                    logger.Error("Album Browse timed out");
                    return null;
                }

                var albumResult = new AlbumMapper().Map(album, true);
                var trackMapper = new TrackMapper();
                var tracks = browse.Tracks
                    .Select(t => trackMapper.Map(t, false, true))
                    .ToArray();

                albumResult.Tracks = tracks;
                logger.Info("Album Browse completed for link {0}", link);
                return albumResult;
            }
        }

        public IEnumerable<Track> GetTracksByArtist(Artist artist, int maxTracks)
        {
            Login();
            logger.Debug("Track search by Artist started for {0}", artist.Name);
            using (var search = session.Search(artist.Name, 0, maxTracks, 0, 0, 0, 0, 0, 0, sp_search_type.STANDARD))
            {
                search.WaitForCompletion();
                if (!search.IsComplete)
                {
                    logger.Error("Search for tracks by {0} timed out", artist.Name);
                    return null;
                }

                return search.Tracks.Select(a => new TrackMapper().Map(a)).ToArray();
            }
        }

        #endregion

        public void Dispose()
        {
            session.Dispose();
        }

        public void PlayTrack(Track trackToPlay)
        {
            logger.Trace("PlayTrack");
            const string prefix = "spotify:track:";
            using (var l = session.ParseLink(prefix + trackToPlay.Link))
            using (var track = l.As<ITrack>())
            {
                logger.Debug("Attempting to load track {0}", trackToPlay);
                //Load the track in the player
                var trackTask = track.Load();
                logger.Debug("Waiting for track {0} to load", trackToPlay);
                trackTask.Wait();
                logger.Debug("Track {0} was loaded", trackToPlay);
                session.PlayerLoad(track);
                logger.Debug("Track {0} was loaded by player", trackToPlay);
                session.PlayerPlay();
                logger.Debug("Track {0} is playing", trackToPlay);
            }
        }

        public void SkipTrack()
        {
            player.Resume();
            session.PlayerUnload();
        }

        public void PauseTrack()
        {
            player.Pause();
        }

        public void ResumeTrack()
        {
            player.Resume();
        }

        public void IncreaseVolume()
        {
            player.Volume = player.Volume + VolumeStep;
        }

        public void DecreaseVolume()
        {
            player.Volume = player.Volume - VolumeStep;
        }

    }
}
