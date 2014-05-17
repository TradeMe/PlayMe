using System;
using System.Collections.Generic;
using System.ServiceModel;
using PlayMe.Common.Model;

namespace PlayMe.Server.Interfaces
{
    [ServiceContract]
    public interface IMusicService
    {
        #region Searching
        [OperationContract]
        IEnumerable<MusicProviderDescriptor> GetActiveProviders();

        [OperationContract]
        SearchResults SearchAll(string searchTerm, string provider, string user);

        [OperationContract]
        IEnumerable<string> MatchSearchTermHistory(string partialSearchTerm);
        #endregion

        #region Browsing
        [OperationContract]
        Artist BrowseArtist(string link, string provider);

        [OperationContract]
        Album BrowseAlbum(string link, string provider, string user);

        [OperationContract]
        Track GetTrack(string link, string provider, string user);
        #endregion

        #region Queue
        [OperationContract]
        string QueueTrack(QueuedTrack queuedTrack);

        [OperationContract]
        void VetoTrack(Guid queuedTrackId, string user);

        [OperationContract]
        IEnumerable<QueuedTrack> GetQueue();

        [OperationContract]
        void ForgetTrack(Guid queuedTrackId, string user);

        [OperationContract]
        void SkipTrack(Guid queuedTrackId, string user);

        [OperationContract]
        QueuedTrack GetPlayingTrack();

        [OperationContract]
        void PauseTrack(string user);

        [OperationContract]
        void ResumeTrack(string user);

        [OperationContract]
        int GetCurrentVolume();

        [OperationContract]
        void IncreaseVolume(string user);

        [OperationContract]
        void DecreaseVolume(string user);
        #endregion

        #region History
        [OperationContract]
        PagedResult<QueuedTrack> GetTrackHistory(int start, int limit, string user);
        #endregion

        #region Likes
        [OperationContract]
        PagedResult<Track> GetLikes(int start, int limit, string user);

        [OperationContract]
        void LikeTrack(Guid queuedTrackId, string user);
        #endregion

        #region RickRoll

        [OperationContract]
        IEnumerable<RickRoll> GetAllRickRolls();

        [OperationContract]
        RickRoll AddRickRoll(PlayMeObject playMeObject);

        [OperationContract]
        void RemoveRickRoll(Guid id);

        #endregion

        #region Admin
        [OperationContract]
        bool IsUserAdmin(string user);

        [OperationContract]
        bool IsUserSuperAdmin(string user);

        [OperationContract]
        IEnumerable<User> GetAdminUsers();

        [OperationContract]
        User AddAdminUser(User toAdd, string addedBy);
       
        [OperationContract]
        void RemoveAdminUser(string username, string removedBy);

        [OperationContract]
        PagedResult<LogEntry> GetLogEntries(SortDirection direction, int start, int take);

        [OperationContract]
        string GetDomain();
        #endregion
    }
}
