using System;

namespace PlayMe.Common.Model
{
    public class SoundBoardInfo:DataObject
    {
        public int skippedSongsCount { get; set; }
        public DateTime lastSkippedSongTime { get; set; }
    }
}
