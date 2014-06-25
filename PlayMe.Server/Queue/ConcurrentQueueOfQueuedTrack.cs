using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Queue.Interfaces;

namespace PlayMe.Server.Queue
{
    public class ConcurrentQueueOfQueuedTrack : ConcurrentQueue<QueuedTrack> , IConcurrentQueueOfQueuedTrack
    {
    }
}
