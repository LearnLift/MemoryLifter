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
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Web;
using System.Web.Services;
using MLifter.BusinessLayer;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Generics;
using Npgsql;

namespace MLifterSyncService
{
    /// <summary>
    /// Summary Description for MLifterLearningModulesService
    /// </summary>
    [WebService(Namespace = "http://www.memorylifter.com/sync/learningmodules/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class MLifterLearningModulesService : System.Web.Services.WebService
    {
        private static bool logWritten = false;

        public MLifterLearningModulesService()
        {
            MLifter.DAL.User.IsWebService = true;
            try
            {
                if ((int)Session["uid"] <= 0)
                    Session["uid"] = -1;
            }
            catch { Session["uid"] = -1; }
            Session["lmIndex"] = null;

            if (!logWritten)
            {
                WriteLogEntry("MLifterLearningModulesService started.");
                logWritten = true;
            }
        }

        private NpgsqlConnection GetPgConnection()
        {
            NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString);
            conn.Open();
            return conn;
        }

        [WebMethod(Description = "Authenticates the user.", EnableSession = true)]
        public int Login(string username, string password)
        {
            using (NpgsqlConnection con = GetPgConnection())
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    //check user name
                    cmd.CommandText = "SELECT id FROM \"UserProfiles\" WHERE username=:user;";
                    cmd.Parameters.Add("user", username);

                    object result = cmd.ExecuteScalar();
                    int uid = (result == null || result is DBNull) ? -1 : Convert.ToInt32(result);

                    if (uid >= 0)
                    {
                        //check authentication type
                        cmd.CommandText = "SELECT id FROM \"UserProfiles\" WHERE id=:userid AND user_type=:usertype";
                        cmd.Parameters.Add("userid", uid);
                        cmd.Parameters.Add("usertype", UserAuthenticationTyp.FormsAuthentication.ToString()); //only allow forms authentication
                        result = cmd.ExecuteScalar();
                        uid = (result == null || result is DBNull) ? -4 : Convert.ToInt32(result);

                        if (uid >= 0)
                        {
                            //check user password
                            cmd.CommandText = "SELECT id FROM \"UserProfiles\" WHERE id=:userid AND password=:pass";
                            cmd.Parameters.Add("pass", password);
                            result = cmd.ExecuteScalar();
                            uid = (result == null || result is DBNull) ? -2 : Convert.ToInt32(result);
                        }

                    }
                    Session["uid"] = uid;
                    Session["username"] = username;
                    Session["password"] = password;

                    return uid;
                }
            }
        }
        [WebMethod(Description = "Gets the list of users from the server.", EnableSession = true)]
        public SerializableDictionary<string, UserAuthenticationTyp> GetUserList()
        {
            SerializableDictionary<string, UserAuthenticationTyp> users = new SerializableDictionary<string, UserAuthenticationTyp>();

            using (NpgsqlConnection con = GetPgConnection())
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"GetUserList\"()";

                    NpgsqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);

                    while (reader.Read())
                    {
                        string username = reader["username"].ToString();
                        UserAuthenticationTyp authTyp = (UserAuthenticationTyp)Enum.Parse(typeof(UserAuthenticationTyp), reader["typ"].ToString());

                        if (authTyp == UserAuthenticationTyp.FormsAuthentication) //only allow forms authentication
                            users.Add(username, authTyp);
                    }

                    reader.Close();
                }
            }

            return users;
        }

        [WebMethod(Description = "Gets the list of learning modules from the server.", EnableSession = true)]
        public SerializableDictionary<int, string> GetLearningModulesList()
        {
            if ((int)Session["uid"] < 0)
                throw new NoValidUserException();

            SerializableDictionary<int, string> LMs = new SerializableDictionary<int, string>();

            using (NpgsqlConnection con = GetPgConnection())
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, title FROM \"LearningModules\"";
                    NpgsqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SequentialAccess);

                    while (reader.Read())
                        LMs.Add(Convert.ToInt32(reader["id"]), reader["title"].ToString());
                }
            }

            return LMs;
        }
        private static IFormatter formatter = new BinaryFormatter();
        [WebMethod(Description = "Gets a detailed learning module index entry.", EnableSession = true)]
        public byte[] GetLearningModuleIndexEntry(int learningModuleId, string clientId)
        {
            try
            {
                if ((int)Session["uid"] < 0)
                    throw new NoValidUserException();

                string key = string.Format("user-{0}", (int)Session["uid"]);
                string lmKey = string.Format("lm-{0}", learningModuleId);

                IUser user = null;
                lock (formatter)
                {
                    try
                    {
                        user = HttpContext.Current.Cache[key] as IUser;
                    }
                    catch { }
                    if (user == null)
                    {
                        user = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c)
                                {
                                    if (u.LastLoginError != LoginError.NoError)
                                        throw new InvalidCredentialsException("Some of the submited credentials are wrong!");

                                    string username = Session["username"].ToString();
                                    string password = Session["password"].ToString();
                                    UserAuthenticationTyp authType = password == string.Empty ? UserAuthenticationTyp.ListAuthentication : UserAuthenticationTyp.FormsAuthentication;

                                    return new UserStruct(username, password, authType, true, true);
                                }, new ConnectionStringStruct(DatabaseType.PostgreSQL, ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString, -1),
                                    (DataAccessErrorDelegate)delegate { return; }, ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString, true);

                        HttpContext.Current.Cache[key] = user;
                    }
                }

                IDictionary dic = new MLifter.DAL.DB.DbDictionary(learningModuleId, user);
                LearningModulesIndexEntry entry = new LearningModulesIndexEntry();
                entry.User = user;
                entry.Dictionary = dic;
                entry.DisplayName = dic.Title;
                entry.Type = LearningModuleType.Remote;
                entry.Type = LearningModuleType.Remote;
                entry.ConnectionString = new ConnectionStringStruct(DatabaseType.PostgreSQL, dic.Connection, dic.Id, user.ConnectionString.SessionId);

                LearningModulesIndex.LoadEntry(entry);

                if (entry.Dictionary.ContentProtected)
                {
                    // MemoryLifter > 2.3 does not support DRM protected content
                    entry.IsAccessible = false;
                    entry.NotAccessibleReason = LearningModuleNotAccessibleReason.Protected;
                }

                ConnectionStringStruct conString = entry.ConnectionString;
                conString.ConnectionString = string.Empty;
                entry.ConnectionString = conString;

                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, entry);

                return stream.ToArray();
            }
            catch (Exception exp)
            {
                try
                {
                    WriteLogEntry(exp.ToString());
                }
                catch
                {
                    throw exp;
                }

                return null;
            }
        }

        /// <summary>
        /// Writes the log entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-09-01</remarks>
        private void WriteLogEntry(string entry)
        {
            try
            {
                entry = DateTime.Now.ToString() + " " + entry + Environment.NewLine;
                File.AppendAllText(Server.MapPath(ConfigurationManager.AppSettings["LogPath"]), entry);
            }
            catch
            {
                throw;
            }
        }
    }
}
