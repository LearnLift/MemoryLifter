using System;
using System.Collections.Generic;
using System.Text;

using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using Npgsql;

namespace MLifter.DAL.DB.PostgreSQL
{
    /// <summary>
    /// PgSqlStatisticConnector
    /// </summary>
    /// <remarks>Documented by Dev08, 2008-11-12</remarks>
    class PgSqlStatisticConnector : IDbStatisticConnector
    {
        #region Initialization of PgSqlStatisticConnector

        internal int RunningSession = -1;

        private static Dictionary<ConnectionStringStruct, PgSqlStatisticConnector> instance = new Dictionary<ConnectionStringStruct, PgSqlStatisticConnector>();
        public static PgSqlStatisticConnector GetInstance(ParentClass parent)
        {
            lock (instance)
            {
                ConnectionStringStruct connection = parent.CurrentUser.ConnectionString;

                if (!instance.ContainsKey(connection))
                    instance.Add(connection, new PgSqlStatisticConnector(parent));

                return instance[connection];
            }
        }

        private ParentClass parent;
        private PgSqlStatisticConnector(ParentClass parentClass)
        {
            parent = parentClass;
            parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
        }

        void parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instance.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #endregion

        #region IDbStatisticConnector Members (with extended LearningSession Table)

        /// <summary>
        /// Gets the wrong cards.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-28</remarks>
        public int? GetWrongCards(int sessionId)
        {
            object wrongCardsCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticWrongCards, sessionId)];
            if (wrongCardsCache == null || RunningSession == sessionId)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT sum_wrong FROM \"LearningSessions\" WHERE \"LearningSessions\".id=:sessId AND user_id=:uid AND lm_id=:lmid";
                        cmd.Parameters.Add("sessId", sessionId);
                        cmd.Parameters.Add("uid", parent.CurrentUser.Id);
                        cmd.Parameters.Add("lmid", parent.GetParentDictionary().Id);

                        int? wrongCards = PostgreSQLConn.ExecuteScalar<int>(cmd, parent.CurrentUser);

                        //Save to Cache
                        parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticWrongCards, sessionId, Cache.DefaultStatisticValidationTime)] = wrongCards;

                        return wrongCards;
                    }
                }
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
            object correctCardsCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticCorrectCards, sessionId)];
            if (correctCardsCache == null || RunningSession == sessionId)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT sum_right FROM \"LearningSessions\" WHERE \"LearningSessions\".id = :sessId AND user_id=:uid AND lm_id=:lmid";
                        cmd.Parameters.Add("sessId", sessionId);
                        cmd.Parameters.Add("uid", parent.CurrentUser.Id);
                        cmd.Parameters.Add("lmid", parent.GetParentDictionary().Id);

                        int? correctCards = PostgreSQLConn.ExecuteScalar<int>(cmd, parent.CurrentUser);

                        //Save to Cache
                        parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticCorrectCards, sessionId, Cache.DefaultStatisticValidationTime)] = correctCards;

                        return correctCards;
                    }
                }
            }

            return correctCardsCache as int?;
        }

        /// <summary>
        /// Gets the content of boxes.
        /// </summary>
        /// <param name="sessionId">The end session id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-28</remarks>
        public List<int> GetContentOfBoxes(int sessionId)
        {
            object contentOfBoxesCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticContentOfBoxes, sessionId)];
            if (contentOfBoxesCache == null || RunningSession == sessionId)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT box1_content, box2_content, box3_content, box4_content, box5_content, box6_content, box7_content, box8_content, box9_content, box10_content " +
                            "FROM \"LearningSessions\" WHERE id=:sessId AND user_id=:uid AND lm_id=:lmid";
                        cmd.Parameters.Add("sessId", sessionId);
                        cmd.Parameters.Add("uid", parent.CurrentUser.Id);
                        cmd.Parameters.Add("lmid", parent.GetParentDictionary().Id);

                        NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, parent.CurrentUser);
                        reader.Read();

                        object box1 = reader["box1_content"];
                        object box2 = reader["box2_content"];
                        object box3 = reader["box3_content"];
                        object box4 = reader["box4_content"];
                        object box5 = reader["box5_content"];
                        object box6 = reader["box6_content"];
                        object box7 = reader["box7_content"];
                        object box8 = reader["box8_content"];
                        object box9 = reader["box9_content"];
                        object box10 = reader["box10_content"];

                        List<int> output = new List<int>();
                        output.Add(box1 == DBNull.Value ? 0 : Convert.ToInt32(box1));
                        output.Add(box2 == DBNull.Value ? 0 : Convert.ToInt32(box2));
                        output.Add(box3 == DBNull.Value ? 0 : Convert.ToInt32(box3));
                        output.Add(box4 == DBNull.Value ? 0 : Convert.ToInt32(box4));
                        output.Add(box5 == DBNull.Value ? 0 : Convert.ToInt32(box5));
                        output.Add(box6 == DBNull.Value ? 0 : Convert.ToInt32(box6));
                        output.Add(box7 == DBNull.Value ? 0 : Convert.ToInt32(box7));
                        output.Add(box8 == DBNull.Value ? 0 : Convert.ToInt32(box8));
                        output.Add(box9 == DBNull.Value ? 0 : Convert.ToInt32(box9));
                        output.Add(box10 == DBNull.Value ? 0 : Convert.ToInt32(box10));

                        //Save to Cache
                        parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticContentOfBoxes, sessionId, Cache.DefaultStatisticValidationTime)] = output;

                        return output;
                    }
                }
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
            object startTimeStamp = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticStartTime, sessionId)];
            if (startTimeStamp == null || RunningSession == sessionId)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT starttime FROM \"LearningSessions\" WHERE \"LearningSessions\".id = :sessId AND user_id=:uid AND lm_id=:lmid";
                        cmd.Parameters.Add("sessId", sessionId);
                        cmd.Parameters.Add("uid", parent.CurrentUser.Id);
                        cmd.Parameters.Add("lmid", parent.GetParentDictionary().Id);

                        DateTime? dt = PostgreSQLConn.ExecuteScalar<DateTime>(cmd, parent.CurrentUser);

                        //Save to Cache
                        parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticStartTime, sessionId, Cache.DefaultStatisticValidationTime)] = dt;

                        return dt;
                    }
                }
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

            object endTimeStamp = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticEndTime, sessionId)];
            if (endTimeStamp == null || RunningSession == sessionId)
            {
                using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(parent.CurrentUser))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT endtime FROM \"LearningSessions\" WHERE \"LearningSessions\".id = :sessId AND user_id=:uid AND lm_id=:lmid";
                        cmd.Parameters.Add("sessId", sessionId);
                        cmd.Parameters.Add("uid", parent.CurrentUser.Id);
                        cmd.Parameters.Add("lmid", parent.GetParentDictionary().Id);

                        object dt = PostgreSQLConn.ExecuteScalar<DateTime>(cmd, parent.CurrentUser);

                        DateTime? dateTime;
                        if (dt == null || !(dt is DateTime))     //either null or not DateTime
                            dateTime = DateTime.Now;
                        else
                        {
                            dateTime = (DateTime)dt;
                        }

                        //Save to Cache
                        parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticEndTime, sessionId, Cache.DefaultStatisticValidationTime)] = dateTime;

                        return dateTime;
                    }
                }
            }

            return endTimeStamp as DateTime?;
        }


        #endregion
    }
}
