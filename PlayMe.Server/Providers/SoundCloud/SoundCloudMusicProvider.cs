using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using PlayMe.Common.Model;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.Player;
using RestSharp;
using SoundMapper = PlayMe.Server.Providers.SoundCloud.Mappers;
using SoundTrack = PlayMe.Server.Providers.SoundCloud.Model;

namespace PlayMe.Server.Providers.SoundCloud
{

    public class SoundCloudMusicProvider : IMusicProvider
    {       
        private readonly string token;
        private readonly ILogger logger;
        private readonly IStreamedPlayer player;
        private readonly SoundMapper.ITrackMapper trackMapper;
        private readonly ISoundCloudSettings soundCloudSettings;
        private const string BaseUrl = "https://api.soundcloud.com";
        public SoundCloudMusicProvider(ILogger logger, IStreamedPlayer player, ISoundCloudSettings soundCloudSettings, SoundMapper.ITrackMapper trackMapper)
        {
            this.soundCloudSettings = soundCloudSettings;
            this.trackMapper = trackMapper;
            this.player = player;
            this.logger = logger;

            if (IsEnabled)
            {
                token = GetToken();
            }

            player.PlaybackEnded += player_PlaybackEnded;
        }

        protected T ExecuteRequest<T>(RestRequest request) where T : new()
        {
            var client = new RestClient(BaseUrl)
                             {
                                 Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token)
                             };
            var response = client.Execute(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
        protected string GetToken()
        {
            //Authentication data
            string postData =
                string.Format(
                    "client_id={0}&client_secret={1}&grant_type=password&username={2}&password={3}&scope=non-expiring",
                    soundCloudSettings.ClientId,
                    soundCloudSettings.ClientSecret,
                    soundCloudSettings.UserName,
                    soundCloudSettings.Password);

            string tokenInfo = new WebClient().UploadString(BaseUrl + "/oauth2/token", postData);

            //Parse the token
            tokenInfo = tokenInfo.Remove(0, tokenInfo.IndexOf("token\":\"", StringComparison.Ordinal) + 8);
            return tokenInfo.Remove(tokenInfo.IndexOf("\"", StringComparison.Ordinal));            
        }

        public SearchResults SearchAll(string searchTerm, string user)
        {
           
            var restRequest = new RestRequest
                                  {
                                      Resource = "tracks",
                                      Method = Method.GET,
                                      OnBeforeDeserialization =
                                          resp => { resp.ContentType = "application/json"; },
                                      RequestFormat = DataFormat.Json,                                      
                                  };            
            restRequest.AddParameter("q", searchTerm);

            var tracks = ExecuteRequest<List<SoundTrack.Track>>(restRequest);
            var results = new SearchResults();

            //set up tracks
            var pagedTracks = new TrackPagedList
            {
                Total = tracks.Count,
                Tracks = tracks
                    .Where(scTrack => scTrack.streamable)
                    .Select(t => trackMapper.Map(t, this, user, true))
                    .ToArray()
            };

            results.PagedTracks = pagedTracks;
            results.PagedAlbums = new AlbumPagedList
                {
                    Total = 0,
                    Albums = new List<Album>().ToArray()
                };
            results.PagedArtists = new ArtistPagedList
            {
                Total = 0,
                Artists = new List<Artist>().ToArray()
            };

            logger.Debug("Search completed for term {0}", searchTerm);
            return results;
        }

        public Artist BrowseArtist(string link, bool mapTracks)
        {
            return null;
        }

        public Album BrowseAlbum(string link, string user)
        {
            return null;
        }

        public Track GetTrack(string link, string user)
        {
            var request = new RestRequest
                {
                    Resource = "tracks/{link}.json",
                    Method = Method.GET,
                    OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; },
                    RequestFormat = DataFormat.Json
                };
            request.AddUrlSegment("link", link);


            var track = ExecuteRequest<SoundTrack.Track>(request);

            return trackMapper.Map(track, this, user);
        }

        public void PlayTrack(Track trackToPlay)
        {
            var url = new Uri(string.Format("{0}/tracks/{1}/stream?oauth_token={2}",BaseUrl,trackToPlay.Link,token), UriKind.Absolute);
            player.PlayFromUrl(url);
        }
       
        public void EndTrack()
        {
            player.Reset();
        }

        public IPlayer Player
        {
            get { return player; }
        }

        public event EventHandler TrackEnded;
      

        public bool IsEnabled
        {
            get { return soundCloudSettings.IsEnabled; }
        }

        private void player_PlaybackEnded(object sender, EventArgs e)
        {
            if (TrackEnded != null)
            {
                TrackEnded(this, new EventArgs());
            }
        }

        public MusicProviderDescriptor Descriptor
        {
            get
            {
                return new MusicProviderDescriptor
                {
                    Identifier = "sc",
                    Name = "SoundCloud"
                };
            }
        }


        public int Priority
        {
            get { return 10; }
        }
    }
}