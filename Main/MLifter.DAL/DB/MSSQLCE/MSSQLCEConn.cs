//# define debug_output

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using MLifter.DAL.Interfaces;
using System.Data;
using MLifter.DAL.Tools;
using System.Diagnostics;
using System.Threading;
using MLifter.Generics;
using System.IO;

namespace MLifter.DAL.DB.MsSqlCe
{
	/// <summary>
	/// Class to create SQL CE connections and execute commands.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-01-16</remarks>
	public class MSSQLCEConn
	{
		#region connection
		private static Dictionary<string, SqlCeConnection> connections = new Dictionary<string, SqlCeConnection>();

		/// <summary>
		/// Creates the connection an directly opens it.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		internal static SqlCeConnection GetConnection(IUser user)
		{
			lock (connections)
			{
				if (connections.ContainsKey(user.ConnectionString.ConnectionString))
				{
					SqlCeConnection con = connections[user.ConnectionString.ConnectionString];
					if (con.State != ConnectionState.Open || !con.ConnectionString.Contains(user.ConnectionString.ConnectionString))
						return CreateNewConnection(user);
					return con;
				}
				else
				{
					return CreateNewConnection(user);
				}
			}
		}

		private static SqlCeConnection CreateNewConnection(IUser user)
		{
			try
			{
				SqlCeConnection con = new SqlCeConnection(GetFullConnectionString(user.ConnectionString.ConnectionString));
				con.Open();
				lock (connections)
				{
					connections[user.ConnectionString.ConnectionString] = con;
				}
				return con;
			}
			catch (SqlCeException exp)
			{
				switch (exp.NativeError) //Error-List: http://msdn.microsoft.com/en-us/library/ms171879(SQL.90).aspx
				{
					case 25079:             //25079 --> SSCE_M_ENCRYPTEDDBMUSTHAVEPWD --> A password must be specified when a database is created by using encryption.
					case 25028:             //25028	--> SSCE_M_INVALIDPASSWORD  --> The specified password does not match the database password.
						throw new ProtectedLearningModuleException();
					case 25035:             //25035 --> SSCE_M_FILESHAREVIOLATION   --> There is a file-sharing violation. A different process might be using the file.
						throw new IOException(exp.Message, exp);
					case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
						throw new NotEnoughtDiskSpaceException();
					default:
						throw;
				}
			}
		}

		/// <summary>
		/// Gets the full connection string from a file path.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-16</remarks>
		public static string GetFullConnectionString(string filePath)
		{
			bool exclusive = true;
			if (Path.IsPathRooted(filePath))
			{
				try
				{
					DriveInfo driveInfo = new DriveInfo(filePath[0].ToString());
					exclusive = (driveInfo.DriveType == DriveType.Fixed || driveInfo.DriveType == DriveType.Removable) ? false : true;
				}
				catch { }
			}
			if (exclusive)
				return "Data Source=" + filePath + ";Max Database Size=4000;File Mode=Exclusive;Persist Security Info=False;";
			else
				return "Data Source=" + filePath + ";Max Database Size=4000;Persist Security Info=False;";
		}

		/// <summary>
		/// Gets the full connection string from a file path.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-16</remarks>
		public static string GetFullConnectionString(string filePath, string password)
		{
			return GetFullConnectionString(filePath) + "Encrypt Database=True;Password=" + password + ";Encryption Mode='PPC2003 Compatibility';";
		}

		/// <summary>
		/// Closes all connections.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-01-16</remarks>
		public static void CloseAllConnections()
		{
			List<string> connectionStrings = new List<string>();
			lock (connections)
			{
				foreach (SqlCeConnection con in connections.Values)
				{
					try
					{
						if (con.State == ConnectionState.Open)
						{
							if (!connectionStrings.Contains(con.ConnectionString))
								connectionStrings.Add(con.ConnectionString);
							con.Close();
						}
					}
					catch (Exception e) { Trace.WriteLine("Error closing Connection: " + e.ToString()); }
				}
			}
			connections.Clear();

			SqlCeEngine sqlEngine = new SqlCeEngine();
			foreach (string cs in connectionStrings)
			{
				try
				{
					sqlEngine.LocalConnectionString = cs;
					sqlEngine.Shrink();
				}
				catch (Exception exp) { Trace.Write(exp.ToString()); }
			}
			sqlEngine.Dispose();
		}

		/// <summary>
		/// Closes my (owners) connection.
		/// </summary>
		/// <param name="filename">The filename (from the ConnectionString).</param>
		/// <remarks>Documented by Dev08, 2009-03-09</remarks>
		public static void CloseMyConnection(string filename)
		{
			lock (connections)
			{
				if (connections.ContainsKey(filename))
				{
					try
					{
						connections[filename].Close();
					}
					catch (Exception e) { Trace.WriteLine("Error closing Connection: " + e.ToString()); }
					connections.Remove(filename);
				}
				else
					Debug.WriteLine("MSSQLCEConn.cs: Can not close my connection. Propably the dispose was executed more than 1 time");
			}

#if DEBUG && debug_output
			using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sqlCommandCounter.log")))
			{
				foreach (string command in sqlCommandCounter.Keys)
					sw.WriteLine(sqlCommandCounter[command].Counter + " times; " + Math.Round(sqlCommandCounter[command].DurationMilliSeconds, 1) + "ms \t\t" + command);
			}
			sqlCommandCounter.Clear();

			using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sqlDoubleCommandCounter.log")))
			{
				foreach (string command in sqlDoubleCommandCounter.Keys)
					sw.WriteLine(sqlDoubleCommandCounter[command].Counter.ToString() + " times:\t\t" + command);
			}
			sqlDoubleCommandCounter.Clear();
#endif
		}

		/// <summary>
		/// Checks if the given database can be opened.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		/// <returns>LearningModuleIOStatus</returns>
		/// <remarks>Documented by Dev08, 2009-04-29</remarks>
		public static LearningModuleIOStatus CanOpenLearningModule(ConnectionStringStruct connectionString)
		{
			//1. Check if there is already a open connection on this file
			bool learningModuleWasAlreadyConnected = false;
			lock (connections)
			{
				if (connections.ContainsKey(connectionString.ConnectionString) && connections[connectionString.ConnectionString].State == ConnectionState.Open)
				{
					learningModuleWasAlreadyConnected = true;
					connections[connectionString.ConnectionString].Close();
				}
			}

			//2. Create a new connection and check, if it can be opened...
			SqlCeConnection myConnection = new SqlCeConnection(GetFullConnectionString(connectionString.ConnectionString, connectionString.Password));
			try
			{
				myConnection.Open();
			}
			catch (SqlCeException exp)
			{
				switch (exp.NativeError)
				{
					case 25079:
					case 25028:
						return LearningModuleIOStatus.Protected;
					case 25035:
						return LearningModuleIOStatus.FileUsedByAnotherProcess;
				}
			}
			finally
			{
				myConnection.Close();

				//3. If necessary, open the last connection.
				if (learningModuleWasAlreadyConnected)
				{
					lock (connections)
					{
						try
						{
							connections[connectionString.ConnectionString].Open();
						}
						catch { }
					}
				}
			}

			return LearningModuleIOStatus.NotProtected;
		}
		#endregion

		#region command
		/// <summary>
		/// Creates a command to the given user-connection.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-01-16</remarks>
		public static SqlCeCommand CreateCommand(IUser user)
		{
			return GetConnection(user).CreateCommand();
		}

		/// <summary>
		/// Executes the query and returns the affected rows.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		public static int ExecuteNonQuery(SqlCeCommand command)
		{
			command.CommandText = command.CommandText.TrimEnd(' ', ';');
			WriteDebugInfos(command);

			int count = 0;
			if (!command.CommandText.Contains(";"))
			{
				#region ExecuteNonQuery
				try
				{
					count = command.ExecuteNonQuery();
				}
				catch (SqlCeException ex)
				{
					switch (ex.NativeError)
					{
						case 25016:             //Native error: SSCE_M_KEYDUPLICATE ==> A duplicate value cannot be inserted into a unique index.
							command.Connection.Close();
							VerifyDataBase(command.Connection.DataSource, false, string.Empty);
							command.Connection.Open();
							command.ExecuteNonQuery();
							break;
						case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
							throw new NotEnoughtDiskSpaceException();
						default:
							throw ex;
					}
				}
				#endregion
			}
			else
			{
				foreach (string q in command.CommandText.Split(';'))
				{
					if (q.TrimStart(' ', '\r', '\n').StartsWith("--") || q.TrimStart(' ', '\r', '\n').Length < 5)
						continue;

					SqlCeCommand cmd = command.Connection.CreateCommand();
					cmd.CommandText = q;
					foreach (SqlCeParameter param in command.Parameters)
						cmd.Parameters.Add(param.ParameterName, param.Value == null ? DBNull.Value : param.Value);
					#region ExecuteNonQuery
					int tempCount = 0;
					try { tempCount = cmd.ExecuteNonQuery(); }
					catch (SqlCeException ex)
					{
						switch (ex.NativeError)
						{
							case 25016:             //Native error: SSCE_M_KEYDUPLICATE ==> A duplicate value cannot be inserted into a unique index.
								command.Connection.Close();
								VerifyDataBase(command.Connection.DataSource, false, string.Empty);
								command.Connection.Open();
								command.ExecuteNonQuery();
								break;
							case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
								throw new NotEnoughtDiskSpaceException();
							default:
								throw ex;
						}
					}
					finally { count += tempCount; }
					#endregion
				}
			}
#if DEBUG && debug_output
			sqlCommandCounter[currentSqlCommandCounter].DurationMilliSeconds += stopwatch.ElapsedMilliseconds;
#endif
			return count;
		}

		/// <summary>
		/// Executes the query and returns a reader.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		public static SqlCeDataReader ExecuteReader(SqlCeCommand command)
		{
			command.CommandText = command.CommandText.TrimEnd(' ', ';');
			WriteDebugInfos(command);

			if (!command.CommandText.Contains(";"))
			{
				#region ExecuteReader()
				SqlCeDataReader reader;
				try
				{
					reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
#if DEBUG && debug_output
					sqlCommandCounter[currentSqlCommandCounter].DurationMilliSeconds += stopwatch.ElapsedMilliseconds;
#endif
					return reader;
				}
				catch (SqlCeException ex)
				{
					switch (ex.NativeError)
					{
						case 25016:             //Native error: SSCE_M_KEYDUPLICATE ==> A duplicate value cannot be inserted into a unique index.
							command.Connection.Close();
							VerifyDataBase(command.Connection.DataSource, false, string.Empty);
							command.Connection.Open();
							reader = command.ExecuteReader();
							return reader;
						case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
							throw new NotEnoughtDiskSpaceException();
						default:
							throw ex;
					}
				}
				#endregion
			}
			else
			{
				SqlCeCommand cmd = null;
				foreach (string q in command.CommandText.Split(';'))
				{
					if (cmd != null)
					{
						#region ExecuteNonQuery
						try
						{
							cmd.ExecuteNonQuery();
						}
						catch (SqlCeException ex)
						{
							switch (ex.NativeError)
							{
								case 25016:             //Native error: SSCE_M_KEYDUPLICATE ==> A duplicate value cannot be inserted into a unique index.
									command.Connection.Close();
									VerifyDataBase(command.Connection.DataSource, false, string.Empty);
									command.Connection.Open();
									command.ExecuteNonQuery();
									break;
								case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
									throw new NotEnoughtDiskSpaceException();
								default:
									throw ex;
							}
						}
						#endregion
						cmd = null;
					}

					cmd = command.Connection.CreateCommand();
					cmd.CommandText = q;
					foreach (SqlCeParameter param in command.Parameters)
						cmd.Parameters.Add(param.ParameterName, param.Value);
				}
				#region ExecuteReader()
				SqlCeDataReader reader;
				try
				{
					reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
#if DEBUG && debug_output
					sqlCommandCounter[currentSqlCommandCounter].DurationMilliSeconds += stopwatch.ElapsedMilliseconds;
#endif
					return reader;
				}
				catch (SqlCeException ex)
				{
					switch (ex.NativeError)
					{
						case 25016:             //Native error: SSCE_M_KEYDUPLICATE ==> A duplicate value cannot be inserted into a unique index.
							command.Connection.Close();
							VerifyDataBase(command.Connection.DataSource, false, string.Empty);
							command.Connection.Open();
							reader = command.ExecuteReader();
							return reader;
						case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
							throw new NotEnoughtDiskSpaceException();
						default:
							throw ex;
					}
				}
				#endregion
			}
		}

		/// <summary>
		/// Executes the query and returns the first cell.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-11-13</remarks>
		public static object ExecuteScalar(SqlCeCommand command)
		{
			command.CommandText = command.CommandText.TrimEnd(' ', ';');
			WriteDebugInfos(command);

			if (!command.CommandText.Contains(";"))
			{
				#region ExecuteScalar()
				object obj;
				try
				{
					obj = command.ExecuteScalar();
#if DEBUG && debug_output
				sqlCommandCounter[currentSqlCommandCounter].DurationMilliSeconds += stopwatch.ElapsedMilliseconds;
#endif
					return obj;
				}
				catch (SqlCeException ex)
				{
					switch (ex.NativeError)
					{
						case 25016:             //Native error: SSCE_M_KEYDUPLICATE ==> A duplicate value cannot be inserted into a unique index.
							command.Connection.Close();
							VerifyDataBase(command.Connection.DataSource, false, string.Empty);
							command.Connection.Open();
							obj = command.ExecuteScalar();
							return obj;
						case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
							throw new NotEnoughtDiskSpaceException();
						default:
							throw ex;
					}
				}
				#endregion;
			}
			else
			{
				SqlCeCommand cmd = null;
				foreach (string q in command.CommandText.Split(';'))
				{
					if (cmd != null)
					{
						#region ExecuteNonQuery
						try
						{
							cmd.ExecuteNonQuery();
						}
						catch (SqlCeException ex)
						{
							switch (ex.NativeError)
							{
								case 25016:             //Native error: SSCE_M_KEYDUPLICATE ==> A duplicate value cannot be inserted into a unique index.
									cmd.Connection.Close();
									VerifyDataBase(cmd.Connection.DataSource, false, string.Empty);
									cmd.Connection.Open();
									cmd.ExecuteNonQuery();
									break;
								case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
									throw new NotEnoughtDiskSpaceException();
								default:
									throw ex;
							}
						}
						#endregion
						cmd = null;
					}

					cmd = command.Connection.CreateCommand();
					cmd.CommandText = q;
					foreach (SqlCeParameter param in command.Parameters)
						cmd.Parameters.Add(param.ParameterName, param.Value);
				}
				#region ExecuteScalar()
				object obj;
				try
				{
					obj = cmd.ExecuteScalar();
#if DEBUG && debug_output
					sqlCommandCounter[currentSqlCommandCounter].DurationMilliSeconds += stopwatch.ElapsedMilliseconds;
#endif
					return obj;
				}
				catch (SqlCeException ex)
				{
					switch (ex.NativeError)
					{
						case 25016:             //Native error: SSCE_M_KEYDUPLICATE ==> A duplicate value cannot be inserted into a unique index.
							command.Connection.Close();
							VerifyDataBase(command.Connection.DataSource, false, string.Empty);
							command.Connection.Open();
							obj = command.ExecuteScalar();
							return obj;
						case 25037:             //25037 --> SSCE_M_DISKFULL             --> There is not enough disk space left for the database.
							throw new NotEnoughtDiskSpaceException();
						default:
							throw ex;
					}
				}
				#endregion
			}
		}

		/// <summary>
		/// Executes the query and returns the first cell in its nullable representative.
		/// </summary>
		/// <typeparam name="T">The type from which the nullable representative is returned.</typeparam>
		/// <param name="command">The command.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2008-08-12</remarks>
		public static T? ExecuteScalar<T>(SqlCeCommand command) where T : struct
		{
			object res = ExecuteScalar(command);
			return (res == null || res is DBNull) ? (T?)null : (T?)Convert.ChangeType(res, typeof(T));
		}
		#endregion

		#region maintenance
		/// <summary>
		/// The required DB version.
		/// </summary>
		public static readonly Version RequiredDatabaseVersion = new Version(1, 0, 2);
		/// <summary>
		/// Applies the indices to the database.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="contentProtected">if set to <c>true</c> [content protected].</param>
		/// <param name="password">The password.</param>
		/// <remarks>Documented by Dev03, 2009-04-30</remarks>
		public static void ApplyIndicesToDatabase(string path, bool contentProtected, string password)
		{
			string connectionString = contentProtected ? MSSQLCEConn.GetFullConnectionString(path, password) : MSSQLCEConn.GetFullConnectionString(path);
			using (SqlCeConnection connection = new SqlCeConnection(connectionString))
			{
				connection.Open();
				ApplyIndicesToDatabase(connection);
			}
		}
		/// <summary>
		/// Applies the indices to the database.
		/// </summary>
		/// <param name="connection">An Sql Ce Connection.</param>
		/// <remarks>Documented by Dev03, 2009-04-30</remarks>
		internal static void ApplyIndicesToDatabase(SqlCeConnection connection)
		{
			using (SqlCeCommand cmd = connection.CreateCommand())
			{
				foreach (string query in Properties.Resources.MsSqlCeDbCreateIdxScript.Split(';'))
				{
					if (query.TrimStart(' ', '\r', '\n').StartsWith("--") || query.TrimStart(' ', '\r', '\n').Length < 5)
						continue;

					cmd.CommandText = query;
					try
					{
						cmd.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Verifies the data base.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="contentProtected">if set to <c>true</c> [content protected].</param>
		/// <param name="password">The password.</param>
		/// <remarks>Documented by Dev03, 2009-04-30</remarks>
		public static void VerifyDataBase(string path, bool contentProtected, string password)
		{
			string connectionString = contentProtected ? MSSQLCEConn.GetFullConnectionString(path, password) : MSSQLCEConn.GetFullConnectionString(path);
			using (SqlCeEngine sqlEngine = new SqlCeEngine())
			{
				sqlEngine.LocalConnectionString = connectionString;
				sqlEngine.Repair(connectionString, RepairOption.RecoverAllPossibleRows);
				sqlEngine.Shrink();
				try { File.Delete(Path.ChangeExtension(path, ".log")); }
				catch { Debug.WriteLine("Can not delete logfile "); }
			}
		}

		/// <summary>
		/// Upgrades the database to the current version.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="currentVersion">The current version of the database.</param>
		/// <returns>
		/// [true] if success.
		/// </returns>
		/// <remarks>
		/// Documented by AAB, 30.04.2009.
		/// </remarks>
		internal static bool UpgradeDatabase(IUser user, Version currentVersion)
		{
			using (SqlCeConnection connection = GetConnection(user))
			{
				if (connection.State != ConnectionState.Open)
					connection.Open();

				SqlCeTransaction upgradeTransaction = connection.BeginTransaction();
				try
				{
					if (currentVersion == new Version(1, 0, 0))
					{
						UpgradeDatabase_100_101(connection);
						currentVersion = new Version(1, 0, 1);
					}

					if (currentVersion == new Version(1, 0, 1))
					{
						UpgradeDatabase_101_102(connection);
						currentVersion = new Version(1, 0, 2);
					}

					ApplyIndicesToDatabase(connection);
					upgradeTransaction.Commit(CommitMode.Immediate);
					return true;
				}
				catch (Exception)
				{
					upgradeTransaction.Rollback();
				}
			}
			return false;
		}


		/// <summary>
		/// Upgrades the database from version 1.0.0 to 1.0.1.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <remarks>Documented by Dev02, 2009-07-02</remarks>
		private static void UpgradeDatabase_100_101(SqlCeConnection connection)
		{
			using (SqlCeCommand cmd = connection.CreateCommand())
			{
				cmd.CommandText = "ALTER TABLE Cards ADD chapters_id integer NOT NULL DEFAULT 0, lm_id integer NOT NULL DEFAULT 0";
				cmd.ExecuteNonQuery();

				cmd.CommandText = "SELECT id FROM LearningModules";
				using (SqlCeDataReader lmReader = cmd.ExecuteReader())
				{
					while (lmReader.Read())
					{
						int lmId = Convert.ToInt32(lmReader["id"]);
						using (SqlCeCommand cmd2 = connection.CreateCommand())
						{
							cmd2.CommandText = "UPDATE Cards SET lm_id = @lm_id WHERE id IN (SELECT cards_id FROM LearningModules_Cards WHERE lm_id = @lm_id)";
							cmd2.Parameters.Add("@lm_id", lmId);
							cmd2.ExecuteNonQuery();
							cmd2.Parameters.Clear();

							cmd2.CommandText = "SELECT id FROM Chapters WHERE lm_id = @lm_id";
							cmd2.Parameters.Add("@lm_id", lmId);
							using (SqlCeDataReader chapterReader = cmd2.ExecuteReader())
							{
								while (chapterReader.Read())
								{
									int chapterId = Convert.ToInt32(chapterReader["id"]);
									while (chapterReader.Read())
									{
										using (SqlCeCommand cmd3 = connection.CreateCommand())
										{
											cmd3.CommandText = "UPDATE Cards SET chapters_id = @chapter_id WHERE id IN (SELECT cards_id FROM Chapters_Cards WHERE chapters_id = @chapter_id)";
											cmd3.Parameters.Add("@chapter_id", chapterId);
											cmd3.ExecuteNonQuery();
										}
									}
								}
							}
						}
					}

				}
				cmd.CommandText = "DELETE FROM DatabaseInformation WHERE property = 'Version'";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "DELETE FROM DatabaseInformation WHERE property = 'SupportedDataLayerVersions'";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "INSERT INTO DatabaseInformation VALUES ('Version','1.0.1')";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "INSERT INTO DatabaseInformation VALUES ('SupportedDataLayerVersions','2.2.406,>2.2.406')";
				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Upgrades the database from version 1.0.1 to 1.0.2.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <remarks>Documented by Dev02, 2009-07-02</remarks>
		private static void UpgradeDatabase_101_102(SqlCeConnection connection)
		{
			using (SqlCeCommand cmd = connection.CreateCommand())
			{
				cmd.CommandText = @"
					CREATE TABLE Extensions
					(
						guid nvarchar(36) PRIMARY KEY NOT NULL,
						lm_id int,
						name ntext NOT NULL,
						version nvarchar(10) NOT NULL,
						type nvarchar(100) NOT NULL,
						data image NOT NULL,
						startfile ntext,
						FOREIGN KEY (lm_id) REFERENCES LearningModules(id)
					);";
				cmd.ExecuteNonQuery();

				cmd.CommandText = @"
					CREATE TABLE ExtensionActions
					(
						guid nvarchar(36) NOT NULL,
						action nvarchar(100) NOT NULL,
						CONSTRAINT ExtensionActions_PK PRIMARY KEY(guid, action),
						execution nvarchar(100) NOT NULL,
						FOREIGN KEY (guid) REFERENCES Extensions(guid)
					);";
				cmd.ExecuteNonQuery();

				cmd.CommandText = "UPDATE DatabaseInformation SET value='1.0.2' WHERE property = 'Version'";
				cmd.ExecuteNonQuery();
				cmd.CommandText = "UPDATE DatabaseInformation SET value='2.2.484,>2.2.484' WHERE property = 'SupportedDataLayerVersions'";
				cmd.ExecuteNonQuery();
			}
		}
		#endregion

		#region Debug Methods and Fields
#if DEBUG && debug_output
		public static Dictionary<string, PerformanceInfo> sqlCommandCounter = new Dictionary<string, PerformanceInfo>();
		public static string currentSqlCommandCounter = string.Empty;
		public static Dictionary<string, PerformanceInfo> sqlDoubleCommandCounter = new Dictionary<string, PerformanceInfo>();
		public static DateTime start;
		public static DateTime stop;
		public static Stopwatch stopwatch = new Stopwatch();
#endif

		/// <summary>
		/// Writes the debug infos.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <remarks>Documented by Dev02, 2008-07-30</remarks>
		private static void WriteDebugInfos(SqlCeCommand command)
		{
#if DEBUG && debug_output
			//0. Start Timer:
			start = DateTime.Now;
			stopwatch.Reset();
			stopwatch.Start();

			//1. Write Debug Information
			string str = string.Format("Query ({0}): {1}", DateTime.Now.ToString("HH:mm:ss"), command.CommandText);
			if (command.Parameters.Count > 0)
			{
				str += "\tParameters:";
				foreach (SqlCeParameter parameter in command.Parameters)
					str += string.Format(" {0}={1};", parameter.ParameterName, parameter.Value);
			}
			Debug.WriteLine(str);

			//2.5 Save current command:
			currentSqlCommandCounter = command.CommandText;

			//2. Save number of called Commands!
			if (sqlCommandCounter.ContainsKey(command.CommandText))      //Key (Command) already available...
				++sqlCommandCounter[command.CommandText].Counter;
			else
				sqlCommandCounter.Add(command.CommandText, new PerformanceInfo());

			//3. Save number of double called Commands!
			if (sqlDoubleCommandCounter.ContainsKey(str))      //Key (Command) already available...
				++sqlDoubleCommandCounter[str].Counter;
			else
				sqlDoubleCommandCounter.Add(str, new PerformanceInfo());

#endif
		}
		#endregion
	}

	/// <summary>
	/// Performance counter class.
	/// </summary>
	public class PerformanceInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PerformanceInfo"/> class.
		/// </summary>
		public PerformanceInfo()
		{
			Counter = 1;
		}

		/// <summary>
		/// Gets or sets the counter.
		/// </summary>
		/// <value>
		/// The counter.
		/// </value>
		public int Counter { get; set; }

		/// <summary>
		/// Gets or sets the duration milli seconds.
		/// </summary>
		/// <value>
		/// The duration milli seconds.
		/// </value>
		public double DurationMilliSeconds { get; set; }
	}

	/// <summary>
	/// The stat of the learning module
	/// </summary>
	public enum LearningModuleIOStatus
	{
		/// <summary>
		/// The learning module is protected.
		/// </summary>
		Protected,
		/// <summary>
		/// The learning module is not protected
		/// </summary>
		NotProtected,
		/// <summary>
		/// The learning module is used by another process.
		/// </summary>
		FileUsedByAnotherProcess
	}
}
