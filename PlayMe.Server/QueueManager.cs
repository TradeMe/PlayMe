using System;
using System.Linq;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.Extensions;
using PlayMe.Server.Queue.Interfaces;

namespace PlayMe.Server
{
    public class QueueManager : IQueueManager
    {
        private readonly IConcurrentQueueOfQueuedTrack queue;
        private readonly ILogger logger;

        public QueueManager(IConcurrentQueueOfQueuedTrack queue, ILogger logger)
        {
            this.logger = logger;
            this.queue = queue;
        }

        public QueuedTrack Dequeue()
        {
            QueuedTrack dequeued;
            if (!queue.TryDequeue(out dequeued))
            {
                dequeued = null;
            }

            if (dequeued != null && (dequeued.IsSkipped))
            {
                logger.Debug("Maximum vetoes reached or admin skipped track {0}", dequeued.ToLoggerFriendlyTrackName());
                return Dequeue();
            }
            return dequeued;
        }

        public void Enqueue(QueuedTrack trackToQueue)
        {
            queue.Enqueue(trackToQueue);
        }

        public bool Contains(Guid queuedTrackId)
        {
            return queue.Any(t => t.Id == queuedTrackId);
        }

        public QueuedTrack Get(Guid queuedTrackId)
        {
            return queue.SingleOrDefault(t => t.Id == queuedTrackId);
        }


        public System.Collections.Generic.IEnumerable<QueuedTrack> GetAll()
        {
            return queue.ToArray();
        }
    }
}
