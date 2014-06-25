using System;

namespace PlayMe.Data.NHibernate.Entities
{
    public class SoundBoardInfo:DataObject
    {
        public int skippedSongsCount { get; set; }
        public DateTime lastSkippedSongTime { get; set; }
    }
}
