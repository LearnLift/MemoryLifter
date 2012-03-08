using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// The database connector for PgSqlSessionConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-09-05</remarks>
    interface IDbSessionConnector
    {
        int OpenUserSession(int lm_id);
        void RecalculateBoxSizes(int sessionId);
        void CardFromBoxDeleted(int sessionId, int boxId);
        void CardAdded(int sessionId);
        void CloseUserSession(int last_entry);
        IDictionary RestartLearningSuccess(int lm_id);
    }
}
