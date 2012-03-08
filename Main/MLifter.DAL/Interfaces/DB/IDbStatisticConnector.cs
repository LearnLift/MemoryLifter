using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// IDbStatistic
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-11-12</remarks>
    interface IDbStatisticConnector
    {
        int? GetWrongCards(int sessionId);

        int? GetCorrectCards(int sessionId);

        List<int> GetContentOfBoxes(int sessionId);

        DateTime? GetStartTimeStamp(int sessionId);

        DateTime? GetEndTimeStamp(int sessionId);
    }
}