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
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlChaptersConnector : Interfaces.DB.IDbChaptersConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlChaptersConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlChaptersConnector>();
        public static PgSqlChaptersConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlChaptersConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlChaptersConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);            
        }

        #region IDbChaptersConnector Members

        public IList<int> GetChapterIds(int lmid)
        {
            IList<int> chaptersCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid)] as IList<int>;
            if (chaptersCache != null)
                return chaptersCache;

            IList<int> list = new List<int>();

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM \"Chapters\" WHERE lm_id=:lmid ORDER BY position ASC";
                    cmd.Parameters.Add("lmid", lmid);
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);
                    while (reader.Read())
                        list.Add(Convert.ToInt32(reader["id"]));
                }
            }

            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChaptersList, lmid)] = list;
            return list;
        }

        public int AddNewChapter(int lmid)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"Chapters\" (lm_id, position) VALUES (:lmid, COALESCE((SELECT position FROM \"Chapters\" WHERE lm_id=:lmid ORDER BY position DESC LIMIT 1), 0) + 10) RETURNING id; ";
                    cmd.Parameters.Add("lmid", lmid);
                    int chapterId = Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid));
                    return chapterId;
                }
            }
        }

        public void DeleteChapter(int lmid, int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM \"Cards\" WHERE \"Cards\".id IN (SELECT \"Chapters_Cards\".cards_id FROM \"Chapters_Cards\" WHERE chapters_id=:id); ";
                    cmd.CommandText += "DELETE FROM \"Chapters\" WHERE id=:id";
                    cmd.Parameters.Add("id", id);
                    int rowcount = PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser); //ToDo the old version of postgres did not return affected rows for deletes
                    if (rowcount < 1)
                        throw new IdAccessException(id);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid));
                }
            }
        }

        public int FindChapter(int lmid, string title)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM \"Chapters\" WHERE title=:title AND lm_id=:lmid";
                    cmd.Parameters.Add("title", title);
                    cmd.Parameters.Add("lmid", lmid);
                    object result = PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser);
                    if (result == null)
                        return -1;
                    return Convert.ToInt32(result);
                }
            }
        }

        public void MoveChapter(int lmid, int first_id, int second_id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                NpgsqlTransaction transaction = con.BeginTransaction();

                int position1, position2;

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT position FROM \"Chapters\" WHERE id=:id AND lm_id=:lmid";
                    cmd.Parameters.Add("id", first_id);
                    cmd.Parameters.Add("lmid", lmid);
                    object result = PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser);
                    if (result == null)
                        throw new IdAccessException(first_id);
                    position1 = Convert.ToInt32(result);
                }

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT position FROM \"Chapters\" WHERE id=:id AND lm_id=:lmid";
                    cmd.Parameters.Add("id", second_id);
                    cmd.Parameters.Add("lmid", lmid);
                    object result = PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser);
                    if (result == null)
                        throw new IdAccessException(second_id);
                    position2 = Convert.ToInt32(result);
                }

                int newfirstposition;

                if (position1 < position2)
                    newfirstposition = position2 + 5; //insert first after second
                else if (position1 > position2)
                    newfirstposition = position2 - 5; //insert first before second
                else
                {
                    //positions were equal => no reordering possible
                    return;
                }

                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE \"Chapters\" SET position=:position WHERE id=:id; ";
                    cmd.CommandText += "SELECT \"RedistributeChapterPositions\"(:lmid)";
                    cmd.Parameters.Add("position", newfirstposition);
                    cmd.Parameters.Add("id", first_id);
                    cmd.Parameters.Add("lmid", lmid);
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }

                transaction.Commit();
                Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid));
            }
        }

        /// <summary>
        /// Redistributes the chapter positions in steps of 10, preserving the current position order.
        /// </summary>
        /// <param name="con">The con.</param>
        /// <param name="lmid">The lmid.</param>
        /// <remarks>Documented by Dev02, 2008-07-28</remarks>
        [Obsolete("Use the stored procedure RedistributeChapterPositions(lmid) instead!")]
        private void RedistributeChapterPositions(NpgsqlConnection con, int lmid)
        {
            Queue<int> ids = new Queue<int>();

            using (NpgsqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM \"Chapters\" WHERE lm_id=:lmid ORDER BY position ASC";
                cmd.Parameters.Add("lmid", lmid);
                NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser);

                while (reader.Read())
                    ids.Enqueue(Convert.ToInt32(reader["id"]));
            }

            if (ids.Count > 0)
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    int position = 0;

                    int positionparamnum = 0;
                    string positionparamname;

                    int idparamnum = 0;
                    string idparamname;

                    while (ids.Count > 0)
                    {
                        int id = ids.Dequeue();
                        position += 10;

                        positionparamname = string.Format("position{0}", positionparamnum++);
                        idparamname = string.Format("id{0}", idparamnum++);

                        cmd.CommandText += string.Format("UPDATE \"Chapters\" SET position=:{0} WHERE id=:{1}; ", positionparamname, idparamname);
                        cmd.Parameters.Add(positionparamname, position);
                        cmd.Parameters.Add(idparamname, id);
                    }
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                }
            }
        }

        public int GetChapterCount(int lmid)
        {
            IList<int> chaptersCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid)] as IList<int>;
            if (chaptersCache != null)
                return chaptersCache.Count;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"Chapters\" WHERE lm_id=:lmid";
                    cmd.Parameters.Add("lmid", lmid);
                    return Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser));
                }
            }
        }

        public void CheckLMId(int lmid)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM \"LearningModules\" WHERE id=:lmid";
                    cmd.Parameters.Add("lmid", lmid);
                    if (Convert.ToInt32(PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser)) != 1)
                        throw new IdAccessException(lmid);
                }
            }
        }

        #endregion
    }
}
