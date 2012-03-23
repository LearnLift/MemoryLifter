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
//#define debug_output

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.BusinessLayer.Properties;
using MLifter.BusinessLayer.LearningModulesService;
using MLifter.Generics;
using System.Collections.ObjectModel;

namespace MLifter.BusinessLayer
{
	public class ConnectionStringHandler
	{
		private static XmlSerializer PgSerializer = new XmlSerializer(typeof(PostgreSqlConnectionStringBuilder));
		private static XmlSerializer UncSerializer = new XmlSerializer(typeof(UncConnectionStringBuilder));
		private static XmlSerializer WebSerializer = new XmlSerializer(typeof(WebConnectionStringBuilder));
		private static XmlSerializer FeedSerializer = new XmlSerializer(typeof(ModuleFeedConnection));

		private List<IConnectionString> connectionStrings = new List<IConnectionString>();
		/// <summary>
		/// Gets or sets the connection strings.
		/// </summary>
		/// <value>
		/// The connection strings.
		/// </value>
		/// <remarks>CFI, 2012-03-03</remarks>
		public List<IConnectionString> ConnectionStrings
		{
			get { return connectionStrings; }
			set { connectionStrings = value; }
		}
		/// <summary>
		/// Gets or sets the feeds.
		/// </summary>
		/// <value>
		/// The feeds.
		/// </value>
		/// <remarks>CFI, 2012-03-03</remarks>
		public ObservableCollection<ModuleFeedConnection> Feeds { get; private set; }

		/// <summary>
		/// Creates the unc connection.
		/// </summary>
		/// <param name="connectionName">Name of the connection.</param>
		/// <param name="connectionPath">The connection path.</param>
		/// <param name="configPath">The config path.</param>
		/// <param name="configFile">The config file.</param>
		/// <param name="isDefault">if set to <c>true</c> [is default].</param>
		/// <remarks>Documented by Dev08, 2009-03-02</remarks>
		public static void CreateUncConnection(string connectionName, string learningModulesPath, string configPath, string configFile, bool isDefault, bool isOnStick)
		{
			if (connectionName == null)
				throw new ArgumentNullException("connectionName can't be null");
			if (learningModulesPath == null)
				throw new ArgumentNullException("connectionPath can't be null. Connection Name: " + connectionName);
			if (connectionName.Trim().Length == 0)
				throw new ArgumentException("connectionName can't be null");
			if (learningModulesPath.Trim().Length == 0)
				throw new ArgumentException("connectionPath can't be null");

			UncConnectionStringBuilder uncConnection = new UncConnectionStringBuilder(learningModulesPath, isDefault, isOnStick);
			uncConnection.Name = connectionName;
			CreateUncConnection(uncConnection, configPath, configFile);
		}

		/// <summary>
		/// Creates a new UNC connection.
		/// </summary>
		/// <param name="connection">The UNC connection.</param>
		/// <param name="configPath">The config path.</param>
		/// <param name="configFile">The config file.</param>
		/// <remarks>Documented by Dev03, 2008-12-19</remarks>
		public static void CreateUncConnection(UncConnectionStringBuilder connection, string configPath, string configFile)
		{
			if (connection == null)
				throw new ArgumentNullException("connection can't be null");
			if (configPath == null)
				throw new ArgumentNullException("configPath can't be null");
			if (configFile == null)
				throw new ArgumentNullException("configFile can't be null");
			if (configPath.Trim().Length == 0)
				throw new ArgumentException("configPath can't be null");
			if (configFile.Trim().Length == 0)
				throw new ArgumentException("configFile can't be null");

			XmlSerializer uncSerializer = new XmlSerializer(typeof(UncConnectionStringBuilder));
			Directory.CreateDirectory(configPath);
			using (Stream uncStream = File.Create(Path.Combine(configPath, configFile)))
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				XmlWriter writer = XmlTextWriter.Create(uncStream, settings);
				writer.WriteStartElement("Configuration");
				writer.WriteStartElement("Connections");
				uncSerializer.Serialize(writer, connection);
				writer.WriteEndElement();
				writer.WriteEndElement();

				writer.Close();
				uncStream.Close();
			}
		}

		/// <summary>
		/// Creates a new PostgreSql connection.
		/// </summary>
		/// <param name="connection">The PostgreSql connection.</param>
		/// <param name="configPath">The config path.</param>
		/// <param name="configFile">The config file.</param>
		/// <remarks>Documented by Dev03, 2008-12-19</remarks>
		public static void CreatePostgreSqlConnection(PostgreSqlConnectionStringBuilder connection, string configPath, string configFile)
		{
			if (connection == null)
				throw new ArgumentNullException("connection can't be null");
			if (configPath == null)
				throw new ArgumentNullException("configPath can't be null");
			if (configFile == null)
				throw new ArgumentNullException("configFile can't be null");
			if (configPath.Trim().Length == 0)
				throw new ArgumentException("configPath can't be null");
			if (configFile.Trim().Length == 0)
				throw new ArgumentException("configFile can't be null");

			XmlSerializer uncSerializer = new XmlSerializer(typeof(PostgreSqlConnectionStringBuilder));
			Directory.CreateDirectory(configPath);
			using (Stream uncStream = File.Create(Path.Combine(configPath, configFile)))
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				XmlWriter writer = XmlTextWriter.Create(uncStream, settings);
				writer.WriteStartElement("Configuration");
				writer.WriteStartElement("Connections");
				uncSerializer.Serialize(writer, connection);
				writer.WriteEndElement();
				writer.WriteEndElement();

				writer.Close();
				uncStream.Close();
			}
		}

		/// <summary>
		/// Creates a new web connection.
		/// </summary>
		/// <param name="connection">The PostgreSql connection.</param>
		/// <param name="configPath">The config path.</param>
		/// <param name="configFile">The config file.</param>
		/// <remarks>Documented by Dev03, 2008-12-19</remarks>
		public static void CreateWebConnection(WebConnectionStringBuilder connection, string configPath, string configFile)
		{
			if (connection == null)
				throw new ArgumentNullException("connection can't be null");
			if (configPath == null)
				throw new ArgumentNullException("configPath can't be null");
			if (configFile == null)
				throw new ArgumentNullException("configFile can't be null");
			if (configPath.Trim().Length == 0)
				throw new ArgumentException("configPath can't be null");
			if (configFile.Trim().Length == 0)
				throw new ArgumentException("configFile can't be null");

			XmlSerializer uncSerializer = new XmlSerializer(typeof(WebConnectionStringBuilder));
			Directory.CreateDirectory(configPath);
			using (Stream uncStream = File.Create(Path.Combine(configPath, configFile)))
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				XmlWriter writer = XmlTextWriter.Create(uncStream, settings);
				writer.WriteStartElement("Configuration");
				writer.WriteStartElement("Connections");
				uncSerializer.Serialize(writer, connection);
				writer.WriteEndElement();
				writer.WriteEndElement();

				writer.Close();
				uncStream.Close();
			}
		}

		/// <summary>
		/// Saves all connections from the list into a NEW configFile
		/// </summary>
		/// <param name="connections">The connection list (can contain different types).</param>
		/// <param name="configFilePath">The config file path (path + filename).</param>
		/// <returns>The number of correct imported connections</returns>
		/// <remarks>Documented by Dev08, 2009-03-03</remarks>
		public static int CreateConnections(List<IConnectionString> connections, string configFilePath, List<ModuleFeedConnection> feeds = null)
		{
			if ((connections == null || connections.Count == 0) && (feeds == null || feeds.Count == 0))
				return 0;

			if (File.Exists(configFilePath))
				throw new Exception("Config File already exists.");

			int correctConnections = 0;
			Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));   //[ML-1575] Importing a connection (mlcfg) fails if no config folder exists
			using (Stream stream = File.Create(configFilePath))
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				XmlWriter writer = XmlTextWriter.Create(stream, settings);

				writer.WriteStartElement("Configuration");
				if (connections != null && connections.Count > 0)
				{
					writer.WriteStartElement("Connections");
					foreach (IConnectionString conString in connections)
					{
						XmlSerializer serializer;
						if (conString.ConnectionType == DatabaseType.PostgreSQL)
							serializer = PgSerializer;
						else if (conString.ConnectionType == DatabaseType.Unc)
							serializer = UncSerializer;
						else if (conString.ConnectionType == DatabaseType.Web)
							serializer = WebSerializer;
						else
							throw new NotImplementedException();

						serializer.Serialize(writer, conString);

						++correctConnections;
					}
					writer.WriteEndElement();
				}
				if (feeds != null && feeds.Count > 0)
				{
					writer.WriteStartElement("Feeds");
					foreach (ModuleFeedConnection feed in feeds)
					{
						FeedSerializer.Serialize(writer, feed);
						++correctConnections;
					}
					writer.WriteEndElement();
				}
				writer.WriteEndElement();

				writer.Close();
			}

			return correctConnections;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringHandler"/> class.
		/// </summary>
		/// <param name="generalPath">The path to the general config files (e.g. "%ProgrammFiles%\MemoryLifter\Config").</param>
		/// <param name="userPath">The path to the user config files (e.g. "%AppData%\MemoryLifter\Config").</param>
		/// <remarks>Documented by Dev05, 2008-12-01</remarks>
		public ConnectionStringHandler(string generalPath, string userPath)
			: this(generalPath, userPath, true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringHandler"/> class.
		/// </summary>
		/// <param name="generalPath">The path to the general config files (e.g. "%ProgrammFiles%\MemoryLifter\Config").</param>
		/// <param name="userPath">The path to the user config files (e.g. "%AppData%\MemoryLifter\Config").</param>
		/// <remarks>Documented by Dev05, 2008-12-01</remarks>
		public ConnectionStringHandler(string generalPath, string userPath, bool searchForSticks)
			: this()
		{
			if (generalPath != null && generalPath.Length > 0 && Directory.Exists(generalPath))
			{
				string[] configFiles = Directory.GetFiles(generalPath, "*" + MLifter.DAL.Helper.ConfigFileExtension, SearchOption.AllDirectories);
				ParseConfigFiles(configFiles);
			}

			if (userPath != null && userPath.Length > 0 && Directory.Exists(userPath))
			{
				string[] configFiles = Directory.GetFiles(userPath, "*" + MLifter.DAL.Helper.ConfigFileExtension, SearchOption.AllDirectories);
				ParseConfigFiles(configFiles);
			}

			if (!searchForSticks)
				return;

			foreach (DriveInfo drive in Methods.GetMLifterSticks())
			{
				if ((generalPath != null && generalPath.Length > 0 && Path.GetPathRoot(generalPath).ToLower() == drive.RootDirectory.ToString().ToLower())
					|| (userPath != null && userPath.Length > 0 && Path.GetPathRoot(userPath).ToLower() == drive.RootDirectory.ToString().ToLower()))
					continue;

				AddUsbDrive(drive);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringHandler"/> class.
		/// </summary>
		/// <param name="configFile">The path to one config file.</param>
		/// <remarks>Documented by Dev02, 2009-06-26</remarks>
		public ConnectionStringHandler(string configFile)
			: this()
		{
			if (File.Exists(configFile))
			{
				ParseConfigFiles(new string[] { configFile });
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionStringHandler"/> class.
		/// </summary>
		/// <remarks>CFI, 2012-03-03</remarks>
		internal ConnectionStringHandler() { Feeds = new ObservableCollection<ModuleFeedConnection>(); }

		/// <summary>
		/// Parses the config files.
		/// </summary>
		/// <param name="configFiles">The config files.</param>
		/// <remarks>Documented by Dev02, 2009-06-26</remarks>
		private void ParseConfigFiles(string[] configFiles)
		{
			foreach (string cfgFile in configFiles)
			{
				try
				{
					XmlReader reader = XmlReader.Create(cfgFile);
					reader.ReadToFollowing("Configuration");
					while (!reader.EOF)
					{
						reader.Read();
						while (reader.NodeType == XmlNodeType.Whitespace)
							reader.Read();
						if (reader.Name == "Connections")
						{
							reader.Read();
							while (reader.NodeType == XmlNodeType.Whitespace)
								reader.Read();
							IConnectionString cs = null;
							switch (reader.Name)
							{
								case "PostgreConnection":
									cs = PgSerializer.Deserialize(reader) as IConnectionString;
									break;
								case "UncConnection":
									cs = UncSerializer.Deserialize(reader) as IConnectionString;
									break;
								case "WebConnection":
									cs = WebSerializer.Deserialize(reader) as IConnectionString;
									break;
								default:
#if debug_output
								Trace.WriteLine("Unknow Connection-Property: " + reader.Name);
#endif
									break;
							}
							if (cs != null)
								cs.ConfigFileName = cfgFile;

							//only unique connections are allowed
							if (cs != null && !connectionStrings.Exists(c => c.ConnectionString == cs.ConnectionString && c.ConnectionType == cs.ConnectionType))
							{
								//only one default connection is allowed
								if (cs.IsDefault && connectionStrings.Exists(c => c.IsDefault))
									connectionStrings.ForEach(c => c.IsDefault = false);
								connectionStrings.Add(cs);
							}
						}
						else if (reader.Name == "Feeds")
						{
							reader.Read();
							while (reader.NodeType == XmlNodeType.Whitespace)
								reader.Read();
							ModuleFeedConnection feed = FeedSerializer.Deserialize(reader) as ModuleFeedConnection;
							if (feed != null)
								Feeds.Add(feed);
						}
					}
					reader.Close();
				}
				catch (Exception e) { Trace.WriteLine(e.ToString()); }
			}
		}
		/// <summary>
		/// Adds the usb drive connection.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>Documented by Dev05, 2009-04-01</remarks>
		public IConnectionString AddUsbDrive(DriveInfo drive)
		{
			//TODO this needs to be fixed
			UncConnectionStringBuilder uncCon = new UncConnectionStringBuilder(Path.Combine(drive.RootDirectory.FullName, Properties.Settings.Default.StickLMFolder), false, true);
			uncCon.Name = String.Format(Resources.CSH_CONNECTION_NAME, drive.RootDirectory.FullName);
			uncCon.ConfigFileName = string.Empty;
			if (!connectionStrings.Contains(uncCon as IConnectionString))
				connectionStrings.Add(uncCon as IConnectionString);

			return uncCon;
		}

		/// <summary>
		/// Gets the connection strings from file.
		/// </summary>
		/// <param name="cfgFile">The CFG file.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-03-03</remarks>
		public static List<IConnectionString> GetConnectionStringsFromFile(string cfgFile)
		{
			List<IConnectionString> conStrings = new List<IConnectionString>();
			XmlReader reader = XmlReader.Create(cfgFile);
			reader.ReadToFollowing("Connections");
			while (!reader.EOF)
			{
				switch (reader.Name)
				{
					case "PostgreConnection":
						conStrings.Add(PgSerializer.Deserialize(reader) as IConnectionString);
						break;
					case "UncConnection":
						conStrings.Add(UncSerializer.Deserialize(reader) as IConnectionString);
						break;
					case "WebConnection":
						conStrings.Add(WebSerializer.Deserialize(reader) as IConnectionString);
						break;
					default:
#if debug_output
						Trace.WriteLine("Unknow Connection-Property: " + reader.Name);
#endif
						reader.Read();
						break;
				}
			}
			reader.Close();

			return conStrings;
		}
		/// <summary>
		/// Gets the feeds from file.
		/// </summary>
		/// <param name="cfgFile">The CFG file.</param>
		/// <returns></returns>
		/// <remarks>CFI, 2012-03-03</remarks>
		public static List<ModuleFeedConnection> GetFeedsFromFile(string cfgFile)
		{
			List<ModuleFeedConnection> feeds = new List<ModuleFeedConnection>();
			XmlReader reader = XmlReader.Create(cfgFile);
			reader.ReadToFollowing("Feeds");
			while (!reader.EOF)
			{
				reader.Read();
				while (reader.NodeType == XmlNodeType.Whitespace)
					reader.Read();
				ModuleFeedConnection feed = FeedSerializer.Deserialize(reader) as ModuleFeedConnection;
				if (feed != null)
					feeds.Add(feed);
			}
			reader.Close();

			return feeds;
		}

		/// <summary>
		/// Puts the learning module to default UNC.
		/// </summary>
		/// <param name="sourcePath">The source path.</param>
		/// <param name="destinationPath">The destination path.</param>
		/// <param name="deleteSource">if set to <c>true</c> [delete source].</param>
		/// <remarks>Documented by Dev08, 2009-03-03</remarks>
		public static void PutLearningModuleToDefaultUNC(string sourcePath, string destinationPath, bool deleteSource)
		{
			PutLearningModuleToDefaultUNC(sourcePath, destinationPath, deleteSource, false);
		}

		/// <summary>
		/// Puts the learning module to default UNC.
		/// </summary>
		/// <param name="sourcePath">The source path.</param>
		/// <param name="destinationPath">The destination path.</param>
		/// <param name="deleteSource">if set to <c>true</c> [delete source].</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
		/// <remarks>Documented by Dev08, 2009-03-03</remarks>
		public static void PutLearningModuleToDefaultUNC(string sourcePath, string destinationPath, bool deleteSource, bool overwrite)
		{
			if (overwrite && File.Exists(Path.Combine(destinationPath, Path.GetFileName(sourcePath))))
				File.Delete(Path.Combine(destinationPath, Path.GetFileName(sourcePath)));

			File.Copy(sourcePath, Path.Combine(destinationPath, Path.GetFileName(sourcePath)));

			if (deleteSource)
				File.Delete(sourcePath);
		}

		/// <summary>
		/// Imports the mlcfg-file.
		/// </summary>
		/// <param name="filepath">The filepath.</param>
		/// <param name="generalPath">The general path.</param>
		/// <param name="userPath">The user path.</param>
		/// <returns>The number of successfully imported/updated connections</returns>
		/// <remarks>Documented by Dev08, 2009-03-02</remarks>
		public static int ImportConfigFile(string filepath, string generalPath, string userPath)
		{
			ConnectionStringHandler currentHandler = new ConnectionStringHandler(generalPath, userPath);
			List<IConnectionString> fromNewFileConnectionStrings;
			List<IConnectionString> importConnectionList = new List<IConnectionString>();
			List<ModuleFeedConnection> feedsInFile;
			List<ModuleFeedConnection> importFeeds = new List<ModuleFeedConnection>();

			try
			{
				fromNewFileConnectionStrings = GetConnectionStringsFromFile(filepath);
				feedsInFile = GetFeedsFromFile(filepath);
			}
			catch
			{
				throw new InvalidConfigFileException();
			}

			if (fromNewFileConnectionStrings.Count + feedsInFile.Count == 0)
				return 0;

			//Compare and write new connections to importConnectionList
			foreach (IConnectionString newConString in fromNewFileConnectionStrings)
			{
				bool isEqual = false;
				foreach (IConnectionString oldConString in currentHandler.ConnectionStrings)
					isEqual = newConString.ConnectionString == oldConString.ConnectionString &&	newConString.ConnectionType == oldConString.ConnectionType;

				if (!isEqual)
					importConnectionList.Add(newConString);
			}

			//compare and write new feeds to importFeeds
			foreach (ModuleFeedConnection feed in feedsInFile)
			{
				bool isEqual = false;
				foreach (ModuleFeedConnection oldFeed in currentHandler.Feeds)
					isEqual = oldFeed.ModulesUri == feed.ModulesUri && oldFeed.CategoriesUri == feed.CategoriesUri;
				
				if (!isEqual)
					importFeeds.Add(feed);
			}

			if (importConnectionList.Count + importFeeds.Count == 0)
				return 0;

			//Search for a config file with the same filename...
			int loopKillCounter = 1;
			string newConfigFile = Path.Combine(userPath, Path.GetFileName(filepath));
			while (File.Exists(newConfigFile) && loopKillCounter <= 10000)
			{
				newConfigFile = Path.Combine(Path.GetDirectoryName(newConfigFile),
											 Path.GetFileNameWithoutExtension(newConfigFile) + "_" + loopKillCounter.ToString() + Path.GetExtension(newConfigFile));
				++loopKillCounter;
			}

			return CreateConnections(importConnectionList, newConfigFile, importFeeds);
		}

		/// <summary>
		/// Checks if the given filename is in one of the available UNC connections
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns>
		/// 	<c>true</c> if [is file in unc connection] [the specified filename]; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev08, 2009-03-03</remarks>
		public static bool IsFileInUncConnection(string filename, string generalPath, string userPath)
		{
			ConnectionStringHandler handler = new ConnectionStringHandler(generalPath, userPath);
			foreach (IConnectionString conString in handler.ConnectionStrings)
			{
				if (conString.ConnectionType != DatabaseType.Unc)
					continue;

				if (Path.GetDirectoryName(filename).StartsWith(conString.ConnectionString))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the connection from connection string struct.
		/// </summary>
		/// <param name="connectionsToSearchIn">The connections to search in.</param>
		/// <param name="connectionStringStructToFind">The connection string struct to find.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-04</remarks>
		public IConnectionString GetConnectionFromConnectionStringStruct(ConnectionStringStruct connectionStringStructToFind)
		{
			switch (connectionStringStructToFind.Typ)
			{
				case DatabaseType.Xml:
				case DatabaseType.Unc:
				case DatabaseType.MsSqlCe:
					return ConnectionStrings.Find(c => c.ConnectionString == connectionStringStructToFind.LearningModuleFolder);
				case DatabaseType.PostgreSQL:
					return ConnectionStrings.Find(c => c.ConnectionString == connectionStringStructToFind.ConnectionString);
				case DatabaseType.Web:
					return ConnectionStrings.Find(c => c.ConnectionString == connectionStringStructToFind.ConnectionString);
				default:
					throw new ArgumentException();
			}
		}

		/// <summary>
		/// Gets the default connection string. If there are more than one default conStrings (which isn't allowed), this methods returns the first
		/// located default connection. If there is no default connection string, it returns null.
		/// </summary>
		/// <param name="configPath">The config path (e.g. %ProgramFiles%\MemoryLifter\Config).</param>
		/// <returns>The default connection string if there is one, otherwise null</returns>
		/// <remarks>Documented by Dev08, 2009-04-01</remarks>
		public static IConnectionString GetDefaultConnectionString(string configPath)
		{
			ConnectionStringHandler csh = new ConnectionStringHandler(configPath, string.Empty, false);
			foreach (IConnectionString conString in csh.ConnectionStrings)
			{
				if (conString.IsDefault)
					return conString;
			}
			return null;
		}

		/// <summary>
		/// Saves the given connection string to the configPath. If the conString.Name doesn't exists, a new connection will be created.
		/// If the conString.Name exists it will be updated.
		/// </summary>
		/// <param name="configPath">The config path (e.g. %ProgramFiles%\MemoryLifter\Config).</param>
		/// <param name="conString">The ConnectionString. ATTENTION: conString.ConfigFileName must be a valid FileName!!!</param>
		/// <param name="identifier">The identifier.</param>
		/// <exception cref="T:System.IO.IOException" />
		/// <remarks>Documented by Dev08, 2009-04-01</remarks>
		public static void SaveConnectionString(string configPath, IConnectionString conString, ConnectionStringIdentifier identifier)
		{
			//todo: get all connections from the configPath
			//todo: search for the connectionString based on the identifier
			//todo: update/create the new connection

			//1. Get all connections
			ConnectionStringHandler handler = new ConnectionStringHandler(configPath, string.Empty, false);

			//2. Check if the given connection is already available (search based on identifier)
			bool connectionStringLocated = false;
			foreach (IConnectionString cs in handler.ConnectionStrings)
			{
				if (identifier == ConnectionStringIdentifier.IdentifyByConnectionStringAndType)
				{
					if (cs.ConnectionString == conString.ConnectionString && cs.ConnectionType == conString.ConnectionType)
					{
						conString.ConfigFileName = cs.ConfigFileName;
						connectionStringLocated = true;
						break;
					}
				}
				else
				{
					if (cs.Name == conString.Name)
					{
						conString.ConfigFileName = cs.ConfigFileName;
						connectionStringLocated = true;
						break;
					}
				}
			}

			//3.1 Update connectionstring
			if (!Directory.Exists(Path.GetDirectoryName(conString.ConfigFileName)))
				throw new IOException("The given connectionString does not have a valid value in ConfigFileName property.");

			if (connectionStringLocated)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(File.OpenWrite(conString.ConfigFileName));

				foreach (XmlNode item in doc.GetElementsByTagName("Connections"))
				{
					foreach (XmlNode subItem in item.ChildNodes)
					{

					}
				}
			}
			else    //3.2 Create new connection; Should not happen, because the startupWiz always creates a connection
			{
				List<IConnectionString> list = new List<IConnectionString>();
				list.Add(conString);
				CreateConnections(list, conString.ConfigFileName);
			}
		}
	}

	/// <summary>
	/// Enum to set the identifier Parameter for a ConnectionString. (e.g. identify by name, identify by ConnectionString)
	/// </summary>
	public enum ConnectionStringIdentifier
	{
		/// <summary>
		/// Identifies the Connection by Connection.Name
		/// </summary>
		IdentifyByName,
		/// <summary>
		/// Identifies the Connection by Connection.ConnectionString
		/// </summary>
		IdentifyByConnectionStringAndType
	}

	/// <summary>
	/// This is the implementation of a XML-Serialzable WebConnection-ConnectionString Builder.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-01-28</remarks>
	[XmlRoot("WebConnection"), Serializable()]
	public class WebConnectionStringBuilder : ISyncableConnectionString
	{
		/// <summary>
		/// Gets or sets the learning modules URI.
		/// </summary>
		/// <value>The learning modules URI.</value>
		/// <remarks>Documented by Dev05, 2009-01-29</remarks>
		public string LearningModulesURI { get; set; }

		/// <summary>
		/// Gets or sets the type of the sync.
		/// </summary>
		/// <value>The type of the sync.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public SyncType SyncType { get; set; }

		/// <summary>
		/// Gets or sets the sync URI.
		/// </summary>
		/// <value>The sync URI.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public string SyncURI { get; set; }

		/// <summary>
		/// Gets or sets the Media URI.
		/// </summary>
		/// <value>The Media URI.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public string MediaURI { get; set; }

		/// <summary>
		/// Gets or sets the extension URI.
		/// </summary>
		/// <value>The extension URI.</value>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		public string ExtensionURI { get; set; }

		public WebConnectionStringBuilder() { }

		#region IConnectionString Members

		public string Name { get; set; }

		public string ConnectionString
		{
			get { return LearningModulesURI; }
			set { LearningModulesURI = value; }
		}

		public DatabaseType ConnectionType
		{
			get { return DatabaseType.Web; }
		}

		public bool IsDefault { get; set; }

		public string ConfigFileName { get; set; }

		#endregion
		#region IXmlSerializable Members

		/// <summary>
		/// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
		/// </returns>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public void ReadXml(XmlReader reader)
		{
			reader.Read();

			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				switch (reader.Name)
				{
					case "Name":
						Name = reader.ReadElementContentAsString().Trim();
						break;
					case "LearningModulesURI":
						LearningModulesURI = reader.ReadElementContentAsString().Trim();
						break;
					case "SyncType":
						SyncType = (SyncType)Enum.Parse(typeof(SyncType), reader.ReadElementContentAsString().Trim());
						break;
					case "SyncURI":
						SyncURI = reader.ReadElementContentAsString().Trim();
						break;
					case "MediaURI":
						MediaURI = reader.ReadElementContentAsString().Trim() + Resources.MediaURI_Suffix;
						break;
					case "ExtensionURI":
						ExtensionURI = reader.ReadElementContentAsString().Trim() + Resources.ExtensionURI_Suffix;
						break;
					default:
#if debug_output
						Trace.WriteLine("Unknow Connection-Property: " + reader.Name);
#endif
						reader.Read();
						break;
				}
			}

			if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(LearningModulesURI))
				throw new Exception("Invalid connection name or path.");

			reader.ReadEndElement();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Name");
			writer.WriteValue(Name);
			writer.WriteEndElement();

			writer.WriteStartElement("LearningModulesURI");
			writer.WriteValue(LearningModulesURI);
			writer.WriteEndElement();

			writer.WriteStartElement("SyncType");
			writer.WriteValue(SyncType.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("SyncURI");
			writer.WriteValue(SyncURI);
			writer.WriteEndElement();

			writer.WriteStartElement("MediaURI");
			writer.WriteValue((MediaURI == null) ? String.Empty : MediaURI.Replace(Resources.MediaURI_Suffix, string.Empty));
			writer.WriteEndElement();

			writer.WriteStartElement("ExtensionURI");
			writer.WriteValue((ExtensionURI == null) ? String.Empty : ExtensionURI.Replace(Resources.ExtensionURI_Suffix, string.Empty));
			writer.WriteEndElement();
		}

		#endregion
	}

	/// <summary>
	/// This is the implementation of a XML-Serialzable PostgreSQL-ConnectionString Builder.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-12-01</remarks>
	[XmlRoot("PostgreConnection"), Serializable()]
	public class PostgreSqlConnectionStringBuilder : ISyncableConnectionString
	{
		private static XmlSerializer otherPropertiesSerializer = new XmlSerializer(typeof(SerializableDictionary<string, object>), new XmlRootAttribute("OtherProperties"));

		#region fixed values
		[ConnectionStringProperty]
		private int protocol = Properties.Settings.Default.PgConnectionProtocol;
		[ConnectionStringProperty]
		private bool pooling = Properties.Settings.Default.PgConnectionPooling;
		[ConnectionStringProperty]
		private int minPoolSize = Properties.Settings.Default.PgConnectionMinPoolSize;
		[ConnectionStringProperty]
		private int maxPoolSize = Properties.Settings.Default.PgConnectionMaxPoolSize;
		[ConnectionStringProperty]
		private string encoding = Properties.Settings.Default.PgConnectionEncoding;
		[ConnectionStringProperty]
		private int timeout = Properties.Settings.Default.PgConnectionTimeout;
		[ConnectionStringProperty]
		private string SslMode = "Disable";
		#endregion
		#region Properties

		private string server;
		/// <summary>
		/// Gets or sets the server.
		/// </summary>
		/// <value>The server.</value>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		[ConnectionStringProperty]
		public string Server
		{
			get { return server; }
			set { server = value; }
		}

		private int port;
		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		[ConnectionStringProperty]
		public int Port
		{
			get { return port; }
			set { port = value; }
		}

		private string userId;
		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The user id.</value>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		[ConnectionStringProperty]
		public string UserId
		{
			get { return userId; }
			set { userId = value; }
		}

		private string password;
		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		[ConnectionStringProperty]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		private bool ssl;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PostgreSqlConnectionStringBuilder"/> use SSL.
		/// </summary>
		/// <value><c>true</c> if SSL; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		[ConnectionStringProperty]
		public bool SSL
		{
			get { return ssl; }
			set
			{
				ssl = value;
				SslMode = value ? "Require" : "Disable";
			}
		}

		private string database;
		/// <summary>
		/// Gets or sets the database.
		/// </summary>
		/// <value>The database.</value>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		[ConnectionStringProperty]
		public string Database
		{
			get { return database; }
			set { database = value; }
		}

		/// <summary>
		/// Gets or sets the type of the sync.
		/// </summary>
		/// <value>The type of the sync.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public SyncType SyncType { get; set; }

		/// <summary>
		/// Gets or sets the sync URI.
		/// </summary>
		/// <value>The sync URI.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public string SyncURI { get; set; }

		/// <summary>
		/// Gets or sets the Media URI.
		/// </summary>
		/// <value>The Media URI.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		public string MediaURI { get; set; }

		/// <summary>
		/// Gets or sets the extension URI.
		/// </summary>
		/// <value>The extension URI.</value>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		public string ExtensionURI { get; set; }

		private SerializableDictionary<string, object> otherProperties = new SerializableDictionary<string, object>();
		public SerializableDictionary<string, object> OtherProperties
		{
			get { return otherProperties; }
			set { otherProperties = value; }
		}

		#endregion

		public PostgreSqlConnectionStringBuilder() { }

		public override string ToString()
		{
			string connectionString = string.Empty;

			foreach (PropertyInfo prop in this.GetType().GetProperties())
			{
				if (prop.IsDefined(typeof(ConnectionStringProperty), true))
					connectionString += string.Format("{0}={1};", prop.Name, prop.GetValue(this, null).ToString());
			}
			foreach (KeyValuePair<string, object> pair in otherProperties)
				connectionString += string.Format("{0}={1};", pair.Key, pair.Value.ToString());

			connectionString += string.Format("protocol={0};pooling={1};MinPoolSize={2};MaxPoolSize={3};Encoding={4};Timeout={5};SslMode={6};",
				protocol, pooling, minPoolSize, maxPoolSize, encoding, timeout, SslMode);

			return connectionString;
		}

		#region IXmlSerializable Members

		/// <summary>
		/// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
		/// </returns>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		public System.Xml.Schema.XmlSchema GetSchema() { return null; }

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.Read();

			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				switch (reader.Name)
				{
					case "Name":
						name = reader.ReadElementContentAsString().Trim();
						break;
					case "Server":
						server = reader.ReadElementContentAsString().Trim();
						break;
					case "Port":
						port = reader.ReadElementContentAsInt();
						break;
					case "UserId":
						userId = reader.ReadElementContentAsString().Trim();
						break;
					case "Password":
						password = reader.ReadElementContentAsString().Trim();
						break;
					case "Ssl":
						ssl = reader.ReadElementContentAsBoolean();
						break;
					case "Database":
						database = reader.ReadElementContentAsString().Trim();
						break;
					case "SyncType":
						SyncType = (SyncType)Enum.Parse(typeof(SyncType), reader.ReadElementContentAsString().Trim());

						// [ML-2349]  Deactivate HalfSynchronizedWithDbAccess sync mode
						if (SyncType == SyncType.HalfSynchronizedWithDbAccess)
							SyncType = SyncType.HalfSynchronizedWithoutDbAccess;

						break;
					case "SyncURI":
						SyncURI = reader.ReadElementContentAsString().Trim();
						break;
					case "MediaURI":
						MediaURI = reader.ReadElementContentAsString().Trim() + Resources.MediaURI_Suffix;
						break;
					case "ExtensionURI":
						ExtensionURI = reader.ReadElementContentAsString().Trim() + Resources.ExtensionURI_Suffix;
						break;
					case "OtherProperties":
						otherProperties = otherPropertiesSerializer.Deserialize(reader) as SerializableDictionary<string, object>;
						break;
					default:
#if debug_output
						Trace.WriteLine("Unknow Connection-Property: " + reader.Name);
#endif
						reader.Read();
						break;
				}
			}

			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(server))
				throw new Exception("Invalid connection name or path.");

			reader.ReadEndElement();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Name");
			writer.WriteValue(name);
			writer.WriteEndElement();

			writer.WriteStartElement("Server");
			writer.WriteValue(server);
			writer.WriteEndElement();

			writer.WriteStartElement("Port");
			writer.WriteValue(port);
			writer.WriteEndElement();

			writer.WriteStartElement("UserId");
			writer.WriteValue(userId);
			writer.WriteEndElement();

			writer.WriteStartElement("Password");
			writer.WriteValue(password);
			writer.WriteEndElement();

			writer.WriteStartElement("Ssl");
			writer.WriteValue(ssl);
			writer.WriteEndElement();

			writer.WriteStartElement("Database");
			writer.WriteValue(database);
			writer.WriteEndElement();

			writer.WriteStartElement("SyncType");
			writer.WriteValue(SyncType.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("SyncURI");
			writer.WriteValue((SyncURI == null) ? String.Empty : SyncURI);
			writer.WriteEndElement();

			writer.WriteStartElement("MediaURI");
			writer.WriteValue((MediaURI == null) ? String.Empty : MediaURI.Replace(Resources.MediaURI_Suffix, string.Empty));
			writer.WriteEndElement();

			writer.WriteStartElement("ExtensionURI");
			writer.WriteValue((ExtensionURI == null) ? String.Empty : ExtensionURI.Replace(Resources.ExtensionURI_Suffix, string.Empty));
			writer.WriteEndElement();

			otherPropertiesSerializer.Serialize(writer, otherProperties);
		}

		#endregion
		#region IConnectionString Members

		private string name;
		/// <summary>
		/// Gets or sets the name of this connection.
		/// </summary>
		/// <value>The name.</value>
		/// <remarks>Documented by Dev05, 2008-12-02</remarks>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		/// <remarks>Documented by Dev05, 2008-11-27</remarks>
		public string ConnectionString
		{
			get
			{
				return this.ToString();
			}
			set
			{
				Trace.WriteLine("not allowed");
			}
		}

		public DatabaseType ConnectionType
		{
			get { return DatabaseType.PostgreSQL; }
		}

		public bool IsDefault { get; set; }

		public string ConfigFileName { get; set; }

		#endregion
	}

	/// <summary>
	/// This is the implementation of a XML-Serialzable UNC-Path-ConnectionString Builder.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-12-01</remarks>
	[XmlRoot("UncConnection"), Serializable()]
	public class UncConnectionStringBuilder : IConnectionString
	{
		private string connectionString;

		[Obsolete()]
		public UncConnectionStringBuilder()
		{
			this.IsDefault = false;
		}

		public UncConnectionStringBuilder(string uncPath)
		{
			this.connectionString = uncPath;
			this.IsDefault = false;
			this.IsOnStick = false;
		}

		public UncConnectionStringBuilder(string uncPath, bool isDefault, bool isOnStick)
		{
			this.connectionString = uncPath;
			this.IsDefault = isDefault;
			this.IsOnStick = isOnStick;
		}

		#region IXmlSerializable Members

		/// <summary>
		/// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
		/// </returns>
		/// <remarks>Documented by Dev05, 2008-12-01</remarks>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
		/// <remarks>Documented by Dev05, 2008-12-01</remarks>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.Read();

			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				switch (reader.Name)
				{
					case "Name":
						name = reader.ReadElementContentAsString().Trim();
						break;
					case "UncPath":
						connectionString = reader.ReadElementContentAsString().Trim().Replace("%MLMyDocuments%", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
						connectionString = connectionString.Replace("MEMSTICK:\\", Path.GetPathRoot(Environment.CommandLine.Replace("\"", string.Empty)));
						break;
					case "IsDefault":
						IsDefault = reader.ReadElementContentAsBoolean();
						break;
					default:
#if debug_output
						Trace.WriteLine("Unknow Connection-Property: " + reader.Name);
#endif
						reader.Read();
						break;
				}
			}

			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(connectionString))
				throw new Exception("Invalid connection name or path.");

			reader.ReadEndElement();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
		/// <remarks>Documented by Dev05, 2008-12-01</remarks>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("Name");
			writer.WriteValue(name);
			writer.WriteEndElement();

			writer.WriteStartElement("UncPath");
			if (this.IsOnStick)
				writer.WriteValue(connectionString.Replace(Path.GetPathRoot(connectionString), "MEMSTICK:\\"));
			else
				writer.WriteValue(connectionString);
			writer.WriteEndElement();

			writer.WriteStartElement("IsDefault");
			writer.WriteValue(IsDefault);
			writer.WriteEndElement();
		}

		#endregion
		#region IConnectionString Members

		private string name;
		/// <summary>
		/// Gets or sets the name of this connection.
		/// </summary>
		/// <value>The name.</value>
		/// <remarks>Documented by Dev05, 2008-12-02</remarks>
		[ConnectionStringProperty]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string ConnectionString
		{
			get { return connectionString; }
			set { connectionString = value; }
		}

		public DatabaseType ConnectionType
		{
			get { return DatabaseType.Unc; }
		}

		public bool IsDefault { get; set; }

		public bool IsOnStick { get; private set; }

		public string ConfigFileName { get; set; }

		#endregion
	}

	/// <summary>
	/// This is a modules feed connection.
	/// </summary>
	/// <remarks>CFI, 2012-03-03</remarks>
	[Serializable, XmlRoot("ModuleFeed")]
	public class ModuleFeedConnection
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		/// <remarks>CFI, 2012-03-03</remarks>
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the categories URI.
		/// </summary>
		/// <value>
		/// The categories URI.
		/// </value>
		/// <remarks>CFI, 2012-03-03</remarks>
		public string CategoriesUri { get; set; }
		/// <summary>
		/// Gets or sets the modules URI.
		/// </summary>
		/// <value>
		/// The modules URI.
		/// </value>
		/// <remarks>CFI, 2012-03-03</remarks>
		public string ModulesUri { get; set; }
	}

	/// <summary>
	/// General interface for ConnectionString(Builder).
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-12-01</remarks>
	public interface IConnectionString : IXmlSerializable
	{
		string Name { get; set; }
		string ConnectionString { get; set; }
		DatabaseType ConnectionType { get; }
		bool IsDefault { get; set; }
		string ConfigFileName { get; set; }
	}

	/// <summary>
	/// A serializible connection.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-02-27</remarks>
	public interface ISyncableConnectionString : IConnectionString
	{
		/// <summary>
		/// Gets or sets the type of the sync.
		/// </summary>
		/// <value>The type of the sync.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		SyncType SyncType { get; set; }

		/// <summary>
		/// Gets or sets the sync URI.
		/// </summary>
		/// <value>The sync URI.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		string SyncURI { get; set; }

		/// <summary>
		/// Gets or sets the Media URI.
		/// </summary>
		/// <value>The Media URI.</value>
		/// <remarks>Documented by Dev05, 2009-01-28</remarks>
		string MediaURI { get; set; }

		/// <summary>
		/// Gets or sets the extension URI.
		/// </summary>
		/// <value>The extension URI.</value>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		string ExtensionURI { get; set; }
	}

	/// <summary>
	/// This attribute defines a property which will be appended to the generated connection string.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-01-28</remarks>
	public class ConnectionStringProperty : Attribute { }
}
