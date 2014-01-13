using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayMe.Common.Model;

namespace PlayMe.Server.Queue.Interfaces
{
    public interface IConcurrentQueueOfQueuedTrack : IEnumerable<QueuedTrack>
    {
        void Enqueue(QueuedTrack item);
        bool TryDequeue(out QueuedTrack result);
    }
}
