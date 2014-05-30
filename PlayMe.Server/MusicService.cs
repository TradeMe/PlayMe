using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Data.Mongo;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.AutoPlay;
using PlayMe.Server.Broadcast;
using PlayMe.Server.Extensions;
using PlayMe.Server.Helpers;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Player;
using PlayMe.Server.Providers;
using PlayMe.Server.Queries;
using PlayMe.Server.Queue.Interfaces;
using PlayMe.Server.SoundBoard;

namespace PlayMe.Server
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
	public sealed class MusicService : IMusicService
	{
		private readonly ISkipHelper skipHelper;
        private readonly IVolume volume;
		private readonly IAutoPlay autoplayer;
		private readonly IDataService<QueuedTrack> queuedTrackDataService;
		private readonly IDataService<User> adminUserDataService;
		private readonly ISearchSuggestionService searchSuggestionService;
		private readonly ILogger logger;
        
		private readonly IMusicProviderFactory musicProviderFactory;
		private readonly IRickRollService rickRollService;
		private readonly IBroadcastService broadcastService;
		private readonly ISoundBoardService soundBoardService;
	    private readonly IQueueManager queueManager;
	    private readonly IAlreadyQueuedHelper alreadyQueuedHelper;
	    private readonly IMusicPlayer musicPlayer;
	    private readonly ICallbackClient callbackClient;
        private readonly IUserService userService;
	    private readonly IVetoHelper vetoHelper;
        private readonly IQueueRuleHelper queueRuleHelper;
        private readonly ISearchRuleHelper searchRuleHelper;

	    public MusicService(ILogger logger,
			IMusicProviderFactory musicProviderFactory,
			IAutoPlay autoplayer,
            IDataService<QueuedTrack> queuedTrackDataService,
			IDataService<User> adminUserDataService,
			ISearchSuggestionService searchSuggestionService,
			IRickRollService rickRollService,
			IBroadcastService broadcastService,
			ISoundBoardService soundBoardService,
            ISkipHelper skipHelper,
            IVolume volume,
            IQueueManager queueManager,
            IAlreadyQueuedHelper alreadyQueuedHelper,
            IMusicPlayer musicPlayer,
            ICallbackClient callbackClient,
            IUserService userService,
			IVetoHelper vetoHelper,
            IQueueRuleHelper queueRuleHelper,
            ISettings settings,
            ISearchRuleHelper searchRuleHelper
            )
		{
	        this.vetoHelper = vetoHelper;
	        this.callbackClient = callbackClient;
	        this.alreadyQueuedHelper = alreadyQueuedHelper;
	        this.queueManager = queueManager;
	        this.broadcastService = broadcastService;
			this.rickRollService = rickRollService;
			this.logger = logger;
			this.musicProviderFactory = musicProviderFactory;
			this.autoplayer = autoplayer;
			this.queuedTrackDataService = queuedTrackDataService;
			this.adminUserDataService = adminUserDataService;
			this.searchSuggestionService = searchSuggestionService;
			this.soundBoardService = soundBoardService;
			this.skipHelper = skipHelper;
            this.volume = volume;
            this.musicPlayer = musicPlayer;
	        this.callbackClient = callbackClient;
            this.userService = userService;            
            this.queueRuleHelper = queueRuleHelper;

	        this.searchRuleHelper = searchRuleHelper;
	        foreach (var provider in musicProviderFactory.GetAllMusicProviders())
			{
				provider.TrackEnded += musicProvider_TrackEnded;        
			}

			if (settings.AutoStart)
			{
			    PlayNextTrack();
			}
		}

		private void musicProvider_TrackEnded(object sender, EventArgs e)
		{
			PlayNextTrack();
		}

	    public void PlayNextTrack()
		{
            logger.Debug("Dequeuing next track");
            var nextToPlay = queueManager.Dequeue();
            if (nextToPlay == null)
            {
                logger.Debug("Asking AutoPlay for next track");
                nextToPlay = autoplayer.FindTrack();

                if (nextToPlay != null) logger.Debug("Track {0} found by autoplay", nextToPlay.ToLoggerFriendlyTrackName());
            }

            callbackClient.QueueChanged(queueManager.GetAll());
            			
			if (nextToPlay != null) musicPlayer.PlayTrack(nextToPlay);
		}

        public Track GetTrack(string link, string provider, string user)
        {
            var foundProvider = musicProviderFactory.GetMusicProviderByIdentifier(provider);
            if (foundProvider.IsEnabled)
            {
                return foundProvider.GetTrack(link, user);
            }
            else
            {
                return null;
            }
        }

		#region Browse and Search

		public SearchResults SearchAll(string searchTerm, string provider, string user)
        {
            logger.Debug("User {0} searched for '{1}'", user, searchTerm);

			// currently only searching spotify
			var musicProvider = musicProviderFactory.GetMusicProviderByIdentifier(provider);

            var searchResults = musicProvider.SearchAll(searchTerm, user);
            searchResults.PagedTracks.Tracks = searchResults.PagedTracks.Tracks.Select(t => alreadyQueuedHelper.ResetAlreadyQueued(t,user));
		    searchResults.PagedTracks = searchRuleHelper.FilterTracks(searchResults.PagedTracks);
		    searchResults.PagedAlbums = searchRuleHelper.FilterAlbums(searchResults.PagedAlbums);
		    searchResults.PagedArtists = searchRuleHelper.FilterArtists(searchResults.PagedArtists);

			// save the search if there were results
			if (searchResults.PagedAlbums.Albums.Any() 
				|| searchResults.PagedArtists.Artists.Any() 
				|| searchResults.PagedTracks.Tracks.Any())
					searchSuggestionService.UpdateSearchTermRecords(searchTerm);
		   
			return searchResults;
		}

		/// <summary>
		/// Finds successful search terms that match the 'partialSearchTerm' given
		/// </summary>
		/// <param name="partialSearchTerm"></param>
		/// <returns>A list of most recent matching search terms in alphabetical order</returns>
		public IEnumerable<string> MatchSearchTermHistory(string partialSearchTerm)
		{
			return searchSuggestionService.GetSearchSuggestions(partialSearchTerm);
		}

		public Artist BrowseArtist(string link, string provider)
		{
			// Artists only for spotify
			return musicProviderFactory.GetMusicProviderByIdentifier(provider).BrowseArtist(link, false);
		}

		public Album BrowseAlbum(string link, string provider, string user)
		{
			// Albums only for spotify			
			var album = musicProviderFactory.GetMusicProviderByIdentifier(provider).BrowseAlbum(link, user);
			if (album != null)
			{
				album.Tracks= album.Tracks.Select(t => alreadyQueuedHelper.ResetAlreadyQueued(t, user));
			}
			return album;
		}

		#endregion

		#region Queue

        public string QueueTrack(QueuedTrack trackToQueue)
        {
            if (trackToQueue == null) return "Track could not be found.";

            var errors = queueRuleHelper.CannotQueueTrack(trackToQueue.Track, trackToQueue.User);
            if (errors != null && errors.Any(e => e != string.Empty)) return errors.FirstOrDefault(e => e != string.Empty);

		    trackToQueue.Id = DataObject.GenerateId();
		    queueManager.Enqueue(trackToQueue);

		    //for some reason no track is playing then dequeue and play this track
		    if (musicPlayer.CurrentlyPlayingTrack == null) PlayNextTrack();
		    logger.Debug("Track {0} queued by {1}", trackToQueue.ToLoggerFriendlyTrackName(), trackToQueue.User);
		    callbackClient.QueueChanged(GetQueue());
            return string.Empty;
        }

		public void VetoTrack(Guid queuedTrackId, string user)
		{
            if (queueManager.Contains(queuedTrackId))
            {
                VetoUpcomingTrack(queuedTrackId, user);
            }
            else
            {
		        VetoCurrentTrack(queuedTrackId, user);
            }
		}

	    private void VetoCurrentTrack(Guid queuedTrackId, string user)
	    {
            var foundTrack = musicPlayer.CurrentlyPlayingTrack;
	        if (foundTrack == null || foundTrack.Id != queuedTrackId) return;

	        if (vetoHelper.CantVetoTrack(user, foundTrack)) return;

            foundTrack.Vetoes = foundTrack.Vetoes.ToList();
	        foundTrack.Vetoes.Add(new Veto {ByUser = user});
	        queuedTrackDataService.Update(foundTrack);
	        logger.Info("Track {0} vetoed by {1}", foundTrack.ToLoggerFriendlyTrackName(), user);

            if (foundTrack.Vetoes.Count >= skipHelper.RequiredVetoCount(foundTrack))
	        {
	            logger.Info("Maximum vetoes reached on track {0}. Skipping", foundTrack.ToLoggerFriendlyTrackName());
	            foundTrack.IsSkipped = true;
	            SkipToNextTrack();
	        }
	        else
	        {
	            callbackClient.PlayingTrackChanged(foundTrack);
	        }
	    }

	    private void VetoUpcomingTrack(Guid queuedTrackId, string user)
	    {
	        var foundTrack = queueManager.Get(queuedTrackId);            
	        if (vetoHelper.CantVetoTrack(user,foundTrack)) return;
            foundTrack.Vetoes = foundTrack.Vetoes.ToList();
	        foundTrack.Vetoes.Add(new Veto {ByUser = user});

	        if (foundTrack.Vetoes.Count >= skipHelper.RequiredVetoCount(foundTrack))
	        {
	            foundTrack.IsSkipped = true;
	            logger.Info("Maximum vetoes reached on upcoming track {0}. Track will not be played",
	                        foundTrack.ToLoggerFriendlyTrackName());
	        }
	        else
	        {
                logger.Info("Upcoming track {0} vetoed by {1}", foundTrack.ToLoggerFriendlyTrackName(), user);
	        }
	        callbackClient.QueueChanged(queueManager.GetAll());
	    }
        
        public QueuedTrack GetPlayingTrack()
        {
            return musicPlayer.CurrentlyPlayingTrack;
        }

		#endregion

		#region Admin

        public void SkipTrack(Guid queuedTrackId, string user)
        {
            if (queueManager.Contains(queuedTrackId))
            {
                SkipUpcomingTrack(queuedTrackId, user);
            }
            else
            {
                SkipCurrentTrack(queuedTrackId, user);
            }
        }

        private void SkipCurrentTrack(Guid queuedTrackId, string user)
		{
            if (!IsUserAdmin(user)) return;

            if (queuedTrackId != musicPlayer.CurrentlyPlayingTrack.Id) return;
            
            logger.Info("{0} skipped the current track {1}", user, musicPlayer.CurrentlyPlayingTrack.ToLoggerFriendlyTrackName());
            SkipToNextTrack();
		}

        private void SkipUpcomingTrack(Guid queuedTrackId, string user)
        {
            if (!IsUserAdmin(user)) return;

            var foundTrack = queueManager.Get(queuedTrackId);
            if (foundTrack == null) return;
            foundTrack.IsSkipped = true;
            callbackClient.QueueChanged(GetQueue());
            logger.Info("{0} skipped the upcoming track {1}", user, musicPlayer.CurrentlyPlayingTrack.ToLoggerFriendlyTrackName());
        }

        public void ForgetTrack(Guid queuedTrackId, string user)
        {
            if (queueManager.Contains(queuedTrackId))
            {
                ForgetUpcomingTrack(queuedTrackId, user);
            }
            else
            {
                ForgetCurrentTrack(queuedTrackId, user);
            }
        }

		private void ForgetCurrentTrack(Guid queuedTrackId, string user)
		{
			if (!IsUserAdmin(user)) return;

		    if (queuedTrackId != musicPlayer.CurrentlyPlayingTrack.Id) return;
		    musicPlayer.CurrentlyPlayingTrack.Excluded = true;
		    queuedTrackDataService.Update(musicPlayer.CurrentlyPlayingTrack);
		    logger.Info("{0} forgot the current track {1}", user, musicPlayer.CurrentlyPlayingTrack.ToLoggerFriendlyTrackName());
		    SkipToNextTrack();
		}

        private void ForgetUpcomingTrack(Guid queuedTrackId, string user)
        {
            if (!IsUserAdmin(user)) return;

            var foundTrack = queueManager.Get(queuedTrackId);
            if (foundTrack == null) return;
            foundTrack.Excluded = true;
            callbackClient.QueueChanged(GetQueue());
            logger.Info("{0} forgot the upcoming track {1}", user, musicPlayer.CurrentlyPlayingTrack.ToLoggerFriendlyTrackName());
        }

		private void SkipToNextTrack()
		{
            musicPlayer.EndTrack();
            soundBoardService.PlayVetoSound();
            PlayNextTrack();
		}

        public void PauseTrack(string user)
        {
            if (!IsUserAdmin(user)) return;
            
            musicPlayer.PauseTrack(user);
        }

		public void ResumeTrack(string user)
		{
			if (!IsUserAdmin(user)) return;
		
            musicPlayer.ResumeTrack(user);
		}

        public int GetCurrentVolume()
        {
            return Convert.ToInt32(volume.CurrentVolume * 100);
        }

		public void IncreaseVolume(string user)
		{            
			if (!IsUserAdmin(user)) return;

            musicPlayer.IncreaseVolume();

            logger.Info("{0} increased the volume to {1}", user, volume.CurrentVolume);
		}

		public void DecreaseVolume(string user)
		{            
			if (!IsUserAdmin(user)) return;

            musicPlayer.DecreaseVolume();

            logger.Info("{0} decreased the volume to {1}", user, volume.CurrentVolume);
		}

	    public bool IsUserAdmin(string user)
	    {
	        return userService.IsUserAdmin(user);
	    }

	    public IEnumerable<User> GetAdminUsers() 
		{
            return userService.GetAdminUsers(); 
		}

		/// <summary>
		/// Adds the given user if they don't already have admin rights.
		/// </summary>
		/// <param name="newAdminUser">Throws ArgumentException if null</param>
		/// <returns>The added User, or null if the user existed</returns>
		public User AddAdminUser(User newAdminUser, string addedBy)
		{
			if (newAdminUser == null) 
			{
				throw new ArgumentException("Cannot add a null adminUser");
			}

			bool userAlreadyAdmin = adminUserDataService.GetAll()
				.Any(t => t.Username == newAdminUser.Username);

			if (!userAlreadyAdmin)
			{
				adminUserDataService.Insert(newAdminUser);
			    logger.Info("{0} added {1} to the list of admins.", addedBy, newAdminUser.Username);
				return newAdminUser;
			}
		    return null;
		}

		public void RemoveAdminUser(string username, string removedBy) 
		{ 
			var toRemove = adminUserDataService.GetAll()
				.FirstOrDefault(t=> t.Username.ToLower() == username.ToLower());

			if (toRemove == null)
			{
				throw new ArgumentException("No User could be found with that Guid");
			}

            adminUserDataService.Delete(toRemove);
            logger.Info("{0} removed {1} from the list of admins.", removedBy, username);
		}

        public PagedResult<LogEntry> GetLogEntries(SortDirection direction, int start, int take)
        {	
            var lemdService = new LogEntryMongoDataService();
            var results = lemdService.Get();
            results = (direction == SortDirection.Descending) ? results.OrderByDescending(t => t.timeStamp) : results.OrderBy(t => t.timeStamp);
            int total = results.Count();
            return new PagedResultHelper().GetPagedResult(start, total, results.Skip(start).Take(take));
        }
        
		#endregion

		#region History

		public PagedResult<QueuedTrack> GetTrackHistory(int start, int limit, string user)
		{
			logger.Debug("Getting track history records {0} to {1} for user {2}",start,start+limit,user);
		    int total;
			var results = queuedTrackDataService.GetAll()
				.GetQueuedTracksByUser(user, start, limit, out total)
				.Select(r => alreadyQueuedHelper.ResetAlreadyQueued(r, user));

			logger.Debug("Successfully got track history records {0} to {1} for user {2}", start, start + limit, user);
			return new PagedResultHelper().GetPagedResult(start,total,results);
		}

		#endregion

		#region Likes

        public void LikeTrack(Guid queuedTrackId, string user)
        {
            if (queueManager.Contains(queuedTrackId))
            {
                LikeUpcomingTrack(queuedTrackId, user);
            }
            else
            {
                LikeCurrentTrack(queuedTrackId, user);
            }
        }

        private void LikeUpcomingTrack(Guid queuedTrackId, string user)
        {
            var foundTrack = queueManager.Get(queuedTrackId);
            if (foundTrack.Likes.Any(v => v.ByUser == user)) return;
            foundTrack.Likes = foundTrack.Likes.ToList();
            foundTrack.Likes.Add(new Like { ByUser = user });

            logger.Debug("Upcoming track {0} liked by {1}", foundTrack.ToLoggerFriendlyTrackName(), user);
            
            callbackClient.QueueChanged(queueManager.GetAll());
        }

		private void LikeCurrentTrack(Guid queuedTrackId, string user)
        {
            var foundTrack = musicPlayer.CurrentlyPlayingTrack;
            if (foundTrack == null || foundTrack.Id != queuedTrackId) return;

		    if (foundTrack.Likes.Any(v => v.ByUser == user)) return;

            foundTrack.Likes = foundTrack.Likes.ToList();
		    foundTrack.Likes.Add(new Like {ByUser = user});
		    queuedTrackDataService.Update(foundTrack);
            logger.Debug("{0} liked track {1}", user, foundTrack.ToLoggerFriendlyTrackName());
            
            broadcastService.Broadcast(foundTrack);
            
            callbackClient.PlayingTrackChanged(foundTrack);
		}
	   
		public PagedResult<Track> GetLikes(int start, int limit, string user)
		{
			logger.Debug("Getting like records {0} to {1} for user {2}", start, start + limit, user);
		    int total;
            var results = queuedTrackDataService.GetAll()
				.GetLikedTracks(user, start, limit, out total);
			logger.Debug("Successfully got like records {0} to {1} for user {2}", start, start + limit, user);
			return new PagedResultHelper().GetPagedResult(start, limit, results);
		}

		#endregion
        
		#region RickRoll

		public IEnumerable<RickRoll> GetAllRickRolls()
		{
			return rickRollService.GetAllRickRolls();
		}

		public RickRoll AddRickRoll(PlayMeObject playMeObject)
		{
		   return rickRollService.AddRickRoll(playMeObject);
		}

		public void RemoveRickRoll(Guid id)
		{
			rickRollService.RemoveRickRoll(id);
		}

		#endregion

        public IEnumerable<QueuedTrack> GetQueue()
        {
            return queueManager.GetAll();
        }

        public string GetDomain()
        {
            return userService.GetDomain();
        }

        public IEnumerable<MusicProviderDescriptor> GetActiveProviders()
        {
            return musicProviderFactory.GetAllMusicProviders()
                .Where(p => p.IsEnabled)
                .OrderBy(p => p.Priority)
                .Select(p=> p.Descriptor);
        }
    }
}
