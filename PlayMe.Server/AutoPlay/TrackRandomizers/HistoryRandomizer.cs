using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.AutoPlay.TrackRandomizers.Interfaces;
using PlayMe.Server.Extensions;
using System.Linq;

namespace PlayMe.Server.AutoPlay.TrackRandomizers
{
    public class HistoryRandomizer : ITrackRandomizer
    {
        private readonly IDataService<QueuedTrack> queuedTrackDataService;
        private readonly ILogger logger;

        public HistoryRandomizer(IDataService<QueuedTrack> queuedTrackDataService, ILogger logger)
        {
            this.queuedTrackDataService = queuedTrackDataService;
            this.logger = logger;
        }

        public int Version
        {
            get { return 4; }
        }

        public QueuedTrack Execute(QueuedTrack trackToQueue)
        {
            logger.Debug("HistoryRandomizer, {0}", trackToQueue.ToLoggerFriendlyTrackName());

            var nextTrack = queuedTrackDataService.GetAll().Random().FirstOrDefault();

            if (nextTrack != null)
            {
                return nextTrack;
            }

            return trackToQueue;
        }
    }
}