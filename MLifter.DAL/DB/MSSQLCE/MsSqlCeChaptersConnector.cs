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
using System.Data.SqlServerCe;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// MsSqlCeChaptersConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-12</remarks>
    class MsSqlCeChaptersConnector : IDbChaptersConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeChaptersConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeChaptersConnector>();
        public static MsSqlCeChaptersConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeChaptersConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass parent;
        private MsSqlCeChaptersConnector(ParentClass parentClass)
        {
            parent = parentClass;
            parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
        }

        void parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbChaptersConnector Members

        /// <summary>
        /// Gets the chapter ids.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public IList<int> GetChapterIds(int lmid)
        {
            IList<int> chaptersCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid)] as IList<int>;
            if (chaptersCache != null)
                return chaptersCache;

            IList<int> list = new List<int>();

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT id FROM \"Chapters\" WHERE lm_id=@lmid ORDER BY position ASC";
                cmd.Parameters.Add("@lmid", lmid);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                while (reader.Read())
                    list.Add(Convert.ToInt32(reader["id"]));
                reader.Close();
            }

            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.ChaptersList, lmid)] = list;
            return list;
        }

        /// <summary>
        /// Adds the new chapter.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public int AddNewChapter(int lmid)
        {
            int position = 0;
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT position FROM \"Chapters\" WHERE lm_id=@lmid ORDER BY position DESC";
                cmd.Parameters.Add("@lmid", lmid);
                int? pos = MSSQLCEConn.ExecuteScalar<int>(cmd);
                position = pos.HasValue ? pos.Value + 10 : 10;
            }
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "INSERT INTO \"Chapters\" (lm_id, position, settings_id) VALUES (@lmid, @pos, @sid); SELECT @@IDENTITY;";
                cmd.Parameters.Add("@lmid", lmid);
                cmd.Parameters.Add("@pos", position);
                cmd.Parameters.Add("@sid", MsSqlCeSettingsConnector.CreateNewSettings(parent));
                int chapterId = Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
                parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid));
                return chapterId;
            }
        }

        /// <summary>
        /// Deletes the chapter.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <param name="id">The id.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void DeleteChapter(int lmid, int id)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "DELETE FROM LearningModules_Cards WHERE cards_id IN (SELECT cards_id FROM Chapters_Cards WHERE chapters_id=@id); ";
                cmd.CommandText += "DELETE FROM Cards_MediaContent WHERE cards_id IN (SELECT cards_id FROM Chapters_Cards WHERE chapters_id=@id); ";
                cmd.CommandText += "DELETE FROM UserCardState WHERE cards_id IN (SELECT cards_id FROM Chapters_Cards WHERE chapters_id=@id); ";
                cmd.CommandText += "DELETE FROM TextContent WHERE cards_id IN (SELECT cards_id FROM Chapters_Cards WHERE chapters_id=@id); ";
                cmd.CommandText += "DELETE FROM LearnLog WHERE cards_id IN (SELECT cards_id FROM Chapters_Cards WHERE chapters_id=@id); ";
                cmd.CommandText += "DELETE FROM Chapters_Cards WHERE chapters_id=@id; ";
                cmd.CommandText += "DELETE FROM SelectedLearnChapters WHERE chapters_id=@id; ";
                cmd.CommandText += "DELETE FROM Cards_MediaContent WHERE cards_id NOT IN (SELECT cards_id FROM LearningModules_Cards); ";
                cmd.CommandText += "DELETE FROM UserCardState WHERE cards_id NOT IN (SELECT cards_id FROM LearningModules_Cards); ";
                cmd.CommandText += "DELETE FROM TextContent WHERE cards_id NOT IN (SELECT cards_id FROM LearningModules_Cards); ";
                cmd.CommandText += "DELETE FROM LearnLog WHERE cards_id NOT IN (SELECT cards_id FROM LearningModules_Cards); ";
                cmd.CommandText += "DELETE FROM Cards WHERE id NOT IN (SELECT cards_id FROM LearningModules_Cards); ";
                cmd.CommandText += "DELETE FROM Chapters WHERE id=@id";
                cmd.Parameters.Add("@id", id);
                if (MSSQLCEConn.ExecuteNonQuery(cmd) < 1)
                    throw new IdAccessException(id);

                parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid));
            }
        }

        /// <summary>
        /// Finds the chapter.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public int FindChapter(int lmid, string title)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT id FROM \"Chapters\" WHERE title LIKE @title AND lm_id=@lmid";
                cmd.Parameters.Add("@title", title);
                cmd.Parameters.Add("@lmid", lmid);
                object result = MSSQLCEConn.ExecuteScalar(cmd);
                if (result == null)
                    return -1;
                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// Moves the chapter.
        /// How does this function work (by FabThe, who always forgot how it works):
        /// If the "FirstChapter.Position" is smaller than the "SecondChapter.Position" the new position of "FirstChapter" will be "SecondChapter.Position" + 5
        /// If the "FirstChapter.Position" is greater than the "SecondChapter.Position" the new position of "FirstChapter" will be "SecondChapter.Position" - 5
        /// Summary: If the first chapter is on the top and the second chapter in the middle, the new position of the first chapter will be after the second chapter.
        /// If the first chapter is in the middle and the second chapter on the top, the new position of the first chapter will be before the second chapter.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <param name="first_id">The id of the first chapter.</param>
        /// <param name="second_id">The id of the second chapter..</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void MoveChapter(int lmid, int first_id, int second_id)
        {
            int position1, position2;
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                SqlCeTransaction transaction = cmd.Connection.BeginTransaction();

                cmd.CommandText = "SELECT position FROM \"Chapters\" WHERE id=@id AND lm_id=@lmid";
                cmd.Parameters.Add("@id", first_id);
                cmd.Parameters.Add("@lmid", lmid);
                object result = MSSQLCEConn.ExecuteScalar(cmd);
                if (result == null)
                    throw new IdAccessException(first_id);
                position1 = Convert.ToInt32(result);

                using (SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                {
                    cmd2.CommandText = "SELECT position FROM \"Chapters\" WHERE id=@id AND lm_id=@lmid";
                    cmd2.Parameters.Add("@id", second_id);
                    cmd2.Parameters.Add("@lmid", lmid);
                    object result2 = MSSQLCEConn.ExecuteScalar(cmd2);
                    if (result2 == null)
                        throw new IdAccessException(second_id);
                    position2 = Convert.ToInt32(result2);
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

                int serialchapters = 10, increment = 10;
                using (SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                {
                    cmd3.CommandText = "UPDATE \"Chapters\" SET position=@position WHERE id=@id; ";
                    cmd3.CommandText += "SELECT * FROM \"Chapters\" WHERE lm_id=@lmid ORDER BY position ASC";
                    cmd3.Parameters.Add("@position", newfirstposition);
                    cmd3.Parameters.Add("@id", first_id);
                    cmd3.Parameters.Add("@lmid", lmid);
                    SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd3);
                    while (reader.Read())
                    {
                        using (SqlCeCommand cmd4 = MSSQLCEConn.CreateCommand(parent.CurrentUser))
                        {
                            cmd4.CommandText = "UPDATE \"Chapters\" SET position=@position WHERE \"Chapters\".id=@chapterId";
                            cmd4.Parameters.Add("@position", serialchapters);
                            cmd4.Parameters.Add("@chapterId", reader["id"]);
                            MSSQLCEConn.ExecuteNonQuery(cmd4);

                            serialchapters += increment;
                        }
                    }
                    reader.Close();
                }

                transaction.Commit();
            }

            parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid));
        }

        /// <summary>
        /// Gets the chapter count in the specified learning module.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public int GetChapterCount(int lmid)
        {
            IList<int> chaptersCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ChaptersList, lmid)] as IList<int>;
            if (chaptersCache != null)
                return chaptersCache.Count;

            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"Chapters\" WHERE lm_id=@lmid";
                cmd.Parameters.Add("@lmid", lmid);
                return Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// Checks the id of the learning module for correctness.
        /// </summary>
        /// <param name="lmid">The id of the learning module.</param>
        /// <remarks>Documented by Dev02, 2008-08-05</remarks>
        /// <remarks>Documented by Dev08, 2009-01-12</remarks>
        public void CheckLMId(int lmid)
        {
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser))
            {
                cmd.CommandText = "SELECT count(*) FROM \"LearningModules\" WHERE id=@lmid";
                cmd.Parameters.Add("@lmid", lmid);
                if (Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd)) != 1)
                    throw new IdAccessException(lmid);
            }
        }

        #endregion
    }
}
