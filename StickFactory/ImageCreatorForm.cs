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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace StickFactory
{
	public partial class ImageCreatorForm : Form
	{
		/// <summary>
		/// Gets the created image path.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public string CreatedImage { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageCreatorForm"/> class.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public ImageCreatorForm()
		{
			InitializeComponent();

			textBoxInputFilePath.Text = Properties.Settings.Default.InputFolderPath;

			textBoxStickName.Text = Properties.Settings.Default.StickName;
			textBoxStickID.Text = Properties.Settings.Default.StickID;

			textBoxOutputFile.Text = Properties.Settings.Default.OutputFilePath;
		}

		/// <summary>
		/// Handles the Click event of the buttonBrowse control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.ShowNewFolderButton = false;
			if (Directory.Exists(textBoxInputFilePath.Text))
				fbd.SelectedPath = textBoxInputFilePath.Text;
			if (fbd.ShowDialog() == DialogResult.OK)
				textBoxInputFilePath.Text = fbd.SelectedPath;
		}

		/// <summary>
		/// Handles the Click event of the buttonBrowseOutput control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void buttonBrowseOutput_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "MemoryLifter Stick Archive (*.MLifterStick)|*.MLifterStick";
			if (sfd.ShowDialog() == DialogResult.OK)
				textBoxOutputFile.Text = sfd.FileName;
		}

		/// <summary>
		/// Handles the Click event of the buttonCreate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void buttonCreate_Click(object sender, EventArgs e)
		{
			try
			{
				Enabled = false;

				Properties.Settings.Default.InputFolderPath = textBoxInputFilePath.Text;
				Properties.Settings.Default.StickName = textBoxStickName.Text;
				Properties.Settings.Default.StickID = textBoxStickID.Text;
				Properties.Settings.Default.OutputFilePath = textBoxOutputFile.Text;
				Properties.Settings.Default.Save();

				GenerateImage();

				this.DialogResult = DialogResult.OK;

				Close();
			}
			catch (Exception exp)
			{
				MessageBox.Show("There was an Error creating the image:\n\r" + exp.Message, "Error creating the image", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Enabled = true;
			}
		}

		/// <summary>
		/// Generates the image.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void GenerateImage()
		{
			#region Check parameter:
			
			if (!Directory.Exists(textBoxInputFilePath.Text))
				throw new DirectoryNotFoundException("The input directory was not found!");
			if (!Directory.Exists(Path.GetDirectoryName(textBoxOutputFile.Text)))
				throw new DirectoryNotFoundException("The output path is invalid!");
			if (textBoxStickName.Text.Length > 11 || textBoxStickName.Text == string.Empty)
				throw new ArgumentException("Invalid Stick name!");
			if (textBoxStickID.Text.Length != 9 || textBoxStickID.Text[4] != '-')
				throw new ArgumentException("Invalid Stick ID!");

			#endregion

			string outputFilePath = Path.ChangeExtension(textBoxOutputFile.Text, ".MLifterStick");

			#region Create Zip:
			
			Stream output = File.Create(outputFilePath);
			ZipOutputStream zipStream = new ZipOutputStream(output);
			ZipFile zip = new ZipFile(output);

			#endregion

			#region Fill Zip:

			zip.BeginUpdate();

			int pos = 1;
			string[] files = Directory.GetFiles(textBoxInputFilePath.Text, "*.*", SearchOption.AllDirectories);
			List<string> folders = new List<string>();
			foreach (string file in files)
			{
				toolStripStatusLabelMessage.Text = string.Format("Adding file {0} of {1}...", pos, files.Length);
				toolStripProgressBarStatus.Value = Convert.ToInt32(pos++ * 1.0 / files.Length * 100);
				Application.DoEvents();

				string dir = Path.GetDirectoryName(file).Remove(0, textBoxInputFilePath.Text.Length);
				string zipFile = Path.Combine(dir, Path.GetFileName(file));

				if (dir.Length > 0)
				{
					if (!folders.Contains(dir))
					{
						zip.AddDirectory(dir);
						folders.Add(dir);
					}
				}
				zip.Add(new FileDataSource(file), zipFile);
			}

			toolStripStatusLabelMessage.Text = "Saving image. THIS COULD RUN VERY LONG - PLEASE WAIT!";
			Application.DoEvents();

			zip.CommitUpdate();

			#endregion

			VolumeDataStorage vds = new VolumeDataStorage();

			#region setting file attributes:

			foreach (string file in files)
			{
				string dir = Path.GetDirectoryName(file).Remove(0, textBoxInputFilePath.Text.Length);
				if (dir.StartsWith("\\"))
					dir = dir.Remove(0, 1);
				string zipFile = Path.Combine(dir, Path.GetFileName(file)).Replace(@"\", "/");
				ZipEntry entry = zip.GetEntry(zipFile);
				if (entry == null)
					continue;
				FileAttributes attr = (new FileInfo(file)).Attributes;
				if (!(attr == FileAttributes.Archive || attr == FileAttributes.Normal))
					vds.FileAttributeList.Add(entry.Name, attr);
				toolStripStatusLabelMessage.Text = "Setting file FileAttributes to: " + zipFile;
			}
			foreach (string folder in folders)
			{
				FileAttributes attr = (new DirectoryInfo(Path.Combine(textBoxInputFilePath.Text, folder))).Attributes;
				if (!(attr == FileAttributes.Archive || attr == FileAttributes.Normal || attr == FileAttributes.Directory))
					vds.FolderAttributeList.Add(folder, attr);
				toolStripStatusLabelMessage.Text = "Setting folder FileAttributes to: " + folder;
			}

			#endregion

			#region Set Comment:

			vds.VolumeLabel = textBoxStickName.Text;
			vds.VolumeSerial = textBoxStickID.Text;
			string comment = VolumeDataStorage.SerializeData(vds);

			File.WriteAllText(Path.Combine(Application.StartupPath, "Comment.txt"), comment);

			zip.BeginUpdate();
			zip.SetComment(comment);
			zip.CommitUpdate();

			#endregion

			zip.Close();

			CreatedImage = outputFilePath;
		}

		/// <summary>
		/// Handles the Click event of the buttonCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}
	}

	public class FileDataSource : IStaticDataSource
	{
		private string fileName;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDataSource"/> class.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		public FileDataSource(string fileName)
		{
			this.fileName = fileName;
		}

		/// <summary>
		/// Gets the source stream.
		/// </summary>
		/// <returns></returns>
		/// <remarks>CFI, 2012-02-24</remarks>
		public Stream GetSource()
		{
			return new FileStream(fileName, FileMode.Open, FileAccess.Read);
		}
	}
}
