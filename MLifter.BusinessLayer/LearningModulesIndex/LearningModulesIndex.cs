using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using MLifter.DAL;
using MLifter.DAL.DB.PostgreSQL;
using MLifter.DAL.Interfaces;
using MLifter.Generics;

namespace MLifter.BusinessLayer
{
	/// <summary>
	/// This class holds an LearningModulesIndexEntry for each available learning module.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-02-18</remarks>
	public partial class LearningModulesIndex : IDisposable
	{
		private GetLoginInformation getLogin;
		private DataAccessErrorDelegate dataAccessError;
		private Dictionary<object, Thread> threads = new Dictionary<object, Thread>();
		private System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();

		/// <summary>
		/// Gets a value indicating whether content is loading.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if content is loading; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		public bool IsContentLoading
		{
			get
			{
				lock (loadingFolders)
				{
					lock (loadingModules)
					{
						return loadingFolders.Count > 0 || loadingModules.Count > 0;
					}
				}
			}
		}

		/// <summary>
		/// Occurs when loading finished.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-24</remarks>
		public event EventHandler LoadingFinished;
		/// <summary>
		/// Raises the <see cref="E:LoadingFinished"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-24</remarks>
		protected virtual void OnLoadingFinished(EventArgs e)
		{
			if (LoadingFinished != null)
				LoadingFinished(this, e);
		}
		/// <summary>
		/// Occurs when a folder was added.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public event EventHandler<FolderAddedEventArgs> FolderAdded;
		/// <summary>
		/// Raises the <see cref="E:FolderAdded"/> event.
		/// </summary>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.FolderAddedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		protected virtual void OnFolderAdded(FolderAddedEventArgs e)
		{
			if (FolderAdded != null)
				FolderAdded(this, e);
		}
		/// <summary>
		/// Occurs when a learning module was added.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public event EventHandler<LearningModuleAddedEventArgs> LearningModuleAdded;
		/// <summary>
		/// Raises the <see cref="E:LearningModuleAdded"/> event.
		/// </summary>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.LearningModuleAddedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		protected virtual void OnLearningModuleAdded(LearningModuleAddedEventArgs e)
		{
			if (LearningModuleAdded != null)
				LearningModuleAdded(this, e);
		}
		/// <summary>
		/// Occurs when a learning module is updated.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-02-18</remarks>
		public event LearningModuleUpdatedEventHandler LearningModuleUpdated;
		/// <summary>
		/// Raises the <see cref="E:LearningModuleUpdated"/> event.
		/// </summary>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.LearningModuleUpdatedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		protected virtual void OnLearningModuleUpdated(LearningModuleUpdatedEventArgs e)
		{
			if (LearningModuleUpdated != null)
				LearningModuleUpdated(this, e);
		}

		private List<FolderIndexEntry> loadingFolders = new List<FolderIndexEntry>(100);
		private List<LearningModulesIndexEntry> loadingModules = new List<LearningModulesIndexEntry>(100);

		/// <summary>
		/// Occurs when content is loading.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		public event EventHandler FolderContentLoading;
		/// <summary>
		/// Raises the <see cref="E:FolderContentLoading"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		protected virtual void OnFolderContentLoading(FolderIndexEntry sender, EventArgs e)
		{
			lock (loadingFolders)
			{
				try
				{
					if (!loadingFolders.Contains(sender))
						loadingFolders.Add(sender);
				}
				catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
				updateTimer.Enabled = true;
			}
			if (FolderContentLoading != null)
				FolderContentLoading(sender, e);
		}
		/// <summary>
		/// Occurs when folder content is loaded.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		public event EventHandler FolderContentLoaded;
		/// <summary>
		/// Raises the <see cref="E:FolderContentLoaded"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		protected virtual void OnFolderContentLoaded(FolderIndexEntry sender, EventArgs e)
		{
			lock (loadingFolders)
			{
				try
				{
					if (loadingFolders.Contains(sender))
						loadingFolders.Remove(sender);
				}
				catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
			}
			if (FolderContentLoaded != null)
				FolderContentLoaded(sender, e);
		}

		/// <summary>
		/// Occurs when content is loading.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		public event EventHandler LearningModuleDetailsLoading;
		/// <summary>
		/// Raises the <see cref="E:LearningModuleDetailsLoading"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		protected virtual void OnLearningModuleDetailsLoading(LearningModulesIndexEntry sender, EventArgs e)
		{
			lock (loadingModules)
			{
				try
				{
					if (!loadingModules.Contains(sender) && sender != null)
						loadingModules.Add(sender);
				}
				catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
				updateTimer.Enabled = true;
			}
			if (LearningModuleDetailsLoading != null)
				LearningModuleDetailsLoading(sender, e);
		}
		/// <summary>
		/// Occurs when learning module details loaded.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		public event EventHandler LearningModuleDetailsLoaded;
		/// <summary>
		/// Raises the <see cref="E:LearningModuleDetailsLoaded"/> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		protected virtual void OnLearningModuleDetailsLoaded(LearningModulesIndexEntry sender, EventArgs e)
		{
			lock (loadingModules)
			{
				try
				{
					if (loadingModules.Contains(sender))
						loadingModules.Remove(sender);
				}
				catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
			}
			if (LearningModuleDetailsLoaded != null)
				LearningModuleDetailsLoaded(sender, e);
		}

		public List<FolderIndexEntry> Folders { get; set; }

		private List<LearningModulesIndexEntry> learningModules;

		public List<LearningModulesIndexEntry> getAvailableLearningModules 
		{ 
			get
			{
				return learningModules;
			}
		}

		/// <summary>
		/// Deletes the entry.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev09, 2009-05-15</remarks>
		public bool DeleteEntry(LearningModulesIndexEntry entry)
		{
			if (learningModules.Contains(entry))
				learningModules.Remove(entry);

			return false;
		}

		/// <summary>
		/// Gets or sets the learning modules.
		/// </summary>
		/// <value>The learning modules.</value>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		public List<LearningModulesIndexEntry> LearningModules
		{
			get
			{
				List<IConnectionString> offline = new List<IConnectionString>();

				//TODO not a pretty solution
				foreach (IConnectionString con in ConnectionsHandler.ConnectionStrings.ToArray()) 
						//toarray is necessary to avoid exception when collection is modified during foreach [ML-2333]
					try
					{
						GetFolderOfConnection(con);
					}
					catch (ServerOfflineException)
					{
						offline.Add(con);
					}
					catch (Exception ex)
					{
						Trace.WriteLine("Exception in LearningModulesIndex.LearningModules - " + ex.Message);
					}

				if (offline.Count > 0 && ServersOffline != null)
					ServersOffline(offline);

				return learningModules;
			}
		}

		/// <summary>
		/// Occurs when one or more servers are offline.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-04-02</remarks>
		public event ServersOfflineEventHandler ServersOffline;

		public delegate void ServersOfflineEventHandler(List<IConnectionString> offline);

		/// <summary>
		/// Gets or sets the groups.
		/// </summary>
		/// <value>The groups.</value>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		public static List<ListViewGroup> Groups { get; set; }
		/// <summary>
		/// Gets or sets the users.
		/// </summary>
		/// <value>The users.</value>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		public List<IUser> Users { get; set; }
		/// <summary>
		/// Gets or sets the user to each connection.
		/// </summary>
		/// <value>The connection users.</value>
		/// <remarks>Documented by Dev05, 2008-12-15</remarks>
		public static Dictionary<IConnectionString, IUser> ConnectionUsers { get; private set; }

		/// <summary>
		/// Gets the writable connections.
		/// </summary>
		/// <value>The writable connections.</value>
		/// <remarks>Documented by Dev05, 2009-02-18</remarks>
		public static List<IConnectionString> WritableConnections
		{
			get
			{
				List<IConnectionString> cons = new List<IConnectionString>();
				foreach (IConnectionString con in ConnectionsHandler.ConnectionStrings)
					if (!(typeof(ISyncableConnectionString).IsAssignableFrom(con.GetType())) || (con as ISyncableConnectionString).SyncType == SyncType.NotSynchronized)
						cons.Add(con);
				return cons;
			}
		}

		/// <summary>
		/// Gets the connections handler.
		/// </summary>
		/// <value>The connections handler.</value>
		/// <remarks>Documented by Dev05, 2009-02-18</remarks>
		public static ConnectionStringHandler ConnectionsHandler { get; private set; }

		private string cacheFile = null;
		/// <summary>
		/// Gets the cache file.
		/// </summary>
		/// <value>The cache file.</value>
		/// <remarks>Documented by Dev05, 2009-01-26</remarks>
		public string CacheFile { get { return cacheFile; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="LearningModulesIndex"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-06-05</remarks>
		private LearningModulesIndex()
		{
			Folders = new List<FolderIndexEntry>();
			learningModules = new List<LearningModulesIndexEntry>(100);
			Groups = new List<ListViewGroup>();
			Users = new List<IUser>();
			ConnectionUsers = new Dictionary<IConnectionString, IUser>();
		}

		private string SyncedModulesPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="LearningModulesIndex"/> class.
		/// </summary>
		/// <param name="generalPath">The general path.</param>
		/// <param name="userPath">The path to the user configurations.</param>
		/// <param name="getLoginDelegate">The get login delegate.</param>
		/// <param name="dataAccessErrorDelegate">The data access error delegate.</param>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public LearningModulesIndex(string generalPath, string userPath, GetLoginInformation getLoginDelegate, DataAccessErrorDelegate dataAccessErrorDelegate, string syncedModulesPath)
			: this()
		{
			SyncedModulesPath = syncedModulesPath;

			getLogin = getLoginDelegate;
			dataAccessError = dataAccessErrorDelegate;

			updateTimer.Interval = 1000;
			updateTimer.Tick += new EventHandler(updateTimer_Tick);

			if (userPath != null && userPath != string.Empty)
			{
				cacheFile = System.IO.Path.Combine(userPath, Properties.Settings.Default.LMIndexCacheFilename);
				try
				{
					if (System.IO.File.Exists(cacheFile))
						RestoreIndexCache(cacheFile);
				}
				catch (Exception e)
				{
					Trace.WriteLine("Index entry cache restore failed: " + e.ToString());
				}
			}

			ConnectionsHandler = new ConnectionStringHandler(generalPath, userPath);

			ConnectionUsers.Clear();
			foreach (IConnectionString con in ConnectionsHandler.ConnectionStrings)
			{
				ListViewGroup group = new ListViewGroup(con.Name);
				group.Tag = con;
				Groups.Add(group);
			}
		}
		private void updateTimer_Tick(object sender, EventArgs e)
		{
			if (loadingFolders != null)
			{
				lock (loadingFolders)
				{
					try
					{
						List<FolderIndexEntry> folders = loadingFolders.FindAll(f => f != null && !f.IsLoading);
						folders.ForEach(f => loadingFolders.Remove(f));
					}
					catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
				}
			}

			if (loadingModules != null)
			{
				lock (loadingModules)
				{
					try
					{
						List<LearningModulesIndexEntry> lms = loadingModules.FindAll(m => m == null || m.IsVerified);
						lms.ForEach(m => loadingModules.Remove(m));
					}
					catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
				}
			}


			try
			{
				if (!IsContentLoading)
					updateTimer.Enabled = false;
				else
					OnLoadingFinished(EventArgs.Empty);
			}
			catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
		}

		/// <summary>
		/// Gets the folder of a connection.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		public FolderIndexEntry GetFolderOfConnection(IConnectionString connection)
		{
			if (connection == null)
				return null;

			FolderIndexEntry entry = Folders.Find(f => f.Connection == connection);
			if (entry != null)
				return entry;

			entry = new FolderIndexEntry(connection is UncConnectionStringBuilder ? connection.ConnectionString : string.Empty, connection.Name, connection, null, SyncedModulesPath, getLogin, dataAccessError);
			ConnectionUsers[connection] = entry.CurrentUser;
			Users.Add(entry.CurrentUser);

			Folders.Add(entry);
			OnFolderAdded(new FolderAddedEventArgs(entry));

			entry.ContentCleared += new EventHandler(Folder_ContentCleared);
			entry.ContentLoading += new EventHandler(Folder_ContentLoading);
			entry.ContentLoaded += new EventHandler(Folder_ContentLoaded);
			entry.FolderAdded += new EventHandler<FolderAddedEventArgs>(Folder_SubFolderAdded);
			entry.LearningModuleAdded += new EventHandler<LearningModuleAddedEventArgs>(Folder_LearningModuleAdded);

			entry.BeginLoading();

			if (entry.IsOffline)
			{
				entry.GetContainingEntries(true).ForEach(e => learningModules.Add(e as LearningModulesIndexEntry));
				throw new ServerOfflineException();
			}

			return entry;
		}
		/// <summary>
		/// Handles the ContentCleared event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-05-13</remarks>
		private void Folder_ContentCleared(object sender, EventArgs e)
		{
			List<KeyValuePair<object, Thread>> killThreads = new List<KeyValuePair<object, Thread>>();
			foreach (KeyValuePair<object, Thread> pair in threads)
			{
				if (pair.Key is LearningModulesIndexEntry)
				{
					LearningModulesIndexEntry entry = pair.Key as LearningModulesIndexEntry;
					if (entry.Connection == (sender as FolderIndexEntry).Connection)
						killThreads.Add(pair);
				}
			}

			foreach (KeyValuePair<object, Thread> pair in killThreads)
			{
				try
				{
					if (pair.Value != null)
					{
						pair.Value.Abort();
						threads.Remove(pair.Key);
					}
				}
				catch (ThreadAbortException) { }
				catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
			}
		}
		/// <summary>
		/// Handles the FolderContentLoading event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		private void Folder_ContentLoading(object sender, EventArgs e)
		{
			updateTimer.Enabled = true;
			OnFolderContentLoading(sender as FolderIndexEntry, e);
		}
		/// <summary>
		/// Handles the ContentLoaded event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		private void Folder_ContentLoaded(object sender, EventArgs e)
		{
			OnFolderContentLoaded(sender as FolderIndexEntry, e);
		}
		/// <summary>
		/// Handles the LearningModuleAdded event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.LearningModuleAddedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		private void Folder_LearningModuleAdded(object sender, LearningModuleAddedEventArgs e)
		{
			learningModules.Add(e.LearningModule);
			OnLearningModuleAdded(e);

			GetCacheItem(e.LearningModule);

			Thread loadDetails = new Thread(new ParameterizedThreadStart(LoadLearningModuleDetails));
			loadDetails.Name = "Loading details to " + e.LearningModule.DisplayName;
			loadDetails.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			loadDetails.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			loadDetails.IsBackground = true;
			loadDetails.Priority = ThreadPriority.Lowest;
			if (AddThreadToDictionary(e.LearningModule, loadDetails))
				loadDetails.Start(e.LearningModule);
		}
		/// <summary>
		/// Handles the SubFolderAdded event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.FolderAddedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-06</remarks>
		private void Folder_SubFolderAdded(object sender, FolderAddedEventArgs e)
		{
			if (e.Folder != null && !Folders.Contains(e.Folder))
				Folders.Add(e.Folder);
			OnFolderAdded(e);
		}

		/// <summary>
		/// Begins the learning modules scan for the details asynchronously.
		/// </summary>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public void BeginLearningModulesScan()
		{
			Thread worker = new Thread(new ThreadStart(LoadDictionaryData));
			worker.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			worker.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			worker.Name = "LearninModulesScanThread - Main";
			worker.IsBackground = true;
			if (AddThreadToDictionary(this, worker))
				worker.Start();
		}

		private void LoadDictionaryData()
		{
			try
			{
				loadingDictionaryData = true;

				Dictionary<ListViewGroup, Queue<LearningModulesIndexEntry>> queues = new Dictionary<ListViewGroup, Queue<LearningModulesIndexEntry>>();
				foreach (ListViewGroup group in Groups)
				{
					Queue<LearningModulesIndexEntry> queue = new Queue<LearningModulesIndexEntry>();
					Thread subWorker = new Thread(new ParameterizedThreadStart(LoadDictionaryData));
					subWorker.CurrentCulture = Thread.CurrentThread.CurrentCulture;
					subWorker.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
					subWorker.Name = "LearningModulesScanThread - " + group.Header;
					subWorker.IsBackground = true;
					subWorker.Priority = ThreadPriority.Lowest;
					if (AddThreadToDictionary(group, subWorker))
						subWorker.Start(queue);

					queues.Add(group, queue);
				}

				foreach (LearningModulesIndexEntry entry in LearningModules)
					queues[entry.Group].Enqueue(entry);
			}
			catch (ThreadAbortException) { Trace.WriteLine("LearningModulesScan aborted!"); }
			finally { loadingDictionaryData = false; }
		}

		private bool loadingDictionaryData = false;
		private bool cacheSaved = false;
		private static IFormatter formatter = new BinaryFormatter();
		private void LoadDictionaryData(object entriesQueue)
		{
			try
			{
				cacheSaved = false;
				Queue<LearningModulesIndexEntry> entries = entriesQueue as Queue<LearningModulesIndexEntry>;

				while (loadingDictionaryData || entries.Count > 0)
				{
					while (entries.Count == 0 && loadingDictionaryData) Thread.Sleep(10);

					while (entries.Count > 0)
					{
						LearningModulesIndexEntry entry = entries.Dequeue();
						LoadLearningModuleDetails(entry);
					}
				}

				if (!cacheSaved)
				{
					cacheSaved = true;
					SaveCache();
				}
			}
			catch (ThreadAbortException) { Trace.WriteLine("LearningModulesScan aborted!"); }
		}

		private void LoadLearningModuleDetails(object entryObject)
		{
			LearningModulesIndexEntry entry = entryObject as LearningModulesIndexEntry;
			try
			{
				OnLearningModuleDetailsLoading(entry, EventArgs.Empty);

				FolderIndexEntry folder = GetFolderOfConnection(entry.Connection);
				while (folder.IsLoading) Thread.Sleep(100);

				try
				{
					if (entry.Connection is WebConnectionStringBuilder)
					{
						LoadWebEntry(entry);
					}
					else
					{
						LoadEntry(entry);
					}
				}
				catch (UserSessionInvalidException)
				{
					entry.IsAccessible = false;
					entry.NotAccessibleReason = LearningModuleNotAccessibleReason.UserSessionInvalid;
				}

				entry.IsFromCache = false;
				entry.IsVerified = true;

				RefreshCacheItem(entry);
				OnLearningModuleUpdated(new LearningModuleUpdatedEventArgs(entry));
			}
			finally
			{
				OnLearningModuleDetailsLoaded(entry, EventArgs.Empty);
			}
		}
		/// <summary>
		/// Loads the entry.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-06-04</remarks>
		public static void LoadEntry(LearningModulesIndexEntry entry)
		{
			try
			{
				if (!entry.IsAccessible && entry.NotAccessibleReason == LearningModuleNotAccessibleReason.Protected)
					return;

				LoadIndexEntry(entry);
				GeneratePreview(entry);
				GetStatistics(entry);

				if (entry.Dictionary.ContentProtected)
				{
					ConnectionStringStruct css = entry.ConnectionString;
					css.ProtectedLm = true;
					entry.ConnectionString = css;
				}
			}
			catch (IOException)
			{
				entry.IsAccessible = false;
				entry.NotAccessibleReason = LearningModuleNotAccessibleReason.ServerOffline;
			}
		}
		/// <summary>
		/// Loads the web entry.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-06-04</remarks>
		public static void LoadWebEntry(LearningModulesIndexEntry entry)
		{
			byte[] serializedEntry;
			WebUser user = (entry.User as DAL.User).BaseUser as WebUser;
			lock (user)
			{
				serializedEntry = user.GetLearningModuleData(entry.ConnectionString.LmId);
			}

			if (serializedEntry != null)
			{
				LearningModulesIndexEntry webEntry = (LearningModulesIndexEntry)formatter.Deserialize(new MemoryStream(serializedEntry));

				entry.Author = webEntry.Author;
				entry.Count = webEntry.Count;
				entry.Category = webEntry.Category;
				entry.Description = webEntry.Description;
				entry.DisplayName = webEntry.DisplayName;
				entry.LastTimeLearned = webEntry.LastTimeLearned;
				entry.Logo = webEntry.Logo;
				entry.Preview = webEntry.Preview;
				entry.Statistics = webEntry.Statistics;
				entry.ConnectionString = webEntry.ConnectionString;
				entry.IsAccessible = webEntry.IsAccessible;
				entry.NotAccessibleReason = webEntry.NotAccessibleReason;
				entry.Size = webEntry.Size;
				entry.Type = LearningModuleType.Remote;
			}
			else
			{
				entry.IsAccessible = false;
				entry.NotAccessibleReason = LearningModuleNotAccessibleReason.ServerOffline;
			}
		}

		/// <summary>
		/// Indicates that the index scan was stopped.
		/// </summary>
		private bool shutdown = false;
		/// <summary>
		/// Ends the learning modules scan.
		/// </summary>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public void EndLearningModulesScan()
		{
			shutdown = true;

			Folders.ForEach(f => f.EndLoading());

			AbortThreads();
			threads.Clear();

			SaveCache();
		}

		/// <summary>
		/// Adds the thread do a dictionary.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="thread">The thread.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-06-04</remarks>
		private bool AddThreadToDictionary(object key, Thread thread)
		{
			if (shutdown) return false;
			threads.Add(key, thread);
			return true;
		}

		/// <summary>
		/// Aborts the threads.
		/// </summary>
		private void AbortThreads()
		{
			lock (threads)
			{
				foreach (KeyValuePair<object, Thread> pair in threads)
				{
					try { if (pair.Value != null && pair.Value.IsAlive) pair.Value.Abort(); }
					catch (ThreadAbortException) { }
					catch (Exception e) { Trace.WriteLine(e.ToString()); }
				}
			}
		}

		private void SaveCache()
		{
			//dump index cache
			try
			{
				if (cacheFile != null && cacheFile != String.Empty)
					DumpIndexCache(cacheFile);
			}
			catch (Exception exp)
			{
				Trace.WriteLine("Index entry cache dump failed: " + exp.ToString());
			}
		}

		/// <summary>
		/// Creates the learning module index entry.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev03, 2009-03-16</remarks>
		public LearningModulesIndexEntry CreateLearningModuleIndexEntry(string path)
		{
			LearningModulesIndexEntry entry = new LearningModulesIndexEntry();
			if (DAL.Helper.IsEmbeddedDbFormat(path))
			{
				IUser user = UserFactory.Create(getLogin, new ConnectionStringStruct(DatabaseType.MsSqlCe, path, -1), dataAccessError, path);
				entry.ConnectionString = user.ConnectionString = new ConnectionStringStruct(DatabaseType.MsSqlCe, path, DAL.User.GetIdOfLearningModule(path, user));
				entry.Dictionary = user.Open();
			}
			else if (DAL.Helper.IsOdxFormat(path))
			{
				IUser user = UserFactory.Create(getLogin, new ConnectionStringStruct(DatabaseType.Xml, path, true), dataAccessError, path);
				entry.ConnectionString = user.ConnectionString;
				entry.Dictionary = DAL.User.GetPreviewDictionary(path, user);

			}
			else
				return null;
			LoadIndexEntry(entry);
			return entry;
		}

		/// <summary>
		/// Removes the modules of drive.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>Documented by Dev05, 2009-04-01</remarks>
		public void RemoveModulesOfDrive(DriveInfo drive)
		{
			learningModules.RemoveAll(e => e.ConnectionString.ConnectionString.StartsWith(drive.RootDirectory.FullName));
		}

		#region IDisposable Members

		private bool disposed;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					EndLearningModulesScan();
				}

				disposed = true;
			}
		}

		#endregion
	}

	/// <summary>
	/// This event indicates an update LearningModulesIndexEntry.
	/// </summary>
	public delegate void LearningModuleUpdatedEventHandler(object sender, LearningModuleUpdatedEventArgs e);
	/// <summary>
	/// This EventArgs hold the updatet LearningModulesIndexEntry.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-02-18</remarks>
	public class LearningModuleUpdatedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the entry which was updated.
		/// </summary>
		/// <value>The entry.</value>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public LearningModulesIndexEntry Entry { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LearningModuleUpdatedEventArgs"/> class.
		/// </summary>
		/// <param name="entry">The entry which was updated.</param>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public LearningModuleUpdatedEventArgs(LearningModulesIndexEntry entry)
		{
			Entry = entry;
		}
	}

	/// <summary>
	/// The reason why a module is not accessible.
	/// </summary>
	public enum LearningModuleNotAccessibleReason
	{
		/// <summary>
		/// The module is actualy accessible.
		/// </summary>
		IsAccessible,
		/// <summary>
		/// The user was kicked.
		/// </summary>
		UserSessionInvalid,
		/// <summary>
		/// The server was offline.
		/// </summary>
		ServerOffline,
		/// <summary>
		/// The module is protected - only supported by ML 2.3
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-01-17</remarks>
		Protected
	}

	/// <summary>
	/// The type of the module.
	/// </summary>
	public enum LearningModuleType
	{
		/// <summary>
		/// This module is a local file module.
		/// </summary>
		Local,
		/// <summary>
		/// This module is on a remote server,
		/// </summary>
		Remote,
		/// <summary>
		/// This module is on a stick.
		/// </summary>
		Stick
	}
}
