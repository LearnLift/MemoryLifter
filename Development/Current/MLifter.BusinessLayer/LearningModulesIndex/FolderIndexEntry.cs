using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using MLifter.BusinessLayer.Properties;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Generics;
using System.ComponentModel;

namespace MLifter.BusinessLayer
{
    /// <summary>
    /// This class holds all needed information about a learning modules folder.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-06</remarks>
    [Serializable()]
    public class FolderIndexEntry : IIndexEntry
    {
        private IUser currentUser;
        private string SyncedModulesPath;
        private GetLoginInformation getLogin;
        private DataAccessErrorDelegate dataAccessError;
        private Thread scanningThread;
        private List<IIndexEntry> content = new List<IIndexEntry>();

        /// <summary>
        /// Gets a value indicating whether this instance is root node.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is root node; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-13</remarks>
        public bool IsRootNode { get; private set; }
        /// <summary>
        /// Gets or sets the path to the folder.
        /// </summary>
        /// <value>The path.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public string Path { get; set; }
        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <value>The current user.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public IUser CurrentUser { get { return currentUser; } }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public bool IsLoading { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether this folder is offline.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is offline; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-30</remarks>
        public bool IsOffline { get; private set; }
        /// <summary>
        /// Gets the parent folder.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        public FolderIndexEntry Parent { get; private set; }

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
        /// Occurs when content is loading.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-10</remarks>
        public event EventHandler ContentLoading;
        /// <summary>
        /// Raises the <see cref="E:FolderContentLoading"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-03-10</remarks>
        protected virtual void OnContentLoading(FolderIndexEntry sender, EventArgs e)
        {
            if (ContentLoading != null)
                ContentLoading(sender, e);
        }
        /// <summary>
        /// Occurs when content is loaded.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-10</remarks>
        public event EventHandler ContentLoaded;
        /// <summary>
        /// Raises the <see cref="E:FolderContentLoaded"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-03-10</remarks>
        protected virtual void OnContentLoaded(FolderIndexEntry sender, EventArgs e)
        {
            if (ContentLoaded != null)
                ContentLoaded(sender, e);
        }

        public delegate void ContentLoadExceptionEventHandler(Object sender, Exception exp);
        /// <summary>
        /// Occurs when [content load exception].
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-05-14</remarks>
        public event ContentLoadExceptionEventHandler ContentLoadException;
        /// <summary>
        /// Called when [content load exception].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="exp">The exp.</param>
        /// <remarks>Documented by Dev02, 2009-05-14</remarks>
        protected virtual void OnContentLoadException(Object sender, Exception exp)
        {
            if (ContentLoadException != null)
                ContentLoadException(sender, exp);
        }

        /// <summary>
        /// Occurs when content was cleared.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-05-13</remarks>
        public event EventHandler ContentCleared;
        /// <summary>
        /// Raises the <see cref="E:ContentCleared"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-05-13</remarks>
        protected virtual void OnContentCleared(object sender, EventArgs e)
        {
            if (ContentCleared != null)
                ContentCleared(sender, e);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderIndexEntry"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public FolderIndexEntry(string path, string name, IConnectionString parentConnection, IUser user, string syncedModulesPath,
            GetLoginInformation getLoginDelegate, DataAccessErrorDelegate dataAccessErrorDelegate)
            : this(path, name, parentConnection, user, syncedModulesPath, getLoginDelegate, dataAccessErrorDelegate, null) { IsRootNode = true; }
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderIndexEntry"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <param name="parentConnection">The parent connection.</param>
        /// <param name="user">The user.</param>
        /// <param name="getLoginDelegate">The get login delegate.</param>
        /// <param name="dataAccessErrorDelegate">The data access error delegate.</param>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev05, 2009-03-12</remarks>
        private FolderIndexEntry(string path, string name, IConnectionString parentConnection, IUser user, string syncedModulesPath,
            GetLoginInformation getLoginDelegate, DataAccessErrorDelegate dataAccessErrorDelegate, FolderIndexEntry parent)
        {
            IsRootNode = false;
            Path = path;
            DisplayName = name;
            Connection = parentConnection;
            getLogin = getLoginDelegate;
            dataAccessError = dataAccessErrorDelegate;
            Parent = parent;
            SyncedModulesPath = syncedModulesPath;

            Login(user);
        }

        /// <summary>
        /// Logins a new user.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-27</remarks>
        public void LoginNewUser()
        {
            try
            {
                EndLoading();
                lock (content)
                {
                    content.Clear();
                    OnContentCleared(this, EventArgs.Empty);
                }

                User.PreventAutoLogin = true;
                LearningModulesIndex.ConnectionUsers.Remove(connection);
                Login(null);
                LearningModulesIndex.ConnectionUsers.Add(connection, CurrentUser);

                BeginLoading();
            }
            finally { User.PreventAutoLogin = false; }
        }
        private void Login(IUser user)
        {
            try
            {
                Exception unexpactedException = null;
                if (user == null)
                {
                    try
                    {
                        currentUser = UserFactory.Create(getLogin, new ConnectionStringStruct(Connection.ConnectionType, Path, Connection.ConnectionString, true), dataAccessError, this);
                    }
                    catch (System.Net.WebException e) { Trace.WriteLine(e.ToString()); throw new Generics.ServerOfflineException(); }
                    catch (Npgsql.NpgsqlException e) { Trace.WriteLine(e.ToString()); throw new Generics.ServerOfflineException(); }
                    catch (System.Web.Services.Protocols.SoapException e) { Trace.WriteLine(e.ToString()); throw new Generics.ServerOfflineException(); }
                    catch (Exception e) { unexpactedException = e; Trace.WriteLine(e.ToString()); }
                }
                else
                    currentUser = user;

                if (currentUser == null)
                    throw new NoValidUserException(unexpactedException);
            }
            catch (ServerOfflineException e)
            {
                if ((connection as ISyncableConnectionString).SyncType == SyncType.FullSynchronized)
                {
                    IsOffline = true;
                    List<LearningModulesIndexEntry> deadEntrys = new List<LearningModulesIndexEntry>();
                    foreach (LearningModulesIndexEntry entry in SyncedModulesIndex.Get(connection))
                    {
                        if (File.Exists(Tools.GetFullSyncPath(entry.SyncedPath, SyncedModulesPath, entry.ConnectionName, entry.UserName)))
                            lock (content)
                                content.Add(entry);
                        else
                            deadEntrys.Add(entry);
                    }

                    deadEntrys.ForEach(m => SyncedModulesIndex.Remove(connection, m));
                }
                else
                    throw e;
            }
        }

        /// <summary>
        /// Begins the loading.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public void BeginLoading()
        {
            if (IsOffline)
                return;

            EndLoading();

            scanningThread = new Thread(new ThreadStart(LoadContent));
            scanningThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            scanningThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            scanningThread.IsBackground = true;
            scanningThread.Name = "Scanning Folder: " + Path;
            scanningThread.Priority = ThreadPriority.Lowest;

            IsLoading = true;
            OnContentLoading(this, EventArgs.Empty);
            scanningThread.Start();
        }
        private void LoadContent()
        {
            try
            {
                lock (content)
                    content.Clear();

                if (Connection is UncConnectionStringBuilder && Directory.Exists(Path))
                {
                    foreach (string file in Directory.GetFiles(Path, "*" + DAL.Helper.EmbeddedDbExtension, SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            LearningModulesIndexEntry entry;
                            try
                            {
                                entry = CreateNewEmbeddedDbLearningModuleEntry(file);
                            }
                            catch (DatabaseVersionNotSupported e)
                            {
                                Trace.WriteLine("FolderIndexEntry.LoadContent() found an old version db - use File->Open to convert the DB.");

                                if (!e.CanUpdate)
                                    continue;
                                else
                                    entry = CreateNewEmbeddedDbLearningModuleEntry(file);
                            }
                            catch (System.Data.SqlServerCe.SqlCeException ex)
                            {
                                //probably an  invalid db file
                                Trace.WriteLine("FolderIndexEntry.LoadContent() found an invalid db - " + ex.Message);
                                continue;
                            }
                            lock (content)
                                content.Add(entry);
                            OnLearningModuleAdded(new LearningModuleAddedEventArgs(entry));
                        }
                        catch (IOException exp) { Trace.WriteLine(exp.ToString()); }
                    }
                    foreach (string file in Directory.GetFiles(Path, "*" + DAL.Helper.OdxExtension, SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            LearningModulesIndexEntry entry = CreateNewOdxLearningModuleEntry(file);
                            lock (content)
                                content.Add(entry);
                            OnLearningModuleAdded(new LearningModuleAddedEventArgs(entry));
                        }
                        catch (IOException exp) { Trace.WriteLine(exp.ToString()); }
                    }
                    foreach (string folder in Directory.GetDirectories(Path, "*", SearchOption.TopDirectoryOnly))
                    {
                        try
                        {
                            FolderIndexEntry entry = CreateNewFolderEntry(folder);
                            lock (content)
                                content.Add(entry);
                            OnFolderAdded(new FolderAddedEventArgs(entry));
                        }
                        catch (IOException exp) { Trace.WriteLine(exp.ToString()); }
                    }
                }
                else
                {
                    foreach (IDictionary dic in currentUser.List().Dictionaries)
                    {
                        try
                        {
                            LearningModulesIndexEntry entry = CreateLerningModuleEntry(dic, currentUser);
                            lock (content)
                                content.Add(entry);
                            OnLearningModuleAdded(new LearningModuleAddedEventArgs(entry));
                        }
                        catch (IOException exp) { Trace.WriteLine(exp.ToString()); }
                    }
                }
            }
            catch (Exception exp)
            {
                OnContentLoadException(this, exp);
            }
            finally
            {
                IsLoading = false;
                OnContentLoaded(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Ends the loading.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-07</remarks>
        public void EndLoading()
        {
            if (scanningThread != null)
            {
                try { scanningThread.Abort(); }
                catch (ThreadAbortException) { }
                catch (Exception e) { Trace.WriteLine(e.ToString()); }
            }
        }

        private LearningModulesIndexEntry CreateNewEmbeddedDbLearningModuleEntry(string path)
        {
            try
            {
                ConnectionStringStruct css = new ConnectionStringStruct(DatabaseType.MsSqlCe, path, DAL.User.GetIdOfLearningModule(path, currentUser));
                IUser user = UserFactory.Create(getLogin, css, dataAccessError, path);
                IDictionary dic = user.Open();

                return CreateLerningModuleEntry(dic, user);
            }
            catch (ProtectedLearningModuleException)
            {
                LearningModulesIndexEntry entry = new LearningModulesIndexEntry();
                entry.IsAccessible = false;
                entry.NotAccessibleReason = LearningModuleNotAccessibleReason.Protected;
                entry.ConnectionString = new ConnectionStringStruct(DatabaseType.MsSqlCe, path);
                entry.DisplayName = System.IO.Path.GetFileNameWithoutExtension(path);
                entry.Connection = Connection;
                return entry;
            }
        }
        private LearningModulesIndexEntry CreateNewOdxLearningModuleEntry(string path)
        {
            ConnectionStringStruct css = new ConnectionStringStruct(DatabaseType.Xml, path, true);
            IUser user = UserFactory.Create(getLogin, css, dataAccessError, path);
            IDictionary dic = DAL.User.GetPreviewDictionary(path, user, false);

            return CreateLerningModuleEntry(dic, user);
        }
        /// <summary>
        /// Creates the new odx learning module entry.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2009-06-15</remarks>
        public static LearningModulesIndexEntry CreateNewOdxLearningModuleEntryStatic(string path)
        {
            ConnectionStringStruct css = new ConnectionStringStruct(DatabaseType.Xml, path, true);
            IUser user = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; }, css, (DataAccessErrorDelegate)delegate(object sender, Exception e) { }, path);
            IDictionary dic = DAL.User.GetPreviewDictionary(path, user, false);

            string directory = new FileInfo(path).DirectoryName;
            UncConnectionStringBuilder connection = new UncConnectionStringBuilder(directory);
            connection.Name = directory;

            LearningModulesIndexEntry entry = new LearningModulesIndexEntry();
            entry.Connection = connection;
            entry.User = user;
            entry.Dictionary = dic;
            entry.DisplayName = dic.Title;
            entry.Type = LearningModuleType.Local;
            css.ProtectedLm = false;
            entry.ConnectionString = css;

            return entry;
        }
        private LearningModulesIndexEntry CreateLerningModuleEntry(IDictionary dic, IUser user) { return CreateLerningModuleEntry(dic, user, false); }
        private LearningModulesIndexEntry CreateLerningModuleEntry(IDictionary dic, IUser user, bool protectedLm)
        {
            LearningModulesIndexEntry entry = new LearningModulesIndexEntry();
            entry.Connection = Connection;
            entry.User = user;
            entry.Dictionary = dic;
            entry.DisplayName = dic.Title;
            entry.Author = dic.Author;
            entry.Category = dic.Category;
            entry.Type = currentUser.ConnectionString.Typ == DatabaseType.Unc || currentUser.ConnectionString.Typ == DatabaseType.MsSqlCe || currentUser.ConnectionString.Typ == DatabaseType.Xml ?
                LearningModuleType.Local : LearningModuleType.Remote;
            entry.Group = LearningModulesIndex.Groups.Find(g => g.Tag == Connection);
            ConnectionStringStruct css = new ConnectionStringStruct(user.ConnectionString.Typ, dic.Connection, dic.Id);
            css.ProtectedLm = protectedLm;
            entry.ConnectionString = css;
            entry.Connection = Connection;
            return entry;
        }
        private FolderIndexEntry CreateNewFolderEntry(string path)
        {
            FolderIndexEntry entry = new FolderIndexEntry(path, path.Substring(path.LastIndexOf("\\") + 1), Connection, currentUser, SyncedModulesPath, getLogin, dataAccessError, this);
            entry.ContentCleared += new EventHandler(Folder_ContentCleared);
            entry.ContentLoading += new EventHandler(Folder_ContentLoading);
            entry.ContentLoaded += new EventHandler(Folder_ContentLoaded);
            entry.FolderAdded += new EventHandler<FolderAddedEventArgs>(Folder_SubFolderAdded);
            entry.LearningModuleAdded += new EventHandler<LearningModuleAddedEventArgs>(Folder_SubFolder_LearningModuleAdded);
            entry.BeginLoading();
            return entry;
        }
        private void Folder_ContentCleared(object sender, EventArgs e)
        {
            OnContentCleared(sender, e);
        }
        private void Folder_ContentLoading(object sender, EventArgs e)
        {
            OnContentLoading(sender as FolderIndexEntry, e);
        }
        private void Folder_ContentLoaded(object sender, EventArgs e)
        {
            OnContentLoaded(sender as FolderIndexEntry, e);
        }
        private void Folder_SubFolder_LearningModuleAdded(object sender, LearningModuleAddedEventArgs e)
        {
            OnLearningModuleAdded(e);
        }
        private void Folder_SubFolderAdded(object sender, FolderAddedEventArgs e)
        {
            OnFolderAdded(e);
        }

        /// <summary>
        /// Gets the containing entries (folders and learning modules).
        /// </summary>
        /// <param name="searchRecursive">if set to <c>true</c> it searches recursive and delivers all learning modules inside this folder and all subfolders; 
        /// otherwise it delivers the learning modules inside this folder and its subfolders.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public List<IIndexEntry> GetContainingEntries(bool searchRecursive)
        {
            List<IIndexEntry> result = new List<IIndexEntry>();

            lock (content)
            {
                foreach (IIndexEntry entry in content)
                {
                    if (!searchRecursive || entry is LearningModulesIndexEntry)
                        result.Add(entry);
                    else
                    {
                        foreach (IIndexEntry subEntry in (entry as FolderIndexEntry).GetContainingEntries(true))
                            result.Add(subEntry);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-27</remarks>
        public bool DeleteEntry(IIndexEntry entry)
        {
            lock (content)
            {
                if (content.Contains(entry))
                    return content.Remove(entry);
                else
                {
                    bool success = false;
                    content.FindAll(e => e is FolderIndexEntry).ForEach((Action<IIndexEntry>)delegate(IIndexEntry f)
                    {
                        if ((f as FolderIndexEntry).DeleteEntry(entry))
                            success = true;
                    });

                    return success;
                }
            }
        }

        #region IIndexEntry Members

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is verified (=not from cache).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is verified; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is from cache.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is from cache; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public bool IsFromCache { get; set; }

        /// <summary>
        /// Gets or sets the card or learning modules count.
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public int Count
        {
            get
            {
                lock (content)
                    return content.FindAll(entry => entry is LearningModulesIndexEntry).Count;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>The logo.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public Image Logo
        {
            get
            {
                return Resources.Folder;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        [NonSerialized()]
        private IConnectionString connection;
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        /// <remarks>Documented by Dev05, 2009-02-18</remarks>
        public IConnectionString Connection { get { return connection; } set { connection = value; } }

        #endregion
    }

    /// <summary>
    /// EventArgs to deliver a new folder.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-06</remarks>
    public class FolderAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        /// <value>The folder.</value>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public FolderIndexEntry Folder { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderAddedEventArgs"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public FolderAddedEventArgs(FolderIndexEntry entry)
        {
            Folder = entry;
        }
    }

    /// <summary>
    /// EventArgs to deliver a new learning module.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-06</remarks>
    public class LearningModuleAddedEventArgs : EventArgs
    {
        public LearningModulesIndexEntry LearningModule { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModuleAddedEventArgs"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <remarks>Documented by Dev05, 2009-03-06</remarks>
        public LearningModuleAddedEventArgs(LearningModulesIndexEntry entry)
        {
            LearningModule = entry;
        }
    }
}
