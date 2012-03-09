using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    class MsSqlCeCardsConnector : IDbCardsConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeCardsConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeCardsConnector>();
        public static MsSqlCeCardsConnector GetInstance(ParentClass parentClass)
        {
            ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

            if (!instances.ContainsKey(connection))
                instances.Add(connection, new MsSqlCeCardsConnector(parentClass));

            return instances[connection];
        }

        private ParentClass Parent;
        private MsSqlCeCardsConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbCardsConnector Members

        public List<ICard> GetCards(int id)
        {
            List<ICard> cardsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, id)] as List<ICard>;
            if (cardsCache != null)
                return cardsCache;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=@id";
                cmd.Parameters.Add("@id", id);
                SqlCeDataReader reader;
                try { reader = MSSQLCEConn.ExecuteReader(cmd); }
                catch { throw new IdAccessException(id); }

                List<ICard> cards = new List<ICard>();
                while (reader.Read())
                    cards.Add(new DbCard(Convert.ToInt32(reader["cards_id"]), false, Parent));
                reader.Close();
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardsList, id)] = cards;

                return cards;
            }
        }

        public int GetCardsCount(int id)
        {
            List<ICard> cardsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, id)] as List<ICard>;
            if (cardsCache != null)
                return cardsCache.Count;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"LearningModules_Cards\" WHERE lm_id=@id";
                cmd.Parameters.Add("@id", id);
                return Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
            }
        }

        public ICard GetNewCard(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "INSERT INTO \"Cards\"(settings_id) VALUES (@sid); SELECT @@IDENTITY;";
                cmd.Parameters.Add("@sid", MsSqlCeSettingsConnector.CreateNewSettings(Parent));
                ICard card = new DbCard(Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)), false, Parent);

                List<ICard> cardsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, id)] as List<ICard>;
                if (cardsCache != null)
                    cardsCache.Add(card);

                return card;
            }
        }

        public void SetCardLearningModule(int LmId, int CardId)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "DELETE FROM LearningModules_Cards WHERE cards_id=@cardid; ";
                cmd.CommandText += "INSERT INTO LearningModules_Cards (lm_id, cards_id) VALUES (@lmid, @cardid);";
                cmd.CommandText += "UPDATE Cards SET lm_id=@lmid WHERE id=@cardid;";
                cmd.Parameters.Add("@cardid", CardId);
                cmd.Parameters.Add("@lmid", LmId);
                MSSQLCEConn.ExecuteNonQuery(cmd);
                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, LmId));
            }
        }

        /// <summary>
        /// Deletes the card. Following table-entries have to be deleted:
        /// LearningModules_Cards, Chapters_Cards, TextContent, Cards_MediaContent, LearnLog, UserCardState
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="lmid">The lmid.</param>
        /// <remarks>Documented by Dev08, 2009-01-20</remarks>
        public void DeleteCard(int id, int lmid)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = " DELETE FROM LearningModules_Cards WHERE cards_id=@cid; ";
                cmd.CommandText += "DELETE FROM Chapters_Cards WHERE cards_id=@cid; ";
                cmd.CommandText += "DELETE FROM TextContent WHERE cards_id=@cid; ";
                cmd.CommandText += "DELETE FROM Cards_MediaContent WHERE cards_id=@cid; ";
                cmd.CommandText += "DELETE FROM LearnLog WHERE cards_id=@cid; ";
                cmd.CommandText += "DELETE FROM UserCardState WHERE cards_id=@cid; ";
                cmd.CommandText += "DELETE FROM \"Cards\" WHERE id=@cid; ";
                cmd.Parameters.Add("@cid", id);
                MSSQLCEConn.ExecuteNonQuery(cmd);
                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, lmid));
            }
        }

        private int cardStateCount = -1;
        /// <summary>
        /// Gets the cards by query.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="query">The query.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="orderDir">The order dir.</param>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-19</remarks>
        public List<ICard> GetCardsByQuery(int id, QueryStruct[] query, QueryOrder orderBy, QueryOrderDir orderDir, int number)
        {
            List<int> ids = new List<int>();
            if (cardStateCount < 0)
            {
                int cardCount;
                using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
                {
                    //create entries in UserCardState for all cards in lm
                    cmd.CommandText = "SELECT count(*) FROM \"LearningModules_Cards\" WHERE lm_id=@lmid";
                    cmd.Parameters.Add("@lmid", id);
                    cardCount = MSSQLCEConn.ExecuteScalar<int>(cmd).Value;
                }
                using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
                {
                    //create entries in UserCardState for all cards in lm
                    cmd.CommandText = "SELECT count(*) FROM UserCardState CS INNER JOIN Cards C ON C.ID = CS.cards_id WHERE CS.user_id=@userid AND C.lm_id=@lmid";
                    cmd.Parameters.Add("@userid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("@lmid", id);
                    cardStateCount = MSSQLCEConn.ExecuteScalar<int>(cmd).Value;
                }
                if (cardCount != cardStateCount)
                {
                    using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
                    {
                        cmd.CommandText = "INSERT INTO UserCardState (user_id, cards_id, box, active) " +
                            String.Format("SELECT {0}, C.id, 0, 1 FROM Cards C LEFT OUTER JOIN (SELECT * FROM UserCardState WHERE user_id = {0}) CS ON C.ID = CS.cards_id WHERE CS.cards_id IS NULL AND C.lm_id=@lmid", Parent.CurrentUser.Id);
                        cmd.Parameters.Add("@lmid", id);
                        MSSQLCEConn.ExecuteNonQuery(cmd);
                    }
                }
            }

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = string.Format("SELECT {0}C.id as cid FROM Cards C INNER JOIN UserCardState CS ON C.id=CS.cards_id", number > 0 ? "TOP (" + number + ") " : string.Empty);

                List<string> conditions = new List<string>();
                foreach (QueryStruct q in query)
                {
                    string cond = "CS.user_id=@uid";
                    if (q.ChapterId != -1)
                        cond += string.Format(" AND C.chapters_id={0}", q.ChapterId);
                    if (q.BoxId != -1)
                        cond += string.Format(" AND CS.box={0}", q.BoxId);
                    switch (q.CardState)
                    {
                        case QueryCardState.Active:
                            cond += " AND CS.active=1";
                            break;
                        case QueryCardState.Inactive:
                            cond += " AND CS.active=0";
                            break;
                        default:
                            break;
                    }

                    conditions.Add(cond);
                }
                cmd.CommandText += " WHERE C.lm_id=@lmid AND (";
                if (conditions.Count > 0)
                {
                    cmd.CommandText += " " + conditions[0];
                    conditions.RemoveAt(0);
                    foreach (string cond in conditions)
                        cmd.CommandText += " OR " + cond;
                }
                else
                    cmd.CommandText += " 1=1";
                cmd.CommandText += ")";

                switch (orderBy)
                {
                    case QueryOrder.Id:
                        cmd.CommandText += " ORDER BY C.id";
                        break;
                    case QueryOrder.Chapter:
                        cmd.CommandText += " ORDER BY C.chapters_id";
                        break;
                    case QueryOrder.Random:
                        cmd.CommandText += " ORDER BY newid()";
                        break;
                    case QueryOrder.Timestamp:
                        cmd.CommandText += " ORDER BY CS.timestamp";
                        break;
                    default:
                        break;
                }
                if (orderBy == QueryOrder.Chapter || orderBy == QueryOrder.Id)
                    switch (orderDir)
                    {
                        case QueryOrderDir.Ascending:
                            cmd.CommandText += " ASC";
                            break;
                        case QueryOrderDir.Descending:
                            cmd.CommandText += " DESC";
                            break;
                    }
                if (orderBy == QueryOrder.Timestamp)
                    cmd.CommandText += ", C.id " + (orderDir == QueryOrderDir.Ascending ? "ASC" : "DESC");

                cmd.Parameters.Add("@lmid", id);
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                List<ICard> cards = new List<ICard>();
                //Performance: 180ms!!!
                while (reader.Read())
                    cards.Add(new DbCard(Convert.ToInt32(reader["cid"]), false, Parent));
                reader.Close();

                return cards;
            }
        }

        public void ClearAllBoxes(int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                cmd.CommandText = "UPDATE \"UserCardState\" SET box=0, active=1 WHERE user_id=@userid " +
                    "AND cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=@lmid);";
                cmd.Parameters.Add("@lmid", id);
                cmd.Parameters.Add("@userid", Parent.CurrentUser.Id);
                MSSQLCEConn.ExecuteNonQuery(cmd);
            }
        }

        #endregion
    }
}
