using System;
using System.Collections.Generic;
using System.Text;

using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using Npgsql;
using System.Diagnostics;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlStatisticsConnector : IDbStatisticsConnector
    {
        #region Initialization of PgSqlStatisticConnector

        private int runningSessionCopy = -1;
        private static Dictionary<ConnectionStringStruct, PgSqlStatisticsConnector> instance = new Dictionary<ConnectionStringStruct, PgSqlStatisticsConnector>();
        public static PgSqlStatisticsConnector GetInstance(ParentClass parent)
        {
            lock (instance)
            {
                ConnectionStringStruct connection = parent.CurrentUser.ConnectionString;

                if (!instance.ContainsKey(connection))
                    instance.Add(connection, new PgSqlStatisticsConnector(parent));

                return instance[connection];
            }
        }

        private ParentClass parent;
        private PgSqlStatisticsConnector(ParentClass parentClass)
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

        /// <summary>
        /// Gets all learn sessions from a LM.
        /// </summary>
        /// <param name="lmId">The lm id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev08, 2008-11-13</remarks>
        public List<int> GetLearnSessions(int lmId)
        {
            object learnSessionsCache = parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.StatisticsLearnSessions, lmId)];
            PgSqlStatisticConnector c = PgSqlStatisticConnector.GetInstance(parent);

            //The data of GetLearnSession can be cached until a new session was created.
            if (runningSessionCopy == c.RunningSession && learnSessionsCache != null)
                return learnSessionsCache as List<int>;

            //if cache is empty or the RunningSession has changed...
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM \"LearningSessions\" WHERE lm_id = :lmId AND user_id=:uid ORDER BY endtime ASC";
                    cmd.Parameters.Add("lmId", lmId);
                    cmd.Parameters.Add("uid", parent.CurrentUser.Id);

                    List<int> output = new List<int>();
                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, parent.CurrentUser);
                    while (reader.Read())
                    {
                        object id = reader["id"];
                        int id_converted = Convert.ToInt32(id);

                        output.Add(id_converted);
                    }
                    runningSessionCopy = c.RunningSession;

                    //Save to Cache
                    parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.StatisticsLearnSessions, lmId, Cache.DefaultStatisticValidationTime)] = output;
                    return output;
                }
            }
        }

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

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO \"LearningSessions\"(user_id, lm_id, starttime, endtime, sum_right, sum_wrong, " +
                                      "pool_content, box1_content, box2_content, box3_content, box4_content, box5_content, box6_content, box7_content, box8_content, box9_content, box10_content)" +
                                      "VALUES(:userid, :lmid, :starttime, :endtime, :sumright, :sumwrong, :pool, :b1, :b2, :b3, :b4, :b5, :b6, :b7, :b8, :b9, :b10)";

                    cmd.Parameters.Add("userid", parent.CurrentUser.Id);
                    cmd.Parameters.Add("lmid", lmId);
                    cmd.Parameters.Add("starttime", statistic.StartTimestamp);
                    cmd.Parameters.Add("endtime", statistic.EndTimestamp);
                    cmd.Parameters.Add("sumright", statistic.Right);
                    cmd.Parameters.Add("sumwrong", statistic.Wrong);
                    int counter = 0;
                    foreach (int box in statistic.Boxes)
                    {
                        ++counter;
                        cmd.Parameters.Add("b" + counter.ToString(), box);
                    }
                    cmd.Parameters.Add("pool", parent.GetParentDictionary().Boxes.Box[0].MaximalSize);     //pool max size => cards in pools

                    PostgreSQLConn.ExecuteNonQuery(cmd, parent.CurrentUser);
                }
            }
        }

        #endregion
    }
}
