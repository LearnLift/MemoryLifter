using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// Database interface for DbCard.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-25</remarks>
    interface IDbCardConnector
    {
        /// <summary>
        /// Checks if the card exists and throws an IdAccessException if not.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        void CheckCardId(int id);
        /// <summary>
        /// Gets the chapter for a card.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        int GetChapter(int id);
        /// <summary>
        /// Sets the chapter for a card.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <param name="chapter">The chapter id.</param>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        void SetChapter(int id, int chapter);

        ISettings GetSettings(int id);
        void SetSettings(int id, ISettings Settings);

        int GetBox(int id);
        void SetBox(int id, int Box);

        bool GetActive(int id);
        void SetActive(int id, bool Active);

        DateTime GetTimestamp(int id);
        void SetTimestamp(int id, DateTime Timestamp);
    }
}
