using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.XML;
using MLifter.DAL.DB;
using MLifter.DAL.Tools;
using SecurityFramework;

namespace MLifter.DAL
{
    /// <summary>
    /// UNC implementation of IUser. This combines all file db user types (XML and SQL CE).
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-14</remarks>
    internal class UncUser : IUser
    {
        private XmlUser xmlUser;
        private DbUser dbUser;
        private ConnectionStringStruct con;

        internal UncUser(XmlUser xmluser, DbUser dbuser, ConnectionStringStruct connection)
        {
            xmlUser = xmluser;
            dbUser = dbuser;
            con = connection;
        }

        #region IUser Members

        public int Id
        {
            get { return dbUser.Id; }
        }

        public UserStruct AuthenticationStruct
        {
            get { return dbUser.AuthenticationStruct; }
        }

        public string UserName
        {
            get { return string.Empty; }
        }

        public string Password
        {
            get { throw new NotImplementedException(); }
        }

        public GetLoginInformation GetLoginDelegate
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DataAccessErrorDelegate ErrorMessageDelegate
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can open a learning module.
        /// </summary>
        /// <value><c>true</c> if this instance can open; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-03-02</remarks>
        public bool CanOpen { get { return false; } }

        /// <summary>
        /// Opens the learning module selected in the connection string.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-09-10</remarks>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public IDictionary Open()
        {
            throw new NotImplementedException();
        }

        public IDictionaries List()
        {
            return new UncDictionaries(xmlUser.List(), dbUser.List());
        }

        public ConnectionStringStruct ConnectionString
        {
            get
            {
                return con;
            }
            set
            {
                con = value;
                if (con.Typ == DatabaseType.MsSqlCe)
                    dbUser.ConnectionString = con;
                else
                    xmlUser.ConnectionString = con;
            }
        }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="css">The CSS.</param>
        /// <remarks>Documented by Dev05, 2010-02-03</remarks>
        public void SetConnectionString(ConnectionStringStruct css)
        {
            con = css;
            if (con.Typ == DatabaseType.MsSqlCe)
                dbUser.SetConnectionString(con);
            else
                xmlUser.SetConnectionString(con);
        }

        public void CheckConnection(ConnectionStringStruct connection)
        {
            throw new NotImplementedException();
        }

        public void Login()
        {
            xmlUser.Login();
            dbUser.Login();
        }

        public void Logout()
        {
            xmlUser.Logout();
            dbUser.Logout();
        }

        public Cache Cache
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the underlying database for this user.
        /// </summary>
        /// <value>The database.</value>
        /// <remarks>Documented by Dev03, 2009-05-01</remarks>
        public IDatabase Database
        {
            get { throw new NotImplementedException(); }
        }

        public Framework SecurityFramework
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasPermission(object o, string permissionName)
        {
            return true;
        }

        public List<PermissionInfo> GetPermissions(object o)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IParent Members

        public MLifter.DAL.Tools.ParentClass Parent
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public string getClientPwd(int lmId, string client)
        {
            throw new NotImplementedException();
        }
    }
}
