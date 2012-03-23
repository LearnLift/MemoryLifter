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

using Npgsql;
using NpgsqlTypes;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlSessionConnector : IDbSessionConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlSessionConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlSessionConnector>();
        public static PgSqlSessionConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlSessionConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlSessionConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        /// <summary>
        /// Opens the user learning session.
        /// </summary>
        /// <param name="lm_id">The lm_id.</param>
        /// <remarks>Documented by Dev08, 2008-09-05</remarks>
        public int OpenUserSession(int lm_id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"StartLearningSession\"(:usrid, :lmid, :pool, :b1, :b2, :b3, :b4, :b5, :b6, :b7, :b8, :b9, :b10)";

                    cmd.Parameters.Add("usrid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lmid", lm_id);

                    int counter = 0;
                    int cardsInBoxes = 0;
                    BoxSizes boxContent = GetCurrentBoxContent();
                    foreach (int box in boxContent.Sizes)
                    {
                        if (counter == 0)
                        {
                            cmd.Parameters.Add("pool", box);
                            ++counter;
                            continue;
                        }

                        cmd.Parameters.Add("b" + Convert.ToString(counter++), box);
                        cardsInBoxes += box;
                    }

                    int newSessionId = PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser).Value;

                    //Following Statement does add the "RunningSession" = true to the current Statistic.
                    PgSqlStatisticConnector connector = PgSqlStatisticConnector.GetInstance(Parent);
                    connector.RunningSession = newSessionId;

                    return newSessionId;
                }
            }
        }

        public void RecalculateBoxSizes(int sessionId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningSessions\" SET pool_content=:pool, box1_content=:b1, box2_content=:b2, box3_content=:b3, " +
                                      "box4_content=:b4, box5_content=:b5, box6_content=:b6, box7_content=:b7, box8_content=:b8, box9_content=:b9, box10_content=:b10 WHERE id=:sid AND user_id=:uid AND lm_id=:lmid";
                    cmd.Parameters.Add("sid", sessionId);
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lmid", Parent.GetParentDictionary().Id);

                    int counter = 0;
                    int cardsInBoxes = 0;
                    BoxSizes boxContent = GetCurrentBoxContent();
                    foreach (int box in boxContent.Sizes)
                    {
                        if (counter == 0)
                        {
                            cmd.Parameters.Add("pool", box);
                            ++counter;
                            continue;
                        }

                        cmd.Parameters.Add("b" + Convert.ToString(counter++), box);
                        cardsInBoxes += box;
                    }

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public void CardFromBoxDeleted(int sessionId, int boxId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE \"LearningSessions\" SET {0}_content={0}_content - 1 WHERE id=:sid AND user_id=:uid AND lm_id=:lmid", boxId > 0 ? "box" + boxId.ToString() : "pool");
                    cmd.Parameters.Add("sid", sessionId);
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lmid", Parent.GetParentDictionary().Id);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public void CardAdded(int sessionId)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningSessions\" SET pool_content=pool_content + 1 WHERE id=:sid AND user_id=:uid AND lm_id=:lmid";
                    cmd.Parameters.Add("sid", sessionId);
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lmid", Parent.GetParentDictionary().Id);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        private BoxSizes GetCurrentBoxContent()
        {
            BoxSizes sizes;
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    sizes = new BoxSizes(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    cmd.CommandText = @"SELECT CS.box AS box, count(*) AS count FROM ""UserCardState"" CS
                                        INNER JOIN ""Cards"" C ON C.id = CS.cards_id AND C.lm_id=:lm_id
                                        WHERE CS.active = TRUE AND CS.user_id = :user_id  
                                      GROUP BY CS.box";
                    cmd.Parameters.Add("user_id", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", Parent.CurrentUser.ConnectionString.LmId);

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    while (reader.Read())
                        sizes.Sizes[Convert.ToInt32(reader["box"])] = Convert.ToInt32(reader["count"]);
                }
            }
            return sizes;
        }

        /// <summary>
        /// Closes the user learning session.
        /// </summary>
        /// <param name="last_entry">The last_entry.</param>
        /// <remarks>Documented by Dev08, 2008-09-05</remarks>
        public void CloseUserSession(int last_entry)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"LearningSessions\" SET endtime=CURRENT_TIMESTAMP WHERE id=:id";
                    cmd.Parameters.Add("id", last_entry);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser, false);

                    //Following Statement does add the "RunningSession" = false to the current Statistic.
                    PgSqlStatisticConnector connector = PgSqlStatisticConnector.GetInstance(Parent);
                    connector.RunningSession = -1;
                }
            }
        }

        /// <summary>
        /// Restarts the learning success.
        /// </summary>
        /// <param name="lm_id">The lm_id.</param>
        /// <remarks>Documented by Dev08, 2008-11-19</remarks>
        public IDictionary RestartLearningSuccess(int lm_id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"LearningSessions\" WHERE lm_id = :lmId AND user_id = :userId;";
                    cmd.Parameters.Add("lmId", lm_id);
                    cmd.Parameters.Add("userId", Parent.CurrentUser.Id);

                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser, false);

                    return Parent.GetParentDictionary();
                }
            }
        }
    }
}
