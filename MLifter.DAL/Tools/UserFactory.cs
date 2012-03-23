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
using System.DirectoryServices.Protocols;
using System.Security;
using System.Security.Principal;
using System.Text;
using MLifter.DAL.DB;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;
using MLifter.DAL.LearningModulesService;
using MLifter.DAL.Tools;
using MLifter.DAL.XML;
using Npgsql;
using SecurityFramework;
using MLifter.Generics;

namespace MLifter.DAL
{
	/// <summary>
	/// This class is for creating a user and stores the connection and the current user.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-09-10</remarks>
	public static class UserFactory
	{
		private static IDbUserConnector GetConnector(ConnectionStringStruct connection)
		{
			switch (connection.Typ)
			{
				case DatabaseType.PostgreSQL:
					return PgSqlUserConnector.GetInstance(new ParentClass(new DummyUser(connection), null));
				case DatabaseType.MsSqlCe:
					return MsSqlCeUserConnector.GetInstance(new ParentClass(new DummyUser(connection), null));
				default:
					throw new UnsupportedDatabaseTypeException(connection.Typ);
			}
		}

		private static Dictionary<object, IUser> currentUser = new Dictionary<object, IUser>();
		/// <summary>
		/// Gets the current user.
		/// </summary>
		/// <value>The current user.</value>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		[Obsolete("Use Parent.CurrentUser!")]
		public static Dictionary<object, IUser> CurrentUser
		{
			get { return currentUser; }
		}

		/// <summary>
		/// Creates the specified user depending on the settings and the user received through the login method delegate.
		/// </summary>
		/// <param name="loginMethodDelegate">The login method delegate.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="errorMessageDelegate">The error message delegate.</param>
		/// <param name="userContext">The context to which the user belongs (e.g. the LearnLogic).</param>
		/// <returns>A user</returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		public static IUser Create(GetLoginInformation loginMethodDelegate, ConnectionStringStruct connectionString, DataAccessErrorDelegate errorMessageDelegate, object userContext)
		{
			return Create(loginMethodDelegate, connectionString, errorMessageDelegate, userContext, false);
		}

		/// <summary>
		/// Creates the specified user depending on the settings and the user received through the login method delegate.
		/// </summary>
		/// <param name="loginMethodDelegate">The login method delegate.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="errorMessageDelegate">The error message delegate.</param>
		/// <param name="userContext">The context to which the user belongs (e.g. the LearnLogic).</param>
		/// <param name="standAlone">if set to <c>true</c> the user is stand alone (does not affect other [e.g. close session]).</param>
		/// <returns>A user</returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		public static IUser Create(GetLoginInformation loginMethodDelegate, ConnectionStringStruct connectionString, DataAccessErrorDelegate errorMessageDelegate, object userContext, bool standAlone)
		{
			if (currentUser.ContainsKey(userContext) && !standAlone && !(connectionString.Typ == DatabaseType.MsSqlCe && connectionString.SyncType == SyncType.HalfSynchronizedWithDbAccess))
				currentUser[userContext].Logout();

			User newUser = new User(loginMethodDelegate, connectionString, errorMessageDelegate, standAlone);

			if (!standAlone)
				currentUser[userContext] = newUser;

			return newUser;
		}

		/// <summary>
		/// Gets the user list.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		/// <returns>A list of users available on the db</returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		public static IList<UserStruct> GetUserList(ConnectionStringStruct connectionString)
		{
			if (connectionString.Typ == DatabaseType.Web)
			{
				MLifterLearningModulesService webService = new MLifterLearningModulesService();
				webService.Url = connectionString.ConnectionString;
				List<UserStruct> users = new List<UserStruct>();
				foreach (KeyValuePair<string, UserAuthenticationTyp> pair in webService.GetUserList())
					users.Add(new UserStruct(pair.Key, pair.Value));
				return users;
			}
			return GetConnector(connectionString).GetUserList();
		}
	}

	/// <summary>
	/// Dummy user which only stores a connection or a id.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-09-24</remarks>
	[Serializable()]
	public class DummyUser : IUser
	{
		private int id;
		private string username = string.Empty;
		private ConnectionStringStruct connectionString;

		/// <summary>
		/// Initializes a new instance of the <see cref="DummyUser"/> class.
		/// </summary>
		/// <param name="conStr">The con STR.</param>
		public DummyUser(ConnectionStringStruct conStr)
		{
			connectionString = conStr;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DummyUser"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="user">The user.</param>
		public DummyUser(int id, string user)
		{
			this.id = id;
			username = user;
		}

		#region IUser Members

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public int Id
		{
			get { return id; }
		}

		/// <summary>
		/// Gets the authentication struct.
		/// </summary>
		/// <value>The authentication struct.</value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public UserStruct AuthenticationStruct
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public string UserName
		{
			get { return username; }
		}

		/// <summary>
		/// Gets the password. DO NOT STORE THIS IN A string-VARIABLE TO ENSURE A SECURE STORAGE!!!
		/// </summary>
		/// <value>The password.</value>
		/// <remarks>Documented by Dev05, 2008-08-28</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public string Password
		{
			get;
			set;
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
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public IDictionary Open()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Lists all available learning modules.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public IDictionaries List()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Logins this user.
		/// </summary>
		public void Login() { }

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
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
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public void CheckConnection(ConnectionStringStruct connection)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		private Cache cache;
		/// <summary>
		/// Gets the cache for this user.
		/// </summary>
		/// <value>The cache.</value>
		/// <remarks>Documented by Dev05, 2008-09-24</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public Cache Cache
		{
			get
			{
				if (cache == null)
					cache = new Cache(true);
				return cache;
			}
		}

		/// <summary>
		/// Gets or sets the get login delegate.
		/// </summary>
		/// <value>
		/// The get login delegate.
		/// </value>
		public GetLoginInformation GetLoginDelegate { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

		private DataAccessErrorDelegate errMsgDel;
		/// <summary>
		/// Gets or sets the error message delegate.
		/// </summary>
		/// <value>The error message delegate.</value>
		/// <remarks>Documented by Dev05, 2008-11-17</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public DataAccessErrorDelegate ErrorMessageDelegate { get { return errMsgDel; } set { errMsgDel = value; } }

		/// <summary>
		/// Closes the session.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public void Logout() { }


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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}
		#endregion

		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public ParentClass Parent
		{
			get { return null; }
		}

		#endregion

		/// <summary>
		/// Gets the client PWD for a LM.
		/// </summary>
		/// <param name="lmId"></param>
		/// <param name="client"></param>
		/// <returns></returns>
		public string getClientPwd(int lmId, string client)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// This is the delegate to receive user authentication information.
	/// </summary>
	public delegate UserStruct? GetLoginInformation(UserStruct user, ConnectionStringStruct connection);

	/// <summary>
	/// This delegate is to inform of error during the data access.
	/// </summary>
	public delegate void DataAccessErrorDelegate(object sender, Exception exp);

	/// <summary>
	/// The connection is not set!
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-09-10</remarks>
	public class ConnectionNotSetException : Exception { }
	/// <summary>
	/// The connection is not valide.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-09-10</remarks>
	public class ConnectionInvalidException : Exception { }
	/// <summary>
	/// There was no valid user submitted.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-09-10</remarks>
	public class NoValidUserException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NoValidUserException"/> class.
		/// </summary>
		/// <param name="innerException">The inner exception.</param>
		public NoValidUserException(Exception innerException)
			: base(innerException == null ? string.Empty : innerException.Message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="NoValidUserException"/> class.
		/// </summary>
		public NoValidUserException()
		{ }
	}

	/// <summary>
	/// Stores the general information about an user.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-09-10</remarks>
	public struct UserStruct
	{
		/// <summary>
		/// The username of this user.
		/// </summary>
		public string UserName;
		/// <summary>
		/// The password (hashed).
		/// </summary>
		public string Password;
		/// <summary>
		/// The local directory identifier.
		/// </summary>
		public string Identifier;
		/// <summary>
		/// The typ of authentification of this user.
		/// </summary>
		public UserAuthenticationTyp? AuthenticationType;
		/// <summary>
		/// Cloe open sessions if ther are one.
		/// </summary>
		public bool CloseOpenSessions;
		/// <summary>
		/// The error which occured during the last login atemt.
		/// </summary>
		public LoginError LastLoginError;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserStruct"/> struct.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <remarks>Documented by Dev05, 2009-03-04</remarks>
		private UserStruct(string username)
		{
			UserName = username;
			Password = string.Empty;
			Identifier = string.Empty;
			AuthenticationType = null;
			CloseOpenSessions = false;
			LastLoginError = LoginError.NoError;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UserStruct"/> struct.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="authTyp">The auth typ.</param>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public UserStruct(string username, UserAuthenticationTyp authTyp)
			: this(username)
		{
			AuthenticationType = authTyp;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="UserStruct"/> struct.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="identifier">The identifier.</param>
		/// <param name="authTyp">The auth typ.</param>
		/// <remarks>
		/// Documented by CFI, 13.01.2009.
		/// </remarks>
		public UserStruct(string username, string identifier, UserAuthenticationTyp authTyp)
			: this(username)
		{
			Identifier = identifier;
			AuthenticationType = authTyp;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="UserStruct"/> struct.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="authTyp">The auth typ.</param>
		/// <param name="closeOpenSessions">if set to <c>true</c> [close open sessions].</param>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public UserStruct(string username, UserAuthenticationTyp authTyp, bool closeOpenSessions)
			: this(username)
		{
			AuthenticationType = authTyp;
			CloseOpenSessions = closeOpenSessions;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="UserStruct"/> struct.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="authTyp">The auth typ.</param>
		/// <param name="passwordIsHashed">if set to <c>true</c> password is already hashed.</param>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public UserStruct(string username, string password, UserAuthenticationTyp authTyp, bool passwordIsHashed)
			: this(username)
		{
			Password = passwordIsHashed ? password : Methods.GetHashedPassword(password);
			AuthenticationType = authTyp;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="UserStruct"/> struct.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="authTyp">The auth typ.</param>
		/// <param name="passwordIsHashed">if set to <c>true</c> password is hashed.</param>
		/// <param name="closeOpenSessions">if set to <c>true</c> close open sessions.</param>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public UserStruct(string username, string password, UserAuthenticationTyp authTyp, bool passwordIsHashed, bool closeOpenSessions)
			: this(username)
		{
			Password = passwordIsHashed ? password : Methods.GetHashedPassword(password);
			AuthenticationType = authTyp;
			CloseOpenSessions = closeOpenSessions;
		}

		/// <summary>
		/// Returns the username.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing the username.
		/// </returns>
		/// <remarks>Documented by Dev03, 2008-11-27</remarks>
		public override string ToString()
		{
			return UserName;
		}
	}

	/// <summary>
	/// The authentication typ of the user.
	/// </summary>
	[Flags]
	public enum UserAuthenticationTyp
	{
		/// <summary>
		/// The user is only authenticated by it's username.
		/// </summary>
		ListAuthentication = 1,
		/// <summary>
		/// The user is authenticated by a username / password combination.
		/// </summary>
		FormsAuthentication = 2,
		/// <summary>
		/// The user is authenticated by his local directory user profile.
		/// </summary>
		LocalDirectoryAuthentication = 4
	}

	/// <summary>
	/// The type of the used local directory.
	/// </summary>
	public enum LocalDirectoryType
	{
		/// <summary>
		/// Microsoft Active Directory Services
		/// </summary>
		ActiveDirectory,
		/// <summary>
		/// Novell eDirectory (formal NDS - Novell directory service)
		/// </summary>
		eDirectory
	}

	/// <summary>
	/// The type of the error which occured during the login procedure.
	/// </summary>
	[Flags]
	public enum LoginError
	{
		/// <summary>
		/// The last login was successfull.
		/// </summary>
		NoError = 0,
		/// <summary>
		/// The given username was wrong.
		/// </summary>
		InvalidUsername = 1,
		/// <summary>
		/// The given password was wrong.
		/// </summary>
		InvalidPassword = 2,
		/// <summary>
		/// The given user/authenticationtyp wasn't a valid combination (e.g. you tried to login a forms user as a list user).
		/// </summary>
		WrongAuthentication = 4,
		/// <summary>
		/// The given authentication typ is not allowd on this database.
		/// </summary>
		ForbiddenAuthentication = 8,
		/// <summary>
		/// The selected User is already logged in!
		/// </summary>
		AlreadyLoggedIn = 16
	}
}
