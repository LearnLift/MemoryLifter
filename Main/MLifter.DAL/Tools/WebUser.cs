using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Services.Protocols;
using MLifter.DAL.Interfaces;
using MLifter.DAL.LearningModulesService;
using MLifter.DAL.Tools;
using MLifter.Generics;

namespace MLifter.DAL
{
	/// <summary>
	/// A user to a web service.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-03-06</remarks>
	public class WebUser : IUser
	{
		/// <summary>
		/// Gets or sets the web service.
		/// </summary>
		/// <value>The web service.</value>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public MLifterLearningModulesService WebService { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WebUser"/> class.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="authenticationStruct">The authentication struct.</param>
		/// <param name="connection">The connection.</param>
		/// <param name="service">The service.</param>
		/// <param name="parent">The parent.</param>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		internal WebUser(int userId, UserStruct authenticationStruct, ConnectionStringStruct connection, MLifterLearningModulesService service, ParentClass parent)
		{
			id = userId;
			authStruct = authenticationStruct;
			ConnectionString = connection;
			WebService = service;
			this.parent = parent;
		}

		/// <summary>
		/// Gets the learning module data.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>A binary serialization of the LearningModulesIndex. [null] if the server is offline.</returns>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public byte[] GetLearningModuleData(int id)
		{
			try
			{
				return WebService.GetLearningModuleIndexEntry(id, Methods.GetMID());
			}
			catch (WebException)
			{
				return null;
			}
			catch (SoapException)
			{
				return null;
			}
		}
		/// <summary>
		/// Gets the client PWD.
		/// </summary>
		/// <param name="lmId">The lm id.</param>
		/// <param name="clientId">The client id.</param>
		/// <returns></returns>
		public string getClientPwd(int lmId, string clientId)
		{
			return WebService.getClientPwd(lmId, clientId);
		}
		#region IUser Members

		int id;
		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public int Id { get { return id; } }

		private UserStruct authStruct;
		/// <summary>
		/// Gets the authentication struct.
		/// </summary>
		/// <value>The authentication struct.</value>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public UserStruct AuthenticationStruct { get { return authStruct; } }

		/// <summary>
		/// Gets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public string UserName { get { return authStruct.UserName; } }

		/// <summary>
		/// Gets the password.
		/// </summary>
		/// <value>The password.</value>
		/// <remarks>Documented by Dev05, 2008-08-28</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public string Password { get { return authStruct.Password; } }

		/// <summary>
		/// Gets or sets the get login delegate.
		/// </summary>
		/// <value>The get login delegate.</value>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public GetLoginInformation GetLoginDelegate { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

		/// <summary>
		/// Gets or sets the error message delegate.
		/// </summary>
		/// <value>The error message delegate.</value>
		/// <remarks>Documented by Dev05, 2008-11-17</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public DataAccessErrorDelegate ErrorMessageDelegate { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

		/// <summary>
		/// Gets a value indicating whether this instance can open a learning module.
		/// </summary>
		/// <value><c>true</c> if this instance can open; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2009-03-02</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public bool CanOpen { get { return false; } }

		/// <summary>
		/// Opens the learning module selected in the connection string.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public IDictionary Open() { throw new NotImplementedException(); }

		/// <summary>
		/// Lists all available learning modules.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public IDictionaries List() { return new WebDictionaries(WebService.GetLearningModulesList(), parent); }

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public ConnectionStringStruct ConnectionString { get; set; }

		/// <summary>
		/// Sets the connection string.
		/// </summary>
		/// <param name="css">The CSS.</param>
		/// <remarks>Documented by Dev05, 2010-02-03</remarks>
		public void SetConnectionString(ConnectionStringStruct css)
		{
			ConnectionString = css;
		}

		/// <summary>
		/// Checks the connection and throws an exception depending on the error, if the connection is invalid.
		/// </summary>
		/// <param name="connection">The connection to test.</param>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public void CheckConnection(ConnectionStringStruct connection) { }

		/// <summary>
		/// Logins this user.
		/// </summary>
		/// <remarks>Documented by Dev05, 2008-12-17</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public void Login() { throw new NotImplementedException(); }

		/// <summary>
		/// Logs this user out.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public void Logout() { }

		private Cache cache = new Cache(false);
		/// <summary>
		/// Gets the cache for this user.
		/// </summary>
		/// <value>The cache.</value>
		/// <remarks>Documented by Dev05, 2008-09-24</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public Cache Cache { get { throw new NotImplementedException(); } }

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
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public bool HasPermission(object o, string permissionName) { return false; }

		/// <summary>
		/// Gets the permissions for an object.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <returns>The list of permissions.</returns>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public List<SecurityFramework.PermissionInfo> GetPermissions(object o) { throw new NotImplementedException(); }

		#endregion

		#region IParent Members

		private ParentClass parent;
		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent { get { return parent; } }

		#endregion
	}
}
