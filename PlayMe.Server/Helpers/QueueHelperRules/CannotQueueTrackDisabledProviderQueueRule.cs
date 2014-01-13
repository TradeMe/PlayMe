using PlayMe.Common.Model;
using PlayMe.Server.Helpers.QueueHelperRules.Interfaces;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server.Helpers.QueueHelperRules
{
    public class CannotQueueTrackDisabledProviderQueueRule : IQueueRule
    {
        private readonly IMusicPlayer musicPlayer;
        public CannotQueueTrackDisabledProviderQueueRule(IMusicPlayer musicPlayer)
        {
            this.musicPlayer = musicPlayer;
        }

        public string CannotQueue(Track track, string user)
        {
            if (track == null)
            {
                return "Track can no longer be found. Its music provider might be disabled.";
            }

            return string.Empty;
        }
    }
}
