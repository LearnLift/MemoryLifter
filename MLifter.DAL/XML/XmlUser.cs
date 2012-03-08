using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;
using SecurityFramework;

namespace MLifter.DAL.XML
{
    /// <summary>
    /// Xml implementation of IUser
    /// </summary>
    /// <remarks>Documented by Dev03, 2009-01-14</remarks>
    class XmlUser : IUser
    {
        internal XmlUser(ConnectionStringStruct connection, DataAccessErrorDelegate errorMessageDelegate) { connectionString = connection; ErrorMessageDelegate = errorMessageDelegate; }

        private IUser user = null;

        #region IUser Members

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public int Id { get { return -1; } }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public string UserName { get { return string.Empty; } }

        /// <summary>
        /// Gets the password. DO NOT STORE THIS IN A string-VARIABLE TO ENSURE A SECURE STORAGE!!!
        /// </summary>
        /// <value>The password.</value>
        /// <remarks>Documented by Dev05, 2008-08-28</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public string Password { get { return string.Empty; } }

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
            user = Parent.CurrentUser;
            if (ConnectionString.ReadOnly)
                return new XmlDictionary(ConnectionString.ConnectionString, true, XmlDictionary.AccessMode.ForPreview, user);
            else
                return new XmlDictionary(ConnectionString.ConnectionString, true, user);
        }

        /// <summary>
        /// Lists all available learning modules.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2008-09-10</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public IDictionaries List()
        {
            return new XmlDictionaries(ConnectionString.LearningModuleFolder, Parent.GetChildParentClass(this));
        }

        /// <summary>
        /// Gets the authentication struct.
        /// </summary>
        /// <value>The authentication struct.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public UserStruct AuthenticationStruct
        {
            get { return new UserStruct(); }
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
            }
        }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="css">The CSS.</param>
        /// <remarks>Documented by Dev05, 2010-02-03</remarks>
        public void SetConnectionString(ConnectionStringStruct css)
        {
            connectionString = css;
        }

        /// <summary>
        /// Logins this user.
        /// </summary>
        /// <remarks>Documented by Dev05, 2008-12-17</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public void Login() { }

        /// <summary>
        /// Checks the connection and throws an exception depending on the error, if the connection is invalid.
        /// </summary>
        /// <param name="connection">The connection to test.</param>
        /// <remarks>Documented by Dev02, 2008-09-23</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public void CheckConnection(ConnectionStringStruct connection)
        {
            throw new NotImplementedException("This method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the cache for this user.
        /// </summary>
        /// <value>The cache.</value>
        /// <remarks>Documented by Dev05, 2008-09-24</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public Cache Cache { get { return null; } }

        /// <summary>
        /// Logs this user out.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-11-13</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public void Logout()
        { }

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
        /// Gets the security framework.
        /// </summary>
        /// <value>The security framework.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public Framework SecurityFramework
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

        /// <summary>
        /// Determines whether the specified object has the given permission.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <param name="permissionName">Name of the permission.</param>
        /// <returns>
        /// 	<c>true</c> if the specified o has permission; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public bool HasPermission(object o, string permissionName)
        {
            return true;
        }

        /// <summary>
        /// Gets the permissions for an object.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>The list of permissions.</returns>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public List<PermissionInfo> GetPermissions(object o)
        {
            return new List<PermissionInfo>();
        }
        #endregion

        #region IParent Members

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev03, 2009-01-14</remarks>
        public ParentClass Parent
        {
            get { return new ParentClass(this, null); }
        }

        #endregion

        public string getClientPwd(int lmId, string client)
        {
            throw new NotImplementedException();
        }
    }
}
