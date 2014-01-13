using PlayMe.Common.Model;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.AutoPlay.TrackRandomizers.Interfaces;
using PlayMe.Server.Extensions;

namespace PlayMe.Server.AutoPlay.TrackRandomizers
{
    public class PassThroughRandomizer : ITrackRandomizer
    {
        private readonly ILogger logger;

        public PassThroughRandomizer(ILogger logger)
        {
            this.logger = logger;
        }

        public int Version
        {
            get { return 0; }
        }

        public QueuedTrack Execute(QueuedTrack trackToQueue)
        {
            logger.Debug("PassThroughRandomizer, {0}", trackToQueue.ToLoggerFriendlyTrackName());
            return trackToQueue;
        }
    }
}