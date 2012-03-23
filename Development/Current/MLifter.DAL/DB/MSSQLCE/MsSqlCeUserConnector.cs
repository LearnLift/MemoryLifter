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
using MLifter.DAL.Properties;
using MLifter.DAL.Tools;

namespace MLifter.DAL.DB.MsSqlCe
{
    /// <summary>
    /// The MS SQL CE implementation of IDbUserConnector.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-01-16</remarks>
    class MsSqlCeUserConnector : IDbUserConnector
    {
        private static Dictionary<ConnectionStringStruct, MsSqlCeUserConnector> instances = new Dictionary<ConnectionStringStruct, MsSqlCeUserConnector>();
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public static MsSqlCeUserConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new MsSqlCeUserConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlCeUserConnector"/> class.
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        private MsSqlCeUserConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        /// <summary>
        /// Handles the DictionaryClosed event of the Parent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbUserConnector Members

        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public IList<UserStruct> GetUserList()
        {
            IList<UserStruct> users = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserList, 0)] as IList<UserStruct>;
            if (users != null)
                return users;

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT * FROM \"UserProfiles\";";

            SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);

            users = new List<UserStruct>();
            users.Add(new UserStruct(Resources.CREATE_NEW_USER, UserAuthenticationTyp.ListAuthentication));
            while (reader.Read())
            {
                UserStruct user = new UserStruct(reader["username"].ToString(), reader["local_directory_id"].ToString(), UserAuthenticationTyp.ListAuthentication);

                users.Add(user);
            }
            reader.Close();

            Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.UserList, 0, new TimeSpan(0, 0, 30))] = users;

            return users;
        }

        /// <summary>
        /// Gets the allowed authentication modes.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public UserAuthenticationTyp? GetAllowedAuthenticationModes()
        {
            return UserAuthenticationTyp.ListAuthentication;
        }
        /// <summary>
        /// Gets the type of the local directory.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public LocalDirectoryType? GetLocalDirectoryType()
        {
            return null;
        }
        /// <summary>
        /// Gets the LDAP server.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public string GetLdapServer()
        {
            return string.Empty;
        }
        /// <summary>
        /// Gets the LDAP port.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public int GetLdapPort()
        {
            return -1;
        }
        /// <summary>
        /// Gets the LDAP user.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public string GetLdapUser()
        {
            return string.Empty;
        }
        /// <summary>
        /// Gets the LDAP password.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public string GetLdapPassword()
        {
            return string.Empty;
        }
        /// <summary>
        /// Gets the LDAP context.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public string GetLdapContext()
        {
            return string.Empty;
        }
        /// <summary>
        /// Gets the LDAP use SSL.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public bool GetLdapUseSSL()
        {
            return false;
        }

        /// <summary>
        /// Gets the user learning module settings.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public int GetUserLearningModuleSettings(int id)
        {
            int cnt;
            SqlCeCommand cmd_main = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd_main.CommandText = "SELECT count(*) FROM \"UserProfilesLearningModulesSettings\" " +
                "WHERE user_id=@uid and lm_id=@lm_id;";
            cmd_main.Parameters.Add("@uid", Parent.CurrentUser.Id);
            cmd_main.Parameters.Add("@lm_id", id);

            cnt = MSSQLCEConn.ExecuteScalar<int>(cmd_main).Value;
            if (cnt > 0)
            {
                //Workaround for Issue: ML-2458 If System Date is older than Timestamps only one card is asked 
                SqlCeCommand timeCmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                timeCmd.CommandText = "SELECT cards_id FROM \"UserCardState\" WHERE user_id=@uid AND timestamp > @now";
                timeCmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                timeCmd.Parameters.Add("@now", DateTime.Now);
                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(timeCmd);

                List<int> wrongCards = new List<int>();
                while (reader.Read())
                    wrongCards.Add(Convert.ToInt32(reader["cards_id"]));
                reader.Close();

                foreach (int cid in wrongCards)
                {
                    SqlCeCommand updateCmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                    updateCmd.CommandText = "UPDATE \"UserCardState\" SET timestamp = @time WHERE user_id=@uid AND cards_id=@cid";
                    updateCmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                    updateCmd.Parameters.Add("@cid", cid);
                    updateCmd.Parameters.Add("@time", DateTime.Now.AddSeconds(-Math.PI / 2));
                    MSSQLCEConn.ExecuteNonQuery(updateCmd);
                }
                //End of Workaround for Issue: ML-2458

                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                    "WHERE user_id=@uid and lm_id=@lm_id;";
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", id);

                return MSSQLCEConn.ExecuteScalar<int>(cmd).Value;
            }
            else
            {
                int newSettings = MsSqlCeSettingsConnector.CreateNewSettings(Parent);
                SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "INSERT INTO \"UserProfilesLearningModulesSettings\" (user_id, lm_id, settings_id) " +
                    "VALUES (@uid, @lm_id, @settings_id)";
                cmd.Parameters.Add("@uid", Parent.CurrentUser.Id);
                cmd.Parameters.Add("@lm_id", id);
                cmd.Parameters.Add("@settings_id", newSettings);
                MSSQLCEConn.ExecuteNonQuery(cmd);

                cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd.CommandText = "SELECT id FROM \"Chapters\" " +
                    "WHERE lm_id=@lm_id;";
                cmd.Parameters.Add("@lm_id", id);

                SqlCeDataReader reader = MSSQLCEConn.ExecuteReader(cmd);
                List<int> chapterIds = new List<int>();
                while (reader.Read())
                    chapterIds.Add(Convert.ToInt32(reader["id"]));
                reader.Close();

                foreach (int chapterId in chapterIds)
                {
                    SqlCeCommand cmd2 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                    cmd2.CommandText = "INSERT INTO \"SelectedLearnChapters\" (chapters_id, settings_id) VALUES (@cid, @sid);";
                    cmd2.Parameters.Add("@cid", chapterId);
                    cmd2.Parameters.Add("@sid", newSettings);

                    MSSQLCEConn.ExecuteNonQuery(cmd2);
                }

                SqlCeCommand cmd3 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                cmd3.CommandText = "SELECT id FROM \"Cards\" WHERE id IN " +
                    "(SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=@lm_id)";
                cmd3.Parameters.Add("@lm_id", id);

                SqlCeDataReader reader2 = MSSQLCEConn.ExecuteReader(cmd3);
                List<int> cardIds = new List<int>();
                while (reader2.Read())
                    cardIds.Add(Convert.ToInt32(reader2["id"]));
                reader2.Close();

                foreach (int cid in cardIds)
                {
                    SqlCeCommand cmd4 = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
                    cmd4.CommandText = "INSERT INTO \"UserCardState\" (user_id, cards_id, box, active) VALUES (@param_user_id, @param_cards_id, 0, 1);";
                    cmd4.Parameters.Add("@param_user_id", Parent.CurrentUser.Id);
                    cmd4.Parameters.Add("@param_cards_id", cid);

                    MSSQLCEConn.ExecuteNonQuery(cmd4);
                }

                Parent.CurrentUser.Cache.Clear();

                return newSettings;
            }
        }

        /// <summary>
        /// Gets the user id of the given user name (--> normal login).
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="sid">NOT used.</param>
        /// <param name="closeOpenSessions">NOT used.</param>
        /// <param name="standAlone">NOT used.</param>
        /// <returns>The user id.</returns>
        /// <remarks>Documented by Dev05, 2009-01-13</remarks>
        public int LoginListUser(string username, Guid sid, bool closeOpenSessions, bool standAlone)
        {
            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserList, 0));

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "SELECT id FROM \"UserProfiles\" " +
                "WHERE username=@user;";
            cmd.Parameters.Add("@user", username);

            return MSSQLCEConn.ExecuteScalar<int>(cmd).Value;
        }

        /// <summary>
        /// Creates the given user and returns his id (--> Create New User).
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="sid">NOT used.</param>
        /// <param name="closeOpenSessions">NOT used.</param>
        /// <param name="standAlone">NOT used.</param>
        /// <returns>The user id.</returns>
        /// <remarks>Documented by Dev05, 2009-01-13</remarks>
        public int LoginFormsUser(string username, string userId, Guid sid, bool closeOpenSessions, bool standAlone)
        {
            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserList, 0));

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "INSERT INTO \"UserProfiles\" (username, local_directory_id, user_type) " +
                "VALUES (@user, @id, 'ListAuthentication'); SELECT @@IDENTITY;";
            cmd.Parameters.Add("@user", username);
            cmd.Parameters.Add("@id", userId);

            return MSSQLCEConn.ExecuteScalar<int>(cmd).Value;
        }

        /// <summary>
        /// Alters the user id to the given one and returns the user id (--> Map old user to new one).
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="sid">NOT used.</param>
        /// <param name="closeOpenSessions">NOT used.</param>
        /// <param name="standAlone">NOT used.</param>
        /// <returns>The user id.</returns>
        /// <remarks>Documented by Dev05, 2009-01-13</remarks>
        public int LoginLocalDirectoryUser(string username, string userId, Guid sid, bool closeOpenSessions, bool standAlone)
        {
            Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserList, 0));

            SqlCeCommand cmd = MSSQLCEConn.CreateCommand(Parent.CurrentUser);
            cmd.CommandText = "UPDATE \"UserProfiles\" " +
                "SET local_directory_id=@id " +
                "WHERE username=@user; " +
                "SELECT id FROM \"UserProfiles\" " +
                "WHERE username=@user;";
            cmd.Parameters.Add("@user", username);
            cmd.Parameters.Add("@id", userId);

            return MSSQLCEConn.ExecuteScalar<int>(cmd).Value;
        }

        /// <summary>
        /// Logouts the user session.
        /// </summary>
        /// <param name="sid">The sid.</param>
        /// <remarks>Documented by Dev05, 2009-01-16</remarks>
        public void LogoutUserSession(Guid sid) { }

        #endregion

        #region IDbUserConnector Members


        public string GetClientPwd(int lm_id, string client)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
