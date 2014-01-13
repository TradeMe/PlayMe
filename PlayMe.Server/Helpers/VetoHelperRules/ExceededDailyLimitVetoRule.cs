using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.VetoHelperRules.Interfaces;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server.Helpers.VetoHelperRules
{
    public class ExceededDailyLimitVetoRule : IVetoRule
    {
        private readonly IDataService<QueuedTrack> queuedTrackDataService;
        private readonly INowHelper nowHelper;
        private readonly IVetoHelperSettings vetoHelperSettings;

        public ExceededDailyLimitVetoRule(IDataService<QueuedTrack> queuedTrackDataService, IVetoHelperSettings vetoHelperSettings, INowHelper nowHelper)
        {
            this.vetoHelperSettings = vetoHelperSettings;
            this.nowHelper = nowHelper;
            this.queuedTrackDataService = queuedTrackDataService;
        }

        public bool CantVetoTrack(string vetoedByUser, QueuedTrack track)
        {
            int vetoCount = queuedTrackDataService.GetAll().Count(q => q.StartedPlayingDateTime > nowHelper.Now.Date && q.Vetoes.Any(v => v.ByUser == vetoedByUser));
            return vetoCount >= vetoHelperSettings.DailyVetoLimit;
        }
    }
}
