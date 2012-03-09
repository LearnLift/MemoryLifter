using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.Tools;
using SecurityFramework;

namespace MLifter.DAL.DB
{
    /// <summary>
    /// DB implementation of IUser.
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-14</remarks>
    class DbUser : IUser
    {
        private IDbUserConnector connector
        {
            get
            {
                switch (ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlUserConnector.GetInstance(new ParentClass(Parent.CurrentUser, null));
                    case DatabaseType.MsSqlCe:
                        return MLifter.DAL.DB.MsSqlCe.MsSqlCeUserConnector.GetInstance(new ParentClass(Parent.CurrentUser, null));
                    default:
                        throw new UnsupportedDatabaseTypeException(ConnectionString.Typ);
                }
            }
        }
        private IDbDictionariesConnector dictionariesConnector
        {
            get
            {
                switch (ConnectionString.Typ)
                {
                    case DatabaseType.PostgreSQL:
                        return PgSqlDictionariesConnector.GetInstance(new ParentClass(Parent.CurrentUser, null));
                    case DatabaseType.MsSqlCe:
                        return MsSqlCeDictionariesConnector.GetInstance(new ParentClass(Parent.CurrentUser, null));
                    default:
                        throw new UnsupportedDatabaseTypeException(ConnectionString.Typ);
                }
            }
        }

        private bool standAlone;
        private bool loggedIn = false;
        private bool cachePermissions = true;
        private UserStruct? user;
        private Framework securityFramework;
        private SecurityToken securityToken;

        internal DbUser(UserStruct? user, ParentClass parent, ConnectionStringStruct connection, DataAccessErrorDelegate errorMessageDelegate, bool standAlone)
        {
            this.parent = parent;
            connectionString = connection;
            ErrorMessageDelegate = errorMessageDelegate;

            cache = new Cache(true);

            if (!user.HasValue)
                throw new NoValidUserException();

            this.authenticationStruct = user.Value;
            this.username = user.Value.UserName;
            this.hashedPassword = user.Value.Password;

            this.standAlone = standAlone;
            this.user = user;

            securityFramework = MLifter.DAL.Security.SecurityFramework.GetDataAdapter(this);
            if (username != null && securityFramework != null)
            {
                try
                {
                    securityToken = securityFramework.CreateSecurityToken(this.username);
                    securityToken.IsCaching = cachePermissions;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to create security token! (" + ex.Message + ")");
                }
            }

            Login();
        }

        private int id;
        internal void SetId(int newId) { id = newId; }
        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public int Id { get { return id; } }

        #region IUser Members

        private UserStruct authenticationStruct;
        /// <summary>
        /// Gets the authentication struct.
        /// </summary>
        /// <value>The authentication struct.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public UserStruct AuthenticationStruct { get { return authenticationStruct; } }

        private string username;
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public string UserName { get { return username.ToLower(); } }

        private string hashedPassword;
        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>The password.</value>
        /// <remarks>Documented by Dev05, 2008-08-28</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public string Password
        {
            get
            {
                return hashedPassword;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can open a learning module.
        /// </summary>
        /// <value><c>true</c> if this instance can open; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-03-02</remarks>
        public bool CanOpen { get { return true; } }

        /// <summary>
        /// Opens the learning module selected in the connection string.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-09-10</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public IDictionary Open()
        {
            Cache.Clear();

            CheckConnection(connectionString);
            if (ConnectionString.LmId >= 0)
                connector.GetUserLearningModuleSettings(ConnectionString.LmId);

            switch (ConnectionString.Typ)
            {
                case DatabaseType.PostgreSQL:
                    return new DbDictionaries(Parent.GetChildParentClass(this)).Get(ConnectionString.LmId);
                case DatabaseType.MsSqlCe:
                    return new DbDictionary(ConnectionString.LmId, Parent.CurrentUser);
                default:
                    throw new UnsupportedDatabaseTypeException(ConnectionString.Typ);
            }
        }

        /// <summary>
        /// Lists all available learning modules.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-09-10</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public IDictionaries List()
        {
            return new DbDictionaries(Parent.GetChildParentClass(this));
        }

        /// <summary>
        /// Logins this user on the DB.
        /// </summary>
        /// <remarks>Documented by Dev05, 2008-12-17</remarks>
        public void Login()
        {
            if (loggedIn)
                return;

            if (connectionString.Typ != DatabaseType.MsSqlCe && connectionString.Typ != DatabaseType.Unc)
                switch (user.Value.AuthenticationType)
                {
                    case UserAuthenticationTyp.ListAuthentication:
                        id = connector.LoginListUser(user.Value.UserName, Parent.CurrentSessionId, user.Value.CloseOpenSessions, standAlone);
                        break;
                    case UserAuthenticationTyp.FormsAuthentication:
                        id = connector.LoginFormsUser(user.Value.UserName, Password, Parent.CurrentSessionId, user.Value.CloseOpenSessions, standAlone);
                        break;
                    case UserAuthenticationTyp.LocalDirectoryAuthentication:
                        id = connector.LoginLocalDirectoryUser(user.Value.UserName, string.Empty, Parent.CurrentSessionId, user.Value.CloseOpenSessions, standAlone);
                        break;
                    default:
                        throw new NoValidUserException();
                }

            loggedIn = true;
        }

        private ConnectionStringStruct connectionString;
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        /// <remarks>Documented by Dev02, 2008-09-23</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public ConnectionStringStruct ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
                // only try to get the LM id if the file db already exists
                if (!(connectionString.Typ == DatabaseType.MsSqlCe && !System.IO.File.Exists(connectionString.ConnectionString)))
                    if (connectionString.LmId < 0)
                    {
                        IList<int> ids = dictionariesConnector.GetLMIds();
                        if (ids.Count > 0)
                            connectionString.LmId = ids[0];
                    }
            }
        }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="newConnection">The new connection.</param>
        /// <remarks>Documented by Dev05, 2010-02-03</remarks>
        public void SetConnectionString(ConnectionStringStruct newConnection)
        {
            connectionString = newConnection;
        }

        /// <summary>
        /// Checks the connection and throws an exception depending on the error, if the connection is invalid.
        /// </summary>
        /// <param name="connection">The connection to test.</param>
        /// <remarks>Documented by Dev02, 2008-09-23</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public void CheckConnection(ConnectionStringStruct connection)
        {
            Parent.CurrentUser.CheckConnection(connection);
        }

        private Cache cache;
        /// <summary>
        /// Gets the cache for this user.
        /// </summary>
        /// <value>The cache.</value>
        /// <remarks>Documented by Dev05, 2008-09-24</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public Cache Cache { get { return cache; } }

        public void Logout()
        {
            try
            {
                if (loggedIn && connectionString.Typ != DatabaseType.MsSqlCe && connectionString.Typ != DatabaseType.Unc)
                    connector.LogoutUserSession(Parent.CurrentSessionId);

                if (connectionString.ServerUser != null && (connectionString.ServerUser as User).BaseUser != this)
                    connectionString.ServerUser.Logout();
            }
            catch (Exception exp)
            {
                Trace.WriteLine("User could not be logged out: " + exp.Message);
            }
            finally
            {
                loggedIn = false;
            }
        }

        /// <summary>
        /// Gets or sets the get login delegate.
        /// </summary>
        /// <value>The get login delegate.</value>
        /// <remarks>Documented by Dev05, 2008-12-12</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public GetLoginInformation GetLoginDelegate { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        private DataAccessErrorDelegate errMsgDel;
        /// <summary>
        /// Gets or sets the error message delegate.
        /// </summary>
        /// <value>The error message delegate.</value>
        /// <remarks>Documented by Dev05, 2008-11-17</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public DataAccessErrorDelegate ErrorMessageDelegate { get { return errMsgDel; } set { errMsgDel = value; } }

        /// <summary>
        /// Gets the underlying database for this user.
        /// </summary>
        /// <value>The database.</value>
        /// <remarks>Documented by Dev03, 2009-05-01</remarks>
        public IDatabase Database
        {
            get { return DbDatabase.GetInstance(Parent); }
        }

        /// <summary>
        /// Gets the security framework.
        /// </summary>
        /// <value>The security framework.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public Framework SecurityFramework
        {
            get { return securityFramework; }
        }

        public bool HasPermission(object o, string permissionName)
        {
            // local SQL CE DBs do not need security
            if (this.ConnectionString.Typ == DatabaseType.MsSqlCe && this.ConnectionString.SyncType == SyncType.NotSynchronized)
                return true;
            // no permission if no token exists
            if (securityToken == null) return false;
            if (o is DbChapters || o is DbCards)
                return securityToken.HasPermission((o as IParent).Parent.GetParentDictionary(), permissionName);
            else
                return securityToken.HasPermission(o, permissionName);
        }

        public List<PermissionInfo> GetPermissions(object o)
        {
            if (securityToken == null) return new List<PermissionInfo>();
            if (o is DbChapters || o is DbCards)
                return securityToken.GetPermissions((o as IParent).Parent.GetParentDictionary());
            else
                return securityToken.GetPermissions(o);
        }
        #endregion

        #region IParent Members

        private ParentClass parent;
        public ParentClass Parent
        {
            get { return parent; }
        }

        #endregion


        #region IUser Members


        public string getClientPwd(int lmId, string client)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
