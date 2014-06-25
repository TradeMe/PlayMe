namespace PlayMe.Common.Model
{
    public class TrackScore : DataObject
    {
        public int Score { get; set; }
        public bool IsExcluded { get; set; }
        public double MillisecondsSinceLastPlay { get; set; }
        public Track Track { get; set; }
    }
}
