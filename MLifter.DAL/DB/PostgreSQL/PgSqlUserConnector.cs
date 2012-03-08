using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces.DB;
using Npgsql;
using MLifter.DAL.Tools;
using MLifter.DAL.Interfaces;
using MLifter.Generics;

namespace MLifter.DAL.DB.PostgreSQL
{
    class PgSqlUserConnector : IDbUserConnector
    {
        private static Dictionary<ConnectionStringStruct, PgSqlUserConnector> instances = new Dictionary<ConnectionStringStruct, PgSqlUserConnector>();
        public static PgSqlUserConnector GetInstance(ParentClass parentClass)
        {
            lock (instances)
            {
                ConnectionStringStruct connection = parentClass.CurrentUser.ConnectionString;

                if (!instances.ContainsKey(connection))
                    instances.Add(connection, new PgSqlUserConnector(parentClass));

                return instances[connection];
            }
        }

        private ParentClass Parent;
        private PgSqlUserConnector(ParentClass parentClass)
        {
            Parent = parentClass;
            Parent.DictionaryClosed += new EventHandler(Parent_DictionaryClosed);
        }

        void Parent_DictionaryClosed(object sender, EventArgs e)
        {
            IParent parent = sender as IParent;
            instances.Remove(parent.Parent.CurrentUser.ConnectionString);
        }

        #region IDbUserConnector Members

        public IList<UserStruct> GetUserList()
        {
            IList<UserStruct> users = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserList, 0)] as IList<UserStruct>;
            if (users != null)
                return users;

            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"GetUserList\"()";

                    NpgsqlDataReader reader = PostgreSQLConn.ExecuteReader(cmd, Parent.CurrentUser, false);

                    users = new List<UserStruct>();
                    while (reader.Read())
                    {
                        UserStruct user = new UserStruct(reader["username"].ToString(),
                            (UserAuthenticationTyp)Enum.Parse(typeof(UserAuthenticationTyp), reader["typ"].ToString()));

                        users.Add(user);
                    }
                    reader.Close();

                    Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.Create(CacheObject.UserList, 0, new TimeSpan(0, 0, 30))] = users;

                    return users;
                }
            }
        }

        public UserAuthenticationTyp? GetAllowedAuthenticationModes()
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                bool list_auth, forms_auth, ld_auth;
                list_auth = forms_auth = ld_auth = false;

                bool? val = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ListAuthentication, 0)] as bool?;
                if (!val.HasValue)
                    PgSqlDatabaseConnector.GetDatabaseValues(Parent);
                list_auth = (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.ListAuthentication, 0)] as bool?).Value;

                val = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.FormsAuthentication, 0)] as bool?;
                if (!val.HasValue)
                    PgSqlDatabaseConnector.GetDatabaseValues(Parent);
                forms_auth = (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.FormsAuthentication, 0)] as bool?).Value;

                val = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LocalDirectoryAuthentication, 0)] as bool?;
                if (!val.HasValue)
                    PgSqlDatabaseConnector.GetDatabaseValues(Parent);
                ld_auth = (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LocalDirectoryAuthentication, 0)] as bool?).Value;

                if (list_auth)
                {
                    if (forms_auth)
                    {
                        if (ld_auth)
                            return (UserAuthenticationTyp.ListAuthentication | UserAuthenticationTyp.FormsAuthentication | UserAuthenticationTyp.LocalDirectoryAuthentication);
                        else
                            return (UserAuthenticationTyp.ListAuthentication | UserAuthenticationTyp.FormsAuthentication);
                    }
                    else
                    {
                        if (ld_auth)
                            return (UserAuthenticationTyp.ListAuthentication | UserAuthenticationTyp.LocalDirectoryAuthentication);
                        else
                            return (UserAuthenticationTyp.ListAuthentication);
                    }
                }
                else
                {
                    if (forms_auth)
                    {
                        if (ld_auth)
                            return (UserAuthenticationTyp.FormsAuthentication | UserAuthenticationTyp.LocalDirectoryAuthentication);
                        else
                            return (UserAuthenticationTyp.FormsAuthentication);
                    }
                    else
                    {
                        if (ld_auth)
                            return (UserAuthenticationTyp.LocalDirectoryAuthentication);
                        else
                            return null;            //Errors
                    }
                }
            }
        }

        public LocalDirectoryType? GetLocalDirectoryType()
        {
            LocalDirectoryType? ld_typ = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LocalDirectoryType, 0)] as LocalDirectoryType?;
            if (ld_typ.HasValue)
                return ld_typ;

            PgSqlDatabaseConnector.GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LocalDirectoryType, 0)] as LocalDirectoryType?;
        }

        public string GetLdapServer()
        {
            string ldap = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapServer, 0)] as string;
            if (ldap != null && ldap.Length > 0)
                return ldap;

            PgSqlDatabaseConnector.GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapServer, 0)] as string;
        }

        public int GetLdapPort()
        {
            int? ldap = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapPort, 0)] as int?;
            if (ldap.HasValue)
                return ldap.Value;

            PgSqlDatabaseConnector.GetDatabaseValues(Parent);

            return (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapPort, 0)] as int?).Value;
        }

        public string GetLdapUser()
        {
            string ldap = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapUser, 0)] as string;
            if (ldap != null)
                return ldap;

            PgSqlDatabaseConnector.GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapUser, 0)] as string;
        }

        public string GetLdapPassword()
        {
            string ldap = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapPassword, 0)] as string;
            if (ldap != null)
                return ldap;

            PgSqlDatabaseConnector.GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapPassword, 0)] as string;
        }

        public string GetLdapContext()
        {
            string ldap = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapContext, 0)] as string;
            if (ldap != null && ldap.Length > 0)
                return ldap;

            PgSqlDatabaseConnector.GetDatabaseValues(Parent);

            return Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapContext, 0)] as string;
        }

        public bool GetLdapUseSSL()
        {
            bool? ldap = Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapUseSsl, 0)] as bool?;
            if (ldap.HasValue)
                return ldap.Value;

            PgSqlDatabaseConnector.GetDatabaseValues(Parent);

            return (Parent.CurrentUser.Cache[ObjectLifetimeIdentifier.GetIdentifier(CacheObject.LdapUseSsl, 0)] as bool?).Value;
        }

        public int GetUserLearningModuleSettings(int id)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"GetUserSettings\"(:uid, :lm_id);";
                    cmd.Parameters.Add("uid", Parent.CurrentUser.Id);
                    cmd.Parameters.Add("lm_id", id);

                    return PostgreSQLConn.ExecuteScalar<int>(cmd, Parent.CurrentUser).Value;
                }
            }
        }

        public int LoginListUser(string username, Guid sid, bool closeOpenSessions, bool standAlone)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"LoginListUser\"(:user, :sessionid, :close_open_sessions, :standalone)";
                    cmd.Parameters.Add("user", username.ToLower());
                    cmd.Parameters.Add("sessionid", sid.ToString());
                    cmd.Parameters.Add("close_open_sessions", closeOpenSessions);
                    cmd.Parameters.Add("standalone", standAlone);
                    int value = (int)PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser, false);

                    Methods.CheckUserId(value);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserList, 0));

                    return value;
                }
            }
        }

        public int LoginFormsUser(string username, string password, Guid sid, bool closeOpenSessions, bool standAlone)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"LoginFormsUser\"(:user, :pass, :sessionid, :close_open_sessions, :standalone)";
                    cmd.Parameters.Add("user", username.ToLower());
                    cmd.Parameters.Add("pass", password);
                    cmd.Parameters.Add("sessionid", sid.ToString());
                    cmd.Parameters.Add("close_open_sessions", closeOpenSessions);
                    cmd.Parameters.Add("standalone", standAlone);
                    int value = (int)PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser, false);

                    Methods.CheckUserId(value);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserList, 0));

                    return value;
                }
            }
        }

        public int LoginLocalDirectoryUser(string username, string localDirectoryIdentifier, Guid sid, bool closeOpenSessions, bool standAlone)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM \"LoginLocalDirectoryUser\"(:user, :pass, :sessionid, :close_open_sessions, :standalone)";
                    cmd.Parameters.Add("user", username.ToLower());
                    cmd.Parameters.Add("pass", localDirectoryIdentifier);
                    cmd.Parameters.Add("sessionid", sid.ToString());
                    cmd.Parameters.Add("close_open_sessions", closeOpenSessions);
                    cmd.Parameters.Add("standalone", standAlone);
                    int value = (int)PostgreSQLConn.ExecuteScalar(cmd, Parent.CurrentUser, false);

                    Methods.CheckUserId(value);
                    Parent.CurrentUser.Cache.Uncache(ObjectLifetimeIdentifier.GetIdentifier(CacheObject.UserList, 0));

                    return value;
                }
            }
        }

        public void LogoutUserSession(Guid sid)
        {
            using (NpgsqlConnection con = PostgreSQLConn.CreateConnection(Parent.CurrentUser))
            {
                using (NpgsqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT \"LogoutUser\"(:sessionid)";
                    cmd.Parameters.Add("sessionid", sid.ToString());
                    PostgreSQLConn.ExecuteNonQuery(cmd, Parent.CurrentUser);
                    Parent.GenerateNewSession();
                }
            }
        }

        #endregion

        #region IDbUserConnector Members




        #endregion
    }
}
