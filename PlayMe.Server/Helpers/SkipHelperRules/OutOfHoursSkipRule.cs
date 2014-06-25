using System;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Server.Helpers.Interfaces;
using PlayMe.Server.Interfaces;

namespace PlayMe.Server.Helpers.SkipHelperRules
{
    public class OutOfHoursSkipRule : ISkipRule
    {
        private readonly INowHelper nowHelper;

        public OutOfHoursSkipRule(INowHelper nowHelper)
        {
            this.nowHelper = nowHelper;
        }

        public int GetRequiredVetoCount(QueuedTrack track)
        {
            const int requiredVetoCount = 1;
            return (IsOutOfHours()) ? requiredVetoCount : int.MaxValue;
        }
        private bool IsOutOfHours()
        {
            return nowHelper.Now.Hour >= 18
                   || nowHelper.Now.Hour < 8
                   || nowHelper.Now.DayOfWeek == DayOfWeek.Saturday
                   || nowHelper.Now.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}