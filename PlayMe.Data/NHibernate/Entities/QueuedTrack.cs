using System;
using System.Collections.Generic;

namespace PlayMe.Data.NHibernate.Entities
{
    public class QueuedTrack: DataObject
    {
        private IList<Veto> vetoes=new List<Veto>();
        private IList<Like> likes = new List<Like>();

        public virtual IList<Veto> Vetoes
        { 
            get 
            {
                return vetoes;
            } 
            set 
            {
                vetoes=value;
            } 
        }

        public virtual int VetoCount
        {
            get
            {
                return vetoes.Count;
            }
        }

        public virtual IList<Like> Likes
        {
            get
            {
                return likes;
            }
            set
            {
                likes = value;
            }
        }

        public virtual int LikeCount
        {
            get
            {
                return likes.Count;
            }
        }

        public virtual DateTime? StartedPlayingDateTime { get; set; }

        public virtual long PausedDurationAsMilliseconds { get; set; }
        public virtual bool IsPaused { get; set; }

        public virtual string User { get; set; }

        /// <summary>
        /// Forgotten
        /// </summary>
        public virtual bool Excluded { get; set; }

        /// <summary>
        /// This QueuedTrack should not be played when it gets
        /// dequeued. Could be that it has been vetoed, or 
        /// skipped by admin.
        /// </summary>
        public virtual bool IsSkipped { get; set; }

        public virtual Track Track { get; set; }

        /// <summary>
        /// Optional freetext reason for queuing this track
        /// </summary>
        public virtual string Reason { get; set; }
    }
}
