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
using System.Text;
using System.Windows.Forms;
using MLifter.BusinessLayer;
using MLifter.Controls.Properties;

namespace MLifter.Controls.LearningModulesPage
{
	/// <summary>
	/// A ListViewItem which holds a folder.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-03-07</remarks>
	public class FolderListViewItem : ListViewItem
	{
		private bool imageSet = false;
		/// <summary>
		/// Gets or sets the folder.
		/// </summary>
		/// <value>The folder.</value>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		public FolderIndexEntry Folder { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FolderListViewItem"/> class.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <remarks>Documented by Dev05, 2009-03-07</remarks>
		public FolderListViewItem(FolderIndexEntry entry)
			: base(entry.DisplayName)
		{
			while (SubItems.Count < 3) SubItems.Add(new ListViewSubItem());

			Folder = entry;

			Folder.FolderAdded += new EventHandler<FolderAddedEventArgs>(Folder_FolderAdded);
			Folder.LearningModuleAdded += new EventHandler<LearningModuleAddedEventArgs>(Folder_LearningModuleAdded);
			UpdateImage();
		}

		void Folder_LearningModuleAdded(object sender, LearningModuleAddedEventArgs e)
		{
			UpdateDetails();
		}

		void Folder_FolderAdded(object sender, FolderAddedEventArgs e)
		{
			UpdateDetails();
		}

		/// <summary>
		/// Updates the details.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-16</remarks>
		public void UpdateDetails()
		{
			if (this.Folder == null || this.ListView == null)
				return;

			if (this.ListView.InvokeRequired)
				this.ListView.BeginInvoke((MethodInvoker)SetDetails);
			else
				SetDetails();
		}
		private void SetDetails()
		{
			List<IIndexEntry> folders = Folder.GetContainingEntries(false).FindAll(e => e is FolderIndexEntry);
			List<IIndexEntry> lms = Folder.GetContainingEntries(false).FindAll(e => e is LearningModulesIndexEntry);
			SubItems[1].Text = string.Format(Resources.FOLDER_AMOUNT_SUBFOLDERS, folders.Count);
			SubItems[2].Text = string.Format(Resources.FOLDER_AMOUNT_LEARNINGMODULES, lms.Count);
		}

		/// <summary>
		/// Sets the image.
		/// </summary>
		/// <remarks>Documented by Dev05, 2009-03-08</remarks>
		public void UpdateImage()
		{
			if (imageSet || this.ListView == null)
				return;

			if (this.ListView.InvokeRequired)
				this.ListView.BeginInvoke((MethodInvoker)SetImage);
			else
				SetImage();
		}
		private void SetImage()
		{
			if (!imageSet && this.ListView != null)
			{
				imageSet = true;
				this.ListView.LargeImageList.Images.Add(Folder.Logo);
				this.ListView.SmallImageList.Images.Add(Folder.Logo);
				ImageIndex = this.ListView.LargeImageList.Images.Count - 1;
			}
		}
	}
}
