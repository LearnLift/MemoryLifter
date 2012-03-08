using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;
using MLifter.DAL;
using MLifter.Generics;

namespace MLifter.Controls.LearningModulesPage
{
	/// <summary>
	/// A ListViewItem which holds a LearningModule.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-03-07</remarks>
	public class LearningModuleListViewItem : ListViewItem
	{
		/// <summary>
		/// Gets or sets the learning module.
		/// </summary>
		/// <value>The learning module.</value>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		public LearningModulesIndexEntry LearningModule { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LearningModuleListViewItem"/> class.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		public LearningModuleListViewItem(LearningModulesIndexEntry entry)
			: base(entry.DisplayName)
		{
			LearningModule = entry;
			LearningModule.IsVerifiedChanged += new EventHandler(LearningModule_IsVerifiedChanged);

			while (SubItems.Count < 6) SubItems.Add(new ListViewItem.ListViewSubItem());

			//grey pictures
			ImageIndex = entry.Type == LearningModuleType.Local ? 1 : 5;

			Group = entry.Group;

			if (entry.IsVerified)
				UpdateDetails();
			else
				SubItems[1].Text = Resources.STARTPAGE_LOADING;
		}

		/// <summary>
		/// Handles the IsVerifiedChanged event of the LearningModule control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		private void LearningModule_IsVerifiedChanged(object sender, EventArgs e)
		{
			if (!LearningModule.IsVerified)
				return;

			UpdateDetails();
		}

		/// <summary>
		/// Updates the details.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		public void UpdateDetails()
		{
			if (this.ListView == null || !LearningModule.IsVerified)
				return;

			try
			{
				if (this.ListView.InvokeRequired)
					this.ListView.BeginInvoke((MethodInvoker)SetDetails);
				else
					SetDetails();
			}
			catch (NullReferenceException) { }
		}

		/// <summary>
		/// Sets the details.
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-01-17</remarks>
		private void SetDetails()
		{
			SubItems[0].Text = LearningModule.DisplayName;

			if (LearningModule.IsAccessible) //LM accessible -> show actual author
				SubItems[1].Text = LearningModule.Author;
			else if (LearningModule.NotAccessibleReason == LearningModuleNotAccessibleReason.ServerOffline) //Remote LM location not available -> show appropriate error message
				SubItems[1].Text = Resources.LEARNING_MODULE_SERVER_OFFLINE;
			else if (LearningModule.NotAccessibleReason == LearningModuleNotAccessibleReason.Protected) //Protected LM -> not supported
				SubItems[1].Text = Resources.LEARNING_MODULE_NOT_SUPPORTED;
			else //LM not accessible because of an unknown reason -> show generic error message
				SubItems[1].Text = Resources.LEARNING_MODULE_NOT_ACCESSIBLE;

			SubItems[1].ForeColor = Color.Gray;
			SubItems[2].Text = LearningModule.IsFromCache ? Resources.LEARNING_MODULES_PAGE_LOADING :
				LearningModule.LastTimeLearned.Year > 1900 ? LearningModule.LastTimeLearned.ToLongDateString() : Resources.LAST_LEARNED_NEVER;
			SubItems[2].ForeColor = Color.Gray;
			SubItems[3].Text = LearningModule.Category != null ? LearningModule.Category.ToString() : string.Empty;
			SubItems[4].Text = LearningModule.Count >= 0 ? LearningModule.Count.ToString() : Resources.NotAvailable;
			SubItems[5].Text = Methods.GetFileSize(LearningModule.Size, false);

			if (LearningModule.IsAccessible) //LM accessible -> show details
				ToolTipText = string.Format(Resources.LEARNING_MODULES_PAGE_ITEM_TOOLTIP,
					LearningModule.Category != null ? LearningModule.Category.ToString() : string.Empty, LearningModule.Count);
			else if(LearningModule.NotAccessibleReason == LearningModuleNotAccessibleReason.ServerOffline) //LM N/A -> show error
				ToolTipText = Resources.LEARNING_MODULE_SERVER_OFFLINE;
			else if(LearningModule.NotAccessibleReason == LearningModuleNotAccessibleReason.Protected) //Protected LM -> use ML 2.3
				ToolTipText = Resources.LEARNING_MODULE_NOT_SUPPORTED_DETAIL;
			else
				ToolTipText = Resources.LEARNING_MODULE_NOT_ACCESSIBLE;

			if (LearningModule.IsAccessible)
				ImageIndex = (LearningModule.Type == LearningModuleType.Local) ? LearningModulesPage.IMAGE_INDEX_LOCAL : LearningModulesPage.IMAGE_INDEX_REMOTE;
			else
				ImageIndex = (LearningModule.Type == LearningModuleType.Local) ? LearningModulesPage.IMAGE_INDEX_LOCAL_GREY : LearningModulesPage.IMAGE_INDEX_REMOTE_GREY;
		}
	}
}
