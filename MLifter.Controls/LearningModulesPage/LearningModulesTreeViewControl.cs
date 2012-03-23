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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;
using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Components;
using System.Diagnostics;
using MLifter.Generics;
using System.Collections.ObjectModel;

namespace MLifter.Controls.LearningModulesPage
{
	public partial class LearningModulesTreeViewControl : UserControl
	{
		/// <summary>
		/// Occurs when [tree view selection changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		public event TreeViewEventHandler TreeViewSelectionChanged;
		/// <summary>
		/// Occurs when [show learning modules of sub folder state changed].
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		public event EventHandler ShowLearningModulesOfSubFolderStateChanged;
		/// <summary>
		/// Occurs when [import learning module] is requested.
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		public event ImportLearningModuleEventHandler OnImportLearningModule;
		/// <summary>
		/// Occurs when [import config file] is requested.
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		public event ImportConfigFileEventHandler OnImportConfigFile;
		/// <summary>
		/// Occurs when [open learning module] is requested.
		/// </summary>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		public event LearningModuleSelectedEventHandler OnOpenLearningModule;
		private List<IConnectionString> connectionStringList = new List<IConnectionString>();

		/// <summary>
		/// Initializes a new instance of the <see cref="LearningModulesTreeViewControl"/> class.
		/// </summary>
		/// <remarks>Documented by Dev07, 2009-03-05</remarks>
		public LearningModulesTreeViewControl()
		{
			InitializeComponent();
			treeViewLearnModules.ContentLoadException += new FolderIndexEntry.ContentLoadExceptionEventHandler(treeViewLearnModules_ContentLoadException);
			treeViewLearnModules.Nodes[0].Text = Resources.LEARNMODULES_MAINNODE;
		}

		private List<FolderIndexEntry> errorFolders = new List<FolderIndexEntry>();
		/// <summary>
		/// Trees the view learn modules_ content load exception.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="exp">The exp.</param>
		/// <remarks>Documented by Dev02, 2009-05-14</remarks>
		private void treeViewLearnModules_ContentLoadException(object sender, Exception exp)
		{
			if (exp is IOException || exp is UnauthorizedAccessException)
			{
				FolderIndexEntry entry = sender as FolderIndexEntry;
				if (!errorFolders.Contains(entry))
				{
					errorFolders.Add(entry);
					TaskDialog.MessageBox(Resources.LEARNING_MODULES_PAGE_FOLDERFAILED_TITLE,
						string.Format(Resources.LEARNING_MODULES_PAGE_FOLDERFAILED_MAIN, (sender as FolderIndexEntry).Path),
						Resources.LEARNING_MODULES_PAGE_FOLDERFAILED_CONTENT, TaskDialogButtons.OK, TaskDialogIcons.Warning);
				}
			}
		}

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether [show lines].
		/// </summary>
		/// <value><c>true</c> if [show lines]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev07, 2009-03-04</remarks>
		public bool ShowLines
		{
			get
			{
				return treeViewLearnModules.ShowLines;
			}
			set
			{
				treeViewLearnModules.ShowLines = value;
			}
		}

		/// <summary>
		/// Gets or sets the tree view border style.
		/// </summary>
		/// <value>The tree view border style.</value>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		public BorderStyle TreeViewBorderStyle
		{
			get
			{
				return treeViewLearnModules.BorderStyle;
			}
			set
			{
				treeViewLearnModules.BorderStyle = value;
			}
		}

		/// <summary>
		/// Gets or sets a value if the the ListView is like Explorer or like a filter.
		/// </summary>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		public bool ShowLearningModulesOfSubFolder
		{
			get
			{
				return checkBoxShowLearningModulesOfSubFolder.Checked;
			}
			set
			{
				checkBoxShowLearningModulesOfSubFolder.Checked = value;
			}
		}

		/// <summary>
		/// Gets the selected node.
		/// </summary>
		/// <value>The selected node.</value>
		/// <remarks>Documented by Dev05, 2009-06-29</remarks>
		public TreeNode SelectedNode { get { return treeViewLearnModules.SelectedNode; } set { treeViewLearnModules.SelectedNode = value; } }

		#endregion

		/// <summary>
		/// Sets the connection string list. (This has to be a Function, because the designer has problems with a property like List[IConnectionStrings]
		/// </summary>
		/// <param name="conStrings">The connection strings.</param>
		/// <remarks>Documented by Dev08, 2009-03-04</remarks>
		public void SetConnectionStringList(List<IConnectionString> conStrings)
		{
			connectionStringList = conStrings;
			treeViewLearnModules.ReplaceConnections(conStrings);
		}

		/// <summary>
		/// Sets the feed list.
		/// </summary>
		/// <param name="feeds">The feeds.</param>
		/// <remarks>CFI, 2012-03-03</remarks>
		public void SetFeedList(ObservableCollection<ModuleFeedConnection> feeds)
		{
			treeViewLearnModules.SetFeeds(feeds);
		}

		/// <summary>
		/// Folder was added.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-03-13</remarks>
		public void AddFolder(FolderIndexEntry entry)
		{
			treeViewLearnModules.Folders.Add(entry);
		}

		/// <summary>
		/// Removes the folder.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-04-01</remarks>
		public void RemoveFolder(FolderIndexEntry entry)
		{
			treeViewLearnModules.Folders.Remove(entry);
		}

		/// <summary>
		/// Removes the connections from drive.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>Documented by Dev05, 2009-04-01</remarks>
		public void RemoveConnectionsFromDrive(DriveInfo drive)
		{
			//treeViewLearnModules.Folders.RemoveAll(f => f.Connection.ConnectionString.StartsWith(drive.RootDirectory.FullName));
			treeViewLearnModules.Connections.RemoveAll(c => c.ConnectionString.StartsWith(drive.RootDirectory.FullName));
		}

		/// <summary>
		/// Goes one level up.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public void OneLevelUp()
		{
			if (treeViewLearnModules.SelectedNode.Parent != null)
				treeViewLearnModules.SelectedNode = treeViewLearnModules.SelectedNode.Parent;
		}

		/// <summary>
		/// Sets and expands the selected folder.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev08, 2009-03-09</remarks>
		public void SetSelectedFolder(FolderIndexEntry entry)
		{
			treeViewLearnModules.SelectedFolder = entry;
		}

		/// <summary>
		/// Selects the default connection node.
		/// </summary>
		/// <remarks>Documented by Dev07, 2009-03-05</remarks>
		public void SelectDefaultConnectionNode()
		{
			try
			{
				treeViewLearnModules.SelectedNode = treeViewLearnModules.Nodes.Find(n => n.Folder != null && n.Folder.Connection.IsDefault && n.Folder.Parent == null);
			}
			catch
			{
				treeViewLearnModules.SelectedNode = treeViewLearnModules.Nodes[0];
			}
		}

		/// <summary>
		/// Saves the checked state of the subfolder checkbox while it is disabled.
		/// </summary>
		private bool wasCheckboxSubfolderChecked;

		/// <summary>
		/// Handles the AfterSelect event of the treeViewLearnModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-03-05</remarks>
		private void treeViewLearnModules_AfterSelect(object sender, TreeViewEventArgs e)
		{
			OnTreeViewSelectionChanged(e);

			try
			{
				if (e.Node is FeedTreeNode || e.Node is FeedCategoryTreeNode)
				{
					checkBoxShowLearningModulesOfSubFolder.Visible = true;
					linkLabelLogout.Visible = false;

					if (checkBoxShowLearningModulesOfSubFolder.Enabled)
					{
						wasCheckboxSubfolderChecked = checkBoxShowLearningModulesOfSubFolder.Checked;
						checkBoxShowLearningModulesOfSubFolder.Checked = true;
						checkBoxShowLearningModulesOfSubFolder.Enabled = false;
					}
				}
				else
				{
					if (!checkBoxShowLearningModulesOfSubFolder.Enabled)
					{
						checkBoxShowLearningModulesOfSubFolder.Checked = wasCheckboxSubfolderChecked;
						checkBoxShowLearningModulesOfSubFolder.Enabled = true;
					}

					if (e.Node.Parent == null)
					{
						checkBoxShowLearningModulesOfSubFolder.Visible = false;
						linkLabelLogout.Visible = false;
					}
					else if ((e.Node as FolderTreeNode).Folder.Connection is UncConnectionStringBuilder)
					{
						linkLabelLogout.Visible = false;
						checkBoxShowLearningModulesOfSubFolder.Visible = true;
					}
					else
					{
						checkBoxShowLearningModulesOfSubFolder.Visible = false;
						linkLabelLogout.Visible = LearningModulesIndex.ConnectionUsers[(treeViewLearnModules.SelectedNode as FolderTreeNode).Connection].AuthenticationStruct.AuthenticationType != UserAuthenticationTyp.LocalDirectoryAuthentication;
					}
				}
			}
			catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
		}

		/// <summary>
		/// Raises the <see cref="E:TreeViewSelectionChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.TreeViewEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev07, 2009-03-05</remarks>
		protected virtual void OnTreeViewSelectionChanged(TreeViewEventArgs e)
		{
			if (TreeViewSelectionChanged != null)
				TreeViewSelectionChanged(this, e);
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxShowLearningModulesOfSubFolder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-05</remarks>
		private void checkBoxShowLearningModulesOfSubFolder_CheckedChanged(object sender, EventArgs e)
		{
			OnCheckBoxShowLearningModulesOfSubFolderChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="E:CheckBoxShowLearningModulesOfSubFolderChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-03-05</remarks>
		protected virtual void OnCheckBoxShowLearningModulesOfSubFolderChanged(EventArgs e)
		{
			if (TreeViewSelectionChanged != null)
				ShowLearningModulesOfSubFolderStateChanged(this, e);
		}

		/// <summary>
		/// Handles the DragEnter event of the treeViewLearnModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		private void treeViewLearnModules_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (MLifter.DAL.Helper.CheckFileName(((string[])e.Data.GetData(DataFormats.FileDrop))[0]))
					e.Effect = DragDropEffects.Link;
				else
					e.Effect = DragDropEffects.None;
			}
		}

		/// <summary>
		/// Handles the DragDrop event of the treeViewLearnModules control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		private void treeViewLearnModules_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
				FileDropData data = new FileDropData(file, treeViewLearnModules.PointToClient(new Point(e.X, e.Y)));
				ActivateFileDropTimer(data);
			}
		}

		/// <summary>
		/// Activates the file drop timer.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <remarks>Documented by Dev03, 2009-03-11</remarks>
		private void ActivateFileDropTimer(FileDropData data)
		{
			//fix for [ML-642] TaskDialog window not visible / File dropping blocks explorer process
			System.Windows.Forms.Timer FileDropTimer = new System.Windows.Forms.Timer();
			FileDropTimer.Interval = 50;
			FileDropTimer.Tick += new EventHandler(FileDropTimer_Tick);
			FileDropTimer.Tag = data;
			FileDropTimer.Start();
		}

		/// <summary>
		/// Handles the Tick event of the FileDropTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2009-02-27</remarks>
		void FileDropTimer_Tick(object sender, EventArgs e)
		{
			System.Windows.Forms.Timer timer = sender as System.Windows.Forms.Timer;
			timer.Stop();
			if (timer.Tag != null && timer.Tag is FileDropData)
			{
				FileDropData data = (FileDropData)timer.Tag;
				if (MLifter.DAL.Helper.IsLearningModuleFileName(data.File))
				{
					FolderTreeNode dstNode = treeViewLearnModules.GetNodeAt(data.Coordinates) as FolderTreeNode;
					if (dstNode == null)
						OpenLearningModule(data.File);
					else
						ImportLearningModule(data.File, dstNode);
				}
				else
					OpenConfigFile(data.File);
			}
			timer.Dispose();
		}

		/// <summary>
		/// Opens the learning module.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		private void OpenLearningModule(string file)
		{
			if (OnOpenLearningModule != null)
			{
				OnOpenLearningModule(treeViewLearnModules,
					new LearningModuleSelectedEventArgs(
						new LearningModulesIndexEntry(
							new ConnectionStringStruct(
								(Path.GetExtension(file) == DAL.Helper.OdxExtension ||
									Path.GetExtension(file) == DAL.Helper.DzpExtension ||
									Path.GetExtension(file) == DAL.Helper.OdfExtension)
									? DatabaseType.Xml : DatabaseType.MsSqlCe, file, false)
					)));
			}
		}

		/// <summary>
		/// Imports the learning module.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <param name="targetEntry">The target entry.</param>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		private void ImportLearningModule(string file, FolderTreeNode targetEntry)
		{
			if (targetEntry.Level == 0)
				OpenLearningModule(file);
			else
				if (OnImportLearningModule != null)
					OnImportLearningModule(treeViewLearnModules, new ImportLearningModuleEventArgs(targetEntry,
						new LearningModulesIndexEntry(
								new ConnectionStringStruct(
									(Path.GetExtension(file) == DAL.Helper.OdxExtension ||
										Path.GetExtension(file) == DAL.Helper.DzpExtension ||
										Path.GetExtension(file) == DAL.Helper.OdfExtension)
										? DatabaseType.Xml : DatabaseType.MsSqlCe, file, false)
						)));
		}

		/// <summary>
		/// Opens the config file.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <remarks>Documented by Dev03, 2009-03-10</remarks>
		private void OpenConfigFile(string file)
		{
			if (OnImportConfigFile != null)
				OnImportConfigFile(treeViewLearnModules, new ImportConfigFileEventArgs(file));
		}

		/// <summary>
		/// Handles the LinkClicked event of the linkLabelLogout control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-27</remarks>
		private void linkLabelLogout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (!(treeViewLearnModules.SelectedFolder.Connection is UncConnectionStringBuilder))
			{
				treeViewLearnModules.SelectedFolder.LoginNewUser();
				OnTreeViewSelectionChanged(new TreeViewEventArgs(treeViewLearnModules.SelectedNode, TreeViewAction.Unknown));
			}
		}
	}

	/// <summary>
	/// Delegate used to define the import learning module event of the <see cref="LearningModulesTreeViewControl"/> class.
	/// </summary>
	public delegate void ImportLearningModuleEventHandler(object sender, ImportLearningModuleEventArgs e);
	/// <summary>
	/// Event args for the <see cref="ImportLearningModuleEventHandler"/> event.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-03-10</remarks>
	public class ImportLearningModuleEventArgs : EventArgs
	{
		private FolderTreeNode treeNode;
		private LearningModulesIndexEntry learningModule;
		/// <summary>
		/// Gets or sets the tree node to which the learning module should be imported.
		/// </summary>
		/// <value>The the tree node.</value>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public FolderTreeNode TreeNode
		{
			get { return treeNode; }
			set { treeNode = value; }
		}

		/// <summary>
		/// Gets or sets the learning module.
		/// </summary>
		/// <value>The learning module.</value>
		/// <remarks>Documented by Dev03, 2009-03-11</remarks>
		public LearningModulesIndexEntry LearningModule
		{
			get { return learningModule; }
			set { learningModule = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportLearningModuleEventArgs"/> class.
		/// </summary>
		/// <param name="treeNode">The the tree node to which the learning module should be imported.</param>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public ImportLearningModuleEventArgs(FolderTreeNode treeNode, LearningModulesIndexEntry learningModule)
		{
			this.treeNode = treeNode;
			this.learningModule = learningModule;
		}
	}

	/// <summary>
	/// Delegate used to define the import config file event of the <see cref="LearningModulesTreeViewControl"/> class.
	/// </summary>
	public delegate void ImportConfigFileEventHandler(object sender, ImportConfigFileEventArgs e);
	/// <summary>
	/// Event args for the <see cref="ImportConfigFileEventHandler"/> event.
	/// </summary>
	/// <remarks>Documented by Dev03, 2009-03-10</remarks>
	public class ImportConfigFileEventArgs : EventArgs
	{
		private string configFile;
		/// <summary>
		/// Gets or sets the config file that should be imported.
		/// </summary>
		/// <value>The config file.</value>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public string ConfigFile
		{
			get { return configFile; }
			set { configFile = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportConfigFileEventArgs"/> class.
		/// </summary>
		/// <param name="configFile">The config file.</param>
		/// <remarks>Documented by Dev05, 2008-12-03</remarks>
		public ImportConfigFileEventArgs(string configFile)
		{
			this.configFile = configFile;
		}
	}

	internal struct FileDropData
	{
		public FileDropData(string filePath, Point coordinates)
		{
			File = filePath;
			Coordinates = coordinates;
		}
		public string File;
		public Point Coordinates;
	}

	public struct TreeNodeData
	{
		public IConnectionString ConnectionString;
		public string FullUncPath;
		public bool IsMainNodeSelected;

		public TreeNodeData(bool isMainNodeSelected)
		{
			IsMainNodeSelected = isMainNodeSelected;
			FullUncPath = string.Empty;
			ConnectionString = null;
		}

		public TreeNodeData(IConnectionString connectionString)
			: this(connectionString, string.Empty)
		{
		}

		public TreeNodeData(IConnectionString connectionString, string uncPath)
		{
			IsMainNodeSelected = false;
			ConnectionString = connectionString;
			FullUncPath = uncPath;
		}
	}
}
