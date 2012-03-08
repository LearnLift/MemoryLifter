using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MLifter.DAL.Properties;
using MLifter.DAL.Interfaces;
using System.Data.SqlServerCe;
using MLifter.DAL.DB.MsSqlCe;
using MLifter.DAL.DB;

namespace MLifter.DAL.Tools
{

	/// <summary>
	/// Creates an Extension File
	/// </summary>
	/// <remarks>Documented by Dev07, 2009-07-06</remarks>
	public class ExtensionFile : IDisposable
	{
		/// <summary>
		/// Gets or sets the extension path.
		/// </summary>
		/// <value>
		/// The extension path.
		/// </value>
		public string ExtensionPath { get; set; }

		IDictionaries LearningModules { get; set; }
		DummyUser User { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionFile"/> class.
		/// </summary>
		/// <param name="fullPath">The full path.</param>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		public ExtensionFile(string fullPath)
		{
			ExtensionPath = fullPath;
			User = new DummyUser(new ConnectionStringStruct(DatabaseType.MsSqlCe, ExtensionPath));
		}

		/// <summary>
		/// Creates a new extension file.
		/// </summary>
		/// <remarks>Documented by Dev07, 2009-07-06</remarks>
		public void Create()
		{
			if (!Directory.Exists(Path.GetDirectoryName(User.ConnectionString.ConnectionString)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(User.ConnectionString.ConnectionString));
			}
			if (!File.Exists(User.ConnectionString.ConnectionString))
			{
				using (SqlCeEngine clientEngine = new SqlCeEngine("Data Source=" + User.ConnectionString.ConnectionString))
				{
					clientEngine.CreateDatabase();
					clientEngine.Dispose();
				}
			}
			using (SqlCeCommand cmd = MSSQLCEConn.CreateCommand(User))
			{
				cmd.CommandText = Resources.MsSqlCeDbCreateScript;
				MSSQLCEConn.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Opens an existing extension file.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		public void Open(GetLoginInformation loginDelegate)
		{
			IUser user = UserFactory.Create(loginDelegate, new ConnectionStringStruct(DatabaseType.MsSqlCe, ExtensionPath, -1), delegate { return; }, this);
			ConnectionStringStruct css = user.ConnectionString;
			css.LmId = MLifter.DAL.User.GetIdOfLearningModule(ExtensionPath, user);
			user.ConnectionString = css;
			LearningModules = user.List();
		}

		/// <summary>
		/// Gets the extension.
		/// </summary>
		/// <value>The extension.</value>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		public IExtension Extension
		{
			get
			{
				if (LearningModules == null)
					return null;

				if (LearningModules.Extensions.Count < 1)
					return GetNewExtension();

				return LearningModules.Extensions[0];
			}
		}

		/// <summary>
		/// Creates a new extension (and deletes existing extensions!).
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2009-07-09</remarks>
		public IExtension GetNewExtension()
		{
			if (LearningModules == null)
				return null;

			foreach (IExtension extension in LearningModules.Extensions)
				LearningModules.DeleteExtension(extension);

			return LearningModules.ExtensionFactory();
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-07-10</remarks>
		public void Dispose()
		{ }

		#endregion
	}
}
