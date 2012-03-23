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
using System.Text;
using System.Security;
using MLifter.DAL.Tools;
using SecurityFramework;

namespace MLifter.DAL.Interfaces
{
	/// <summary>
	/// The interface for the memorylifter DAL user.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-08-28</remarks>
	public interface IUser : IParent
	{
		/// <summary>
		/// Gets the id.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets the authentication struct.
		/// </summary>
		/// <value>The authentication struct.</value>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		UserStruct AuthenticationStruct { get; }
		/// <summary>
		/// Gets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		string UserName { get; }
		/// <summary>
		/// Gets the password. DO NOT STORE THIS IN A string-VARIABLE TO ENSURE A SECURE STORAGE!!!
		/// </summary>
		/// <value>The password.</value>
		/// <remarks>Documented by Dev05, 2008-08-28</remarks>
		string Password { get; }

		/// <summary>
		/// Gets or sets the get login delegate.
		/// </summary>
		/// <value>The get login delegate.</value>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		GetLoginInformation GetLoginDelegate { get; set; }
		/// <summary>
		/// Gets or sets the error message delegate.
		/// </summary>
		/// <value>The error message delegate.</value>
		/// <remarks>Documented by Dev05, 2008-11-17</remarks>
		DataAccessErrorDelegate ErrorMessageDelegate { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance can open a learning module.
		/// </summary>
		/// <value><c>true</c> if this instance can open; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2009-03-02</remarks>
		bool CanOpen { get; }
		/// <summary>
		/// Opens the learning module selected in the connection string.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		IDictionary Open();
		/// <summary>
		/// Lists all available learning modules.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-09-10</remarks>
		IDictionaries List();
		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		ConnectionStringStruct ConnectionString { get; set; }
		/// <summary>
		/// Sets the connection string.
		/// </summary>
		/// <param name="css">The CSS.</param>
		/// <remarks>Documented by Dev05, 2010-02-03</remarks>
		void SetConnectionString(ConnectionStringStruct css);
		/// <summary>
		/// Checks the connection and throws an exception depending on the error, if the connection is invalid.
		/// </summary>
		/// <param name="connection">The connection to test.</param>
		/// <remarks>Documented by Dev02, 2008-09-23</remarks>
		void CheckConnection(ConnectionStringStruct connection);

		/// <summary>
		/// Logins this user.
		/// </summary>
		/// <remarks>Documented by Dev05, 2008-12-17</remarks>
		void Login();
		/// <summary>
		/// Logs this user out.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		void Logout();

		/// <summary>
		/// Gets the cache for this user.
		/// </summary>
		/// <value>The cache.</value>
		/// <remarks>Documented by Dev05, 2008-09-24</remarks>
		Cache Cache { get; }

		/// <summary>
		/// Gets the underlying database for this user.
		/// </summary>
		/// <value>The database.</value>
		/// <remarks>Documented by Dev03, 2009-05-01</remarks>
		IDatabase Database { get; }

		/// <summary>
		/// Determines whether the specified object has the given permission.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="permissionName">Name of the permission.</param>
		/// <returns>
		/// 	<c>true</c> if the specified o has permission; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		bool HasPermission(object o, string permissionName);
		/// <summary>
		/// Gets the permissions for an object.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <returns>The list of permissions.</returns>
		/// <remarks>Documented by Dev03, 2009-01-14</remarks>
		List<PermissionInfo> GetPermissions(object o);
	}

	/// <summary>
	/// This struct stores all required informations about a learning module connection.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-09-10</remarks>
	[Serializable()]
	public struct ConnectionStringStruct
	{
		/// <summary>
		/// The type of the LM.
		/// </summary>
		public DatabaseType Typ;
		/// <summary>
		/// The connection string. For a DB connection this contains the DB connection string. For an XML connection this contains the file path to the learning module.
		/// </summary>
		public string ConnectionString;
		/// <summary>
		/// The file path to the learning module folder.
		/// </summary>
		public string LearningModuleFolder;
		/// <summary>
		/// The extension uri path.
		/// </summary>
		public string ExtensionURI;
		/// <summary>
		/// The learning module id.
		/// </summary>
		public int LmId;
		/// <summary>
		/// <b>true</b> for a protected LM.
		/// </summary>
		public bool ProtectedLm;
		/// <summary>
		/// The password for a protected LM.
		/// </summary>
		public string Password;
		/// <summary>
		/// The Session Id.
		/// </summary>
		public Guid SessionId;
		/// <summary>
		/// Tells the data layer that this should be a read-only connection.
		/// </summary>
		public bool ReadOnly;
		/// <summary>
		/// The sync type of this module.
		/// </summary>
		public SyncType SyncType;
		/// <summary>
		/// The user to connect to the server.
		/// </summary>
		[NonSerialized()]
		public IUser ServerUser;

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing a fully qualified type name.
		/// </returns>
		/// <remarks>Documented by Dev05, 2009-03-05</remarks>
		public override string ToString()
		{
			return "LM-ID " + LmId.ToString() + " on " + ConnectionString + " (" + Typ.ToString() + " - " + SyncType.ToString() + ")";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringStruct"/> struct.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-02-12</remarks>
		private ConnectionStringStruct(DatabaseType typ)
		{
			Typ = typ;
			ConnectionString = string.Empty;
			LearningModuleFolder = string.Empty;
			LmId = -1;
			ProtectedLm = false;
			Password = string.Empty;
			SessionId = new Guid();
			ReadOnly = false;
			SyncType = SyncType.NotSynchronized;
			ServerUser = null;
			ExtensionURI = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringStruct"/> struct.
		/// </summary>
		/// <param name="typ">The typ.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <remarks>
		/// Documented by AAB, 03.12.2008.
		/// </remarks>
		public ConnectionStringStruct(DatabaseType typ, string connectionString)
			: this(typ)
		{
			ConnectionString = connectionString;
			LearningModuleFolder = System.IO.Path.GetDirectoryName(connectionString);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringStruct"/> struct.
		/// </summary>
		/// <param name="typ">The typ.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="readOnly">if set to <c>true</c> the connection is [read only].</param>
		/// <remarks>
		/// Documented by AAB, 03.12.2008.
		/// </remarks>
		public ConnectionStringStruct(DatabaseType typ, string connectionString, bool readOnly)
			: this(typ)
		{
			ConnectionString = connectionString;
			LearningModuleFolder = System.IO.Path.GetDirectoryName(connectionString);
			ReadOnly = readOnly;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringStruct"/> struct.
		/// </summary>
		/// <param name="typ">The typ.</param>
		/// <param name="learningModuleFolder">The learning module folder.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <remarks>
		/// Documented by AAB, 03.12.2008.
		/// </remarks>
		public ConnectionStringStruct(DatabaseType typ, string learningModuleFolder, string connectionString)
			: this(typ)
		{
			ConnectionString = connectionString;
			LearningModuleFolder = learningModuleFolder;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringStruct"/> struct.
		/// </summary>
		/// <param name="typ">The typ.</param>
		/// <param name="learningModuleFolder">The learning module folder.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="readOnly">if set to <c>true</c> the connection is [read only].</param>
		/// <remarks>
		/// Documented by AAB, 03.12.2008.
		/// </remarks>
		public ConnectionStringStruct(DatabaseType typ, string learningModuleFolder, string connectionString, bool readOnly)
			: this(typ)
		{
			ConnectionString = connectionString;
			LearningModuleFolder = learningModuleFolder;
			ReadOnly = readOnly;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringStruct"/> struct.
		/// </summary>
		/// <param name="typ">The typ.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="lmId">The lm id.</param>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ConnectionStringStruct(DatabaseType typ, string connectionString, int lmId)
			: this(typ)
		{
			ConnectionString = connectionString;
			LmId = lmId;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringStruct"/> struct.
		/// </summary>
		/// <param name="typ">The typ.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="lmId">The lm id.</param>
		/// <param name="sessionId">The session id.</param>
		/// <remarks>Documented by Dev03, 2008-12-03</remarks>
		public ConnectionStringStruct(DatabaseType typ, string connectionString, int lmId, Guid sessionId)
			: this(typ)
		{
			ConnectionString = connectionString;
			LmId = lmId;
			SessionId = sessionId;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringStruct"/> struct.
		/// </summary>
		/// <param name="typ">The typ.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="lmId">The lm id.</param>
		/// <param name="synced">if set to <c>true</c> [synced].</param>
		/// <param name="serverUser">The server user.</param>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public ConnectionStringStruct(DatabaseType typ, string connectionString, int lmId, SyncType synced, IUser serverUser)
			: this(typ)
		{
			ConnectionString = connectionString;
			LmId = lmId;
			SyncType = synced;
			ServerUser = serverUser;
		}
	}

	/// <summary>
	/// The different types of synchronization.
	/// </summary>
	public enum SyncType
	{
		/// <summary>
		/// This LM is not synced.
		/// </summary>
		NotSynchronized,
		/// <summary>
		/// The LM is, excepting to the Media-Data, synced and you can access the source DB.
		/// </summary>
		HalfSynchronizedWithDbAccess,
		/// <summary>
		/// The LM is, excepting to the Media-Data, synced and you can NOT access the source DB.
		/// </summary>
		HalfSynchronizedWithoutDbAccess,
		/// <summary>
		/// The LM is synced (including all Media-Data).
		/// </summary>
		FullSynchronized
	}
}
