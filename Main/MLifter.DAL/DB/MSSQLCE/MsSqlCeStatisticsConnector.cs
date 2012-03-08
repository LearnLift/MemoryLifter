using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using System.Data.SqlServerCe;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// The MS SQL CE implementation of IDbStatisticsConnector.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-01-16</remarks>
    class MsSqlCeStatisticsConnector : IDbStatisticsConnector
    {
        private int runningSessionCopy = -1;
        private static Dictionary<ConnectionStringStruct, MsSqlCeStatisticsConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeStatisticsConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public static MsSqlCeStatisticsConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeStatisticsConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass parent;
        private MsSqlCeStatisticsConnector(ParentClass parentClass)
        {
            parent = parentClass;
            parent.DictionaryClosed += new EventHandler(parent_DictionaryClosed);
        }

        void parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbStatisticsConnector Members

        /// <summary>
        /// Gets the learn sessions.
        /// </summary>
        /// <param name="lmId">The lm id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2009-01-13</remarks>
        public List<int> GetLearnSessions(int lmId)
        {
            object learnSessionsCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticsLearnSessions, lmId)];
            MsSqlCeStatisticConnector c = MsSqlCeStatisticConnector.GetInstance(parent);

            //The data of GetLearnSession can be cached until a new session was created.
            if (runningSessionCopy == c.RunningSession && learnSessionsCache != null)
                return learnSessionsCache as List<int>;

            //if cache is empty or the RunningSession has changed...
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser);
            cmd.CommandText = "SELECT id FROM \"LearningSessions\" WHERE lm_id = @lmId AND user_id=@uid ORDER BY endtime ASC";
            cmd.Parameters.Add("@lmId", lmId);
            cmd.Parameters.Add("@uid", parent.CurrentUser.Id);

            List<int> output = new List<int>();
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
            while (reader.Read())
            {
                object id = reader["id"];
                int id_converted = Convert.ToInt32(id);

                output.Add(id_converted);
            }
            reader.Close();
            runningSessionCopy = c.RunningSession;

            //Save to Cache
            parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticsLearnSessions, lmId, Cache.DefaultStatisticValidationTime)] = output;
            return output;
        }

        #endregion

        #region IDbStatisticsConnector Members

        /// <summary>
        /// Copies the statistics.
        /// </summary>
        /// <param name="lmId">The lm id.</param>
        /// <param name="statistic">The statistic.</param>
        /// <remarks>Documented by Dev08, 2009-02-09</remarks>
        public void CopyStatistics(int lmId, IStatistic statistic)
        {
            if (statistic.StartTimestamp == null || statistic.EndTimestamp == null)     //do not save invalid sessions
                return;

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(parent.CurrentUser);
            cmd.CommandText = "INSERT INTO LearningSessions(user_id, lm_id, starttime, endtime, sum_right, sum_wrong, " +
                              "pool_content, box1_content, box2_content, box3_content, box4_content, box5_content, box6_content, box7_content, box8_content, box9_content, box10_content)" +
                              "VALUES(@userid, @lmid, @starttime, @endtime, @sumright, @sumwrong, @pool, @b1, @b2, @b3, @b4, @b5, @b6, @b7, @b8, @b9, @b10)";

            cmd.Parameters.Add("@userid", parent.CurrentUser.Id);
            cmd.Parameters.Add("@lmid", lmId);
            cmd.Parameters.Add("@starttime", statistic.StartTimestamp);
            cmd.Parameters.Add("@endtime", statistic.EndTimestamp);
            cmd.Parameters.Add("@sumright", statistic.Right);
            cmd.Parameters.Add("@sumwrong", statistic.Wrong);
            int counter = 0;
            foreach (int box in statistic.Boxes)
            {
                ++counter;
                cmd.Parameters.Add("@b" + counter.ToString(), box);
            }

            cmd.Parameters.Add("@pool", parent.GetParentDictionary().Boxes.Box[0].MaximalSize);     //pool max size => cards in pools

            MSSQLCEConn.ExecuteNonQuery(cmd);
        }

        #endregion
    }
}
