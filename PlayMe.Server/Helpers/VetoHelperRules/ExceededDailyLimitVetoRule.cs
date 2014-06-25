using System.Linq;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Helpers.VetoHelperRules.Interfaces;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server.Helpers.VetoHelperRules
{
    public class ExceededDailyLimitVetoRule : IVetoRule
    {
        private readonly IRepository<QueuedTrack> _queuedTrackRepository;
        private readonly INowHelper nowHelper;
        private readonly IVetoHelperSettings vetoHelperSettings;

        public ExceededDailyLimitVetoRule(IRepository<QueuedTrack> _queuedTrackRepository, IVetoHelperSettings vetoHelperSettings, INowHelper nowHelper)
        {
            this.vetoHelperSettings = vetoHelperSettings;
            this.nowHelper = nowHelper;
            this._queuedTrackRepository = _queuedTrackRepository;
        }

        public bool CantVetoTrack(string vetoedByUser, QueuedTrack track)
        {
            int vetoCount = _queuedTrackRepository.GetAll().Count(q => q.StartedPlayingDateTime > nowHelper.Now.Date && q.Vetoes.Any(v => v.ByUser == vetoedByUser));
            return vetoCount >= vetoHelperSettings.DailyVetoLimit;
        }
    }
}
