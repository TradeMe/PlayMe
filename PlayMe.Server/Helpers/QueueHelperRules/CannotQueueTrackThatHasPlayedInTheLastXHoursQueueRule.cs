using System;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.QueueHelperRules.Interfaces;

namespace PlayMe.Server.Helpers.QueueHelperRules
{
    public class CannotQueueTrackThatHasPlayedInTheLastXHoursQueueRule : IQueueRule
    {
        private readonly IDataService<QueuedTrack> queuedTrackDataService;
        private readonly IQueueRuleSettings queueRuleSettings;

        public CannotQueueTrackThatHasPlayedInTheLastXHoursQueueRule(IDataService<QueuedTrack> queuedTrackDataService, IQueueRuleSettings queueRuleSettings)
        {
            this.queuedTrackDataService = queuedTrackDataService;
            this.queueRuleSettings = queueRuleSettings;
        }

        public string CannotQueue(Track track, string user)
        {
            if (track == null) return string.Empty;

            var xHoursAgo = DateTime.Now.AddHours(-queueRuleSettings.LastXHours);
            return queuedTrackDataService.GetAll()
                       .Any(q => q.Track.Name == track.Name && q.StartedPlayingDateTime > xHoursAgo && q.User == user)
                       ? string.Format("Cannot queue a track that you queued in the last {0} hours.", queueRuleSettings.LastXHours)
                       : string.Empty;
        }
    }
}
