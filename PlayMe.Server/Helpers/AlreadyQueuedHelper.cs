using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Queue.Interfaces;

namespace PlayMe.Server.Helpers
{
    public class AlreadyQueuedHelper : IAlreadyQueuedHelper
    {
        private readonly IQueueManager queueManager;
        public AlreadyQueuedHelper(IQueueManager queueManager)
        {
            this.queueManager = queueManager;
        }

        public QueuedTrack ResetAlreadyQueued(QueuedTrack queuedTrack, string user)
        {
            queuedTrack.Track = ResetAlreadyQueued(queuedTrack.Track, user);
            return queuedTrack;
        }

        public Track ResetAlreadyQueued(Track track, string user)
        {
            if (track == null) return null;

            track.IsAlreadyQueued = (queueManager != null && queueManager.GetAll().Any(t => t.Track.Link == track.Link));
            return track;
        }
    }
}
