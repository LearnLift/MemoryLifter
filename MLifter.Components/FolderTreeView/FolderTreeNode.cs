using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MLifter.BusinessLayer;
using System.Windows.Forms;
using System.IO;

namespace MLifter.Components
{
	public class FolderTreeNode : TreeNode
	{
		/// <summary>
		/// Gets or sets a value indicating whether this instance is main node.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is main node; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public bool IsMainNode { get; private set; }
		/// <summary>
		/// Gets the folder.
		/// </summary>
		/// <value>The folder.</value>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public FolderIndexEntry Folder { get; private set; }

		private IConnectionString connection;
		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <value>The connection.</value>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public IConnectionString Connection { get { return Folder != null ? Folder.Connection : connection; } }

		/// <summary>
		/// Gets a value indicating whether this instance is loading.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is loading; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		public bool IsLoading
		{
			get
			{
				return Folder != null && (Folder.IsLoading || Folder.GetContainingEntries(true).Find(e => e is LearningModulesIndexEntry ?
					(!(e as LearningModulesIndexEntry).IsVerified || (e as LearningModulesIndexEntry).IsFromCache) :
					(e as FolderIndexEntry).IsLoading) != null);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FolderTreeNode"/> class.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public FolderTreeNode()
		{
			IsMainNode = true;
			Text = "DefaultNode";
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="FolderTreeNode"/> class.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public FolderTreeNode(FolderIndexEntry entry)
		{
			SetFolder(entry);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="FolderTreeNode"/> class.
		/// </summary>
		/// <param name="con">The connection.</param>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public FolderTreeNode(IConnectionString con)
		{
			Folder = null;
			connection = con;
			Text = connection.Name;
		}

		/// <summary>
		/// Sets the folder.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-03-12</remarks>
		public void SetFolder(FolderIndexEntry entry)
		{
			connection = entry.Connection;
			Folder = entry;
			Folder.ContentLoading += new EventHandler(Folder_ContentLoading);
			Folder.ContentLoaded += new EventHandler(Folder_ContentLoaded);
			Folder.ContentLoadException += new FolderIndexEntry.ContentLoadExceptionEventHandler(Folder_ContentLoadException);
			Folder.LearningModuleAdded += new EventHandler<LearningModuleAddedEventArgs>(Folder_LearningModuleAdded);
			Folder.FolderAdded += new EventHandler<FolderAddedEventArgs>(Folder_FolderAdded);

			Text = entry.IsRootNode ? entry.Connection.Name : entry.Path.Remove(0, entry.Parent.Path.Length).Trim('\\');
			UpdateDetails();
		}
		/// <summary>
		/// Occurs when [content load exception].
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-05-14</remarks>
		public event MLifter.BusinessLayer.FolderIndexEntry.ContentLoadExceptionEventHandler ContentLoadException;
		/// <summary>
		/// Handles the ContentLoadException event of the Folder control.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="exp">The exp.</param>
		/// <remarks>Documented by Dev02, 2009-05-14</remarks>
		void Folder_ContentLoadException(object sender, Exception exp)
		{
			if (ContentLoadException != null)
				ContentLoadException(sender, exp);
		}
		/// <summary>
		/// Handles the ContentLoaded event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-13</remarks>
		private void Folder_ContentLoaded(object sender, EventArgs e)
		{
			//UpdateDetails();
		}
		/// <summary>
		/// Handles the ContentLoading event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-13</remarks>
		private void Folder_ContentLoading(object sender, EventArgs e)
		{
			UpdateDetails();
		}
		/// <summary>
		/// Handles the FolderAdded event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.FolderAddedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void Folder_FolderAdded(object sender, FolderAddedEventArgs e)
		{
			e.Folder.ContentLoaded += new EventHandler(Folder_ContentLoaded);
			e.Folder.ContentLoading += new EventHandler(Folder_ContentLoading);
			e.Folder.ContentLoadException += new FolderIndexEntry.ContentLoadExceptionEventHandler(Folder_ContentLoadException);
			e.Folder.FolderAdded += new EventHandler<FolderAddedEventArgs>(Folder_FolderAdded);
			e.Folder.LearningModuleAdded += new EventHandler<LearningModuleAddedEventArgs>(Folder_LearningModuleAdded);

			//UpdateDetails();
		}
		/// <summary>
		/// Handles the LearningModuleAdded event of the Folder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MLifter.BusinessLayer.LearningModuleAddedEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void Folder_LearningModuleAdded(object sender, LearningModuleAddedEventArgs e)
		{
			e.LearningModule.IsVerifiedChanged += new EventHandler(LearningModule_IsVerifiedChanged);

			//UpdateDetails();
		}
		/// <summary>
		/// Handles the IsVerifiedChanged event of the LearningModule control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		private void LearningModule_IsVerifiedChanged(object sender, EventArgs e)
		{
			//UpdateDetails();
		}

		/// <summary>
		/// Updates the icon.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-13</remarks>
		public void UpdateDetails()
		{
			if (TreeView == null)
				return;

			if (TreeView.InvokeRequired)
				TreeView.BeginInvoke((MethodInvoker)SetDetails);
			else
				SetDetails();
		}

		private void SetDetails()
		{
			int index;

			if (IsMainNode)
				index = 0;
			else if (connection.ConnectionType == MLifter.DAL.DatabaseType.Unc)
			{
				if (IsLoading && Folder.Parent != null)
					index = 2;
				else
				{
					if (connection.ConnectionString[1] == ':')
					{
						DriveInfo driveInfo = new DriveInfo(connection.ConnectionString[0].ToString());
						if (driveInfo.DriveType == DriveType.Removable)
						{
							if (Folder == null || Folder.Parent == null)
								index = 6;
							else
								index = 1;
						}
						else
							index = 1;
					}
					else
						index = 1;
				}
			}
			else
			{
				if (Folder == null || Folder.IsOffline)
					index = 4;
				else
					index = 3;
			}

			if (IsLoading && Folder.Parent == null)
				index = 5;

			SelectedImageIndex = ImageIndex = index;
		}
	}
}
