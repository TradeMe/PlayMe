using PlayMe.Common.Model;
using PlayMe.Server.Helpers.QueueHelperRules.Interfaces;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server.Helpers.QueueHelperRules
{
    public class CannotQueueTrackAlreadyPlayingQueueRule : IQueueRule
    {
        private readonly IMusicPlayer musicPlayer;
        public CannotQueueTrackAlreadyPlayingQueueRule(IMusicPlayer musicPlayer)
        {
            this.musicPlayer = musicPlayer;
        }

        public string CannotQueue(Track track, string user)
        {
            if (track == null) return string.Empty;

            return musicPlayer.CurrentlyPlayingTrack != null &&
                   musicPlayer.CurrentlyPlayingTrack.Track.Name == track.Name
                       ? "Cannot queue this track as it is already playing."
                       : string.Empty;
        }
    }
}
