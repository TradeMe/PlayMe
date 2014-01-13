using System;
using System.Collections.Generic;
using PlayMe.Common.Model;

namespace PlayMe.Server.Queue.Interfaces
{
    public interface IQueueManager
    {
        QueuedTrack Dequeue();
        void Enqueue(QueuedTrack trackToQueue);
        bool Contains(Guid queuedTrackId);
        QueuedTrack Get(Guid queuedTrackId);
        IEnumerable<QueuedTrack> GetAll();
    }
}
