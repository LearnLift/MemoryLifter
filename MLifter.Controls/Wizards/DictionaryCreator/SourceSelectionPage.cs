using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.DAL;
using MLifter.Generics;
using MLifter.Controls.Properties;
using MLifter.DAL.Security;

namespace MLifter.Controls.Wizards.DictionaryCreator
{
	public partial class SourceSelectionPage : MLifter.WizardPage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SourceSelection"/> class.
		/// </summary>
		/// <param name="learnLogic">The learn logic.</param>
		/// <remarks>Documented by Dev08, 2008-12-12</remarks>
		public SourceSelectionPage()
		{
			InitializeComponent();
			ColumnHeaderName.Width = (int)(listViewSources.Width * 0.9);
		}

		/// <summary>
		/// Handles the Load event of the SourceSelection control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-12-12</remarks>
		private void SourceSelection_Load(object sender, EventArgs e)
		{
			if (LearningModulesIndex.WritableConnections.Count <= 0)
				throw new NoWritableConnectionAvailableException();
			else if (LearningModulesIndex.WritableConnections.Count == 1)
			{
				connectionString = LearningModulesIndex.WritableConnections[0];
				GoNext();
			}

			listViewSources.SmallImageList = new ImageList();
			listViewSources.SmallImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewSources.SmallImageList.ImageSize = new Size(16, 16);
			listViewSources.SmallImageList.Images.Add(Resources.learning_16);
			listViewSources.SmallImageList.Images.Add(Resources.world_16);
			listViewSources.SmallImageList.Images.Add(Resources.usbDrive);
			List<System.IO.DriveInfo> mLifterSticks = Methods.GetMLifterSticks();

			foreach (IConnectionString conString in LearningModulesIndex.WritableConnections)
			{
				if (listViewSources.Items.Find("ListViewItem_" + conString.Name, true).Length == 0)
				{

					ListViewItem item = new ListViewItem(conString.Name);
					item.Tag = conString;
					item.Name = "ListViewItem_" + conString.Name;
					//Icon depending on connectiontype
					foreach (System.IO.DriveInfo stick in mLifterSticks)
					{
						if (conString.ConnectionString.Contains(stick.Name))
						{
							item.ImageIndex = 2;
							break;
						}
					}
					if (item.ImageIndex != 2) item.ImageIndex = (conString.ConnectionType == MLifter.DAL.DatabaseType.Unc) ? 0 : 1;
					if (conString.ConnectionType == MLifter.DAL.DatabaseType.Unc)
						item.ToolTipText = (conString as UncConnectionStringBuilder).ConnectionString;
					else
						item.ToolTipText = conString.Name;

					listViewSources.Items.Add(item);
				}
			}

			listViewSources.SelectedIndices.Clear();
			for (int i = 0; i < listViewSources.Items.Count; i++)
			{
				if (listViewSources.Items[i].Tag is IConnectionString)
				{
					if ((listViewSources.Items[i].Tag as IConnectionString).IsDefault)
					{
						listViewSources.SelectedIndices.Add(i);
						break;
					}
				}
			}
			if (listViewSources.SelectedIndices.Count == 0)
				listViewSources.SelectedIndices.Add(0);
		}

		/// <summary>
		/// Called if the next-button is clicked.
		/// </summary>
		/// <returns>
		/// 	<i>false</i> to abort, otherwise<i>true</i>
		/// </returns>
		/// <remarks>Documented by Dev05, 2007-11-21</remarks>
		/// <remarks>Documented by Dev08, 2008-12-12</remarks>
		public override bool GoNext()
		{
			if (listViewSources.SelectedItems.Count != 1)
				return false;
			if (listViewSources.SelectedItems[0].Tag is IConnectionString)
			{
				IConnectionString connection = listViewSources.SelectedItems[0].Tag as IConnectionString;
				try
				{
					MLifter.DAL.Interfaces.IUser user = LearningModulesPage.LearningModulesPage.Login(connection);
					if (!user.List().HasPermission(PermissionTypes.CanModify))
						throw new PermissionException();
				}
				catch (ServerOfflineException)
				{
					return false;
				}
				catch (PermissionException)
				{
					TaskDialog.MessageBox(Resources.CREATE_LM_PERMISSION_MBX_CAPTION, Resources.CREATE_LM_PERMISSION_MBX_TEXT, string.Empty, TaskDialogButtons.OK, TaskDialogIcons.Error);
					return false;
				}
				catch
				{
					return false;
				}

				return base.GoNext();
			}
			else
				return false;
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the listViewSources control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-12-12</remarks>
		private void listViewSources_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewSources.SelectedItems.Count == 1)
			{
				NextAllowed = true;
				connectionString = listViewSources.SelectedItems[0].Tag as IConnectionString;
			}
			else
				NextAllowed = false;
		}

		/// <summary>
		/// Handles the DoubleClick event of the listViewSources control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-12-12</remarks>
		private void listViewSources_DoubleClick(object sender, EventArgs e)
		{
			if (listViewSources.SelectedItems.Count == 1)
			{
				OnSubmitPage(EventArgs.Empty);
			}
		}

		private IConnectionString connectionString;
		public IConnectionString ConnectionString
		{
			get { return connectionString; }
			set { connectionString = value; }
		}

		/// <summary>
		/// Called if the Help Button is clicked.
		/// </summary>
		/// <remarks>Documented by Dev09, 2009-05-28</remarks>
		public override void ShowHelp()
		{
			Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/memo9sqf.htm");
		}
	}
}
