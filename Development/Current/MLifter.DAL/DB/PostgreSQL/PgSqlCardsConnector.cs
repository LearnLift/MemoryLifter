/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    /// <summary>
    /// The PostgreSQL Database-Connector for DbCards.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-28</remarks>
    class PgSqlCardsConnector : IDbCardsConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlCardsConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlCardsConnector>();
        public static PgSqlCardsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlCardsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlCardsConnector(ParentClass parentClass)
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

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:id)";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader;
                    try { reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser); }
                    catch { throw new IdAccessException(id); }

                    List<ICard> cards = new List<ICard>();
                    while (reader.Read())
                        cards.Add(new DbCard(Convert.ToInt32(reader["id"]), false, Parent));
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardsList, id)] = cards;

                    return cards;
                }
            }
        }

        public int GetCardsCount(int id)
        {
            List<ICard> cardsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, id)] as List<ICard>;
            if (cardsCache != null)
                return cardsCache.Count;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"LearningModules_Cards\" WHERE lm_id=:id";
                    cmd.Parameters.Add("id", id);
                    return Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }

        public ICard GetNewCard(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"Cards\" VALUES (default) RETURNING id;";
                    ICard card = new DbCard(Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)), false, Parent);

                    List<ICard> cardsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, id)] as List<ICard>;
                    if (cardsCache != null)
                        cardsCache.Add(card);

                    return card;
                }
            }
        }

        public void SetCardLearningModule(int LmId, int CardId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"LearningModules_Cards\" WHERE cards_id=:cardid; ";
                    cmd.CommandText += "INSERT INTO \"LearningModules_Cards\" (lm_id, cards_id) VALUES (:lmid, :cardid);";
                    cmd.CommandText += "UPDATE \"Cards\" SET lm_id=:lmid WHERE id=:cardid;";
                    cmd.Parameters.Add("cardid", CardId);
                    cmd.Parameters.Add("lmid", LmId);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, LmId));
                }
            }
        }

        public void DeleteCard(int id, int lmid)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"Cards\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardsList, lmid));
                }
            }
        }

        public List<ICard> GetCardsByQuery(int id, QueryStruct[] query, QueryOrder orderBy, QueryOrderDir orderDir, int number)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    //create entries in UserCardState for all cards in lm
                    cmd.CommandText = "SELECT \"FillUpUserCardState\"(:userid, :lmid)";
                    cmd.Parameters.Add("userid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lmid", id);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"Cards\".id as cid FROM \"Cards\", \"UserCardState\"";

                    List<string> conditions = new List<string>();
                    foreach (QueryStruct q in query)
                    {
                        string cond = " \"Cards\".lm_id=:lmid AND \"Cards\".id=\"UserCardState\".cards_id AND \"UserCardState\".user_id=:uid";
                        if (q.ChapterId != -1)
                            cond += string.Format(" AND \"Cards\".chapters_id={0}", q.ChapterId);
                        if (q.BoxId != -1)
                            cond += string.Format(" AND \"UserCardState\".box={0}", q.BoxId);
                        switch (q.CardState)
                        {
                            case QueryCardState.Active:
                                cond += " AND \"UserCardState\".active=true";
                                break;
                            case QueryCardState.Inactive:
                                cond += " AND \"UserCardState\".active=false";
                                break;
                            default:
                                break;
                        }

                        conditions.Add(cond);
                    }
                    if (conditions.Count > 0)
                    {
                        cmd.CommandText += " WHERE " + conditions[0];
                        conditions.RemoveAt(0);
                        foreach (string cond in conditions)
                            cmd.CommandText += " OR " + cond;
                    }
                    else
                        cmd.CommandText += " WHERE \"Cards\".id=\"UserCardState\".cards_id AND \"UserCardState\".user_id=:uid AND \"Cards\".lm_id=:lmid";

                    switch (orderBy)
                    {
                        case QueryOrder.Id:
                            cmd.CommandText += " ORDER BY \"Cards\".id";
                            break;
                        case QueryOrder.Chapter:
                            cmd.CommandText += " ORDER BY \"Cards\".chapter_id";
                            break;
                        case QueryOrder.Random:
                            cmd.CommandText += " ORDER BY random()";
                            break;
                        case QueryOrder.Timestamp:
                            cmd.CommandText += " ORDER BY \"UserCardState\".timestamp";
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
                        cmd.CommandText += ", \"Cards\".id " + (orderDir == QueryOrderDir.Ascending ? "ASC" : "DESC");

                    cmd.CommandText = string.Format("SELECT cid FROM ({0}) as cards {1}",
                        cmd.CommandText, number > 0 ? "LIMIT " + number : string.Empty);
                    cmd.Parameters.Add("lmid", id);
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    List<ICard> cards = new List<ICard>();
                    while (reader.Read())
                        cards.Add(new DbCard(Convert.ToInt32(reader["cid"]), false, Parent));
                    return cards;
                }
            }
        }

        public void ClearAllBoxes(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"UserCardState\" SET box=0, active=true WHERE user_id=:userid AND cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:lmid);";
                    cmd.Parameters.Add("lmid", id);
                    cmd.Parameters.Add("userid", Parent.CurrentUser.Id);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        #endregion
    }
}
