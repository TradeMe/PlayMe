using System;
using System.Linq;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.QueueHelperRules.Interfaces;

namespace PlayMe.Server.Helpers.QueueHelperRules
{
    public class CannotQueueTrackThatHasPlayedInTheLastXHoursQueueRule : IQueueRule
    {
        private readonly IRepository<QueuedTrack> _queuedTrackRepository;
        private readonly IQueueRuleSettings queueRuleSettings;

        public CannotQueueTrackThatHasPlayedInTheLastXHoursQueueRule(IRepository<QueuedTrack> _queuedTrackRepository, IQueueRuleSettings queueRuleSettings)
        {
            this._queuedTrackRepository = _queuedTrackRepository;
            this.queueRuleSettings = queueRuleSettings;
        }

        public string CannotQueue(Track track, string user)
        {
            if (track == null) return string.Empty;

            var xHoursAgo = DateTime.Now.AddHours(-queueRuleSettings.LastXHours);
            return _queuedTrackRepository.GetAll()
                       .Any(q => q.Track.Name == track.Name && q.StartedPlayingDateTime > xHoursAgo && q.User == user)
                       ? string.Format("Cannot queue a track that you queued in the last {0} hours.", queueRuleSettings.LastXHours)
                       : string.Empty;
        }
    }
}
