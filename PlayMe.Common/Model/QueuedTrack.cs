using System;
using System.Collections.Generic;

namespace PlayMe.Common.Model
{
    public class QueuedTrack : SavedTrack
    {
        private IList<Veto> vetoes=new List<Veto>();
        private IList<Like> likes = new List<Like>();

        public IList<Veto> Vetoes { 
            get 
            {
                return vetoes;
            } 
            set 
            {
                vetoes=value;
            } 
        }

        public int VetoCount
        {
            get
            {
                return vetoes.Count;
            }
        }

        public IList<Like> Likes
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

        public int LikeCount
        {
            get
            {
                return likes.Count;
            }
        }

        public DateTime? StartedPlayingDateTime { get; set; }

        public long PausedDurationAsMilliseconds { get; set; }
        public bool IsPaused { get; set; }

        public string User { get; set; }

        /// <summary>
        /// Forgotten
        /// </summary>
        public bool Excluded { get; set; }

        /// <summary>
        /// This QueuedTrack should not be played when it gets
        /// dequeued. Could be that it has been vetoed, or 
        /// skipped by admin.
        /// </summary>
        public bool IsSkipped { get; set; }
    }
}
