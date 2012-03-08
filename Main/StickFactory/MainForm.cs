using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using ICSharpCode;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;

namespace StickFactory
{
	public partial class MainForm : Form
	{
		private DriveDetector usbDetector;
		private bool processStarted = false;
		private bool loadingSuccessful = false;

		//Content of ZipFile
		public List<byte[]> zipFileContentList = new List<byte[]>();
		public List<string> zipFilenames = new List<string>();
		public List<long> zipFileSizes = new List<long>();
		public List<FileDirType> zipFileType = new List<FileDirType>();
		public List<FileAttributes> zipFileAttributes = new List<FileAttributes>();

		private List<string> pluggedDrives = new List<string>();

		//LoadingThread:
		private Thread loadingThread;
		private event EventHandler loadingThreadFinished;

		UsbStickWriter usbStickWriter = new UsbStickWriter();
		Dictionary<char, DriveStatusControl> statusControls = new Dictionary<char, DriveStatusControl>();

		/// <summary>
		/// Definies the directory type (File or Folder);
		/// </summary>
		public enum FileDirType
		{
			File,
			Directory
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		public MainForm()
		{
			DoubleBuffered = true;

			InitializeComponent();

			DriveStatusControl ctl;
			char[] letters = new char[] { 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'A', 'B' };
			foreach (char c in letters)
			{
				ctl = new DriveStatusControl();
				ctl.Drive = c;
				statusControls[c] = ctl;
				tableLayoutPanelStatus.Controls.Add(ctl);
				ctl.Dock = DockStyle.Fill;
			}

			textBoxInputFilePath.Text = Properties.Settings.Default.InputPath;

			Text += " V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		#region UsbStickWriter events

		/// <summary>
		/// Handles the UsbStickWriteAbort event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		void usbStickWriter_UsbStickWriteAbort(object sender, EventArgs e)
		{
			UsbProgress process = (UsbProgress)sender;
			ChangeDeviceTextFromList(process.Drive.RootDirectory.ToString()[0], "Aborted by user.");
		}
		/// <summary>
		/// Handles the UsbStickWriteProcess event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		void usbStickWriter_UsbStickWriteProcess(object sender, EventArgs e)
		{
			UsbProgress process = (UsbProgress)sender;
			ChangeDeviceTextFromList(process.Drive.RootDirectory.ToString()[0],
				string.Format("Processing file {0} of {1}.", process.Current, process.Total), process.Current * 1.0 / process.Total * 100, process.Current, process.Total);
		}
		/// <summary>
		/// Handles the UsbStickWriteBegin event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		void usbStickWriter_UsbStickWriteBegin(object sender, EventArgs e)
		{
			DriveInfo d = (DriveInfo)sender;
			ChangeDeviceTextFromList(d.RootDirectory.ToString()[0],
				string.Format("{0} files on Stick...", usbStickWriter.CopyContent ? "Writing" : "Checking"));
		}
		/// <summary>
		/// Handles the UsbStickWriteFinish event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>
		/// CFI, 2012-02-24
		/// </remarks>
		void usbStickWriter_UsbStickWriteFinish(object sender, EventArgs e)
		{
			DriveInfo d = (DriveInfo)sender;
			ChangeDeviceTextFromList(d.RootDirectory.ToString()[0], "Finished.");
		}
		/// <summary>
		/// Handles the UsbStickWriteError event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="StickFactory.UsbStickWriteMessageEventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		void usbStickWriter_UsbStickWriteError(object sender, UsbStickWriteMessageEventArgs e)
		{
			ChangeDeviceTextFromList(e.Drive.RootDirectory.ToString()[0], "Error: " + e.Message, Color.LightCoral);
		}
		/// <summary>
		/// Handles the UsbStickWriteStatusMessage event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="StickFactory.UsbStickWriteMessageEventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		void usbStickWriter_UsbStickWriteStatusMessage(object sender, UsbStickWriteMessageEventArgs e)
		{
			ChangeDeviceTextFromList(e.Drive.RootDirectory.ToString()[0], e.Message);
		}
		/// <summary>
		/// Handles the UsbStickIdSet event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="StickFactory.UsbStickWriteMessageEventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		void usbStickWriter_UsbStickIdSet(object sender, UsbStickWriteMessageEventArgs e)
		{
			this.Invoke((MethodInvoker)delegate
			{
				statusControls[e.Drive.RootDirectory.ToString()[0]].IdSet = true;
			});
		}
		/// <summary>
		/// Handles the UsbStickFormattingFinish event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="StickFactory.UsbStickWriteMessageEventArgs"/> instance containing the event data.</param>
		/// <remarks>
		/// CFI, 2012-02-24
		/// </remarks>
		void usbStickWriter_UsbStickFormattingFinish(object sender, UsbStickWriteMessageEventArgs e)
		{
			this.Invoke((MethodInvoker)delegate
			{
				statusControls[e.Drive.RootDirectory.ToString()[0]].FormattingFinish = true;
			});
		}
		/// <summary>
		/// Handles the UsbStickContentWritten event of the usbStickWriter control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="StickFactory.UsbStickWriteMessageEventArgs"/> instance containing the event data.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		void usbStickWriter_UsbStickContentWritten(object sender, UsbStickWriteMessageEventArgs e)
		{
			this.Invoke((MethodInvoker)delegate
			{
				statusControls[e.Drive.RootDirectory.ToString()[0]].ContentWritten = true;
			});
		}

		#endregion

		/// <summary>
		/// Handles the TextChanged event of the textBoxInputFilePath control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		private void textBoxInputFilePath_TextChanged(object sender, EventArgs e)
		{
			buttonStartChecking.Enabled = buttonStartCopy.Enabled =
				Directory.Exists(textBoxInputFilePath.Text) || File.Exists(textBoxInputFilePath.Text);
		}

		/// <summary>
		/// Handles the Click event of the buttonStart control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		private void buttonStart_Click(object sender, EventArgs e)
		{
			StartProcess(false);
		}

		/// <summary>
		/// Handles the Click event of the buttonStartChecking control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-10-09</remarks>
		private void buttonStartChecking_Click(object sender, EventArgs e)
		{
			StartProcess(true);
		}

		private bool CheckSticks = false;
		/// <summary>
		/// Handles the click events of the buttonStartChecking and buttonStartCopy controls.
		/// </summary>
		/// <param name="checking">if set to <c>true</c> [checking].</param>
		/// <remarks>Documented by Dev02, 2008-10-09</remarks>
		private void StartProcess(bool checking)
		{
			CheckSticks = checking;

			Properties.Settings.Default.InputPath = textBoxInputFilePath.Text;
			Properties.Settings.Default.Save();

			if (!processStarted)        //Start the Stick Detection and Copy Process
			{
				groupBoxInput.Enabled = false;

				ZipFile zipFile = new ZipFile(textBoxInputFilePath.Text);
				VolumeDataStorage vds = VolumeDataStorage.DeserializeData(zipFile.ZipFileComment);
				zipFile.Close();

				if (checking)
					buttonStartChecking.Text = "Stop checking engine";
				else
					buttonStartCopy.Text = "Stop copy engine";

				processStarted = true;
				(checking ? buttonStartCopy : buttonStartChecking).Enabled = false;

				if (loadingThreadFinished != MainForm_mainThreadLoadingFinished)
					loadingThreadFinished += new EventHandler(MainForm_mainThreadLoadingFinished);

				loadingThread = new Thread(LoadingProcessThread);
				loadingThread.IsBackground = true;
				loadingThread.Name = "Loading Thread";
				loadingThread.Start();

				usbStickWriter.StickName = vds.VolumeLabel;
				usbStickWriter.StickID = vds.VolumeSerial;
				usbStickWriter.CopyContent = !checking;
				usbStickWriter.CheckContent = checking;
				usbStickWriter.FileAttributes.Clear();
				foreach (KeyValuePair<string, FileAttributes> pair in vds.FileAttributeList)
					usbStickWriter.FileAttributes.Add(pair.Key, pair.Value);
				foreach (KeyValuePair<string, FileAttributes> pair in vds.FolderAttributeList)
					usbStickWriter.FolderAttributes.Add(pair.Key, pair.Value);
			}
			else                        //Stop the Stick Detection and Copy Process
			{
				groupBoxInput.Enabled = true;

				if (checking)
					buttonStartChecking.Text = "Start checking engine";
				else
					buttonStartCopy.Text = "Start copy engine";

				processStarted = false;
				loadingSuccessful = false;
				(checking ? buttonStartCopy : buttonStartChecking).Enabled = true;
				usbStickWriter.AbortWriting();

				if (loadingThread.IsAlive)
				{
					loadingThread.Abort();
					SetStatusMessage("Process canceled by user");
				}
			}

		}

		/// <summary>
		/// Handles the DeviceArrived event of the usbDetector control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="StickFactory.DriveDetectorEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		void usbDetector_DeviceArrived(object sender, DriveDetectorEventArgs e)
		{
			//if (!processStarted) return;

			DriveInfo drive = new DriveInfo(e.Drive);

			if (drive.DriveType == DriveType.Removable)
			{
				AddDeviceToList(e.Drive, "Ready");
				pluggedDrives.Add(e.Drive);
				e.HookQueryRemove = true;

				if (loadingSuccessful && processStarted)
					usbStickWriter.AddUsbStick(drive);
			}
		}
		/// <summary>
		/// Handles the DeviceRemoved event of the usbDetector control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="StickFactory.DriveDetectorEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		void usbDetector_DeviceRemoved(object sender, DriveDetectorEventArgs e)
		{
			//if (!processStarted) return;
			pluggedDrives.Remove(e.Drive);
			RemoveDeviceFromList(e.Drive);
		}
		/// <summary>
		/// Handles the QueryRemove event of the usbDetector control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="StickFactory.DriveDetectorEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		void usbDetector_QueryRemove(object sender, DriveDetectorEventArgs e)
		{
			ChangeDeviceTextFromList(e.Drive[0], "Unmounting...");
		}

		private void AddDeviceToList(string drive, string status)
		{
			this.BeginInvoke(new MethodInvoker(delegate()
			{
				DriveStatusControl ctl = statusControls[drive[0]];
				ctl.Reset();
				ctl.Set(drive[0]);
				ctl.StatusMessage = status;
			}));
		}

		private void RemoveDeviceFromList(string drive)
		{
			if (statusControls[drive[0]].StatusMessage.StartsWith("Error"))
				Thread.Sleep(2500);
			this.BeginInvoke(new MethodInvoker(delegate()
			{
				statusControls[drive[0]].Reset();
			}));
		}

		private void ChangeDeviceTextFromList(char drive, string message)
		{
			ChangeDeviceTextFromList(drive, message, -1, -1, -1);
		}
		private void ChangeDeviceTextFromList(char drive, string message, double status, int current, int count)
		{
			ChangeDeviceTextFromList(drive, message, status, current, count, Color.Empty);
		}
		private void ChangeDeviceTextFromList(char drive, string message, Color newColor)
		{
			ChangeDeviceTextFromList(drive, message, -1, -1, -1, newColor);
		}
		private void ChangeDeviceTextFromList(char drive, string message, double status, int current, int count, Color newColor)
		{
			this.BeginInvoke(new MethodInvoker(delegate()
			{
				DriveStatusControl ctl = statusControls[drive];
				ctl.StatusMessage = message;
				if (status >= 0)
				{
					ctl.Progress = status;
					ctl.Current = current;
					ctl.Count = count;
				}
				if (newColor != Color.Empty)
					ctl.SignalColor = newColor;
			}));
		}

		/// <summary>
		/// Loads the file/directory structure and their content (file) into the RAM
		/// </summary>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		private void LoadingProcessThread()
		{
			DateTime start = DateTime.Now;

			//1. Step: Clean up all generic Lists
			CleanUp();

			//DirectoryInfo d = new DirectoryInfo(textBoxInputFilePath.Text);
			////int files = XFileDirectory.GetFileCount(d.FullName);
			//int folders = XFileDirectory.GetFolderCount(d.FullName);
			//XFolder xf = new XFolder();
			//xf.FolderName = d.FullName;

			//2. Step: Load every Entry (path) of the Zip File
			ZipFile zipFile = new ZipFile(textBoxInputFilePath.Text);
			ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(textBoxInputFilePath.Text));
			ZipEntry zipEntry = null;

			int counter = 0;
			while ((zipEntry = zipInputStream.GetNextEntry()) != null)
			{
				SetStatusMessage(string.Format("Reading Directory Structure {0}: {1}", counter + 1, zipEntry.Name));
				zipFilenames.Add(zipEntry.Name);
				zipFileSizes.Add(zipEntry.Size);
				zipFileType.Add(zipEntry.IsDirectory ? FileDirType.Directory : FileDirType.File);
				zipFileAttributes.Add((FileAttributes)zipEntry.ExternalFileAttributes);
				++counter;
			}
			DateTime dirStruct = DateTime.Now;

			//3. Step: Load the content of the ZipFile
			counter = 0;
			foreach (string filename in zipFilenames)
			{
				SetStatusMessage(string.Format("Loading {0} of {1}: {2}", counter + 1, zipFilenames.Count, filename));
				ZipEntry zipEntryPath = new ZipEntry(filename);
				Stream fs = zipFile.GetInputStream(zipEntryPath);

				if (!CheckSticks)
				{
					byte[] fileContent = new byte[zipFileSizes[counter]];
					int offset = 0;
					int size = 0;
					byte[] buffer = new byte[4096];
					do
					{
						size = fs.Read(buffer, 0, buffer.Length);
						CopyIntoArray(fileContent, offset, buffer, size);
						offset += size;
					}
					while (size > 0);

					zipFileContentList.Add(fileContent);
				}
				++counter;
			}

			DateTime end = DateTime.Now;
			Debug.WriteLine(string.Format("DirStruct: {0}\nLoading: {1}\n Total: {2}", new TimeSpan(dirStruct.Ticks - start.Ticks), new TimeSpan(end.Ticks - dirStruct.Ticks), new TimeSpan(end.Ticks - start.Ticks)));

			//4. Step: Close all File relevant connections
			zipInputStream.Close();
			zipFile.Close();

			OnMainThreadLoadingFinished();
		}

		private void CopyIntoArray(byte[] target, int targetOffset, byte[] source, int count)
		{
			for (int i = targetOffset; i < targetOffset + count; i++)
				target[i] = source[i - targetOffset];
		}

		/// <summary>
		/// Cleans up.
		/// </summary>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		private void CleanUp()
		{
			zipFilenames.Clear();
			zipFileSizes.Clear();
			zipFileContentList.Clear();
			zipFileType.Clear();
		}

		/// <summary>
		/// Called when [main thread loading finished].
		/// </summary>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		private void OnMainThreadLoadingFinished()
		{
			if (loadingThreadFinished != null)
				loadingThreadFinished(this, EventArgs.Empty);
		}

		/// <summary>
		/// Handles the mainThreadLoadingFinished event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		void MainForm_mainThreadLoadingFinished(object sender, EventArgs e)
		{
			this.BeginInvoke(new MethodInvoker(delegate()
			{
				labelStatusMessage.Text = "Finished loading ZIP-File into memory.";

				foreach (string drive in pluggedDrives)
					usbStickWriter.AddUsbStick(new DriveInfo(drive));

				usbStickWriter.ZipFileContentList.AddRange(zipFileContentList);
				usbStickWriter.ZipFilenames.AddRange(zipFilenames);
				usbStickWriter.ZipFileSizes.AddRange(zipFileSizes);
				usbStickWriter.ZipFileType.AddRange(zipFileType);
				usbStickWriter.ZipFileAttributes.AddRange(zipFileAttributes);

				loadingSuccessful = true;
			}));
		}

		/// <summary>
		/// Sets the status message.
		/// </summary>
		/// <param name="statusMsg">The status MSG.</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		void SetStatusMessage(string statusMsg)
		{
			SetStatusMessage(statusMsg, false);
		}

		/// <summary>
		/// Sets the status message.
		/// </summary>
		/// <param name="statusMsg">The status MSG.</param>
		/// <param name="appendMsg">if set to <c>true</c> [append MSG].</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		void SetStatusMessage(string statusMsg, bool appendMsg)
		{
			this.BeginInvoke(new MethodInvoker(delegate()
			{
				if (appendMsg)
					labelStatusMessage.Text += statusMsg;
				else
					labelStatusMessage.Text = statusMsg;
			}));
		}

		/// <summary>
		/// Handles the Shown event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-10-09</remarks>
		private void MainForm_Shown(object sender, EventArgs e)
		{
			usbDetector = new DriveDetector();
			usbDetector.DeviceArrived += new DriveDetectorEventHandler(usbDetector_DeviceArrived);
			usbDetector.DeviceRemoved += new DriveDetectorEventHandler(usbDetector_DeviceRemoved);
			usbDetector.QueryRemove += new DriveDetectorEventHandler(usbDetector_QueryRemove);

			usbStickWriter.UsbStickWriteFinish += new EventHandler(usbStickWriter_UsbStickWriteFinish);
			usbStickWriter.UsbStickWriteBegin += new EventHandler(usbStickWriter_UsbStickWriteBegin);
			usbStickWriter.UsbStickWriteProcess += new EventHandler(usbStickWriter_UsbStickWriteProcess);
			usbStickWriter.UsbStickWriteAbort += new EventHandler(usbStickWriter_UsbStickWriteAbort);
			usbStickWriter.UsbStickWriteError += new UsbStickWriter.UsbStickWriteMessageEventHandler(usbStickWriter_UsbStickWriteError);
			usbStickWriter.UsbStickWriteStatusMessage += new UsbStickWriter.UsbStickWriteMessageEventHandler(usbStickWriter_UsbStickWriteStatusMessage);
			usbStickWriter.UsbStickContentWritten += new UsbStickWriter.UsbStickWriteMessageEventHandler(usbStickWriter_UsbStickContentWritten);
			usbStickWriter.UsbStickFormattingFinish += new UsbStickWriter.UsbStickWriteMessageEventHandler(usbStickWriter_UsbStickFormattingFinish);
			usbStickWriter.UsbStickIdSet += new UsbStickWriter.UsbStickWriteMessageEventHandler(usbStickWriter_UsbStickIdSet);

		}

		/// <summary>
		/// Handles the Click event of the buttonBrowse control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2008-10-09</remarks>
		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "MemoryLifter Stick Archive (*.MLifterStick)|*.MLifterStick";
			if (dialog.ShowDialog() == DialogResult.OK)
				textBoxInputFilePath.Text = dialog.FileName;
		}

		private void buttonCreateImage_Click(object sender, EventArgs e)
		{
			ImageCreatorForm icf = new ImageCreatorForm();
			if (icf.ShowDialog() == DialogResult.OK)
				textBoxInputFilePath.Text = icf.CreatedImage;
		}

		private void numericUpDownDelay_ValueChanged(object sender, EventArgs e)
		{
			usbStickWriter.EjectionDelay = Convert.ToInt32(numericUpDownDelay.Value * 1000);
		}
	}
}