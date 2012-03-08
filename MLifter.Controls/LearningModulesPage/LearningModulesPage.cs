using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServerCe;
using MLifter.BusinessLayer;
using MLifter.Components;
using MLifter.Controls.Properties;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using XPExplorerBar;
using MLifter.DAL.Security;
using MLifter.Generics;
using System.Text.RegularExpressions;
using System.Net;
using MLifter.DAL.DB.DbMediaServer;
using MLifter.DAL.Tools;
using System.Web;
using MLifter.DAL.DB.MsSqlCe;

namespace MLifter.Controls.LearningModulesPage
{
	/// <summary>
	/// This is the UserControl to display all available LM's.
	/// </summary>
	/// <remarks>Documented by Dev05, 2008-12-02</remarks>
	/// <remarks>Documented by Dev08, 2009-03-04</remarks>
	public partial class LearningModulesPage : UserControl
	{
		/// <summary>
		/// Image index for local LM
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-01-17</remarks>
		public const int IMAGE_INDEX_LOCAL = 0;
		/// <summary>
		/// Image index for local LM which are not not completly loaded or not available
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-01-17</remarks>
		public const int IMAGE_INDEX_LOCAL_GREY = 1;
		/// <summary>
		/// Image index for remote LMs
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-01-17</remarks>
		public const int IMAGE_INDEX_REMOTE = 2;
		/// <summary>
		/// Image index for remote LM which are not not completly loaded or not available
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-01-17</remarks>
		public const int IMAGE_INDEX_REMOTE_GREY = 3;

		#region Private Members (Fields)

		private LearningModulesIndexEntry selectedLearningModule = null;
		private LearningModulesIndex lmIndex;
		private List<IIndexEntry> actualItems = new List<IIndexEntry>();
		private List<ListViewItem> actualModules = new List<ListViewItem>();
		private List<Expando> CollabsidableExpandos = new List<Expando>();
		private static Dictionary<ConnectionStringStruct, bool> defaultUserSubmitted = new Dictionary<ConnectionStringStruct, bool>();
		private FolderTreeNode selectedTreeViewNode;
		private static Dictionary<string, UserStruct> authenticationUsers = new Dictionary<string, UserStruct>();
		ListViewGroup categoriesGroup = new ListViewGroup(Resources.CATEGORIES);

		//To store the item, selected in the StartPage followed by buttonOk_Click
		private LearningModuleListViewItem lmItem = null;

		private Thread filterThread;
		private Thread newsThread;

		#endregion

		#region Events, delegates and other stuff...

		public event LearningModuleSelectedEventHandler LearningModuleSelected;
		public event EventHandler CancelPressed;
		public event EventHandler NewLearningModulePressed;
		public delegate void SyncStatusReportingDelegate(double percentage, string Message);

		#endregion

		#region Constructor

		public LearningModulesPage()
		{
			InitializeComponent();

			ToolTip tTip = new ToolTip();
			tTip.SetToolTip(buttonLevelUp, Resources.ONE_LEVEL_UP);

			taskPaneInformations.UseClassicTheme();

			learningModulesTreeViewControl.ShowLearningModulesOfSubFolder = true;
			learningModulesTreeViewControl.ShowLines = true;
			learningModulesTreeViewControl.TreeViewSelectionChanged += new TreeViewEventHandler(learningModulesTreeViewControl_TreeViewSelectionChanged);
			learningModulesTreeViewControl.ShowLearningModulesOfSubFolderStateChanged += new EventHandler(learningModulesTreeViewControl_ShowLearningModulesOfSubFolderStateChanged);
			learningModulesTreeViewControl.OnImportConfigFile += new ImportConfigFileEventHandler(learningModulesTreeViewControl_OnImportConfigFile);
			learningModulesTreeViewControl.OnImportLearningModule += new ImportLearningModuleEventHandler(learningModulesTreeViewControl_OnImportLearningModule);
			learningModulesTreeViewControl.OnOpenLearningModule += new LearningModuleSelectedEventHandler(learningModulesTreeViewControl_OnOpenLearningModule);

			toolStripMenuItemTile_Click(this, EventArgs.Empty);

			CollabsidableExpandos.Add(expandoRecent);
			CollabsidableExpandos.Add(expandoStatistics);
			CollabsidableExpandos.Add(expandoTreeView);
			CollabsidableExpandos.Add(expandoNews);
			CollabsidableExpandos.Add(expandoPreview);

			ResetStatisticsAndPreview();

			ListViewSortManager listViewSorter = new ListViewSortManager(listViewLearningModules,
				new Type[] 
					{
						typeof(ListViewTextCaseInsensitiveSort),
						typeof(ListViewTextCaseInsensitiveSort),
						typeof(ListViewDateSort),
						typeof(ListViewTextCaseInsensitiveSort),
						typeof(ListViewInt32Sort),
						typeof(ListViewSizeSort)
					}
				);
			ListViewSortManager listViewSorterFeed = new ListViewSortManager(listViewFeed,
				new Type[] 
					{
						typeof(ListViewTextCaseInsensitiveSort),
						typeof(ListViewTextCaseInsensitiveSort),
						typeof(ListViewTextCaseInsensitiveSort),
						typeof(ListViewInt32Sort),
						typeof(ListViewSizeSort)
					}
				);

			listViewLearningModules.LargeImageList = new ImageList();
			listViewLearningModules.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewLearningModules.LargeImageList.ImageSize = new Size(48, 48);
			listViewLearningModules.LargeImageList.TransparentColor = Color.Transparent;

			listViewLearningModules.LargeImageList.Images.Add(Resources.learning_48);
			listViewLearningModules.LargeImageList.Images.Add(Resources.learning_48_grey);
			listViewLearningModules.LargeImageList.Images.Add(Resources.world_48);
			listViewLearningModules.LargeImageList.Images.Add(Resources.world_48_grey);

			listViewLearningModules.SmallImageList = new ImageList();
			listViewLearningModules.SmallImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewLearningModules.SmallImageList.ImageSize = new Size(16, 16);
			listViewLearningModules.SmallImageList.TransparentColor = Color.Transparent;

			listViewLearningModules.SmallImageList.Images.Add(Resources.learning_16);
			listViewLearningModules.SmallImageList.Images.Add(Resources.learning_16_grey);
			listViewLearningModules.SmallImageList.Images.Add(Resources.world_16);
			listViewLearningModules.SmallImageList.Images.Add(Resources.world_16_grey);


			listViewFeed.LargeImageList = new ImageList();
			listViewFeed.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewFeed.LargeImageList.ImageSize = new Size(48, 48);
			listViewFeed.LargeImageList.TransparentColor = Color.Transparent;

			listViewFeed.LargeImageList.Images.Add(Resources.learning_48);
			listViewFeed.LargeImageList.Images.Add(Resources.folder);

			listViewFeed.SmallImageList = new ImageList();
			listViewFeed.SmallImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewFeed.SmallImageList.ImageSize = new Size(16, 16);
			listViewFeed.SmallImageList.TransparentColor = Color.Transparent;

			listViewFeed.SmallImageList.Images.Add(Resources.learning_16);
			listViewFeed.SmallImageList.Images.Add(Resources.folder_closed);

			LoadPersistencyData();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the selected learning module.
		/// </summary>
		/// <value>The selected learning module.</value>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		public LearningModulesIndexEntry SelectedLearningModule
		{
			get { return selectedLearningModule; }
			set { selectedLearningModule = value; }
		}

		/// <summary>
		/// Gets or sets the general configuration path.
		/// </summary>
		/// <value>The general configuration path.</value>
		/// <remarks>Documented by Dev08, 2009-03-09</remarks>
		[Browsable(false)]
		public string GeneralConfigurationPath { get; set; }
		/// <summary>
		/// Gets or sets the user configuration path.
		/// </summary>
		/// <value>The user configuration path.</value>
		/// <remarks>Documented by Dev08, 2009-03-09</remarks>
		[Browsable(false)]
		public string UserConfigurationPath { get; set; }
		/// <summary>
		/// Gets or sets the synced learning module path.
		/// </summary>
		/// <value>The synced learning module path.</value>
		/// <remarks>Documented by Dev08, 2009-03-09</remarks>
		[Browsable(false)]
		public static string SyncedLearningModulePath { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether first use.
		/// </summary>
		/// <value><c>true</c> if [first use]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2009-05-22</remarks>
		[Browsable(false)]
		public bool FirstUse { get; set; }

		/// <summary>
		/// Gets a value indicating whether [show start page at startup].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if show start page at startup; otherwise open last learning module.
		/// </value>
		/// <remarks>Documented by Dev05, 2009-03-09</remarks>
		public bool ShowStartPageAtStartup { get { return !checkBoxOpenLastLearningModule.Checked; } set { checkBoxOpenLastLearningModule.Checked = !value; } }

		/// <summary>
		/// Gets the cancel button.
		/// </summary>
		/// <value>The button cancel.</value>
		/// <remarks>Documented by Dev05, 2009-03-09</remarks>
		public Button ButtonCancel
		{
			get
			{
				return buttonCancel;
			}
		}

		/// <summary>
		/// Gets the authentication users.
		/// </summary>
		/// <value>The authentication users.</value>
		/// <remarks>Documented by Dev05, 2008-12-12</remarks>
		public static Dictionary<string, UserStruct> AuthenticationUsers { get { return authenticationUsers; } }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LearningModulesPage"/> is maximized.
		/// </summary>
		/// <value><c>true</c> if maximized; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev05, 2009-05-25</remarks>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Maximized { get; set; }

		#endregion

		#region GUI Events

		#region ListViewLearningModules Events
		/// <summary>
		/// Handles the SelectedIndexChanged event of the listViewLearningModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2008-12-22</remarks>
		private void listViewLearningModules_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateDetails();
		}

		/// <summary>
		/// Handles the DrawItem event of the listViewLearningModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DrawListViewItemEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void listViewLearningModules_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			e.DrawDefault = true;
		}

		/// <summary>
		/// Handles the MouseClick event of the listViewLearningModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-12-09</remarks>
		private void listViewLearningModules_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
				return;

			contextMenuStripEntry.Show(listViewLearningModules, e.Location);
		}

		/// <summary>
		/// Handles the MouseDoubleClick event of the listViewLearningModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-12-11</remarks>
		private void listViewLearningModules_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || listViewLearningModules.SelectedItems.Count != 1)
				return;

			if (listViewLearningModules.SelectedItems[0] is LearningModuleListViewItem)
			{
				if ((listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule.IsVerified)
					buttonOK.PerformClick();
			}
			else if (listViewLearningModules.SelectedItems[0] is FolderListViewItem)
			{
				FolderListViewItem item = (listViewLearningModules.SelectedItems[0] as FolderListViewItem);
				actualItems = item.Folder.GetContainingEntries(learningModulesTreeViewControl.ShowLearningModulesOfSubFolder);
				LoadLearningModulesList(actualItems, false);
				learningModulesTreeViewControl.SetSelectedFolder(item.Folder);
			}
		}
		#endregion
		#region Miscellaneous Events (ClickEvents, LoadEvents,...)

		/// <summary>
		/// Handles the Load event of the LearningModulesPage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2008-12-22</remarks>
		private void LearningModulesPage_Load(object sender, EventArgs e)
		{
			if (!this.DesignMode)
			{
				multiPaneControlListViews.SelectedPage = multiPanePageLocal;

				taskPaneInformations.Padding.Left = 1;
				taskPaneInformations.Padding.Right = 1;
				taskPaneInformations.Padding.Top = 2;
				taskPaneInformations.Padding.Bottom = 2;

				newsThread = new Thread(new ThreadStart(ShowNews));
				newsThread.Name = "Reading News Thread";
				newsThread.IsBackground = true;
				newsThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
				newsThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
				newsThread.Start();

				LoadRecentLearningModulesIntoGUI();
			}
		}

		internal void LoadLearningModules()
		{
			SyncedModulesIndex.Restore(Path.Combine(SyncedLearningModulePath, Settings.Default.SyncedModulesFile));

			lmIndex = new LearningModulesIndex(GeneralConfigurationPath, UserConfigurationPath, (GetLoginInformation)GetUser, DataAccessError, SyncedLearningModulePath);
			lmIndex.FolderContentLoading += new EventHandler(lmIndex_ContentLoading);
			lmIndex.FolderContentLoaded += new EventHandler(lmIndex_FolderContentLoaded);
			lmIndex.FolderAdded += new EventHandler<FolderAddedEventArgs>(lmIndex_FolderAdded);
			lmIndex.LearningModuleAdded += new EventHandler<LearningModuleAddedEventArgs>(lmIndex_LearningModuleAdded);
			lmIndex.LearningModuleDetailsLoading += new EventHandler(lmIndex_LearningModuleDetailsLoading);
			lmIndex.LearningModuleUpdated += new LearningModuleUpdatedEventHandler(lmIndex_LearningModuleUpdated);
			lmIndex.LearningModuleDetailsLoaded += new EventHandler(lmIndex_LearningModuleDetailsLoaded);
			lmIndex.LoadingFinished += new EventHandler(lmIndex_LoadingFinished);
			lmIndex.ServersOffline += new LearningModulesIndex.ServersOfflineEventHandler(lmIndex_ServersOffline);

			learningModulesTreeViewControl.SetConnectionStringList(LearningModulesIndex.ConnectionsHandler.ConnectionStrings);
			learningModulesTreeViewControl.SetFeedList(LearningModulesIndex.ConnectionsHandler.Feeds);

			foreach (UncConnectionStringBuilder con in LearningModulesIndex.ConnectionsHandler.ConnectionStrings.FindAll(c => c is UncConnectionStringBuilder))
				GetFolderOfConnection(con);

			FolderIndexEntry entry = GetFolderOfConnection(LearningModulesIndex.ConnectionsHandler.ConnectionStrings.Find(c => c.IsDefault));
			if (entry != null)
			{
				actualItems = entry.GetContainingEntries(learningModulesTreeViewControl.ShowLearningModulesOfSubFolder);
				LoadLearningModulesList(actualItems, false);
				learningModulesTreeViewControl.SetSelectedFolder(entry);
			}

			listViewLearningModules.Groups.AddRange(LearningModulesIndex.Groups.ToArray());

			if (!Directory.Exists(SyncedLearningModulePath))
				Directory.CreateDirectory(SyncedLearningModulePath);

		}

		/// <summary>
		/// Handles the LearningModuleDetailsLoaded event of the lmIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		private void lmIndex_LearningModuleDetailsLoaded(object sender, EventArgs e)
		{
			UpdateLoading(sender as IIndexEntry);
		}
		/// <summary>
		/// Handles the LearningModuleDetailsLoading event of the lmIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		private void lmIndex_LearningModuleDetailsLoading(object sender, EventArgs e)
		{
			UpdateLoading(sender as IIndexEntry);
		}
		/// <summary>
		/// Handles the FolderContentLoaded event of the lmIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		private void lmIndex_FolderContentLoaded(object sender, EventArgs e)
		{
			UpdateLoading(sender as IIndexEntry);
		}
		/// <summary>
		/// Handles the FolderContentLoading event of the lmIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-10</remarks>
		private void lmIndex_ContentLoading(object sender, EventArgs e)
		{
			UpdateLoading(sender as IIndexEntry);
		}


		/// <summary>
		/// Handles the LoadingFinished event of the lmIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-03-24</remarks>
		private void lmIndex_LoadingFinished(object sender, EventArgs e)
		{
			UpdateLoading(null);
		}

		private bool isLoading = false;
		/// <summary>
		/// Updates the loading status.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <remarks>Documented by Dev03, 2009-03-24</remarks>
		private void UpdateLoading(IIndexEntry sender)
		{
			bool isCurrentlyLoading = lmIndex.IsContentLoading;

			if (isLoading == isCurrentlyLoading)
				return;


			if (this.InvokeRequired)
			{
				loadingLogoMain.BeginInvoke((MethodInvoker)delegate
				{
					isLoading = loadingLogoMain.IsLoading = lmIndex.IsContentLoading;
				});
			}
			else
				isLoading = loadingLogoMain.IsLoading = lmIndex.IsContentLoading;
		}

		protected delegate bool BoolInvoker();
		private Dictionary<LearningModulesIndexEntry, LearningModuleListViewItem> lmEntryListItemList = new Dictionary<LearningModulesIndexEntry, LearningModuleListViewItem>();
		/// <summary>
		/// Handles the LearningModuleAdded event of the lmIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.LearningModuleAddedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		private void lmIndex_LearningModuleAdded(object sender, LearningModuleAddedEventArgs e)
		{
			if (selectedTreeViewNode == null || (selectedTreeViewNode.Folder == null && !selectedTreeViewNode.IsMainNode) || e.LearningModule == null)
				return;
			LearningModulesIndexEntry entry = e.LearningModule;
			if (selectedTreeViewNode.IsMainNode || entry.Connection == selectedTreeViewNode.Folder.Connection &&
				(!(entry.Connection is UncConnectionStringBuilder) || learningModulesTreeViewControl.ShowLearningModulesOfSubFolder ||
				selectedTreeViewNode.Folder.Path == entry.ConnectionString.ConnectionString.Substring(0, entry.ConnectionString.ConnectionString.LastIndexOf('\\'))))
			{
				lock (actualItems)
				{
					if (!actualItems.Contains(entry))
						actualItems.Add(entry);
				}

				listViewLearningModules.Invoke((MethodInvoker)delegate
				{
					LearningModuleListViewItem item = new LearningModuleListViewItem(entry);
					listViewLearningModules.BeginUpdate();
					listViewLearningModules.Items.Add(item);
					if (listViewLearningModules.SelectedItems.Count == 0)
						item.Selected = true;
					item.UpdateDetails();
					lmEntryListItemList[entry] = item;
					//entry.IsVerifiedChanged += new EventHandler(LearningModule_IsVerifiedChanged);
					SetLearningModuleListViewItemGroup(item);
					listViewLearningModules.EndUpdate();
				});
			}
		}
		/// <summary>
		/// Handles the IsVerifiedChanged event of the LearningModule control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-06-29</remarks>
		private void LearningModule_IsVerifiedChanged(object sender, EventArgs e)
		{
			try
			{
				if (InvokeRequired)
					Invoke((MethodInvoker)delegate { LearningModule_IsVerifiedChanged(sender, e); });
				else
					SetLearningModuleListViewItemGroup(lmEntryListItemList[sender as LearningModulesIndexEntry]);
			}
			catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
		}
		/// <summary>
		/// Handles the AddFolder event of the lmIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.FolderAddedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		private void lmIndex_FolderAdded(object sender, FolderAddedEventArgs e)
		{
			if (e.Folder == null)
				return;

			learningModulesTreeViewControl.AddFolder(e.Folder);

			if (selectedTreeViewNode == null || selectedTreeViewNode.Folder == null)
				return;
			FolderIndexEntry entry = e.Folder;
			if ((selectedTreeViewNode.IsMainNode && entry.Connection.ConnectionString == entry.Path)
				|| entry.Connection == selectedTreeViewNode.Folder.Connection &&
				(!(entry.Connection is UncConnectionStringBuilder) || !learningModulesTreeViewControl.ShowLearningModulesOfSubFolder &&
				selectedTreeViewNode.Folder.Path == entry.Path.Substring(0, entry.Path.Length - 1 - entry.DisplayName.Length)))
			{
				lock (actualItems)
				{
					if (!actualItems.Contains(entry))
						actualItems.Add(entry);
				}

				listViewLearningModules.Invoke((MethodInvoker)delegate
				{
					FolderListViewItem item = new FolderListViewItem(entry);
					listViewLearningModules.BeginUpdate();
					listViewLearningModules.Items.Insert(0, item);
					item.UpdateImage();
					item.UpdateDetails();
					SetFolderListViewItemGroup(item);
					listViewLearningModules.EndUpdate();
				});
			}
		}

		/// <summary>
		/// Handles the LinkClicked event of the linkLabelNew control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-12-15</remarks>
		private void imageLinkLabelNew_Clicked(object sender, EventArgs e)
		{
			OnNewLearningModulePressed();
		}

		/// <summary>
		/// Handles the LinkClicked event of the linkLabelBrowse control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2008-12-15</remarks>
		private void imageLinkLabelBrowse_Clicked(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			IConnectionString initial = LearningModulesIndex.ConnectionsHandler.ConnectionStrings.Find(c => c.IsDefault) ??
				LearningModulesIndex.ConnectionsHandler.ConnectionStrings.Find(c => c.ConnectionType == DatabaseType.Unc);
			ofd.InitialDirectory = initial != null ? initial.ConnectionString : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			ofd.Multiselect = false;
			ofd.Filter = string.Format(Resources.LEARNING_MODULES_PAGE_Browse_Filter,
				DAL.Helper.EmbeddedDbExtension, DAL.Helper.OdxExtension, DAL.Helper.DzpExtension, DAL.Helper.OdfExtension);
			if (ofd.ShowDialog() == DialogResult.OK)
				OnLearningModuleSelected(new LearningModuleSelectedEventArgs(new LearningModulesIndexEntry(
						new ConnectionStringStruct(
							(Path.GetExtension(ofd.FileName) == DAL.Helper.OdxExtension ||
							Path.GetExtension(ofd.FileName) == DAL.Helper.DzpExtension ||
							Path.GetExtension(ofd.FileName) == DAL.Helper.OdfExtension) ?
							DatabaseType.Xml : DatabaseType.MsSqlCe, ofd.FileName, false)) { ConnectionName = Path.GetDirectoryName(ofd.FileName) }));
		}

		/// <summary>
		/// Disposes and disconnectes LM Dictionaries, to ensure that the TCPListner Service disappear 
		/// and mlm files are not accesed by the mlifter process. ML-2414
		/// </summary>
		/// Documented by MatBre,  03.09.2009
		private void disposeAndDisconnectLMs()
		{
			//Dispose the Dictionary objects (leads to removed listener threads)
			//Close Connections
			foreach (LearningModulesIndexEntry lm in lmIndex.getAvailableLearningModules)
			{
				//If there has been selected a LM, e.g. by buttonOk_click,
				//we will not close thisone
				if (!lm.ConnectionString.Typ.Equals(DatabaseType.MsSqlCe) || (lmItem != null && lm.Equals(lmItem.LearningModule)))
					continue;
				else
					try
					{
						if (lm.Dictionary != null) lm.Dictionary.Dispose();
						MSSQLCEConn.CloseMyConnection(lm.ConnectionString.ConnectionString);
					}
					catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
			}
		}


		/// <summary>
		/// Handles the Click event of the buttonOK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2008-12-22</remarks>
		private void buttonOK_Click(object sender, EventArgs e)
		{
			#region NON FEED
			if (!FeedIsVisible)
			{
				foreach (ListViewItem item in listViewLearningModules.SelectedItems)
				{
					if (item is LearningModuleListViewItem)
					{
						lmItem = item as LearningModuleListViewItem;
						break;
					}
				}

				if (lmItem != null)
				{
					try
					{
						LearningModulesIndexEntry entry = lmItem.LearningModule;

						if (!entry.IsVerified || !entry.IsAccessible)
							return;

						#region PgSQL
						if (entry.Connection is PostgreSqlConnectionStringBuilder)
						{
							string serverUri = (entry.Connection as PostgreSqlConnectionStringBuilder).SyncURI;
							string path = Tools.GetFullSyncPath(entry.DisplayName + MLifter.DAL.Helper.SyncedEmbeddedDbExtension,
								SyncedLearningModulePath, entry.ConnectionName, entry.UserName);
							bool createNew = (entry.SyncedPath == string.Empty);
							ConnectionStringStruct css = entry.ConnectionString;

							switch ((entry.Connection as PostgreSqlConnectionStringBuilder).SyncType)
							{
								case SyncType.NotSynchronized:
									entry.SyncedPath = string.Empty;
									entry.ConnectionString = css;
									break;
								case SyncType.HalfSynchronizedWithDbAccess:
									Sync(serverUri, entry.ConnectionString.LmId, entry.UserId, ref path, entry.ConnectionString.ProtectedLm,
										entry.ConnectionString.Password, createNew, false, (SyncStatusReportingDelegate)SyncStatusUpdate);
									DownloadExtensionFiles(entry, path);

									entry.SyncedPath = Tools.GetSyncedPath(path, SyncedLearningModulePath, entry.ConnectionName, entry.UserName);
									css.SyncType = SyncType.HalfSynchronizedWithDbAccess;
									css.ServerUser = entry.User;
									entry.ConnectionString = css;
									break;
								case SyncType.HalfSynchronizedWithoutDbAccess:
									Sync(serverUri, entry.ConnectionString.LmId, entry.UserId, ref path, entry.ConnectionString.ProtectedLm,
										entry.ConnectionString.Password, createNew, false, (SyncStatusReportingDelegate)SyncStatusUpdate);
									DownloadExtensionFiles(entry, path);

									entry.SyncedPath = Tools.GetSyncedPath(path, SyncedLearningModulePath, entry.ConnectionName, entry.UserName);
									css.SyncType = SyncType.HalfSynchronizedWithoutDbAccess;
									css.LearningModuleFolder = (entry.Connection as PostgreSqlConnectionStringBuilder).MediaURI;
									entry.ConnectionString = css;
									entry.User.Logout();
									break;
								case SyncType.FullSynchronized:
									if (entry.User == null)
										break;

									Sync(serverUri, entry.ConnectionString.LmId, entry.UserId, ref path, entry.ConnectionString.ProtectedLm,
										entry.ConnectionString.Password, createNew, false, (SyncStatusReportingDelegate)SyncStatusUpdate);
									DownloadMediaContent(entry, path);
									DownloadExtensionFiles(entry, path);

									entry.SyncedPath = Tools.GetSyncedPath(path, SyncedLearningModulePath, entry.ConnectionName, entry.UserName);
									css.SyncType = SyncType.FullSynchronized;
									css.LearningModuleFolder = (entry.Connection as PostgreSqlConnectionStringBuilder).MediaURI;
									entry.ConnectionString = css;

									SyncedModulesIndex.Add(entry.Connection, entry);
									SyncedModulesIndex.Dump(Path.Combine(SyncedLearningModulePath, Settings.Default.SyncedModulesFile));

									entry.User.Logout();
									break;
								default:
									throw new NotImplementedException();
							}

							lmIndex.DumpIndexCache(lmIndex.CacheFile);
						}
						#endregion
						#region WEB
						else if (entry.Connection is WebConnectionStringBuilder)
						{
							string serverUri = (entry.Connection as WebConnectionStringBuilder).SyncURI;
							string path = Tools.GetFullSyncPath(entry.DisplayName + MLifter.DAL.Helper.SyncedEmbeddedDbExtension,
								SyncedLearningModulePath, entry.ConnectionName, entry.UserName);
							bool createNew = (entry.SyncedPath == string.Empty);
							ConnectionStringStruct css = entry.ConnectionString;

							switch ((entry.Connection as WebConnectionStringBuilder).SyncType)
							{
								case SyncType.HalfSynchronizedWithoutDbAccess:
									Sync(serverUri, entry.ConnectionString.LmId, entry.UserId, ref path, entry.ConnectionString.ProtectedLm,
										entry.ConnectionString.Password, createNew, false, (SyncStatusReportingDelegate)SyncStatusUpdate);
									DownloadExtensionFiles(entry, path);

									entry.SyncedPath = Tools.GetSyncedPath(path, SyncedLearningModulePath, entry.ConnectionName, entry.UserName);
									css.SyncType = SyncType.HalfSynchronizedWithoutDbAccess;
									css.ServerUser = entry.User;
									css.LearningModuleFolder = (entry.Connection as WebConnectionStringBuilder).MediaURI;
									entry.ConnectionString = css;
									break;
								case SyncType.FullSynchronized:
									if (entry.User == null)
										break;

									Sync(serverUri, entry.ConnectionString.LmId, entry.UserId, ref path, entry.ConnectionString.ProtectedLm,
										entry.ConnectionString.Password, createNew, false, (SyncStatusReportingDelegate)SyncStatusUpdate);
									DownloadMediaContent(entry, path);
									DownloadExtensionFiles(entry, path);

									entry.SyncedPath = Tools.GetSyncedPath(path, SyncedLearningModulePath, entry.ConnectionName, entry.UserName);
									css.SyncType = SyncType.FullSynchronized;
									css.ServerUser = entry.User;
									css.LearningModuleFolder = (entry.Connection as WebConnectionStringBuilder).MediaURI;
									entry.ConnectionString = css;

									SyncedModulesIndex.Add(entry.Connection, entry);
									SyncedModulesIndex.Dump(Path.Combine(SyncedLearningModulePath, Settings.Default.SyncedModulesFile));
									break;
								case SyncType.NotSynchronized:
								case SyncType.HalfSynchronizedWithDbAccess:
								default:
									throw new NotImplementedException();
							}

							lmIndex.DumpIndexCache(lmIndex.CacheFile);
						}
						#endregion

						OnLearningModuleSelected(new LearningModuleSelectedEventArgs(entry));
					}
					catch (NotEnoughtDiskSpaceException)
					{
						TaskDialog.MessageBox(Resources.TASK_DIALOG_DISK_FULL_TITLE, Resources.TASK_DIALOG_DISK_FULL_CAPTION,
							Resources.TASK_DIALOG_DISK_FULL_TEXT, TaskDialogButtons.OK, TaskDialogIcons.Error);
					}
					catch (SynchronizationFailedException exp)
					{
						Trace.WriteLine(exp.ToString());
						TaskDialog.MessageBox(Resources.LEARNING_MODULES_PAGE_SYNC_FAILED_TITLE, Resources.LEARNING_MODULES_PAGE_SYNC_FAILED_MAIN,
							Resources.LEARNING_MODULES_PAGE_SYNC_FAILED_CONTENT, exp.ToString(), string.Empty, string.Empty,
							TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
					}
				}
			}
			#endregion
			else
			{
				if (listViewFeed.SelectedIndices.Count == 0)
					return;

				ModuleInfo info = (ModuleInfo)listViewFeed.SelectedItems[0].Tag;

				string remoteFile = info.DownloadUrl;
				string folder = LearningModulesIndex.ConnectionsHandler.ConnectionStrings.Find(c => c.IsDefault).ConnectionString;
				string localFile = Path.Combine(folder, Methods.GetValidFileName(info.Title + DAL.Helper.EmbeddedDbExtension, String.Empty));

				if (File.Exists(localFile))
				{
					EmulatedTaskDialog dialog = new EmulatedTaskDialog();
					dialog.Owner = ParentForm;
					dialog.StartPosition = FormStartPosition.CenterParent;
					dialog.Title = Resources.LEARNING_MODULES_PAGE_DOWNLOAD_EXISTS_CAPTION;
					dialog.MainInstruction = Resources.LEARNING_MODULES_PAGE_DOWNLOAD_EXISTS_HEADER;
					dialog.Content = Resources.LEARNING_MODULES_PAGE_DOWNLOAD_EXISTS_TEXT;
					dialog.CommandButtons = String.Format("{0}|{1}|{2}", Resources.LEARNING_MODULES_PAGE_DOWNLOAD_EXISTS_OPEN, 
						Resources.LEARNING_MODULES_PAGE_DOWNLOAD_EXISTS_OVERWRITE, Resources.LEARNING_MODULES_PAGE_DOWNLOAD_EXISTS_RENAME);
					dialog.Buttons = TaskDialogButtons.Cancel;
					dialog.MainIcon = TaskDialogIcons.Question;
					dialog.MainImages = new Image[] { Resources.document_open_big, Resources.document_save, Resources.document_save_as };
					dialog.HoverImages = new Image[] { Resources.document_open_big, Resources.document_save, Resources.document_save_as };
					dialog.CenterImages = true;
					dialog.BuildForm();
					if (dialog.ShowDialog() == DialogResult.Cancel)
						return;
					
					switch (dialog.CommandButtonClickedIndex)
					{
						case 0:
							LearningModulesIndexEntry entry = new LearningModulesIndexEntry(new ConnectionStringStruct(DatabaseType.MsSqlCe, localFile));
							OnLearningModuleSelected(new LearningModuleSelectedEventArgs(entry));
							return;
						case 1:
							lmIndex.EndLearningModulesScan();
							disposeAndDisconnectLMs();
							File.Delete(localFile);
							break;
						case 2:
						default:
							int cnt = 1;
							while (File.Exists(localFile))
								localFile = Path.Combine(folder, Methods.GetValidFileName(info.Title + "_" + cnt++ + DAL.Helper.EmbeddedDbExtension, String.Empty));
							break;
					}

				}

				info.DownloadUrl = localFile;
				listViewFeed.SelectedItems[0].Tag = info;

				WebClient webClient = new WebClient();
				webClient.DownloadProgressChanged += (s, args) =>
				{
					UpdateStatusMessage(String.Format(Resources.LEARNING_MODULES_PAGE_DOWNLOAD_PROGRESS,
						Methods.GetFileSize(args.BytesReceived), Methods.GetFileSize(args.TotalBytesToReceive)), args.ProgressPercentage);
				};
				webClient.DownloadFileCompleted += (s, args) =>
				{
					HideStatusMessage();
					OnLearningModuleSelected(new LearningModuleSelectedEventArgs(new LearningModulesIndexEntry(
						new ConnectionStringStruct(DatabaseType.MsSqlCe, localFile))));
				};
				ShowStatusMessage(message: Resources.LEARNING_MODULES_PAGE_DOWNLOAD_START);
				webClient.DownloadFileAsync(new Uri(remoteFile), localFile);
			}
		}

		/// <summary>
		/// Handles the Click event of the buttonCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2008-12-22</remarks>
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			OnCancelPressed(EventArgs.Empty);
		}

		/// <summary>
		/// Handles the Click event of the toolStripMenuItemDelete control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-12-09</remarks>
		private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
		{
			int count = 0;
			foreach (ListViewItem item in listViewLearningModules.SelectedItems)
				if (item is LearningModuleListViewItem)
					count++;
			if (count < 1)
				return;

			string title = string.Empty;
			string description = string.Empty;
			if (count == 1)
			{
				title = string.Format(Resources.LEARNING_MODULES_PAGE_DeleteCaptionSingle, (listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule.DisplayName);
				description = string.Format(Resources.LEARNING_MODULES_PAGE_DeleteLM_Single, (listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule.DisplayName);
			}
			else
			{
				title = string.Format(Resources.LEARNING_MODULES_PAGE_DeleteCaptionMultiple, listViewLearningModules.SelectedItems.Count);
				description = string.Format(Resources.LEARNING_MODULES_PAGE_DeleteLM_Multiple, listViewLearningModules.SelectedItems.Count);
			}

			//Todo Show task dialog
			DialogResult dr = TaskDialog.ShowTaskDialogBox(title, description,
								string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
								TaskDialogButtons.YesNo, TaskDialogIcons.Warning, TaskDialogIcons.Warning);

			if (dr == DialogResult.Yes)
			{
				foreach (ListViewItem item in listViewLearningModules.SelectedItems)
				{
					if (!(item is LearningModuleListViewItem))
						continue;

					LearningModulesIndexEntry lmie = (item as LearningModuleListViewItem).LearningModule;

					if (lmie.ConnectionString.Typ == DatabaseType.MsSqlCe && lmie.User == null)
					{
						File.Delete(lmie.ConnectionString.ConnectionString);
						listViewLearningModules.Items.Remove(item);
						GetFolderOfConnection(lmie.Connection).DeleteEntry(lmie);
						lmIndex.DeleteEntry(lmie);
					}
					else if (!(lmie.User == null && lmie.ConnectionString.Typ == DatabaseType.PostgreSQL && lmie.ConnectionString.SyncType == SyncType.FullSynchronized) &&
						!lmie.User.List().HasPermission(PermissionTypes.CanModify))
					{
						TaskDialog.MessageBox(Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_CAPTION, Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_TEXT, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error);
					}
					else
					{
						bool success = false;
						try
						{
							ShowStatusMessage(false);
							UpdateStatusMessage(string.Format(Resources.LEARNING_MODULES_PAGE_DELETING, lmie.DisplayName), -1);

							if (lmie.User == null && lmie.ConnectionString.Typ == DatabaseType.PostgreSQL && lmie.ConnectionString.SyncType == SyncType.FullSynchronized)
								File.Delete(Tools.GetFullSyncPath(lmie.SyncedPath, SyncedLearningModulePath, lmie.ConnectionName, lmie.UserName));
							else
								lmie.User.List().Delete(lmie.ConnectionString);

							success = true;
						}
						catch (PermissionException)
						{
							HideStatusMessage();
							TaskDialog.MessageBox(Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_CAPTION, Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_TEXT, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error);
						}
						catch (Exception ex)
						{
							HideStatusMessage();
							TaskDialog.MessageBox(Resources.ERROR_DIALOG_DELETE_LM_CAPTION, Resources.ERROR_DIALOG_DELETE_LM_TEXT, string.Empty, ex.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Information);
						}
						finally
						{
							HideStatusMessage();
						}

						if (success)
						{
							listViewLearningModules.Items.Remove(item);

							if (!(lmie.User == null && lmie.ConnectionString.Typ == DatabaseType.PostgreSQL && lmie.ConnectionString.SyncType == SyncType.FullSynchronized))
							{
								GetFolderOfConnection(lmie.Connection).DeleteEntry(lmie);
								lmIndex.DeleteEntry(lmie);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Handles the LearningModuleUpdated event of the lmIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.LearningModuleUpdatedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void lmIndex_LearningModuleUpdated(object sender, LearningModuleUpdatedEventArgs e)
		{
			UpdateItem(e.Entry);
		}

		/// <summary>
		/// Handles the StateChanged event of the expando control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="XPExplorerBar.ExpandoEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void expando_StateChanged(object sender, ExpandoEventArgs e)
		{
			CheckHeight(e.Expando);
		}

		/// <summary>
		/// Handles the SizeChanged event of the LearningModulesPage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void LearningModulesPage_SizeChanged(object sender, EventArgs e)
		{
			CheckHeight(null);
		}

		/// <summary>
		/// Handles the Click event of the buttonFolderView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void checkBoxShowTreeView_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxShowTreeView.Checked)
			{
				SetLeftBarView(LeftBarView.TreeView);
				checkBoxShowTreeView.Image = Resources.folder_closed;
			}
			else
			{
				SetLeftBarView(LeftBarView.XPExplorerBar);
				checkBoxShowTreeView.Image = Resources.document_open;
			}
		}

		/// <summary>
		/// Handles the TreeViewSelectionChanged event of the learningModulesTreeViewControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void learningModulesTreeViewControl_TreeViewSelectionChanged(object sender, TreeViewEventArgs e)
		{
			FolderTreeNode safeNode = selectedTreeViewNode;
			if (e.Node is FolderTreeNode)
			{
				SetFeedVisible(false, null);

				selectedTreeViewNode = e.Node as FolderTreeNode;
				if (UpdateActualItems())
				{
					UpdateSearchAndRefreshList();
					buttonLevelUp.Enabled = !selectedTreeViewNode.IsMainNode;
				}
				else
					learningModulesTreeViewControl.SetSelectedFolder(safeNode.Folder);
			}
			else if (e.Node is FeedTreeNode)
			{
				FeedTreeNode node = e.Node as FeedTreeNode;
				if (node.IsListLoaded)
				{
					if (learningModulesTreeViewControl.ShowLearningModulesOfSubFolder)
						SetFeedVisible(true, node.Modules);
					else
					{
						List<ListViewItem> items = new List<ListViewItem>();
						foreach (FeedCategoryTreeNode subNode in node.Nodes)
							items.Add(GetCategoryListViewItem(subNode));
						SetFeedVisible(true, items);
					}
				}
				else if (!node.IsLoading)
				{
					FeedIsVisible = true;
					node.ContentLoaded += new EventHandler(node_ContentLoaded);
					node.BeginLoadWebContent(UserConfigurationPath);
				}
			}
			else if (e.Node is FeedCategoryTreeNode)
			{
				if (learningModulesTreeViewControl.ShowLearningModulesOfSubFolder)
				{
					List<ListViewItem> modules = new List<ListViewItem>((e.Node as FeedCategoryTreeNode).Modules);
					foreach (ListViewItem item in (e.Node as FeedCategoryTreeNode).SubCategoryModules)
					{
						bool exists = false;
						foreach (ModuleCategory category in (e.Node as FeedCategoryTreeNode).Categories)
						{
							if (((ModuleInfo)item.Tag).Categories.Contains(category.Id.ToString()))
							{
								exists = true;
								break;
							}
						}
						if (exists)
							modules.Add(item);
					}

					SetFeedVisible(true, modules);
				}
				else
				{
					List<ListViewItem> items = new List<ListViewItem>();
					foreach (FeedCategoryTreeNode subNode in e.Node.Nodes)
						items.Add(GetCategoryListViewItem(subNode));
					items.AddRange((e.Node as FeedCategoryTreeNode).OwnModules);
					SetFeedVisible(true, items);
				}
			}
		}

		/// <summary>
		/// Handles the ContentLoaded event of the node control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-06-26</remarks>
		private void node_ContentLoaded(object sender, EventArgs e)
		{
			if (FeedIsVisible)
			{
				if (learningModulesTreeViewControl.ShowLearningModulesOfSubFolder)
					SetFeedVisible(true, (sender as FeedTreeNode).Modules);
				else
				{
					List<ListViewItem> items = new List<ListViewItem>();
					foreach (FeedCategoryTreeNode subNode in (sender as FeedTreeNode).Nodes)
						items.Add(GetCategoryListViewItem(subNode));
					SetFeedVisible(true, items);
				}
			}
		}

		private bool FeedIsVisible = false;
		private void SetFeedVisible(bool visible, List<ListViewItem> modules)
		{
			if (InvokeRequired)
				Invoke((MethodInvoker)delegate { SetFeedVisible(visible, modules); });
			else
			{
				FeedIsVisible = visible;

				if (visible)
				{
					listViewLearningModules.SelectedItems.Clear();
					ResetStatisticsAndPreview();
					multiPaneControlListViews.SelectedPage = multiPanePageFeed;

					listViewFeed.Items.Clear();
					actualModules = modules;
					LoadFeedList(modules, false);
					UpdateSearchAndRefreshList();
				}
				else
				{
					listViewFeed.SelectedItems.Clear();
					multiPaneControlListViews.SelectedPage = multiPanePageLocal;
				}
			}
		}

		/// <summary>
		/// Handles the ShowLearningModulesOfSubFolderStateChanged event of the learningModulesTreeViewControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-05</remarks>
		private void learningModulesTreeViewControl_ShowLearningModulesOfSubFolderStateChanged(object sender, EventArgs e)
		{
			UpdateActualItems();
			UpdateSearchAndRefreshList();
		}

		/// <summary>
		/// Handles the OnOpenLearningModule event of the learningModulesTreeViewControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Controls.LearningModuleSelectedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-03-11</remarks>
		private void learningModulesTreeViewControl_OnOpenLearningModule(object sender, LearningModuleSelectedEventArgs e)
		{
			selectedLearningModule = e.LearnModule;

			if (LearningModuleSelected != null)
				LearningModuleSelected(this, e);
		}

		/// <summary>
		/// Handles the OnImportLearningModule event of the learningModulesTreeViewControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Controls.ImportLearningModuleEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-03-11</remarks>
		private void learningModulesTreeViewControl_OnImportLearningModule(object sender, ImportLearningModuleEventArgs e)
		{
			IConnectionString target = e.TreeNode.Connection;

			if (target.ConnectionType == DatabaseType.Web)
			{
				TaskDialog.MessageBox(Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_CAPTION, Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_TEXT, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error);
				return;
			}

			if (e.LearningModule.ConnectionString.Typ == DatabaseType.Xml || DAL.Helper.IsOdfFormat(e.LearningModule.ConnectionString.ConnectionString))
			{
				// open/convert instead of import
				if (target.ConnectionType == DatabaseType.Unc)
				{
					LearningModuleSelectedEventArgs args = new LearningModuleSelectedEventArgs(e.LearningModule);

					if (LearningModuleSelected != null)
						LearningModuleSelected(this, args);
				}
				else
				{
					DialogResult resultConvert = MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.LEARNING_MODULES_PAGE_CONVERT_TITLE,
						Resources.LEARNING_MODULES_PAGE_CONVERT_MAIN, Resources.LEARNING_MODULES_PAGE_CONVERT_CONTENT,
						String.Empty, String.Empty, String.Empty, String.Empty,
						String.Format("{0}|{1}", Resources.LEARNING_MODULES_PAGE_CONVERT_OPTION_YES, Resources.LEARNING_MODULES_PAGE_CONVERT_OPTION_NO),
						MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Warning);
					switch (MLifter.Controls.TaskDialog.CommandButtonResult)
					{
						case 0:
							LearningModuleSelectedEventArgs args = new LearningModuleSelectedEventArgs(e.LearningModule);

							if (LearningModuleSelected != null)
								LearningModuleSelected(this, args);
							break;
						case 1:
						default:
							break;
					}
				}
			}
			else
			{
				DialogResult result = MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.STARTUP_IMPORT_LEARNING_MODULE_MBX_CAPTION,
					Resources.STARTUP_IMPORT_LEARNING_MODULE_MBX_TEXT, String.Format(Resources.STARTUP_IMPORT_LEARNING_MODULE_CONTENT, target.Name),
					String.Empty, String.Empty, String.Empty, String.Empty,
					String.Format("{0}|{1}", Resources.STARTUP_IMPORT_LEARNING_MODULE_MBX_OPTION_YES, Resources.STARTUP_IMPORT_LEARNING_MODULE_MBX_OPTION_NO),
					MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Warning);
				switch (MLifter.Controls.TaskDialog.CommandButtonResult)
				{
					case 0:
						LearningModulesIndexEntry source = null;
						try
						{
							FolderIndexEntry folder = lmIndex.GetFolderOfConnection(target);
							learningModulesTreeViewControl.SetSelectedFolder(folder);
							conTarget = target;
							source = lmIndex.CreateLearningModuleIndexEntry(e.LearningModule.ConnectionString.ConnectionString);
						}
						catch (ServerOfflineException)
						{
							ShowServerOfflineMessage(new List<IConnectionString>() { target }, false);
						}
						catch (NoValidUserException)
						{
							// user pressed cancel on login dialog
						}
						catch
						{
							MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_LOCKED_TEXT, e.LearningModule.ConnectionString.ConnectionString),
								Properties.Resources.DIC_ERROR_LOADING_LOCKED_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}

						if (source == null)
							return;
						ImportLearningModule(source);
						break;
					case 1:
					default:
						selectedLearningModule = e.LearningModule;
						if (LearningModuleSelected != null)
							LearningModuleSelected(this, new LearningModuleSelectedEventArgs(e.LearningModule));
						break;
				}
			}
		}

		/// <summary>
		/// Handles the OnImportConfigFile event of the learningModulesTreeViewControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Controls.ImportConfigFileEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-03-11</remarks>
		private void learningModulesTreeViewControl_OnImportConfigFile(object sender, ImportConfigFileEventArgs e)
		{
			if (ImportConfigFile(e.ConfigFile))
				LoadLearningModules();
		}

		#endregion
		# region View
		/// <summary>
		/// Handles the Click event of the buttonView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void buttonView_Click(object sender, EventArgs e)
		{
			contextMenuStripView.Show(buttonView, new Point(0, buttonView.Height), ToolStripDropDownDirection.BelowRight);
		}
		/// <summary>
		/// Handles the Click event of the toolStripMenuItemLargeIcon control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void toolStripMenuItemLargeIcon_Click(object sender, EventArgs e)
		{
			SetListViewView(View.LargeIcon);
		}
		/// <summary>
		/// Handles the Click event of the toolStripMenuItemSmallIcon control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void toolStripMenuItemSmallIcon_Click(object sender, EventArgs e)
		{
			SetListViewView(View.SmallIcon);
		}
		/// <summary>
		/// Handles the Click event of the toolStripMenuItemList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void toolStripMenuItemList_Click(object sender, EventArgs e)
		{
			SetListViewView(View.List);
		}
		/// <summary>
		/// Handles the Click event of the toolStripMenuItemDetails control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void toolStripMenuItemDetails_Click(object sender, EventArgs e)
		{
			SetListViewView(View.Details);
		}
		/// <summary>
		/// Handles the Click event of the toolStripMenuItemTile control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void toolStripMenuItemTile_Click(object sender, EventArgs e)
		{
			SetListViewView(View.Tile);
		}
		/// <summary>
		/// Sets the list view view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void SetListViewView(View view)
		{
			int test = listViewLearningModules.Columns.Count;
			UnCheckAllViews();

			switch (view)
			{
				case View.Details:
					listViewLearningModules.BeginUpdate();
					listViewLearningModules.ShowGroups = false;
					listViewLearningModules.Columns.Add(columnHeaderCategory);
					listViewLearningModules.Columns.Add(columnHeaderCardCount);
					listViewLearningModules.Columns.Add(columnHeaderSize);
					listViewLearningModules.EndUpdate();

					listViewFeed.BeginUpdate();
					listViewFeed.ShowGroups = false;
					listViewFeed.Columns.Add(columnHeaderFeedCategory);
					listViewFeed.Columns.Add(columnHeaderFeedCardsCount);
					listViewFeed.Columns.Add(columnHeaderFeedSize);
					listViewFeed.EndUpdate();
					toolStripMenuItemDetails.Checked = true;
					break;
				case View.LargeIcon:
					toolStripMenuItemLargeIcon.Checked = true;
					break;
				case View.List:
					toolStripMenuItemList.Checked = true;
					break;
				case View.SmallIcon:
					toolStripMenuItemSmallIcon.Checked = true;
					break;
				case View.Tile:
				default:
					toolStripMenuItemTile.Checked = true;
					break;
			}

			listViewLearningModules.View = view;
			listViewFeed.View = view;
		}
		/// <summary>
		/// Uns the check all views.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void UnCheckAllViews()
		{
			toolStripMenuItemDetails.Checked = false;
			toolStripMenuItemLargeIcon.Checked = false;
			toolStripMenuItemList.Checked = false;
			toolStripMenuItemSmallIcon.Checked = false;
			toolStripMenuItemTile.Checked = false;

			listViewLearningModules.ShowGroups = true;

			if (listViewLearningModules.OwnerDraw)
				listViewLearningModules.OwnerDraw = false;

			if (listViewLearningModules.Columns.Count > 3)
			{
				listViewLearningModules.BeginUpdate();

				listViewLearningModules.Columns.Remove(columnHeaderCategory);
				listViewLearningModules.Columns.Remove(columnHeaderCardCount);
				listViewLearningModules.Columns.Remove(columnHeaderSize);

				foreach (ListViewItem item in listViewLearningModules.Items)
				{
					listViewLearningModules.Items.Remove(item);
					listViewLearningModules.Items.Add(item);
				}

				listViewLearningModules.EndUpdate();
			}

			if (listViewLearningModules.View == View.Details)
				LoadLearningModulesList(actualItems, true);

			listViewFeed.ShowGroups = true;

			if (listViewFeed.OwnerDraw)
				listViewFeed.OwnerDraw = false;

			if (listViewFeed.Columns.Count > 3)
			{
				listViewFeed.BeginUpdate();

				listViewFeed.Columns.Remove(columnHeaderFeedCategory);
				listViewFeed.Columns.Remove(columnHeaderFeedCardsCount);
				listViewFeed.Columns.Remove(columnHeaderFeedSize);

				foreach (ListViewItem item in listViewFeed.Items)
				{
					listViewFeed.Items.Remove(item);
					listViewFeed.Items.Add(item);
				}

				listViewFeed.EndUpdate();
			}

			if (listViewFeed.View == View.Details)
				LoadFeedList(actualModules, true);
		}
		#endregion
		#region Search TextBox

		private void textBoxSearchBox_TextChanged(object sender, EventArgs e)
		{
			if (textBoxSearchBox.Text != string.Empty)
			{
				if (lmIndex != null)
					UpdateSearchAndRefreshList();
			}
			else
			{
				LoadLearningModulesList(actualItems, false);
				LoadFeedList(actualModules, false);
			}
		}

		#endregion
		# region Grouping
		/// <summary>
		/// Handles the Click event of the buttonGroups control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void buttonGroups_Click(object sender, EventArgs e)
		{
			contextMenuStripGrouping.Show(buttonGroups, new Point(0, buttonGroups.Height), ToolStripDropDownDirection.BelowRight);
		}
		/// <summary>
		/// Handles the Click event of the locationToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void locationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listViewFeed.Tag = false;

			if (locationToolStripMenuItem.Checked)
				return;

			SetListViewGrouping(ItemOrderType.Location);
		}
		/// <summary>
		/// Handles the Click event of the categoryToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listViewFeed.Tag = false;

			if (categoryToolStripMenuItem.Checked)
				return;

			SetListViewGrouping(ItemOrderType.Category);
		}
		/// <summary>
		/// Handles the Click event of the authorToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void authorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listViewFeed.Tag = true;

			if (authorToolStripMenuItem.Checked)
				return;

			SetListViewGrouping(ItemOrderType.Author);
		}
		/// <summary>
		/// Sets the list view grouping.
		/// </summary>
		/// <param name="orderType">Type of the order.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void SetListViewGrouping(ItemOrderType orderType)
		{
			UnCheckAllGroups();
			switch (orderType)
			{
				case ItemOrderType.Category:
					categoryToolStripMenuItem.Checked = true;
					break;
				case ItemOrderType.Author:
					authorToolStripMenuItem.Checked = true;
					break;
				case ItemOrderType.Location:
				default:
					locationToolStripMenuItem.Checked = true;
					break;
			}
			UpdateSearchAndRefreshList();
		}
		/// <summary>
		/// Uns the check all groups.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void UnCheckAllGroups()
		{
			textBoxSearchBox.ResetText();

			locationToolStripMenuItem.Checked = false;
			categoryToolStripMenuItem.Checked = false;
			authorToolStripMenuItem.Checked = false;
		}
		# endregion
		#region Send To
		/// <summary>
		/// Handles the Opening event of the contextMenuStripEntry control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2008-12-15</remarks>
		private void contextMenuStripEntry_Opening(object sender, CancelEventArgs e)
		{
			if (listViewLearningModules.SelectedItems.Count == 0 || listViewLearningModules.SelectedItems[0] is FolderListViewItem)
			{
				e.Cancel = true;
				return;
			}

			LearningModulesIndexEntry entry = (listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule;
			sendToToolStripMenuItem.DropDownItems.Clear();

			//Bring Context menu to default value:
			sendToToolStripMenuItem.Enabled = true;
			deleteToolStripMenuItem.Enabled = true;
			toolStripMenuItemOpen.Enabled = true;
			propertiesToolStripMenuItem.Enabled = true;
			renameToolstripMenuItem.Enabled = true;

			if (!entry.IsVerified || entry.NotAccessibleReason != LearningModuleNotAccessibleReason.IsAccessible)
			{
				sendToToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = false;
				toolStripMenuItemOpen.Enabled = false;
				propertiesToolStripMenuItem.Enabled = false;
				renameToolstripMenuItem.Enabled = false;
			}

			if (entry.ConnectionString.Typ == DatabaseType.MsSqlCe && entry.User == null && entry.NotAccessibleReason != LearningModuleNotAccessibleReason.Protected)
			{
				sendToToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = true;
				toolStripMenuItemOpen.Enabled = false;
				propertiesToolStripMenuItem.Enabled = false;
				renameToolstripMenuItem.Enabled = false;
			}
			else if ((entry.ConnectionString.Typ == DatabaseType.MsSqlCe || entry.ConnectionString.Typ == DatabaseType.PostgreSQL && entry.ConnectionString.SyncType == SyncType.FullSynchronized)
				&& entry.User == null)
			{
				sendToToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = true;
				toolStripMenuItemOpen.Enabled = true;
				propertiesToolStripMenuItem.Enabled = false;
				renameToolstripMenuItem.Enabled = false;
			}
			else if (!entry.User.HasPermission(entry.Dictionary, PermissionTypes.CanModify))
			{
				sendToToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = false;
				renameToolstripMenuItem.Enabled = false;
			}

			bool protectedLM = false;
			try
			{
				if (entry.Connection is WebConnectionStringBuilder)
				{
					sendToToolStripMenuItem.Enabled = false;
					deleteToolStripMenuItem.Enabled = false;
					renameToolstripMenuItem.Enabled = false;
					propertiesToolStripMenuItem.Enabled = false;
				}
				else if ((entry.Dictionary != null && entry.Dictionary.ContentProtected) || LearningModulesIndex.WritableConnections.Count == 0 ||
					LearningModulesIndex.WritableConnections.Count == 1 && entry.ConnectionString.ConnectionString.StartsWith(LearningModulesIndex.WritableConnections[0].ConnectionString))
					sendToToolStripMenuItem.Enabled = false;
				else
				{
					foreach (IConnectionString con in LearningModulesIndex.WritableConnections)
					{
						if (!entry.ConnectionString.ConnectionString.StartsWith(con.ConnectionString))
						{
							ToolStripMenuItem item = new ToolStripMenuItem(con.Name, null, new EventHandler(SentToEventHandler));
							item.Tag = con;
							sendToToolStripMenuItem.DropDownItems.Add(item);
						}
					}
				}
			}
			catch (ProtectedLearningModuleException)
			{
				sendToToolStripMenuItem.Enabled = false;
				propertiesToolStripMenuItem.Enabled = false;
				protectedLM = true;
			}
			catch (UserSessionInvalidException)
			{
				e.Cancel = true;
				return;
			}

			// enabling / disabling of the rename option
			// works for accessable MsSqlCe (eDB) and unsynced PostGreSQL
			renameToolstripMenuItem.Enabled = false;
			if (!protectedLM && entry.IsVerified && entry.IsAccessible && entry.User != null && entry.User.HasPermission(entry.Dictionary, PermissionTypes.CanModify))
			{
				if (entry.ConnectionString.Typ == DatabaseType.MsSqlCe)
					renameToolstripMenuItem.Enabled = true;
				else if (entry.ConnectionString.Typ == DatabaseType.PostgreSQL && (entry.Connection as ISyncableConnectionString).SyncType == SyncType.NotSynchronized)
					renameToolstripMenuItem.Enabled = true;
			}

			if (entry.ConnectionString.Typ == DatabaseType.Xml)
				propertiesToolStripMenuItem.Enabled = false;
		}

		/// <summary>
		/// Handles the Click event of the propertiesToolStripMenuItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-27</remarks>
		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listViewLearningModules.SelectedItems.Count > 0)
				PropertiesForm.LoadDictionary((listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule, (Parent as LearningModulesForm).MainHelpObject.HelpNamespace);
		}

		private IConnectionString conTarget;
		private void SentToEventHandler(object sender, EventArgs e)
		{
			conTarget = (sender as ToolStripMenuItem).Tag as IConnectionString;
			LearningModulesIndexEntry source = (listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule;

			ImportLearningModule(source);
		}

		private void ImportLearningModule(LearningModulesIndexEntry source)
		{
			try
			{
				FolderIndexEntry folder = GetFolderOfConnection(conTarget);
				if (folder == null)
					return;

				try
				{
					if (!LearningModulesIndex.ConnectionUsers[conTarget].List().HasPermission(PermissionTypes.CanModify))
					{
						TaskDialog.MessageBox(Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_CAPTION, Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_TEXT, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error);
						return;
					}
				}
				catch (KeyNotFoundException)
				{
					return;
				}

				ShowStatusMessage(false);
				IUser targetUser = folder.CurrentUser;
				if (conTarget.ConnectionType == DatabaseType.Unc)
				{
					string newFileName = Path.Combine(folder.Connection.ConnectionString, source.DisplayName.Replace(@"\", "_") + DAL.Helper.EmbeddedDbExtension);
					targetUser.ConnectionString = new ConnectionStringStruct(DatabaseType.MsSqlCe, newFileName);
				}
				IDictionary target = targetUser.List().AddNew(source.Category.Id > 5 ? MLifter.DAL.Category.DefaultCategory : source.Category.Id, source.DisplayName);

				int newId = target.Id;
				ConnectionStringStruct css = new ConnectionStringStruct(conTarget.ConnectionType == DatabaseType.Unc ? DatabaseType.MsSqlCe : conTarget.ConnectionType,
					target.Connection, newId);

				try
				{
					LearnLogic.CopyToFinished += new EventHandler(LearnLogic_CopyToFinished);
					LearnLogic.CopyLearningModule(source.ConnectionString, css, GetUser, UpdateStatusMessage, DataAccessError, null);
				}
				#region error handling
				catch (DictionaryNotDecryptedException)
				{
					HideStatusMessage();

					LearningModulesIndex.ConnectionUsers[conTarget].List().Delete(css);

					MessageBox.Show(Properties.Resources.DIC_ERROR_NOT_DECRYPTED_TEXT,
						Properties.Resources.DIC_ERROR_NOT_DECRYPTED_CAPTION,
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (DictionaryContentProtectedException)
				{
					HideStatusMessage();

					LearningModulesIndex.ConnectionUsers[conTarget].List().Delete(css);

					MessageBox.Show(string.Format(Properties.Resources.DIC_ERROR_CONTENTPROTECTED_TEXT, source.ConnectionString.ConnectionString),
						Properties.Resources.DIC_ERROR_CONTENTPROTECTED_CAPTION,
						MessageBoxButtons.OK, MessageBoxIcon.Error);

				}
				catch (MLifter.DAL.InvalidDictionaryException)
				{
					HideStatusMessage();

					LearningModulesIndex.ConnectionUsers[conTarget].List().Delete(css);

					MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, source.ConnectionString.ConnectionString),
						Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (System.Xml.XmlException)
				{
					HideStatusMessage();

					LearningModulesIndex.ConnectionUsers[conTarget].List().Delete(css);

					MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, source.ConnectionString.ConnectionString),
						Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (System.IO.IOException)
				{
					HideStatusMessage();

					LearningModulesIndex.ConnectionUsers[conTarget].List().Delete(css);

					MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_LOCKED_TEXT, source.ConnectionString.ConnectionString),
						Properties.Resources.DIC_ERROR_LOADING_LOCKED_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch
				{
					HideStatusMessage();

					LearningModulesIndex.ConnectionUsers[conTarget].List().Delete(css);

					MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, source.DisplayName),
						Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				#endregion
			}
			catch (PermissionException)
			{
				TaskDialog.MessageBox(Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_CAPTION, Resources.STARTUP_IMPORT_LEARNING_MODULE_PERMISSION_MBX_TEXT, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error);
				return;
			}
			catch
			{
				HideStatusMessage();
				MessageBox.Show(String.Format(Properties.Resources.DIC_ERROR_LOADING_TEXT, source.ConnectionString.ConnectionString),
					Properties.Resources.DIC_ERROR_LOADING_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}
		/// <summary>
		/// Handles the CopyToFinished event of the LearnLogic control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2008-12-15</remarks>
		void LearnLogic_CopyToFinished(object sender, EventArgs e)
		{
			HideStatusMessage();
			if (e is CopyToEventArgs)
			{
				if (!(e as CopyToEventArgs).success)
				{
					TaskDialog.MessageBox(Resources.STARTUP_IMPORT_LEARNING_MODULE_FAILED_MBX_CAPTION, Resources.STARTUP_IMPORT_LEARNING_MODULE_FAILED_MBX_TEXT,
						string.Empty, (e as CopyToEventArgs).Exception != null ? (e as CopyToEventArgs).Exception.ToString() : string.Empty, string.Empty, string.Empty,
						TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
				}
			}
			FolderIndexEntry folder = GetFolderOfConnection(conTarget);
			if (folder == null) return;
			actualItems.Clear();
			folder.BeginLoading();
			if (selectedTreeViewNode.IsMainNode || conTarget == selectedTreeViewNode.Folder.Connection)
				UpdateSearchAndRefreshList();
		}
		#endregion
		#region Drag&Drop Implementation
		/// <summary>
		/// Handles the DragEnter event of the listViewLearningModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-02-27</remarks>
		private void listViewLearningModules_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (Helper.CheckFileName(((string[])e.Data.GetData(DataFormats.FileDrop))[0]))
					e.Effect = DragDropEffects.Link;
				else
					e.Effect = DragDropEffects.None;
			}
		}

		/// <summary>
		/// Handles the DragDrop event of the listViewLearningModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-02-27</remarks>
		private void listViewLearningModules_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
				ActivateFileDropTimer(filePath);
			}
		}
		#endregion
		/// <summary>
		/// Handles the Click event of the buttonLevelUp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-03-12</remarks>
		private void buttonLevelUp_Click(object sender, EventArgs e)
		{
			learningModulesTreeViewControl.OneLevelUp();

			ToolTip tTip = new ToolTip();
			tTip.SetToolTip(buttonLevelUp, Resources.ONE_LEVEL_UP);
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Removes the drive.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>Documented by Dev03, 2009-04-06</remarks>
		public void RemoveDrive(string drive)
		{
			if (selectedTreeViewNode.Connection != null && selectedTreeViewNode.Connection.ConnectionString.StartsWith(drive))
				learningModulesTreeViewControl.SelectDefaultConnectionNode();

			DriveInfo driveInfo = new DriveInfo(drive[0].ToString());
			LearningModulesIndex.ConnectionsHandler.ConnectionStrings.RemoveAll(c => c.ConnectionString.StartsWith(drive));
			learningModulesTreeViewControl.RemoveConnectionsFromDrive(driveInfo);
			lmIndex.RemoveModulesOfDrive(driveInfo);

			actualItems.RemoveAll(i => i.Connection.ConnectionString.StartsWith(drive));
			LoadLearningModulesList(actualItems, false);
		}

		/// <summary>
		/// Adds the drive.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>Documented by Dev03, 2009-04-06</remarks>
		public void AddDrive(string drive)
		{
			DriveInfo driveInfo = new DriveInfo(drive[0].ToString());
			if (!File.Exists(Path.Combine(driveInfo.RootDirectory.FullName, Methods.MLifterStickCheckFile)))
				return;

			IConnectionString con = LearningModulesIndex.ConnectionsHandler.AddUsbDrive(driveInfo);
			lmIndex.GetFolderOfConnection(con);
		}

		/// <summary>
		/// Closes all scanning tasks.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		public void Close()
		{
			SavePersistencyData();

			if (lmIndex != null)
				lmIndex.EndLearningModulesScan();

			disposeAndDisconnectLMs();

			// kill all threads if running
			foreach (Thread t in new List<Thread>() { filterThread, newsThread })
			{
				try { if (t != null && t.IsAlive) t.Abort(); }
				catch (ThreadAbortException) { }
				catch (Exception e) { Trace.WriteLine(e.ToString()); }
			}


		}

		#endregion

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (lmIndex != null)
			{
				lmIndex.EndLearningModulesScan();

				foreach (IUser user in lmIndex.Users)
					if (selectedLearningModule == null || user != selectedLearningModule.User)
						if (user != null)
							user.Logout();
			}

			base.OnHandleDestroyed(e);
		}

		#region Private Methods

		/// <summary>
		/// Updates the actual items.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		private bool UpdateActualItems()
		{
			try
			{
				if (selectedTreeViewNode == null)
					return false;

				if (selectedTreeViewNode.IsMainNode)
				{
					actualItems.Clear();
					actualItems.AddRange(lmIndex.LearningModules.ConvertAll<IIndexEntry>(e => e as IIndexEntry));
				}
				else
				{
					this.Cursor = Cursors.WaitCursor;
					FolderIndexEntry entry = (selectedTreeViewNode.Folder == null) ? lmIndex.GetFolderOfConnection(selectedTreeViewNode.Connection) :
						(selectedTreeViewNode.Folder.Connection is UncConnectionStringBuilder) ?
						lmIndex.Folders.Find(f => f.Connection == selectedTreeViewNode.Folder.Connection && f.Path == selectedTreeViewNode.Folder.Path) :
						lmIndex.GetFolderOfConnection(selectedTreeViewNode.Folder.Connection);
					this.Cursor = Cursors.Default;
					if (entry == null)
						throw new ArgumentNullException();
					actualItems = entry.GetContainingEntries(learningModulesTreeViewControl.ShowLearningModulesOfSubFolder);
				}

				LoadLearningModulesList(actualItems, false);

				if (learningModulesTreeViewControl.SelectedNode is FeedTreeNode)
				{
					FeedTreeNode node = learningModulesTreeViewControl.SelectedNode as FeedTreeNode;
					if (node.IsListLoaded)
					{
						if (learningModulesTreeViewControl.ShowLearningModulesOfSubFolder)
							SetFeedVisible(true, node.Modules);
						else
						{
							List<ListViewItem> items = new List<ListViewItem>();
							foreach (FeedCategoryTreeNode subNode in node.Nodes)
								items.Add(GetCategoryListViewItem(subNode));
							SetFeedVisible(true, items);
						}
					}
				}
				else if (learningModulesTreeViewControl.SelectedNode is FeedCategoryTreeNode)
				{
					if (learningModulesTreeViewControl.ShowLearningModulesOfSubFolder)
						SetFeedVisible(true, (learningModulesTreeViewControl.SelectedNode as FeedCategoryTreeNode).Modules);
					else
					{
						List<ListViewItem> items = new List<ListViewItem>();
						foreach (FeedCategoryTreeNode subNode in learningModulesTreeViewControl.SelectedNode.Nodes)
							items.Add(GetCategoryListViewItem(subNode));
						items.AddRange((learningModulesTreeViewControl.SelectedNode as FeedCategoryTreeNode).OwnModules);
						SetFeedVisible(true, items);
					}
				}
			}
			catch (ServerOfflineException)
			{
				ShowServerOfflineMessage(new List<IConnectionString>() { selectedTreeViewNode.Connection }, (selectedTreeViewNode.Connection as ISyncableConnectionString).SyncType == SyncType.FullSynchronized);

				if ((selectedTreeViewNode.Connection as ISyncableConnectionString).SyncType == SyncType.FullSynchronized)
				{
					UpdateActualItems();
					return true;
				}
				else
					return false;
			}
			catch (NoValidUserException)
			{
				//user pressed cancel
				return false;
			}
			catch (ArgumentNullException)
			{
				TaskDialog.MessageBox(Resources.LEARNING_MODULES_PAGE_NO_VALID_USER_TITLE,
					String.Format(Resources.LEARNING_MODULES_PAGE_NO_VALID_USER_MAIN, selectedTreeViewNode.Text),
					Resources.LEARNING_MODULES_PAGE_NO_VALID_USER_CONTENT, TaskDialogButtons.OK, TaskDialogIcons.Error);
				return false;
			}
			catch (UserSessionInvalidException)
			{
				TaskDialog.MessageBox(Resources.SESSION_INVALID_TITLE, Resources.SESSION_INVALID_MAIN, Resources.SESSION_INVALID_DETAIL,
					 TaskDialogButtons.OK, TaskDialogIcons.Error);
				return false;
			}
			catch (Exception ex)
			{
				TaskDialog.MessageBox(Resources.TASK_DIALOG_UNKNOWN_ERROR, ex.Message, ex.Message, ex.StackTrace, string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
				return false;
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
			return true;
		}

		private static ListViewItem GetCategoryListViewItem(FeedCategoryTreeNode subNode)
		{
			ListViewItem subItem = new ListViewItem(subNode.Text);
			subItem.ImageIndex = 1;
			subItem.SubItems.Add(string.Format(Resources.SUBNODE_CATEGORIES, subNode.Nodes.Count));
			subItem.SubItems[1].ForeColor = Color.Gray;
			subItem.SubItems.Add(string.Format(Resources.SUBNODE_MODULES, subNode.OwnModules.Count));
			subItem.SubItems[2].ForeColor = Color.Gray;
			subItem.Tag = subNode;
			return subItem;
		}

		/// <summary>
		/// Occurs when one or more servers are offline during the scan.
		/// </summary>
		/// <param name="offline">The offline.</param>
		/// <remarks>Documented by Dev02, 2009-04-02</remarks>
		void lmIndex_ServersOffline(List<IConnectionString> offline)
		{
			ShowServerOfflineMessage(offline, false);
		}

		/// <summary>
		/// Shows the server offline message for all servers in the list.
		/// </summary>
		/// <param name="offline">The offline.</param>
		/// <remarks>Documented by Dev02, 2009-04-02</remarks>
		private void ShowServerOfflineMessage(List<IConnectionString> offline, bool fullSynced)
		{
			if (offline.Count < 1)
				return;

			string mainMessage;
			if (offline.Count == 1)
				mainMessage = String.Format(Resources.LEARNING_MODULES_PAGE_SERVER_OFFLINE_MAIN_SINGLE, offline[0].Name);
			else
			{
				String serverNames = String.Empty;
				offline.ForEach(c => serverNames += c.Name + Environment.NewLine);
				mainMessage = String.Format(Resources.LEARNING_MODULES_PAGE_SERVER_OFFLINE_MAIN_MULTIPLE, serverNames);
			}

			TaskDialog.MessageBox(Resources.LEARNING_MODULES_PAGE_SERVER_OFFLINE_TITLE, mainMessage,
				fullSynced ? Resources.LEARNING_MODULES_PAGE_SERVER_OFFLINE_CONTENT : Resources.LEARNING_MODULES_PAGE_SERVER_OFFLINE_CONTENT_OFFLINE, TaskDialogButtons.OK, TaskDialogIcons.Error);
		}

		/// <summary>
		/// Loads the list into the ListView.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="groupsOnly">if set to <c>true</c> [groups only].</param>
		/// <remarks>Documented by Dev05, 2008-12-10</remarks>
		private void LoadLearningModulesList(List<IIndexEntry> items, bool groupsOnly)
		{
			listViewLearningModules.BeginUpdate();

			if (!groupsOnly)
			{
				listViewLearningModules.Items.Clear();
				foreach (FolderIndexEntry entry in items.FindAll(en => en is FolderIndexEntry))
				{
					FolderListViewItem item = new FolderListViewItem(entry);
					listViewLearningModules.Items.Add(item);
					item.UpdateImage();
					item.UpdateDetails();
				}
				foreach (LearningModulesIndexEntry entry in items.FindAll(en => en is LearningModulesIndexEntry))
				{
					LearningModuleListViewItem item = new LearningModuleListViewItem(entry);
					listViewLearningModules.Items.Add(item);
					item.UpdateDetails();
				}

				if (listViewLearningModules.Items.Count > 0 && !FeedIsVisible)
					listViewLearningModules.SelectedIndices.Add(0);
			}

			listViewLearningModules.Groups.Clear();
			foreach (ListViewItem item in listViewLearningModules.Items)
			{
				if (item is LearningModuleListViewItem)
					SetLearningModuleListViewItemGroup(item as LearningModuleListViewItem);
				else
					SetFolderListViewItemGroup(item as FolderListViewItem);
			}

			listViewLearningModules.EndUpdate();
		}
		private void LoadFeedList(List<ListViewItem> items, bool groupsOnly)
		{
			listViewFeed.BeginUpdate();

			if (!groupsOnly)
			{
				listViewFeed.Items.Clear();

				foreach (ListViewItem entry in items)
				{
					listViewFeed.Items.Add(entry);

					if (entry.Tag is ModuleInfo)
					{
						ModuleInfo details = (ModuleInfo)entry.Tag;

						if (details.IconSmall != null && details.IconSmall.Length > 0)
						{
							entry.ListView.SmallImageList.Images.Add(Image.FromStream(new MemoryStream(details.IconSmall)));
							entry.ImageIndex = entry.ListView.LargeImageList.Images.Count - 1;
						}
						if (details.IconBig != null && details.IconBig.Length > 0)
						{
							entry.ListView.LargeImageList.Images.Add(Image.FromStream(new MemoryStream(details.IconBig)));
							entry.ImageIndex = entry.ListView.LargeImageList.Images.Count - 1;
						}
					}
				}

				if (listViewFeed.Items.Count > 0 && FeedIsVisible)
					listViewFeed.SelectedIndices.Add(0);
			}

			listViewFeed.Groups.Clear();
			foreach (ListViewItem item in listViewFeed.Items)
				SetFeedItemGroup(item);

			listViewFeed.EndUpdate();
		}
		private void SetFeedItemGroup(ListViewItem item)
		{
			if (locationToolStripMenuItem.Checked)
				SetFeedItemGroup(item, ItemOrderType.Category);
			else if (categoryToolStripMenuItem.Checked)
				SetFeedItemGroup(item, ItemOrderType.Category);
			else if (authorToolStripMenuItem.Checked)
				SetFeedItemGroup(item, ItemOrderType.Author);
		}
		private void SetFeedItemGroup(ListViewItem item, ItemOrderType order)
		{
			item.Group = null;

			if (item.Tag is FeedCategoryTreeNode)
			{
				if (!listViewFeed.Groups.Contains(categoriesGroup))
					listViewFeed.Groups.Add(categoriesGroup);

				item.Group = categoriesGroup;
				return;
			}

			switch (order)
			{
				case ItemOrderType.Category:
					foreach (ListViewGroup group in listViewFeed.Groups)
					{
						if (group.Header == item.SubItems[2].Text)
						{
							item.Group = group;
							break;
						}
					}

					if (item.Group == null)
					{
						ListViewGroup grp = new ListViewGroup(item.SubItems[2].Text);
						listViewFeed.Groups.Add(grp);
						item.Group = grp;
					}
					break;
				case ItemOrderType.Author:
					foreach (ListViewGroup group in listViewFeed.Groups)
					{
						if (group.Header == (item.Tag is ModuleInfo && ((ModuleInfo)item.Tag).Author.Length > 0 ? ((ModuleInfo)item.Tag).Author : Resources.NO_AUTHOR))
						{
							item.Group = group;
							break;
						}
					}

					if (item.Group == null)
					{
						ListViewGroup grp = new ListViewGroup((item.Tag is ModuleInfo && ((ModuleInfo)item.Tag).Author.Length > 0 ?
							((ModuleInfo)item.Tag).Author : Resources.NO_AUTHOR));
						listViewFeed.Groups.Add(grp);
						item.Group = grp;
					}
					break;
				default:
					throw new ArgumentException();
			}
		}
		private void SetLearningModuleListViewItemGroup(LearningModuleListViewItem item)
		{
			if (locationToolStripMenuItem.Checked)
			{
				SetLearningModuleListViewItemGroup(item, ItemOrderType.Location);
			}
			else if (categoryToolStripMenuItem.Checked)
			{
				SetLearningModuleListViewItemGroup(item, ItemOrderType.Category);
			}
			else if (authorToolStripMenuItem.Checked)
			{
				SetLearningModuleListViewItemGroup(item, ItemOrderType.Author);
			}
		}
		private void SetLearningModuleListViewItemGroup(LearningModuleListViewItem item, ItemOrderType order)
		{
			item.Group = null;
			switch (order)
			{
				case ItemOrderType.Location:
					foreach (ListViewGroup group in listViewLearningModules.Groups)
					{
						if (group.Header == item.LearningModule.ConnectionName)
						{
							item.Group = group;
							break;
						}
					}

					if (item.Group == null)
					{
						ListViewGroup grp = new ListViewGroup(item.LearningModule.ConnectionName);
						listViewLearningModules.Groups.Add(grp);
						item.Group = grp;
					}
					break;
				case ItemOrderType.Category:
					foreach (ListViewGroup group in listViewLearningModules.Groups)
					{
						if (group.Header == (item.LearningModule.Category != null ? item.LearningModule.Category.ToString() : Resources.NO_CATEGORY))
						{
							item.Group = group;
							break;
						}
					}

					if (item.Group == null)
					{
						ListViewGroup grp = new ListViewGroup((item.LearningModule.Category != null ? item.LearningModule.Category.ToString() : Resources.NO_CATEGORY));
						listViewLearningModules.Groups.Add(grp);
						item.Group = grp;
					}
					break;
				case ItemOrderType.Author:
					foreach (ListViewGroup group in listViewLearningModules.Groups)
					{
						if (group.Header == (item.LearningModule.Author != string.Empty ? item.LearningModule.Author : Resources.NO_AUTHOR))
						{
							item.Group = group;
							break;
						}
					}

					if (item.Group == null)
					{
						ListViewGroup grp = new ListViewGroup((item.LearningModule.Author != string.Empty ? item.LearningModule.Author : Resources.NO_AUTHOR));
						listViewLearningModules.Groups.Add(grp);
						item.Group = grp;
					}
					break;
				default:
					throw new ArgumentException();
			}
		}
		private void SetFolderListViewItemGroup(FolderListViewItem item)
		{
			if (locationToolStripMenuItem.Checked)
			{
				SetFolderListViewItemGroup(item, ItemOrderType.Location);
			}
			else if (categoryToolStripMenuItem.Checked)
			{
				SetFolderListViewItemGroup(item, ItemOrderType.Category);
			}
			else if (authorToolStripMenuItem.Checked)
			{
				SetFolderListViewItemGroup(item, ItemOrderType.Author);
			}
		}
		private void SetFolderListViewItemGroup(FolderListViewItem item, ItemOrderType order)
		{
			item.Group = null;
			switch (order)
			{
				case ItemOrderType.Location:
					foreach (ListViewGroup group in listViewLearningModules.Groups)
					{
						if (group.Header == item.Folder.Connection.Name)
						{
							item.Group = group;
							break;
						}
					}

					if (item.Group == null)
					{
						ListViewGroup grp = new ListViewGroup(item.Folder.Connection.Name);
						listViewLearningModules.Groups.Add(grp);
						item.Group = grp;
					}
					break;
				case ItemOrderType.Category:
				case ItemOrderType.Author:
					foreach (ListViewGroup group in listViewLearningModules.Groups)
					{
						if (group.Header == Resources.GROUP_FOLDER_NAME)
						{
							item.Group = group;
							break;
						}
					}

					if (item.Group == null)
					{
						ListViewGroup grp = new ListViewGroup(Resources.GROUP_FOLDER_NAME);
						listViewLearningModules.Groups.Insert(0, grp);
						item.Group = grp;
					}
					break;
				default:
					throw new ArgumentException();
			}
		}

		/// <summary>
		/// Updates the LearningModule item.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <param name="onlyRefresh">if set to <c>true</c> [only refresh].</param>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void UpdateItem(LearningModulesIndexEntry entry)
		{
			try
			{
				listViewLearningModules.Invoke((MethodInvoker)delegate
				{
					ListViewItem item = null;
					foreach (ListViewItem listItem in listViewLearningModules.Items)
					{
						if (!(listItem is LearningModuleListViewItem))
							continue;

						LearningModuleListViewItem itm = listItem as LearningModuleListViewItem;
						if (itm.LearningModule == entry)
						{
							item = itm;
							break;
						}
					}

					if (item == null)
						return;

					//Updated the PreviewControl
					if (listViewLearningModules.SelectedItems.Contains(item))
						UpdateDetails();

					if (categoryToolStripMenuItem.Checked && entry.Category != null)
					{
						foreach (ListViewGroup group in listViewLearningModules.Groups)
							if (group.Header == entry.Category.ToString())
							{ item.Group = group; break; }
					}
					else if (authorToolStripMenuItem.Checked && !String.IsNullOrEmpty(entry.Author))
					{
						foreach (ListViewGroup group in listViewLearningModules.Groups)
							if (group.Header == entry.Author)
							{ item.Group = group; break; }
						if (item.Group == null)
						{
							ListViewGroup grp = new ListViewGroup(entry.Author);
							listViewLearningModules.Groups.Add(grp);
							item.Group = grp;
						}
					}
				});
			}
			catch (InvalidOperationException exp) { Trace.WriteLine(exp.ToString()); }
		}

		/// <summary>
		/// Shows the news.
		/// </summary>
		/// <remarks>Documented by Dev05, 2007-12-11</remarks>
		private void ShowNews()
		{
			try
			{
				webBrowserNews.BeginInvoke((MethodInvoker)delegate { webBrowserNews.DocumentText = News.LoadingMessage; });

				if (Tools.IsUserOnline())
				{
					string newsContent;
					int newNewsCount;

					DateTime newsDate = Settings.Default.NewsDate;
					MLifter.BusinessLayer.News news = new MLifter.BusinessLayer.News(Resources.RssFeedTransformer);
					if (news.GetNewsFeed(out newsContent, ref newsDate, out newNewsCount, Settings.Default.NewsFeedRss, 1))
					{
						if (newNewsCount > 0 && !FirstUse)
						{
							expandoNews.BeginInvoke((MethodInvoker)delegate
							{
								expandoNews.Text = string.Format(Resources.NEWS_NEWNEWS, newNewsCount);
								expandoNews.Collapsed = false;
								expandoNews.SpecialGroup = true;
							});
						}

						webBrowserNews.BeginInvoke((MethodInvoker)delegate { webBrowserNews.DocumentText = newsContent; });
						Properties.Settings.Default.NewsDate = newsDate;
						Properties.Settings.Default.Save();
					}
					else
					{
						webBrowserNews.BeginInvoke((MethodInvoker)delegate { webBrowserNews.DocumentText = News.UnavailableMessage; });
					}
				}
				else
					expandoNews.BeginInvoke((MethodInvoker)delegate { expandoNews.Collapsed = true; });
			}
			catch (Exception e)
			{
				Trace.WriteLine("News-Thread exception: " + e.ToString());
			}
		}

		/// <summary>
		/// Updates the details.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-12-22</remarks>
		private void UpdateDetails()
		{
			if (!FeedIsVisible)
			{
				if (listViewLearningModules.SelectedIndices.Count == 0 || listViewLearningModules.SelectedItems[0] is FolderListViewItem)
				{
					ResetStatisticsAndPreview();
					buttonOK.Enabled = false;
					return;
				}
				else if (!(listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule.IsVerified ||
					(!(listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule.IsAccessible &&
					(listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule.NotAccessibleReason != LearningModuleNotAccessibleReason.Protected))
					buttonOK.Enabled = false;
				else
					buttonOK.Enabled = true;

				LearningModulesIndexEntry entry = (listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule;
				if (entry.Preview != null)
				{
					learningModulePreviewControlMain.PreviewImage = entry.Preview.PreviewImage;
					learningModulePreviewControlMain.Description = entry.Preview.Description;
				}
				else
					ResetStatisticsAndPreview();

				if (entry.Statistics == null || entry.Statistics.CardsAsked == 0)
					ResetStatistics();
				else
				{
					labelLastSessionTime.Text = string.Format(Resources.LEARNING_MODULES_PAGE_Last_Session_Time, entry.Statistics.LastSessionTime.Hours,
						entry.Statistics.LastSessionTime.Minutes, entry.Statistics.LastSessionTime.Seconds);
					labelCardsAsked.Text = string.Format(Resources.LEARNING_MODULES_PAGE_CardsCount, entry.Statistics.CardsAsked, entry.Statistics.Right, entry.Statistics.Wrong);
					labelRatio.Text = string.Format(Resources.LEARNING_MODULES_PAGE_Ratio, entry.Statistics.Ratio);
					labelAverageTime.Text = string.Format(Resources.LEARNING_MODULES_PAGE_AverageTime, entry.Statistics.TimePerCard.TotalSeconds);
					labelCardsPerMinute.Text = string.Format(Resources.LEARNING_MODULES_PAGE_CPM, entry.Statistics.CardsPerMinute);
				}
			}
			else
			{
				if (listViewFeed.SelectedIndices.Count == 0)
				{
					ResetStatisticsAndPreview();
					buttonOK.Enabled = false;
					return;
				}
				else if (!(listViewFeed.SelectedItems[0].Tag is ModuleInfo))
				{
					ResetStatisticsAndPreview();
					buttonOK.Enabled = true;
					return;
				}
				else
					buttonOK.Enabled = true;

				ModuleInfo details = (ModuleInfo)listViewFeed.SelectedItems[0].Tag;
				ResetStatisticsAndPreview();
				if (details.Preview != null && details.Preview.Length > 0)
					learningModulePreviewControlMain.PreviewImage = Image.FromStream(new MemoryStream(details.Preview));

				learningModulePreviewControlMain.Description = StripTagsCharArray(details.Description);
			}
		}

		/// <summary>
		/// Remove HTML tags from string using char array.
		/// </summary>
		public static string StripTagsCharArray(string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}

		/// <summary>
		/// Hides the statistics and preview.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-12-22</remarks>
		private void ResetStatisticsAndPreview()
		{
			learningModulePreviewControlMain.PreviewImage = Resources.NoImage;
			learningModulePreviewControlMain.Description = listViewLearningModules.SelectedIndices.Count == 0 || listViewLearningModules.SelectedItems[0] is FolderListViewItem ?
				Resources.LEARNING_MODULES_PAGE_NO_SELECTION : Resources.LEARNING_MODULES_PAGE_NO_INFO;

			ResetStatistics();
		}

		/// <summary>
		/// Resets the statistics.
		/// </summary>
		/// <remarks>Documented by Dev03, 2008-12-22</remarks>
		private void ResetStatistics()
		{
			labelAverageTime.Text = "-";
			labelCardsAsked.Text = "-";
			labelCardsPerMinute.Text = "-";
			labelLastSessionTime.Text = "-";
			labelRatio.Text = "-";
		}

		/// <summary>
		/// Loads the recent learning modules into GUI.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		private void LoadRecentLearningModulesIntoGUI()
		{
			expandoRecent.Items.Clear();


			int numberRecentFiles = RecentLearningModules.GetRecentModules().Count;
			if (numberRecentFiles == 0)
			{
				expandoRecent.Hide();
				return;
			}
			else
			{
				if (numberRecentFiles > MLifter.Controls.Properties.Settings.Default.MaxRecentFilesInExpander)
					numberRecentFiles = MLifter.Controls.Properties.Settings.Default.MaxRecentFilesInExpander;

				int height = 35 + numberRecentFiles * ((new TaskItem().Height) + 5);
				expandoRecent.ExpandedHeight = height;
				//expandoRecent.Height = height;
			}



			int counter = 0;
			ToolTip tTip = new ToolTip();
			Point startPos = new Point(12, 35);
			foreach (LearningModulesIndexEntry entry in RecentLearningModules.GetRecentModules())
			{
				if (counter >= MLifter.Controls.Properties.Settings.Default.MaxRecentFilesInExpander)
					break;

				TaskItem taskItem = new TaskItem();
				taskItem.Location = new Point(startPos.X, startPos.Y + ((taskItem.Height + 5) * counter));
				taskItem.Text = (entry.DisplayName == null || entry.DisplayName == string.Empty ? Resources.ERROR_LEARNINGMODULE_NONAME : entry.DisplayName) +
					(entry.ConnectionName != null || entry.ConnectionName == string.Empty ? " (" + entry.ConnectionName + ")" : string.Empty);

				tTip.SetToolTip(taskItem, taskItem.Text);

				while (taskItem.PreferredWidth > expandoRecent.ClientSize.Width - startPos.X) taskItem.Text = taskItem.Text.Substring(0, taskItem.Text.Length - 4) + "...";
				taskItem.Width = expandoRecent.ClientSize.Width - startPos.X;

				taskItem.Tag = entry;
				taskItem.Click += new EventHandler(RecentItem_Click);
				if (entry.ConnectionString.Typ == DatabaseType.MsSqlCe || entry.ConnectionString.Typ == DatabaseType.Xml)
					taskItem.Image = Resources.learning_16;
				else
					taskItem.Image = Resources.world_16;
				taskItem.TextAlign = ContentAlignment.MiddleLeft;
				expandoRecent.Items.Add(taskItem);

				counter++;
			}

		}

		/// <summary>
		/// Handles the Click event of the RecentItem control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-09</remarks>
		private void RecentItem_Click(object sender, EventArgs e)
		{
			selectedLearningModule = (sender as TaskItem).Tag as LearningModulesIndexEntry;

			if (selectedLearningModule.ConnectionString.SyncType != SyncType.NotSynchronized)
			{
				try
				{
					if (selectedLearningModule.ConnectionString.SyncType != SyncType.FullSynchronized)
					{
						foreach (KeyValuePair<IConnectionString, IUser> pair in LearningModulesIndex.ConnectionUsers)
						{
							if (pair.Key.ConnectionString == selectedLearningModule.ConnectionString.ConnectionString)
							{
								selectedLearningModule.User = pair.Value;
								selectedLearningModule.User.Login();
								break;
							}
						}
						if (selectedLearningModule.User == null)
						{
							IUser user = UserFactory.Create(LoginForm.OpenLoginForm, selectedLearningModule.ConnectionString, DataAccessError, this);
							if (user == null)
								return;
							selectedLearningModule.User = user;
						}
					}
				}
				catch (NoValidUserException) { return; }

				try
				{
					Sync(selectedLearningModule, SyncStatusUpdate);
				}
				catch (SynchronizationFailedException exp)
				{
					TaskDialog.MessageBox(Resources.LEARNING_MODULES_PAGE_SYNC_FAILED_TITLE, Resources.LEARNING_MODULES_PAGE_SYNC_FAILED_MAIN,
						Resources.LEARNING_MODULES_PAGE_SYNC_FAILED_CONTENT, exp.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Information);
					return;
				}
			}

			OnLearningModuleSelected(new LearningModuleSelectedEventArgs(selectedLearningModule));
		}

		/// <summary>
		/// Checks the height.
		/// </summary>
		/// <param name="expando">The expando.</param>
		/// <remarks>Documented by Dev05</remarks>
		private void CheckHeight(Expando expando)
		{
			if (Loading)
				return;

			while (taskPaneInformations.PreferredSize.Height > taskPaneInformations.Height)
			{
				foreach (Expando exp in CollabsidableExpandos)
				{
					if (FirstUse && exp == expandoTreeView)
						continue;

					if (exp != expando && !exp.Collapsed)
					{
						exp.Collapsed = true;
						break;
					}
				}
				if (CollabsidableExpandos.Count == 0)
					return;
			}
		}

		/// <summary>
		/// Imports the config file.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>[true] if success.</returns>
		/// <remarks>Documented by Dev03, 2009-03-11</remarks>
		private bool ImportConfigFile(string file)
		{
			EmulatedTaskDialog dialog = new EmulatedTaskDialog();
			dialog.Owner = ParentForm;
			dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.Title = Resources.DRAG_CFG_DIALOG_TITLE;
			dialog.MainInstruction = Resources.DRAG_CFG_DIALOG_MAIN;
			dialog.Content = Resources.DRAG_CFG_DIALOG_CONTENT;
			dialog.CommandButtons = Resources.DRAG_CFG_DIALOG_BUTTONS;
			dialog.Buttons = TaskDialogButtons.None;
			dialog.MainIcon = TaskDialogIcons.Question;
			dialog.MainImages = new Image[] { Resources.applications_system, Resources.process_stop };
			dialog.HoverImages = new Image[] { Resources.applications_system, Resources.process_stop };
			dialog.CenterImages = true;
			dialog.BuildForm();
			DialogResult dialogResult = dialog.ShowDialog();

			switch (dialog.CommandButtonClickedIndex)
			{
				//Import
				case 0:
					int successfulImportedConnections = 0;
					try
					{
						successfulImportedConnections = ConnectionStringHandler.ImportConfigFile(file, GeneralConfigurationPath, UserConfigurationPath);
					}
					catch (InvalidConfigFileException)
					{
						TaskDialog.MessageBox(Resources.DRAG_INVALID_CFG_FILE_TITLE, Resources.DRAG_INVALID_CFG_FILE_MAININSTRUCTION, Resources.DRAG_INVALID_CFG_FILE_CONTENT,
											  TaskDialogButtons.OK, TaskDialogIcons.Error);
						return false;
					}
					catch (Exception exc)
					{
						TaskDialog.MessageBox(Resources.DRAG_CFG_GENERAL_ERROR_TITLE, Resources.DRAG_CFG_GENERAL_ERROR_MAININSTRUCTION,
							exc.Message, exc.ToString(), string.Empty, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error, TaskDialogIcons.Error);
						return false;
					}

					if (successfulImportedConnections <= 0)
						TaskDialog.MessageBox(Resources.DRAG_CFG_SUCCESS_TITLE, Resources.DRAG_CFG_SUCCESS_MAIN_NOTHING_IMPORTED, 
							Resources.DRAG_CFG_SUCCESS_MAIN_NOTHING_IMPORTED_DETAIL, TaskDialogButtons.OK, TaskDialogIcons.Information);
					else if (successfulImportedConnections == 1)
						TaskDialog.MessageBox(Resources.DRAG_CFG_SUCCESS_TITLE, Resources.DRAG_CFG_SUCCESS_MAIN, Resources.DRAG_CFG_SUCCESS_CONTENT_SING,
							TaskDialogButtons.OK, TaskDialogIcons.Information);
					else
						TaskDialog.MessageBox(Resources.DRAG_CFG_SUCCESS_TITLE, Resources.DRAG_CFG_SUCCESS_MAIN, string.Format(Resources.DRAG_CFG_SUCCESS_CONTENT_PLUR,
							successfulImportedConnections), TaskDialogButtons.OK, TaskDialogIcons.Information);

					return (successfulImportedConnections > 0);
				//Cancel
				case 1:
				default:
					return false;
			}
		}

		# region Search

		/// <summary>
		/// Updates the search parameter.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <remarks>Documented by Dev08, 2009-03-05</remarks>
		private void UpdateSearchAndRefreshList()
		{
			if (lmIndex == null)
				return;

			SearchParameter param = new SearchParameter(textBoxSearchBox.Text);
			param.SelectedConnectionString = GetSelectedConnectionString();
			param.UncPath = GetSelectedSubPathFromUNC();
			param.ShowLearningModulesOfSubFolder = learningModulesTreeViewControl.ShowLearningModulesOfSubFolder;
			param.AreAllConnectionSelected = selectedTreeViewNode == null ? false : selectedTreeViewNode.IsMainNode;

			RefreshListView(param, false);
			ResetStatisticsAndPreview();
		}

		private void RefreshListView(SearchParameter searchParameter, bool preserveSelection)
		{
			List<ListViewItem> oldSelection = new List<ListViewItem>();
			foreach (ListViewItem item in listViewLearningModules.SelectedItems)
				oldSelection.Add(item);

			List<ListViewItem> oldFeedSelection = new List<ListViewItem>();
			foreach (ListViewItem item in listViewFeed.SelectedItems)
				oldFeedSelection.Add(item);

			if (filterThread != null && filterThread.ThreadState == System.Threading.ThreadState.Running)
				filterThread.Abort();

			filterThread = new Thread(new ThreadStart((MethodInvoker)delegate
			{
				if (this.ParentForm.Visible)
				{
					try
					{
						List<IIndexEntry> items = FilterList(actualItems, searchParameter);

						if (listViewLearningModules.InvokeRequired)
							listViewLearningModules.Invoke((MethodInvoker)delegate { LoadLearningModulesList(items, false); });
						else
							LoadLearningModulesList(items, false);

						if (preserveSelection)
							listViewLearningModules.Invoke((MethodInvoker)delegate { oldSelection.ForEach(i => i.Selected = true); });

					}
					catch (ThreadAbortException tae) { Trace.WriteLine(tae.Message); }
					catch (Exception ex) { Trace.WriteLine(ex.Message); }

					try
					{
						List<ListViewItem> items = FilterFeedList(actualModules, searchParameter);

						if (listViewFeed.InvokeRequired)
							listViewFeed.Invoke((MethodInvoker)delegate { LoadFeedList(items, false); });
						else
							LoadFeedList(items, false);

						if (preserveSelection)
							listViewFeed.Invoke((MethodInvoker)delegate { oldFeedSelection.ForEach(i => i.Selected = true); });

					}
					catch (ThreadAbortException tae) { Trace.WriteLine(tae.Message); }
					catch (Exception ex) { Trace.WriteLine(ex.Message); }
				}
			}));
			filterThread.Name = "LMPage Filter Thread";
			filterThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
			filterThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			filterThread.IsBackground = true;
			filterThread.Start();
		}

		private List<IIndexEntry> FilterList(List<IIndexEntry> items, SearchParameter searchParameter)
		{
			List<IIndexEntry> textFilteredItems = new List<IIndexEntry>();
			if (searchParameter.FilterWords.Trim() == string.Empty)
				textFilteredItems.AddRange(items);
			else
			{
				string[] filterWords = searchParameter.FilterWords.ToLower().Split(' ');

				foreach (IIndexEntry entry in items)
				{
					if (entry is FolderIndexEntry)
					{
						textFilteredItems.Add(entry);
						continue;
					}

					bool wordLocatedInEntry = true;
					foreach (string word in filterWords)
					{
						if (word == string.Empty)
							continue;

						if (!(entry as LearningModulesIndexEntry).Contains(word))
						{
							wordLocatedInEntry = false;
							break;
						}
					}
					if (wordLocatedInEntry)
						textFilteredItems.Add(entry);
				}
			}

			return textFilteredItems;
		}
		private List<ListViewItem> FilterFeedList(List<ListViewItem> items, SearchParameter searchParameter)
		{
			List<ListViewItem> textFilteredItems = new List<ListViewItem>();
			List<ListViewItem> textFilteredItemsCategoryResults = new List<ListViewItem>();

			if (searchParameter.FilterWords.Trim() == string.Empty)
				textFilteredItems.AddRange(items);
			else
			{
				string[] filterWords = searchParameter.FilterWords.ToLower().Split(' ');

				foreach (ListViewItem entry in items)
				{
					bool wordLocatedInEntry = true;
					bool wordLocatedInCategory = true;

					foreach (string word in filterWords)
					{
						if (word == string.Empty)
							continue;

						if (!(entry.SubItems[0].Text.ToLower().Contains(word) || entry.SubItems[1].Text.ToLower().Contains(word) ||
							entry.SubItems[2].Text.ToLower().Contains(word) || entry.SubItems[3].Text.ToLower().Contains(word) ||
							entry.SubItems[4].Text.ToLower().Contains(word)))
						{
							wordLocatedInEntry = false;
						}

						if (!(entry.SubItems[2].Text.ToLower().Contains(word) ||
							((ModuleInfo)entry.Tag).Categories.Exists(c => FindInParentCategories(Convert.ToInt32(c), word))))
							wordLocatedInCategory = false;
					}

					if (wordLocatedInEntry && !wordLocatedInCategory)
						textFilteredItems.Add(entry);
					else if (wordLocatedInCategory)
						textFilteredItemsCategoryResults.Add(entry);
				}
			}

			foreach (ListViewItem item in textFilteredItems)
				textFilteredItemsCategoryResults.Add(item);
			return textFilteredItemsCategoryResults;
		}
		private delegate TreeNode TreeNodeDelegate();
		private bool FindInParentCategories(int category, string word)
		{
			TreeNode node;
			if (learningModulesTreeViewControl.InvokeRequired)
				node = learningModulesTreeViewControl.Invoke((TreeNodeDelegate)delegate { return learningModulesTreeViewControl.SelectedNode; }) as TreeNode;
			else
				node = learningModulesTreeViewControl.SelectedNode;
			while (!(node is FeedTreeNode))
				node = node.Parent;
			List<ModuleCategory> cats = (node as FeedTreeNode).Categories;

			int currentCat = category;
			while (cats.Find(c => c.Id == currentCat).ParentCategory > 0)
			{
				ModuleCategory cat = cats.Find(c => c.Id == currentCat);
				currentCat = cat.ParentCategory;
				cat = cats.Find(c => c.Id == currentCat);

				if (cat.Title.ToLower().Contains(word))
					return true;
			}

			return false;
		}

		# endregion
		#region Send To...

		LoadStatusMessage loadStatusMessageImport = new LoadStatusMessage(Properties.Resources.COPYTO_STATUS_MESSAGE, 100, false);
		/// <summary>
		/// Shows the status message and deactivates the form.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-29</remarks>
		private void ShowStatusMessage(bool? importing = null, string message = null)
		{
			ParentForm.Enabled = false;
			loadStatusMessageImport.InfoMessage = importing.HasValue ? (importing.Value ? Properties.Resources.IMPORTING : Properties.Resources.EXPORTING) : message;
			loadStatusMessageImport.Show();
		}
		/// <summary>
		/// Updates the status message dialog with the current progress/infomessage.
		/// </summary>
		/// <param name="statusMessage">The status message.</param>
		/// <param name="currentPercentage">The current percentage.</param>
		/// <remarks>Documented by Dev02, 2008-09-29</remarks>
		private void UpdateStatusMessage(string statusMessage, double currentPercentage)
		{
			this.Invoke((MethodInvoker)delegate
			{
				loadStatusMessageImport.InfoMessage = statusMessage;
				if (currentPercentage <= 100 && currentPercentage >= 0)
				{
					loadStatusMessageImport.EnableProgressbar = true;
					loadStatusMessageImport.SetProgress(Convert.ToInt32(currentPercentage));
				}
				else
				{
					loadStatusMessageImport.EnableProgressbar = false;
					Application.DoEvents();
				}
			});
		}
		/// <summary>
		/// Hides the status message and activates the form.
		/// </summary>
		/// <remarks>Documented by Dev02, 2008-09-29</remarks>
		private void HideStatusMessage()
		{
			LearnLogic.CopyToFinished -= new EventHandler(LearnLogic_CopyToFinished);
			ParentForm.Enabled = true;
			loadStatusMessageImport.Hide();
		}

		private void DataAccessError(object sender, Exception exp)
		{
			throw exp;
		}
		#endregion
		# region Sync
		private static SyncStatusReportingDelegate syncStatusDelegate;

		/// <summary>
		/// Syncs the specified module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <param name="reportingDelegate">The reporting delegate.</param>
		/// <remarks>Documented by Dev08, 2009-04-30</remarks>
		public static void Sync(LearningModulesIndexEntry module, SyncStatusReportingDelegate reportingDelegate)
		{
			Sync(module, reportingDelegate, SyncedLearningModulePath);
		}

		/// <summary>
		/// Syncs the specified module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <param name="reportingDelegate">The reporting delegate.</param>
		/// <param name="syncedLearningModulePath">The synced learning module path (necessary when Startpage isn't initialized).</param>
		/// <remarks>Documented by Dev03, 2009-02-12</remarks>
		public static void Sync(LearningModulesIndexEntry module, SyncStatusReportingDelegate reportingDelegate, string syncedLearningModulePath)
		{
			if (module.Connection == null) //no user is required for syncing
				throw new SynchronizationFailedException(new ServerOfflineException());

			try
			{
				string serverUri = module.Connection is WebConnectionStringBuilder ? (module.Connection as WebConnectionStringBuilder).SyncURI : (module.Connection as PostgreSqlConnectionStringBuilder).SyncURI;
				bool createNew = (module.SyncedPath == string.Empty);
				string path = Tools.GetFullSyncPath(module.SyncedPath, syncedLearningModulePath, module.ConnectionName, module.UserName);

				Sync(serverUri, module.ConnectionString.LmId, module.UserId, ref path, module.ConnectionString.ProtectedLm, module.ConnectionString.Password, createNew, false, reportingDelegate);

				module.SyncedPath = Tools.GetSyncedPath(path, syncedLearningModulePath, module.ConnectionName, module.UserName);
			}
			catch (SynchronizationFailedException) { throw; }
			catch { throw new SynchronizationFailedException(new ServerOfflineException()); }
		}
		/// <summary>
		/// Syncs the specified server URI.
		/// </summary>
		/// <param name="serverUri">The server URI.</param>
		/// <param name="learningModuleId">The learning module id.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="path">The path.</param>
		/// <param name="createNew">if set to <c>true</c> [create new].</param>
		/// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
		/// <param name="reportingDelegate">The reporting delegate.</param>
		/// <remarks>Documented by Dev03, 2009-02-12</remarks>
		private static void Sync(string serverUri, int learningModuleId, int userId, ref string path, bool contentProtected, string password, bool createNew, bool overwrite, SyncStatusReportingDelegate reportingDelegate)
		{
			syncStatusDelegate = reportingDelegate;
			SyncAgent syncAgent = SyncClient.GetAgent(serverUri, learningModuleId, userId, ref path, contentProtected, password, createNew, overwrite);
			syncAgent.SessionProgress += new EventHandler<SessionProgressEventArgs>(syncAgent_SessionProgress);
			(syncAgent.LocalProvider as SqlCeClientSyncProvider).ApplyChangeFailed += new EventHandler<ApplyChangeFailedEventArgs>(syncAgent_ApplyChangeFailed);
			try
			{
				SyncStatistics stats = syncAgent.Synchronize();
				if (SyncClient.IsNewDb(syncAgent))
					SyncClient.ApplyIndicesToDatabase(path, contentProtected, password);

				string text = string.Empty;
				foreach (PropertyInfo info in stats.GetType().GetProperties())
					text += info.Name + ": " + info.GetValue(stats, null).ToString() + Environment.NewLine;
				Trace.WriteLine(text);

				//If IDENTITY Problem apears again --> uncomment this lines
				//syncStatusDelegate.Invoke(-1, Resources.SyncStateCheckingDataIntegrity);
				//SyncClient.VerifyDataBase(path, contentProtected, password);
				//syncStatusDelegate.Invoke(100, string.Empty);
			}
			catch (Exception ex)
			{
				syncStatusDelegate.Invoke(100, "Sync failed!");
				Trace.WriteLine("LearningModulesPage.Sync - " + ex.Message);

				DriveInfo info = new DriveInfo(path[0].ToString());
				if (info.TotalFreeSpace < 1000000)   //FreeSpace <1MB
					throw new NotEnoughtDiskSpaceException();

				throw new SynchronizationFailedException(ex);
			}
		}

		/// <summary>
		/// Downloads the content of the media table.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-03-31</remarks>
		private void DownloadMediaContent(LearningModulesIndexEntry entry, string syncedPath)
		{
			int counter = 1;

			SyncStatusUpdate(0, Resources.SYNC_DOWNLOAD_MEDIA);

			ConnectionStringStruct cssSync = entry.ConnectionString;
			cssSync.Typ = DatabaseType.MsSqlCe;
			cssSync.ConnectionString = syncedPath;
			cssSync.LmId = entry.Dictionary.Id;
			cssSync.SyncType = SyncType.FullSynchronized;
			cssSync.LearningModuleFolder = (entry.Connection as ISyncableConnectionString).MediaURI;
			cssSync.ServerUser = entry.User;

			IUser user = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return AuthenticationUsers[entry.Connection.ConnectionString]; },
				cssSync, DataAccessError, syncedPath);
			IDictionary dic = user.Open();

			List<int> mediaIds = dic.GetEmptyResources();

			WebClient webClient = new WebClient();
			foreach (int mediaId in mediaIds)
			{
				SyncStatusUpdate(counter * 100.0 / mediaIds.Count, string.Format(Resources.SYNC_DOWNLOAD_MEDIA_STEP, counter, mediaIds.Count));
				counter++;
				try
				{
					string uri = DbMediaServer.Instance(dic.Parent).GetMediaURI(mediaId).ToString();
					webClient.DownloadData(uri);
				}
				catch
				{
					Trace.WriteLine("Failed to get media!", "DownloadMediaContent");
				}
			}
			SyncStatusUpdate(100, string.Empty);
		}

		/// <summary>
		/// Downloads the extension files.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <param name="syncedPath">The synced path.</param>
		/// <remarks>Documented by Dev02, 2009-07-06</remarks>
		private void DownloadExtensionFiles(LearningModulesIndexEntry entry, string syncedPath)
		{
			SyncStatusUpdate(0, Resources.SYNC_DOWNLOAD_EXTENSIONS);

			ConnectionStringStruct cssSync = entry.ConnectionString;
			cssSync.Typ = DatabaseType.MsSqlCe;
			cssSync.ConnectionString = syncedPath;
			cssSync.LmId = entry.Dictionary.Id;
			cssSync.SyncType = SyncType.FullSynchronized;
			cssSync.ExtensionURI = (entry.Connection as ISyncableConnectionString).ExtensionURI;
			cssSync.ServerUser = entry.User;

			IUser user = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return AuthenticationUsers[entry.Connection.ConnectionString]; },
				cssSync, DataAccessError, syncedPath);
			IDictionary dic = user.Open();

			WebClient webClient = new WebClient();
			foreach (IExtension ext in dic.Extensions)
			{
				try
				{
					string uri = DbMediaServer.Instance(dic.Parent).GetExtensionURI(ext.Id).ToString();
					webClient.DownloadData(uri);
				}
				catch
				{
					Trace.WriteLine("Failed to get extension files!", "DownloadExtensionFiles");
				}
			}

			SyncStatusUpdate(100, string.Empty);
		}

		/// <summary>
		/// Handles the ApplyChangeFailed event of the syncAgent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="Microsoft.Synchronization.Data.ApplyChangeFailedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-02-12</remarks>
		private static void syncAgent_ApplyChangeFailed(object sender, ApplyChangeFailedEventArgs e)
		{
			Trace.WriteLine(e.Error.ToString());
		}
		/// <summary>
		/// Handles the SessionProgress event of the syncAgent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="Microsoft.Synchronization.SessionProgressEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-02-12</remarks>
		private static void syncAgent_SessionProgress(object sender, SessionProgressEventArgs e)
		{
			syncStatusDelegate.Invoke(e.PercentCompleted, Properties.Resources.ResourceManager.GetString("SyncState" + e.SyncStage.ToString()));
		}
		/// <summary>
		/// Syncs the status update.
		/// </summary>
		/// <param name="percentage">The percentage.</param>
		/// <param name="message">The message.</param>
		/// <remarks>Documented by Dev03, 2009-02-12</remarks>
		private void SyncStatusUpdate(double percentage, string message)
		{
			if (!loadStatusMessageImport.Visible)
				ShowStatusMessage(true);
			UpdateStatusMessage(message, percentage);
			if (percentage >= 100)
				HideStatusMessage();
		}
		# endregion
		#region Drag & Drop Implementation

		/// <summary>
		/// Activates the file drop timer.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev08, 2009-02-27</remarks>
		private void ActivateFileDropTimer(string filePath)
		{
			//fix for [ML-642] TaskDialog window not visible / File dropping blocks explorer process
			System.Windows.Forms.Timer FileDropTimer = new System.Windows.Forms.Timer();
			FileDropTimer.Interval = 50;
			FileDropTimer.Tick += new EventHandler(FileDropTimer_Tick);
			FileDropTimer.Tag = filePath;
			FileDropTimer.Start();
		}

		/// <summary>
		/// Handles the Tick event of the FileDropTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-02-27</remarks>
		private void FileDropTimer_Tick(object sender, EventArgs e)
		{
			System.Windows.Forms.Timer timer = sender as System.Windows.Forms.Timer;
			timer.Stop();
			if (timer.Tag != null && timer.Tag is string)
			{
				string file = (string)timer.Tag;
				if (Helper.CheckFileName(file))
				{
					if (Helper.IsLearningModuleFileName(file))
					{
						LearningModuleSelectedEventArgs args = new LearningModuleSelectedEventArgs(
							new LearningModulesIndexEntry(
								new ConnectionStringStruct(
									(Path.GetExtension(file) == DAL.Helper.OdxExtension ||
										Path.GetExtension(file) == DAL.Helper.DzpExtension ||
										Path.GetExtension(file) == DAL.Helper.OdfExtension) ? DatabaseType.Xml : DatabaseType.MsSqlCe, file, false)
								) { ConnectionName = Path.GetDirectoryName(file) }
							);
						selectedLearningModule = args.LearnModule;
						args.IsUsedDragAndDrop = true;      //[ML-2023] Inconsistent Drag and Drop behaviour

						if (LearningModuleSelected != null)
							LearningModuleSelected(this, args);
					}
					else
					{
						if (ImportConfigFile(file))
							LoadLearningModules();
					}
				}
			}
			timer.Dispose();
		}

		#endregion

		private void SetLeftBarView(LeftBarView view)
		{
			switch (view)
			{
				case LeftBarView.TreeView:
					tableLayoutPanelMain.Controls.Remove(taskPaneInformations);
					tableLayoutPanelMain.Controls.Add(learningModulesTreeViewControl, 0, 2);
					tableLayoutPanelMain.SetColumnSpan(learningModulesTreeViewControl, 2);
					tableLayoutPanelMain.SetRowSpan(learningModulesTreeViewControl, 1);

					learningModulesTreeViewControl.BorderStyle = BorderStyle.FixedSingle;

					break;

				case LeftBarView.XPExplorerBar:
					tableLayoutPanelMain.Controls.Remove(learningModulesTreeViewControl);
					tableLayoutPanelMain.Controls.Add(taskPaneInformations, 0, 1);
					tableLayoutPanelMain.SetColumnSpan(taskPaneInformations, 2);
					tableLayoutPanelMain.SetRowSpan(taskPaneInformations, 2);

					expandoTreeView.Items.Add(learningModulesTreeViewControl);
					learningModulesTreeViewControl.BorderStyle = BorderStyle.None;
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Gets the selected connection string.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-03-05</remarks>
		private IConnectionString GetSelectedConnectionString()
		{
			if (selectedTreeViewNode == null || selectedTreeViewNode.Folder == null)
				return null;

			return selectedTreeViewNode.Folder != null ? selectedTreeViewNode.Folder.Connection : null;
		}

		/// <summary>
		/// Gets the selected sub path from UNC.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-03-05</remarks>
		private string GetSelectedSubPathFromUNC()
		{
			if (selectedTreeViewNode == null && selectedTreeViewNode.Folder == null)
				return null;

			return selectedTreeViewNode.Folder != null ? selectedTreeViewNode.Folder.Path : string.Empty;
		}

		private static UserStruct? GetUser(UserStruct user, ConnectionStringStruct connection)
		{
			if (!MLifter.BusinessLayer.User.PreventAutoLogin && authenticationUsers.ContainsKey(connection.ConnectionString) && (!defaultUserSubmitted.ContainsKey(connection) || !defaultUserSubmitted[connection]))
			{
				defaultUserSubmitted[connection] = true;
				return authenticationUsers[connection.ConnectionString];
			}
			else
			{
				UserStruct? newUser = LoginForm.OpenLoginForm(user, connection);
				if (newUser.HasValue)
					authenticationUsers[connection.ConnectionString] = newUser.Value;
				return newUser;
			}
		}

		internal static IUser Login(IConnectionString connection)
		{
			IUser user = UserFactory.Create(LearningModulesPage.GetUser,
				new MLifter.DAL.Interfaces.ConnectionStringStruct(connection.ConnectionType, connection.ConnectionType == DatabaseType.Unc ? connection.ConnectionString : string.Empty, connection.ConnectionString, true), (DataAccessErrorDelegate)delegate(object s, Exception e) { return; }, connection);
			LearningModulesIndex.ConnectionUsers[connection] = user;

			return user;
		}

		/// <summary>
		/// Gets the folder for a connection.
		/// Centralized exception handling.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <returns>The folder. [null] if an exception occured.</returns>
		/// <remarks>Documented by Dev03, 2009-03-18</remarks>
		private FolderIndexEntry GetFolderOfConnection(IConnectionString connection)
		{
			try
			{
				return lmIndex.GetFolderOfConnection(connection);
			}
			catch (ServerOfflineException)
			{
				ShowServerOfflineMessage(new List<IConnectionString>() { connection }, false);
				return null;
			}
			catch (NoValidUserException)
			{
				return null;
			}
			catch (Exception ex)
			{
				TaskDialog.MessageBox("Exception handling missing!", ex.Message, ex.StackTrace, TaskDialogButtons.OK, TaskDialogIcons.Error);
				return null;
			}
		}

		#endregion

		#region OnEvent() Functions

		protected virtual void OnLearningModuleSelected(LearningModuleSelectedEventArgs e)
		{
			selectedLearningModule = e.LearnModule;

			if (LearningModuleSelected != null)
				LearningModuleSelected(this, e);
		}
		protected virtual void OnCancelPressed(EventArgs e)
		{
			selectedLearningModule = null;

			if (CancelPressed != null)
				CancelPressed(this, e);
		}
		protected virtual void OnNewLearningModulePressed()
		{
			if (NewLearningModulePressed != null)
				NewLearningModulePressed(this, EventArgs.Empty);
		}

		#endregion

		private void renameToolstripMenuItem_Click(object sender, EventArgs e)
		{
			listViewLearningModules.LabelEdit = true;
			listViewLearningModules.SelectedItems[0].BeginEdit();
		}

		private void listViewLearningModules_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			LearningModulesIndexEntry entry = (listViewLearningModules.SelectedItems[0] as LearningModuleListViewItem).LearningModule;
			string oldTitle = entry.Dictionary.Title;
			string newTitle = e.Label;

			// verify that the new name is valid and not the old one
			if (newTitle == null || newTitle == oldTitle)
			{
				e.CancelEdit = true;
				listViewLearningModules.LabelEdit = false;
			}
			else if (newTitle.Trim() != String.Empty)
			{
				listViewLearningModules.LabelEdit = false;
				entry.Dictionary.Title = newTitle;
				entry.DisplayName = newTitle;
			}
			else
			{
				e.CancelEdit = true;
				listViewLearningModules.Items[e.Item].BeginEdit();
			}

			/*if (entry.ConnectionString.Typ == DatabaseType.MsSqlCe)
			{
				 also rename the file if its a local database
				string newFilename = newTitle + ".mlm";
				string newPath = Path.Combine(Path.GetDirectoryName(entry.ConnectionString.ConnectionString), newFilename);
				int i = 1;
				while (File.Exists(newPath))    // get next available filename
				{
					newFilename = newTitle + i + ".mlm";
					newPath = Path.Combine(Path.GetDirectoryName(entry.ConnectionString.ConnectionString), newFilename);
				}

				string oldPath = entry.ConnectionString.ConnectionString;
				ConnectionStringStruct css = entry.ConnectionString;
				css.ConnectionString = newPath;
				entry.ConnectionString = css;

				entry.Dictionary.Dispose();
				File.Move(oldPath, newPath);
			}*/
		}

		private void listViewFeed_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateDetails();
		}

		private void listViewFeed_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left || listViewFeed.SelectedItems.Count != 1)
				return;

			if (listViewFeed.SelectedItems[0].Tag is ModuleInfo)
				buttonOK.PerformClick();
			else if (listViewFeed.SelectedItems[0].Tag is FeedCategoryTreeNode)
				learningModulesTreeViewControl.SelectedNode = listViewFeed.SelectedItems[0].Tag as FeedCategoryTreeNode;
		}

		/// <summary>
		/// Handles the Click event of the helpButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev09, 2010-01-22</remarks>
		private void helpButton_Click(object sender, EventArgs e)
		{
			Help.ShowHelp(this, (Parent as LearningModulesForm).MainHelpObject.HelpNamespace, HelpNavigator.Topic, "/html/Getting_familiar_with_the_My_Learning_Modules_dialog_box.htm");
		}
	}
}
