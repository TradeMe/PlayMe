using System;
using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.Interfaces;
using PlayMe.Server.PlayMeCallbackServiceReference;

namespace PlayMe.Server
{
    public class CallbackClient : ICallbackClient
    {
        private readonly ILogger logger;

        public CallbackClient(ILogger logger)
        {
            this.logger = logger;
        }

        public void TrackHistoryChanged(PagedResult<QueuedTrack> trackHistory)
        {
            try
            {
                using (var client = new SpotifyCallbackServiceClient())
                {
                    client.TrackHistoryChanged(trackHistory);
                }
            }
            catch (Exception ex)
            {
                logger.Error("TrackHistoryChanged failed with exception {0}", ex.Message);
            }
        }

        public void QueueChanged(IEnumerable<QueuedTrack> notifyQueue)
        {
            try
            {
                using (var client = new SpotifyCallbackServiceClient())
                {
                    client.QueueChanged(notifyQueue.ToList());
                }
            }
            catch (Exception ex)
            {
                logger.Error("QueueChanged failed with exception {0}", ex.Message);
            }
        }

        public void PlayingTrackChanged(QueuedTrack track)
        {
            try
            {
                using (var client = new SpotifyCallbackServiceClient())
                {
                    client.PlayingTrackChanged(track);
                }
            }
            catch (Exception ex)
            {
                logger.Error("PlayingTrackChanged failed with exception {0}", ex.Message);
            }
        }
    }
}
