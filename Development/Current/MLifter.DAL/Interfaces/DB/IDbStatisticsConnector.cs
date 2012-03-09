using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbStatisticsConnector
    {
        List<int> GetLearnSessions(int lmId);

        void CopyStatistics(int lmId, IStatistic statistic);
    }
}
