using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.Components.Properties;
using MLifter.Generics;
using MRG.Controls.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Xml.Serialization;
using System.Net;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MLifter.Components
{
	public partial class FolderTreeView : TreeView
	{
		/// <summary>
		/// Occurs when content loading throws an exception.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-05-14</remarks>
		public event MLifter.BusinessLayer.FolderIndexEntry.ContentLoadExceptionEventHandler ContentLoadException;
		/// <summary>
		/// Gets or sets the folders.
		/// </summary>
		/// <value>The folders.</value>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public ObservableList<FolderIndexEntry> Folders { get; private set; }
		/// <summary>
		/// Gets the connections.
		/// </summary>
		/// <value>The connections.</value>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public ObservableList<IConnectionString> Connections { get; private set; }
		/// <summary>
		/// Gets the feeds.
		/// </summary>
		/// <remarks>CFI, 2012-03-03</remarks>
		public ObservableCollection<ModuleFeedConnection> Feeds { get; private set; }

		/// <summary>
		/// Gets the collection of tree nodes that are assigned to the tree view control.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A <see cref="T:System.Windows.Forms.TreeNodeCollection"/> that represents the tree nodes assigned to the tree view control.
		/// </returns>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public new List<FolderTreeNode> Nodes { get; private set; }
		/// <summary>
		/// Gets or sets the tree node that is currently selected in the tree view control.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The <see cref="T:System.Windows.Forms.TreeNode"/> that is currently selected in the tree view control.
		/// </returns>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
		/// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// </PermissionSet>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		[Browsable(false)]
		public FolderIndexEntry SelectedFolder
		{
			get
			{
				try
				{
					if (SelectedNode is FolderTreeNode)
						return (SelectedNode as FolderTreeNode).Folder;
					else
						return null;
				}
				catch { return null; }
			}
			set
			{
				try
				{
					SelectedNode = Nodes.Find(n => n.Folder == value);
				}
				catch { }
			}
		}
		/// <summary>
		/// Gets or sets the tree node that is currently selected in the tree view control.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The <see cref="T:System.Windows.Forms.TreeNode"/> that is currently selected in the tree view control.
		/// </returns>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
		/// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// </PermissionSet>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public new TreeNode SelectedNode { get { return base.SelectedNode; } set { base.SelectedNode = value; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="FolderTreeView"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public FolderTreeView()
			: base()
		{
			this.DoubleBuffered = true;

			InitializeComponent();

			updateTimer.Interval = 1000;
			updateTimer.Tick += new EventHandler(updateTimer_Tick);

			this.ImageList = new ImageList();
			this.ImageList.Images.Add(Resources.learning_16_gif);
			this.ImageList.Images.Add(Resources.folder_gif);
			this.ImageList.Images.Add(Resources.folder_saved_search_gif);
			this.ImageList.Images.Add(Resources.network_gif);
			this.ImageList.Images.Add(Resources.network_offline_gif);
			this.ImageList.Images.Add(Resources.system_search_gif);
			this.ImageList.Images.Add(Resources.usbDrive);
			this.ImageList.Images.Add(Resources.ML);
			InitTreeView();
		}

		private FolderTreeNode mainNode = new FolderTreeNode();
		private void InitTreeView()
		{
			base.Nodes.Add(mainNode);
			mainNode.ContentLoadException += new FolderIndexEntry.ContentLoadExceptionEventHandler(mainNode_ContentLoadException);
			mainNode.Expand();
			mainNode.UpdateDetails();

			Nodes = new List<FolderTreeNode>();
			Nodes.Add(mainNode);
			Folders = new ObservableList<FolderIndexEntry>();
			Folders.ListChanged += new EventHandler<ObservableListChangedEventArgs<FolderIndexEntry>>(Folders_ListChanged);
			Connections = new ObservableList<IConnectionString>();
			Connections.ListChanged += new EventHandler<ObservableListChangedEventArgs<IConnectionString>>(Connections_ListChanged);
		}

		/// <summary>
		/// Sets the feeds.
		/// </summary>
		/// <param name="feeds">The feeds.</param>
		/// <remarks>CFI, 2012-03-03</remarks>
		public void SetFeeds(ObservableCollection<ModuleFeedConnection> feeds)
		{
			List<TreeNode> nodesToRemove = new List<TreeNode>();
			foreach (TreeNode node in base.Nodes)
				if (node is FeedTreeNode)
					nodesToRemove.Add(node);
			nodesToRemove.ForEach(n => base.Nodes.Remove(n));

			if(Feeds != null)
				Feeds.CollectionChanged -= feeds_CollectionChanged;
			Feeds = feeds;
			Feeds.CollectionChanged += new NotifyCollectionChangedEventHandler(feeds_CollectionChanged);

			foreach (ModuleFeedConnection feed in Feeds)
				base.Nodes.Add(new FeedTreeNode(feed));
		}

		/// <summary>
		/// Handles the CollectionChanged event of the feeds control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-03-03</remarks>
		protected void feeds_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (ModuleFeedConnection item in e.NewItems)
						base.Nodes.Add(new FeedTreeNode(item));
					break;
				case NotifyCollectionChangedAction.Remove:
					break;
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Reset:
					SetFeeds(sender as ObservableCollection<ModuleFeedConnection>);
					break;
				case NotifyCollectionChangedAction.Move:
				default:
					break;
			}
		}

		/// <summary>
		/// Mains the node_ content load exception.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="exp">The exp.</param>
		/// <remarks>Documented by Dev02, 2009-05-14</remarks>
		private void mainNode_ContentLoadException(object sender, Exception exp)
		{
			if (ContentLoadException != null)
				ContentLoadException(sender, exp);
		}

		/// <summary>
		/// Replaces the connections and rebuilds the tree.
		/// </summary>
		/// <param name="connections">The connections.</param>
		/// <remarks>Documented by Dev03, 2009-03-20</remarks>
		public void ReplaceConnections(List<IConnectionString> connections)
		{
			if (Connections.Count == 0)
				Connections.AddRange(connections);
			else
			{
				Folders.ListChanged -= new EventHandler<ObservableListChangedEventArgs<FolderIndexEntry>>(Folders_ListChanged);
				Connections.ListChanged -= new EventHandler<ObservableListChangedEventArgs<IConnectionString>>(Connections_ListChanged);
				mainNode.Nodes.Clear();
				Folders.Clear();
				Connections.Clear();
				Folders.ListChanged += new EventHandler<ObservableListChangedEventArgs<FolderIndexEntry>>(Folders_ListChanged);
				Connections.ListChanged += new EventHandler<ObservableListChangedEventArgs<IConnectionString>>(Connections_ListChanged);
				Connections.AddRange(connections);
			}
		}

		/// <summary>
		/// Handles the Tick event of the updateTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void updateTimer_Tick(object sender, EventArgs e)
		{
			BeginUpdate();
			try
			{
				lock (Nodes)
				{
					if (!Nodes.Exists(n => n.IsLoading))
						updateTimer.Enabled = false;
					Nodes.ForEach(n => n.UpdateDetails());
				}
			}
			catch (Exception exp) { Trace.WriteLine(exp.ToString()); }
			EndUpdate();
		}

		/// <summary>
		/// Handles the ListChanged event of the Connections control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Components.ObservableListChangedEventArgs&lt;MLifter.BusinessLayer.IConnectionString&gt;"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		private void Connections_ListChanged(object sender, ObservableListChangedEventArgs<IConnectionString> e)
		{
			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					if (Folders.Find(f => f.Connection == e.Item) == null)
					{
						FolderTreeNode entry = new FolderTreeNode(e.Item);
						entry.ContentLoadException += new FolderIndexEntry.ContentLoadExceptionEventHandler(Folder_ContentLoadException);
						lock (Nodes)
						{
							Nodes.Add(entry);
						}
						if (InvokeRequired)
							Invoke((MethodInvoker)delegate { AddFolderToMainNode(entry); });
						else
							AddFolderToMainNode(entry);
						entry.UpdateDetails();
					}
					break;
				case ListChangedType.ItemDeleted:
					if (InvokeRequired)
						Invoke((MethodInvoker)delegate
						{
							lock (Nodes)
							{
								Nodes.FindAll(f => f.Connection == e.Item).ForEach(f => base.Nodes.Remove(f));
								Nodes.RemoveAll(f => f.Connection == e.Item);
							}
						});
					else
					{
						lock (Nodes)
						{
							Nodes.FindAll(f => f.Connection == e.Item).ForEach(f => base.Nodes.Remove(f));
							Nodes.RemoveAll(f => f.Connection == e.Item);
						}
					}
					break;
				case ListChangedType.ItemChanged:
					break;
				case ListChangedType.Reset:
					foreach (IConnectionString item in Connections)
					{
						if (Folders.Find(f => f.Connection == item) == null)
						{
							FolderTreeNode entry = new FolderTreeNode(item);
							entry.ContentLoadException += new FolderIndexEntry.ContentLoadExceptionEventHandler(Folder_ContentLoadException);
							lock (Nodes)
							{
								Nodes.Add(entry);
							}
							if (InvokeRequired)
								Invoke((MethodInvoker)delegate { AddFolderToMainNode(entry); });
							else
								AddFolderToMainNode(entry);
							entry.UpdateDetails();
						}
					}
					break;
				case ListChangedType.ItemMoved:
				default:
					throw new NotSupportedException();
			}

			updateTimer.Enabled = true;
		}
		private void AddFolderToMainNode(FolderTreeNode entry)
		{
			BeginUpdate();
			lock (Nodes)
			{
				Nodes.Find(n => n.IsMainNode).Nodes.Add(entry);
			}
			EndUpdate();
		}
		/// <summary>
		/// Handles the ListChanged event of the Folders list.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.Components.ObservableListChangedEventArgs&lt;MLifter.BusinessLayer.FolderIndexEntry&gt;"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		private void Folders_ListChanged(object sender, ObservableListChangedEventArgs<FolderIndexEntry> e)
		{
			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					if (!Connections.Contains(e.Item.Connection))
						Connections.Add(e.Item.Connection);
					if (e.Item.Parent != null)
					{
						try
						{
							FolderTreeNode entry = new FolderTreeNode(e.Item);
							entry.ContentLoadException += new FolderIndexEntry.ContentLoadExceptionEventHandler(Folder_ContentLoadException);
							lock (Nodes)
							{
								Nodes.Add(entry);
							}
							while (Nodes.Find(n => n.Folder == e.Item.Parent) == null) Thread.Sleep(10);
							if (InvokeRequired)
								Invoke((MethodInvoker)delegate { AddFolderToParent(e.Item.Parent, entry); });
							else
								entry.UpdateDetails();
						}
						catch (Exception exp) { Trace.WriteLine("Folder-Added: " + exp.ToString()); }
					}
					else
					{
						FolderTreeNode node = Nodes.Find(n => n.Connection == e.Item.Connection);
						if (node != null)
							node.SetFolder(e.Item);
						else
						{
							FolderTreeNode entry = new FolderTreeNode(e.Item);
							entry.ContentLoadException += new FolderIndexEntry.ContentLoadExceptionEventHandler(Folder_ContentLoadException);
							lock (Nodes)
							{
								Nodes.Add(entry);
							}
							Delegate add = (MethodInvoker)delegate
								{
									BeginUpdate();
									lock (Nodes)
									{
										Nodes.Find(n => n.IsMainNode).Nodes.Add(entry);
									}
									EndUpdate();
								};
							if (InvokeRequired)
								Invoke((MethodInvoker)add);
							else
								add.DynamicInvoke();
							entry.UpdateDetails();
						}
					}
					e.Item.ContentLoading += new EventHandler(Folder_ContentLoading);
					break;
				case ListChangedType.ItemDeleted:
					throw new NotImplementedException();
				case ListChangedType.ItemChanged:
					break;
				case ListChangedType.Reset:
				case ListChangedType.ItemMoved:
				default:
					throw new NotSupportedException();
			}

			updateTimer.Enabled = true;
		}

		/// <summary>
		/// Folder_s the content load exception.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="exp">The exp.</param>
		/// <remarks>Documented by Dev05, 2009-05-15</remarks>
		private void Folder_ContentLoadException(object sender, Exception exp)
		{
			if (ContentLoadException != null)
				ContentLoadException(sender, exp);
		}

		private void Folder_ContentLoading(object sender, EventArgs e)
		{
			updateTimer.Enabled = true;
		}
		private void AddFolderToParent(FolderIndexEntry parent, FolderTreeNode entry)
		{
			BeginUpdate();
			lock (Nodes)
			{
				Nodes.Find(n => n.Folder == parent).Nodes.Add(entry);
			}
			EndUpdate();
		}
	}
}
