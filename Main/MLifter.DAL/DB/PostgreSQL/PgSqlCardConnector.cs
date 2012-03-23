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
using MLifter.DAL.Interfaces;
using Npgsql;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    /// <summary>
    /// The PostgreSQL Database-Connector for DbCard.
    /// </summary>
    /// <remarks>Documented by Dev05, 2008-07-25</remarks>
    public class PgSqlCardConnector : Interfaces.DB.IDbCardConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlCardConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlCardConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public static PgSqlCardConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlCardConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlCardConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbCardConnector Members

        /// <summary>
        /// Checks if the card exists and throws an IdAccessException if not.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void CheckCardId(int id)
        {
            List<int> cardIdsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardIdsList, 0)] as List<int>;
            if (cardIdsCache != null && cardIdsCache.Contains(id))
                return;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM \"Cards\" WHERE id IN " +
                        "(SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=(SELECT lm_id FROM \"LearningModules_Cards\" WHERE cards_id=:id))";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader;
                    try { reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser); }
                    catch { throw new IdAccessException(id); }

                    List<int> cardIds = new List<int>();
                    while (reader.Read())
                        cardIds.Add(Convert.ToInt32(reader["id"]));

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardIdsList, 0)] = cardIds;
                }
            }
        }

        /// <summary>
        /// Gets the chapter for a card.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int GetChapter(int id)
        {
            Dictionary<int, int> cardChaptersCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardChapterList, 0)] as Dictionary<int, int>;
            if (cardChaptersCache != null && cardChaptersCache.ContainsKey(id))
                return cardChaptersCache[id];

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"Chapters_Cards\" WHERE chapters_id IN " +
                        "(SELECT chapters_id FROM \"Chapters\" WHERE lm_id=(SELECT lm_id FROM \"LearningModules_Cards\" WHERE cards_id=:id))";
                    cmd.Parameters.Add("id", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                    Dictionary<int, int> cardChapters = new Dictionary<int, int>();

                    while (reader.Read())
                    {
                        int chid = Convert.ToInt32(reader["chapters_id"]);
                        int cid = Convert.ToInt32(reader["cards_id"]);
                        if (!cardChapters.ContainsKey(cid))
                            cardChapters.Add(cid, chid);
                    }
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardChapterList, 0)] = cardChapters;

                    return cardChapters[id];
                }
            }
        }
        /// <summary>
        /// Sets the chapter for a card.
        /// </summary>
        /// <param name="id">The card id.</param>
        /// <param name="chapter">The chapter id.</param>
        /// <remarks>Documented by Dev03, 2008-08-06</remarks>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetChapter(int id, int chapter)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                NpgsqlTransaction transaction = con.BeginTransaction();
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"Chapters\" WHERE id=:chapterid";
                    cmd.Parameters.Add("chapterid", chapter);
                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) < 1)
                        throw new IdAccessException(chapter);
                }

                Dictionary<int, int> cardChapterCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardChapterList, 0)] as Dictionary<int, int>;
                if (cardChapterCache != null)
                    cardChapterCache[id] = chapter;

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"Chapters_Cards\" WHERE cards_id=:id; ";
                    cmd.CommandText += "INSERT INTO \"Chapters_Cards\" (chapters_id, cards_id) VALUES (:chapterid, :id);";
                    cmd.CommandText += "UPDATE \"Cards\" SET chapters_id=:chapterid WHERE id=:id;";
                    cmd.Parameters.Add("chapterid", chapter);
                    cmd.Parameters.Add("id", id);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public ISettings GetSettings(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT settings_id FROM \"Cards\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);

                    int? sid = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser);

                    if (sid.HasValue)
                        return new DbSettings(sid.Value, false, Parent);
                    else
                        return null;
                }
            }
        }
        /// <summary>
        /// Sets the settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Settings">The settings.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetSettings(int id, ISettings Settings)
        {
            throw new NotAllowedInDbModeException();

            //using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            //{
            //    using (NpgsqlCommand cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "UPDATE \"Cards\" SET settings_id=:value WHERE id=:id";
            //        cmd.Parameters.Add("id", id);
            //        cmd.Parameters.Add("value", (Settings as DbSettings).Id);

            //        PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
            //    }
            //}
        }

        /// <summary>
        /// Gets the box.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public int GetBox(int id)
        {
            CardState? state = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?;
            if (state.HasValue)
                return state.Value.Box;

            ReadCardState(id);

            return (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?).Value.Box;
        }
        /// <summary>
        /// Sets the box.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Box">The box.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetBox(int id, int Box)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"SetCardState\"(:id, :card, :box, :active, :timestamp);";
                    cmd.Parameters.Add("id", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("card", id);
                    cmd.Parameters.Add("box", Box);
                    cmd.Parameters.Add("active", GetActive(id));
                    cmd.Parameters.Add("timestamp", GetTimestamp(id));

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id));
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.Score, Parent.GetParentDictionary().Id));
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.BoxSizes, Parent.CurrentUser.ConnectionString.LmId));
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CurrentBoxSizes, Parent.CurrentUser.ConnectionString.LmId));
                }
            }
        }

        /// <summary>
        /// Gets the active.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public bool GetActive(int id)
        {
            CardState? state = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?;
            if (state.HasValue)
                return state.Value.Active;

            ReadCardState(id);

            return (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?).Value.Active;
        }
        /// <summary>
        /// Sets the active.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Active">if set to <c>true</c> [active].</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetActive(int id, bool Active)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"SetCardState\"(:id, :card, :box, :active, :timestamp);";
                    cmd.Parameters.Add("id", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("card", id);
                    cmd.Parameters.Add("box", GetBox(id));
                    cmd.Parameters.Add("active", Active);
                    cmd.Parameters.Add("timestamp", GetTimestamp(id));

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id));
                }
            }
        }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public DateTime GetTimestamp(int id)
        {
            CardState? state = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?;
            if (state.HasValue)
                return state.Value.TimeStamp;

            ReadCardState(id);

            return (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id)] as CardState?).Value.TimeStamp;
        }
        /// <summary>
        /// Sets the timestamp.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Timestamp">The timestamp.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public void SetTimestamp(int id, DateTime Timestamp)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"SetCardState\"(:id, :card, :box, :active, :timestamp);";
                    cmd.Parameters.Add("id", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("card", id);
                    cmd.Parameters.Add("box", GetBox(id));
                    cmd.Parameters.Add("active", GetActive(id));
                    cmd.Parameters.Add("timestamp", Timestamp);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);

                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.CardState, id));
                }
            }
        }

        #endregion

        /// <summary>
        /// Reads the state of the card.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        private void ReadCardState(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT timestamp, box, active FROM \"GetCardState\"(:id, :card);";
                    cmd.Parameters.Add("id", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("card", id);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    reader.Read();
                    object ts = reader["timestamp"];
                    DateTime timestamp = ts is DBNull ? new DateTime(1901, 1, 1) : Convert.ToDateTime(ts);
                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.CardState, id)] = new CardState(Convert.ToInt32(reader["box"]),
                        Convert.ToBoolean(reader["active"]), timestamp);
                }
            }
        }
    }
}
