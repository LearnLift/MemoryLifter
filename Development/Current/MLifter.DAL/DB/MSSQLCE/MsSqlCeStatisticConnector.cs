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
    /// MsSqlCeStatisticConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2009-01-13</remarks>
    class MsSqlCeStatisticConnector : IDbStatisticConnector
    {
        #region Initialization of MsSqlCeStatisticConnector

        internal int RunningSession = -1;

        private static Dictionary<ConnectionStringStruct, MsSqlCeStatisticConnector> instance = new Dictionary<ConnectionStringStruct, MsSqlCeStatisticConnector>();
        public static MsSqlCeStatisticConnector GetInstance(ParentClass parent)
        {
            lock (instance)
            {
                ConnectionStringStruct connection = parent.CurrentUser.ConnectionString;

                if (!instance.ContainsKey(connection))
                    instance.Add(connection, new MsSqlCeStatisticConnector(parent));

                return instance[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeStatisticConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
        }

        void parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instance.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #endregion

        #region New IDbStatisticConnector Members (with extened LearningSession table)

        /// <summary>
        /// Gets the wrong cards.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-28</remarks>
        public int? GetWrongCards(int sessionId)
        {
            object wrongCardsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticWrongCards, sessionId)];
            if (wrongCardsCache == null || RunningSession == sessionId)
            {
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT sum_wrong FROM \"LearningSessions\" WHERE id = @sessId AND user_id=@uid AND lm_id=@lmid";
                cmd.Parameters.Add("@sessId", sessionId);
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

                int? wrongCards = MSSQLCEConn.ExecuteScalar<int>(cmd);

                //Save to Cache
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticWrongCards, sessionId, Cache.DefaultStatisticValidationTime)] = wrongCards;

                return wrongCards;
            }

            return wrongCardsCache as int?;
        }

        /// <summary>
        /// Gets the correct cards.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-28</remarks>
        public int? GetCorrectCards(int sessionId)
        {
            object correctCardsCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticCorrectCards, sessionId)];
            if (correctCardsCache == null || RunningSession == sessionId)
            {
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT sum_right FROM \"LearningSessions\" WHERE id = @sessId AND user_id=@uid AND lm_id=@lmid";
                cmd.Parameters.Add("@sessId", sessionId);
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

                int? correctCards = MSSQLCEConn.ExecuteScalar<int>(cmd);

                //Save to Cache
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticCorrectCards, sessionId, Cache.DefaultStatisticValidationTime)] = correctCards;

                return correctCards;
            }

            return correctCardsCache as int?;
        }

        /// <summary>
        /// Gets the content of boxes.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-28</remarks>
        public List<int> GetContentOfBoxes(int sessionId)
        {
            object contentOfBoxesCache = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticContentOfBoxes, sessionId)];
            if (contentOfBoxesCache == null || RunningSession == sessionId)
            {
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT box1_content, box2_content, box3_content, box4_content, box5_content, box6_content, box7_content, box8_content, box9_content, box10_content " +
                    "FROM LearningSessions WHERE id = @sessionId AND user_id=@uid AND lm_id=@lmid";
                cmd.Parameters.Add("@sessionId", sessionId);
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                reader.Read();

                object box1_content = reader["box1_content"];
                object box2_content = reader["box2_content"];
                object box3_content = reader["box3_content"];
                object box4_content = reader["box4_content"];
                object box5_content = reader["box5_content"];
                object box6_content = reader["box6_content"];
                object box7_content = reader["box7_content"];
                object box8_content = reader["box9_content"];
                object box9_content = reader["box9_content"];
                object box10_content = reader["box10_content"];
                reader.Close();

                List<int> output = new List<int>();
                output.Add(box1_content != DBNull.Value ? Convert.ToInt32(box1_content) : 0);
                output.Add(box2_content != DBNull.Value ? Convert.ToInt32(box2_content) : 0);
                output.Add(box3_content != DBNull.Value ? Convert.ToInt32(box3_content) : 0);
                output.Add(box4_content != DBNull.Value ? Convert.ToInt32(box4_content) : 0);
                output.Add(box5_content != DBNull.Value ? Convert.ToInt32(box5_content) : 0);
                output.Add(box6_content != DBNull.Value ? Convert.ToInt32(box6_content) : 0);
                output.Add(box7_content != DBNull.Value ? Convert.ToInt32(box7_content) : 0);
                output.Add(box8_content != DBNull.Value ? Convert.ToInt32(box8_content) : 0);
                output.Add(box9_content != DBNull.Value ? Convert.ToInt32(box9_content) : 0);
                output.Add(box10_content != DBNull.Value ? Convert.ToInt32(box10_content) : 0);

                //Save to Cache
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticContentOfBoxes, sessionId, Cache.DefaultStatisticValidationTime)] = output;

                return output;
            }

            return contentOfBoxesCache as List<int>;
        }

        /// <summary>
        /// Gets the start time stamp.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-28</remarks>
        public DateTime? GetStartTimeStamp(int sessionId)
        {
            object startTimeStamp = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticStartTime, sessionId)];
            if (startTimeStamp == null || RunningSession == sessionId)
            {
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT starttime FROM \"LearningSessions\" WHERE \"LearningSessions\".id = @sessId AND user_id=@uid AND lm_id=@lmid";
                cmd.Parameters.Add("@sessId", sessionId);
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

                DateTime? dt = MSSQLCEConn.ExecuteScalar<DateTime>(cmd);

                //Save to Cache
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticStartTime, sessionId, Cache.DefaultStatisticValidationTime)] = dt;

                return dt;
            }

            return startTimeStamp as DateTime?;
        }

        /// <summary>
        /// Gets the end time stamp.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-28</remarks>
        public DateTime? GetEndTimeStamp(int sessionId)
        {
            if (RunningSession == sessionId)
                return DateTime.Now;

            object endTimeStamp = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticEndTime, sessionId)];
            if (endTimeStamp == null || RunningSession == sessionId)
            {
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT endtime FROM \"LearningSessions\" WHERE \"LearningSessions\".id = @sessId AND user_id=@uid AND lm_id=@lmid";
                cmd.Parameters.Add("@sessId", sessionId);
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

                object dt = MSSQLCEConn.ExecuteScalar<DateTime>(cmd);

                DateTime? dateTime;
                if (dt == null || !(dt is DateTime))     //either null or not DateTime
                    dateTime = DateTime.Now;
                else
                {
                    dateTime = (DateTime)dt;
                }

                //Save to Cache
                Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticEndTime, sessionId, Cache.DefaultStatisticValidationTime)] = dateTime;

                return dateTime;
            }

            return endTimeStamp as DateTime?;
        }

        #endregion

        /// <summary>
        /// Gets the content of a specific the box. (replacement for the stored procedure GetBoxContent)
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="boxNumber">The box number (from 1 to 10).</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        private int GetBoxContent(int sessionId, int boxNumber)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT COUNT(*) FROM " +
                              "(SELECT DISTINCT ON (\"LearnLog\".cards_id) \"LearnLog\".new_box FROM " +
                              "\"LearnLog\" WHERE \"LearnLog\".session_id IN (SELECT id FROM \"LearningSessions\" WHERE \"LearningSessions\".lm_id = " +
                              "(SELECT lm_id FROM \"LearningSessions\" WHERE \"LearningSessions\".id = @endSessionId AND user_id=@uid AND lm_id=@lmid)) AND " +
                              "\"LearnLog\".session_id <= endSessionId ORDER BY cards_id, timestamp DESC) AS boxes WHERE new_box = @current_box;";
            cmd.Parameters.Add("@endSessionId", sessionId);
            cmd.Parameters.Add("@current_box", boxNumber);
            cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
            cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

            return Convert.ToInt32(MSSQLCEConn.ExecuteScalar(cmd));
        }
    }
}
