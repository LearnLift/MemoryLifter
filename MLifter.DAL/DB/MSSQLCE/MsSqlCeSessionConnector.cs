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
    /// The MS SQL CE connector for the IDbSessionConnector interface.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-13</remarks>
    class MsSqlCeSessionConnector : IDbSessionConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeSessionConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeSessionConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2009-01-13</remarks>
        public static MsSqlCeSessionConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeSessionConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private MsSqlCeSessionConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbSessionConnector Members

        /// <summary>
        /// Creates a new user session.
        /// The old one will be automatically closed, in case of inconsistance data.
        /// </summary>
        /// <param name="lm_id">The lm_id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev10, 2009-01-11</remarks>
        /// <remarks>Documented by Dev08, 2009-04-28</remarks>
        public int OpenUserSession(int lm_id)
        {
            //1. Check if the old session is closed
            bool previousSessionExisting = false;
            DateTime? endtime = null;
            int latestSessionId = 0;

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT * FROM LearningSessions WHERE lm_id=@lmid AND user_id=@userid ORDER BY starttime DESC";
            cmd.Parameters.Add("@lmid", lm_id);
            cmd.Parameters.Add("@userid", Parent.CurrentUser.Id);
            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

            if (reader.Read())
            {
                previousSessionExisting = true;
                try
                {
                    latestSessionId = Convert.ToInt32(reader["id"]);
                    endtime = Convert.ToDateTime(reader["endtime"]);
                }
                catch
                {
                    endtime = null;
                }
            }

            //2. Close the previous session, if it hasn't closed before (maybe cause of crash of ML)
            if (previousSessionExisting && !endtime.HasValue)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "UPDATE LearningSessions SET endtime=GETDATE() WHERE id=@id";
                cmd.Parameters.Add("@id", latestSessionId);
                MSSQLCEConn.ExecuteNonQuery(cmd);
            }

            //3. Add new session entry to DB
            cmd.Parameters.Clear();
            cmd.CommandText = "INSERT INTO LearningSessions (user_id, lm_id, starttime, sum_right, sum_wrong, pool_content, box1_content, box2_content, box3_content, " +
                              "box4_content, box5_content, box6_content, box7_content, box8_content, box9_content, box10_content)" +
                              "VALUES(@userid, @lmid, GETDATE(),  0, 0, @pool, @b1, @b2, @b3, @b4, @b5, @b6, @b7, @b8, @b9, @b10); SELECT @@IDENTITY;";
            cmd.Parameters.Add("@userid", Parent.CurrentUser.Id);
            cmd.Parameters.Add("@lmid", lm_id);
            int counter = 0;
            int cardsInBoxes = 0;
            BoxSizes boxContent = GetCurrentBoxContent();
            foreach (int box in boxContent.Sizes)
            {
                if (counter == 0)
                {
                    cmd.Parameters.Add("@pool", box);
                    ++counter;
                    continue;
                }

                cmd.Parameters.Add("@b" + Convert.ToString(counter++), box);
                cardsInBoxes += box;
            }

            int newSessionId = MSSQLCEConn.ExecuteScalar<int>(cmd).Value;

            //Following Statement does add the "RunningSession" = true to the current Statistic.
            MsSqlCeStatisticConnector connector = MsSqlCeStatisticConnector.GetInstance(Parent);
            connector.RunningSession = newSessionId;

            return newSessionId;
        }

        public void RecalculateBoxSizes(int sessionId)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE LearningSessions SET pool_content=@pool, box1_content=@b1, box2_content=@b2, box3_content=@b3, " +
                              "box4_content=@b4, box5_content=@b5, box6_content=@b6, box7_content=@b7, box8_content=@b8, box9_content=@b9, box10_content=@b10 WHERE id=@sid AND user_id=@uid AND lm_id=@lmid";
            cmd.Parameters.Add("sid", sessionId);
            cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
            cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

            int counter = 0;
            int cardsInBoxes = 0;
            BoxSizes boxContent = GetCurrentBoxContent();
            foreach (int box in boxContent.Sizes)
            {
                if (counter == 0)
                {
                    cmd.Parameters.Add("@pool", box);
                    ++counter;
                    continue;
                }

                cmd.Parameters.Add("@b" + Convert.ToString(counter++), box);
                cardsInBoxes += box;
            }

            MSSQLCEConn.ExecuteNonQuery(cmd);
        }

        public void CardFromBoxDeleted(int sessionId, int boxId)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = string.Format("UPDATE LearningSessions SET {0}_content={0}_content - 1 WHERE id=@sid AND user_id=@uid AND lm_id=@lmid", boxId > 0 ? "box" + boxId.ToString() : "pool");
            cmd.Parameters.Add("sid", sessionId);
            cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
            cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

            MSSQLCEConn.ExecuteNonQuery(cmd);
        }

        public void CardAdded(int sessionId)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE LearningSessions SET pool_content=pool_content + 1 WHERE id=@sid AND user_id=@uid AND lm_id=@lmid";
            cmd.Parameters.Add("sid", sessionId);
            cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
            cmd.Parameters.Add("@lmid", Parent.GetParentDictionary().Id);

            MSSQLCEConn.ExecuteNonQuery(cmd);
        }

        private BoxSizes GetCurrentBoxContent()
        {
            BoxSizes sizes;
            using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser))
            {
                sizes = new BoxSizes(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                cmd.CommandText = @"SELECT CS.box AS box, count(*) AS count FROM UserCardState CS
                                        INNER JOIN Cards C ON C.id = CS.cards_id AND C.lm_id=@lm_id
                                        WHERE CS.active = 1 AND CS.user_id = @user_id  
                                      GROUP BY CS.box";
                cmd.Parameters.Add("@user_id", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", Parent.CurrentUser.ConnectionString.LmId);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                while (reader.Read())
                    sizes.Sizes[Convert.ToInt32(reader["box"])] = Convert.ToInt32(reader["count"]);
                reader.Close();

                return sizes;
            }
        }


        /// <summary>
        /// Closes the user session.
        /// </summary>
        /// <param name="last_entry">The last_entry.</param>
        /// <remarks>Documented by Dev10, 2009-01-11</remarks>
        public void CloseUserSession(int last_entry)
        {
            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE LearningSessions SET endtime=GETDATE() WHERE id=@id";
            cmd.Parameters.Add("@id", last_entry);

            MSSQLCEConn.ExecuteNonQuery(cmd);

            //Following Statement does add the "RunningSession" = false to the current Statistic.
            MsSqlCeStatisticConnector connector = MsSqlCeStatisticConnector.GetInstance(Parent);
            connector.RunningSession = -1;
        }


        /// <summary>
        /// Restarts the learning success.
        /// </summary>
        /// <param name="lm_id">The lm_id.</param>
        /// <remarks>Documented by Dev10, 2009-01-11</remarks>
        public IDictionary RestartLearningSuccess(int lm_id)
        {
            if (Parent.CurrentUser.ConnectionString.SyncType != SyncType.NotSynchronized)
            {
                object firstLearningSessionId = null, lastLearningSessionId = null;

                //Get all LearningSessions.Ids
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT id FROM LearningSessions WHERE lm_id = @lmId AND user_id = @userId ORDER BY id ASC;";
                cmd.Parameters.Add("@lmId", lm_id);
                cmd.Parameters.Add("@userId", Parent.CurrentUser.Id);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                int counter = 0;
                while (reader.Read())
                {
                    ++counter;
                    if (firstLearningSessionId == null)
                        firstLearningSessionId = reader["id"];
                    else
                        lastLearningSessionId = reader["id"];
                }
                reader.Close();

                if (lastLearningSessionId == null)
                    lastLearningSessionId = firstLearningSessionId;

                int firstLearningSession = Convert.ToInt32(firstLearningSessionId);
                int lastLearningSession = Convert.ToInt32(lastLearningSessionId);

                if (counter == 0)
                    return Parent.GetParentDictionary();

                cmd.CommandText = "DELETE FROM LearnLog WHERE session_id BETWEEN @sessionStart AND @sessionStop; DELETE FROM LearningSessions WHERE lm_id = @lmId AND user_id = @userId;";
                cmd.Parameters.Add("@sessionStart", firstLearningSession);
                cmd.Parameters.Add("@sessionStop", lastLearningSession);

                MSSQLCEConn.ExecuteNonQuery(cmd);

                return Parent.GetParentDictionary();
            }
            else
            {
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "UPDATE UserProfiles SET local_directory_id=NULL, username=@user WHERE id=@id";
                cmd.Parameters.Add("@id", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@user", "BACKUP_" + Parent.CurrentUser.AuthenticationStruct.UserName);
                MSSQLCEConn.ExecuteNonQuery(cmd);

                SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd2.CommandText = "INSERT INTO UserProfiles(username, local_directory_id, user_type) VALUES(@user, @id, 'ListAuthentication'); SELECT @@IDENTITY";
                cmd2.Parameters.Add("@user", User.CurrentWindowsUserName);
                cmd2.Parameters.Add("@id", User.CurrentWindowsUserId);
                int uid = MSSQLCEConn.ExecuteScalar<int>(cmd2).Value;

                ((Parent.CurrentUser as User).BaseUser as DbUser).SetId(uid);
                return ((Parent.CurrentUser as User).BaseUser as DbUser).Open();
            }
        }


        #endregion
    }
}
