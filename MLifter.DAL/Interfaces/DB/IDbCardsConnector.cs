using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    /// <summary>
    /// The database interface for DbCards.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-28</remarks>
    interface IDbCardsConnector
    {
        List<ICard> GetCards(int id);
        int GetCardsCount(int id);
        ICard GetNewCard(int id);
        void SetCardLearningModule(int LmId, int CardId);
        void DeleteCard(int id, int lmid);
        List<ICard> GetCardsByQuery(int id, QueryStruct[] query, QueryOrder orderBy, QueryOrderDir orderDir, int number);
        void ClearAllBoxes(int id);
    }
}
