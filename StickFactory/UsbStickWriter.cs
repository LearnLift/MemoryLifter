using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace StickFactory
{
	/// <summary>
	/// Main Class for writing data on the USB-drives/sticks
	/// </summary>
	/// <remarks>Documented by Dev08, 2008-10-08</remarks>
	class UsbStickWriter
	{
		private List<DriveInfo> usbSticks = new List<DriveInfo>();
		private List<Thread> subThreads = new List<Thread>();

		private List<byte[]> zipFileContentList = new List<byte[]>();
		private List<string> zipFilenames = new List<string>();
		private List<long> zipFileSizes = new List<long>();
		private List<StickFactory.MainForm.FileDirType> zipFileType = new List<StickFactory.MainForm.FileDirType>();
		private List<FileAttributes> zipFileAttributes = new List<FileAttributes>();
		private Dictionary<string, FileAttributes> fileAttributes = new Dictionary<string, FileAttributes>();
		private Dictionary<string, FileAttributes> folderAttributes = new Dictionary<string, FileAttributes>();
		private bool copyContent = false;
		private bool checkContent = false;

		#region properties
		public Dictionary<string, FileAttributes> FileAttributes { get { return fileAttributes; } set { fileAttributes = value; } }
		public Dictionary<string, FileAttributes> FolderAttributes { get { return folderAttributes; } set { folderAttributes = value; } }

		public List<byte[]> ZipFileContentList
		{
			get
			{
				return zipFileContentList;
			}
		}

		public List<string> ZipFilenames
		{
			get
			{
				return zipFilenames;
			}
		}

		public List<long> ZipFileSizes
		{
			get
			{
				return zipFileSizes;
			}
		}

		public List<StickFactory.MainForm.FileDirType> ZipFileType
		{
			get
			{
				return zipFileType;
			}
		}

		public List<FileAttributes> ZipFileAttributes
		{
			get
			{
				return zipFileAttributes;
			}
		}

		private int ejectionDelay = 10000;
		public int EjectionDelay
		{
			get { return ejectionDelay; }
			set { ejectionDelay = value; }
		}

		private string stickName;
		/// <summary>
		/// Gets or sets the name of the stick.
		/// </summary>
		/// <value>The name of the stick.</value>
		/// <remarks>Documented by Dev05, 2008-10-10</remarks>
		public string StickName
		{
			get { return stickName; }
			set { stickName = value; }
		}

		private string stickId;
		/// <summary>
		/// Gets or sets the stick ID.
		/// </summary>
		/// <value>The stick ID.</value>
		/// <remarks>Documented by Dev05, 2008-10-10</remarks>
		public string StickID
		{
			get { return stickId; }
			set { stickId = value; }
		}


		/// <summary>
		/// Gets or sets a value indicating whether to [copy the content].
		/// </summary>
		/// <value><c>true</c> if [copy content]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-10-09</remarks>
		public bool CopyContent
		{
			get { return copyContent; }
			set { copyContent = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to [check the content].
		/// </summary>
		/// <value><c>true</c> if [check content]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2008-10-09</remarks>
		public bool CheckContent
		{
			get { return checkContent; }
			set { checkContent = value; }
		}
		#endregion

		public delegate void UsbStickWriteMessageEventHandler(object sender, UsbStickWriteMessageEventArgs e);

		/// <summary>
		/// Occurs when the usb stick formatting is finished.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public event UsbStickWriteMessageEventHandler UsbStickFormattingFinish;
		/// <summary>
		/// Occurs when a usb stick write is finished.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public event EventHandler UsbStickWriteFinish;      //this is for external use
		/// <summary>
		/// Occurs when a usb stick write begins.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public event EventHandler UsbStickWriteBegin;       //---""---
		/// <summary>
		/// Occurs when a usb stick write processes a file/folder.
		/// </summary>
		/// <remarks>
		/// CFI, 2012-02-24
		/// </remarks>
		public event EventHandler UsbStickWriteProcess;     //---""---
		/// <summary>
		/// Occurs when a usb stick write was aborted.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public event EventHandler UsbStickWriteAbort;
		/// <summary>
		/// Occurs when a usb stick content was written.
		/// </summary>
		/// <remarks>
		/// CFI, 2012-02-24
		/// </remarks>
		public event UsbStickWriteMessageEventHandler UsbStickContentWritten;
		/// <summary>
		/// Occurs when a usb stick id was set.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public event UsbStickWriteMessageEventHandler UsbStickIdSet;
		/// <summary>
		/// Occurs when a usb stick writer has a status message.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public event UsbStickWriteMessageEventHandler UsbStickWriteStatusMessage;
		/// <summary>
		/// Occurs when a usb stick writer has an error.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public event UsbStickWriteMessageEventHandler UsbStickWriteError;

		/// <summary>
		/// Initializes a new instance of the <see cref="UsbStickWriter"/> class.
		/// </summary>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		public UsbStickWriter()
		{
			CleanUp();
		}

		/// <summary>
		/// Adds an usb stick.
		/// </summary>
		/// <param name="Drive">The Drive.</param>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		public void AddUsbStick(DriveInfo drive)
		{
			usbSticks.Add(drive);

			Thread subThread = new Thread(SubThreadProcess);
			subThread.Name = drive.RootDirectory.ToString();            //is necessary to identify a aborted process (see AbortWriting())
			subThread.IsBackground = true;
			subThread.Start((object)drive);

			subThreads.Add(subThread);
		}

		/// <summary>
		/// Aborts the writing.
		/// </summary>
		/// <remarks>CFI, 2012-02-24</remarks>
		public void AbortWriting()
		{
			foreach (Thread subThread in subThreads)
			{
				if (subThread.IsAlive)
				{
					subThread.Abort();
					if (UsbStickWriteAbort != null)
						UsbStickWriteAbort(new UsbProgress(0, 0, new DriveInfo(subThread.Name)), EventArgs.Empty);
				}
			}

			CleanUp();
		}
		
		private void CleanUp()
		{
			zipFileAttributes.Clear();
			zipFileContentList.Clear();
			zipFilenames.Clear();
			zipFileSizes.Clear();
			zipFileType.Clear();
		}

		/// <summary>
		/// Starts the writing on the USB Sticks.
		/// </summary>
		/// <remarks>Documented by Dev08, 2008-10-08</remarks>
		private void SubThreadProcess(object drive)
		{
			OnSubThreadBegin(drive);

			DriveInfo driveInfo = drive as DriveInfo;
			string root = driveInfo.RootDirectory.ToString();

			#region COPY
			if (CopyContent)
			{
				//1. Delete everything on Stick
				OnSubThreadStatusMessage("Formatting drive...", driveInfo);
				try
				{
					FormatDrive(root);
				}
				catch (Exception e) { OnSubThreadError("Could not format drive! --> " + e.ToString(), driveInfo); return; }
				OnSubThreadFormattingFinish(driveInfo);

				//2. Write files from zip file onto the stick
				int counter = 0;
				foreach (string filename in zipFilenames)          //Write the structure of the ZipFile
				{
					try
					{
						string absoluteFilename = Path.Combine(root, filename);

						if (zipFileType[counter] == StickFactory.MainForm.FileDirType.Directory)
						{
							Directory.CreateDirectory(absoluteFilename);

							if (folderAttributes.ContainsKey(filename.Replace('/', '\\').Trim(new char[] { '\\', '/' })))
								(new DirectoryInfo(absoluteFilename.Trim(new char[] { '\\', '/' }))).Attributes = folderAttributes[filename.Replace('/', '\\').Trim(new char[] { '\\', '/' })];
						}
						else
						{
							FileStream fs = File.Create(absoluteFilename);
							fs.Write(zipFileContentList[counter], 0, Convert.ToInt32(zipFileSizes[counter]));
							fs.Close();

							//File.AppendAllText("log.txt", string.Format("Setting on File {0}: {1}\r\n", absoluteFilename, fileAttributes[filename].ToString()));
							if (fileAttributes.ContainsKey(filename))
								File.SetAttributes(absoluteFilename, fileAttributes[filename]);
						}
					}
					catch (FileNotFoundException) { OnSubThreadError("File not found! Probably the stick was removed...", driveInfo); return; }
					catch (DirectoryNotFoundException) { OnSubThreadError("Directory not found! Probably the stick was removed...", driveInfo); return; }
					catch (IOException e) { OnSubThreadError("Could not access File! --> " + e.ToString(), driveInfo); return; }

					counter++;
					OnSubThreadWriting(counter, zipFilenames.Count, (DriveInfo)drive);
				}
				OnSubThreadContentWritten(driveInfo);

				//3. Set VolumeId:
				SetVolumeId(root, StickID);
				OnSubThreadIdSet(driveInfo);
			}
			#endregion
			#region CHECK
			if (CheckContent)
			{

				//1. Check volume serial
				string expectedVolumeSerial = StickID.Replace("-", "");
				string volumeSerial = GetVolumeSerial((DriveInfo)drive);
				if (expectedVolumeSerial != volumeSerial)
				{
					OnSubThreadError(string.Format("Volume serial wrong: expected {0}, actually {1}",
						expectedVolumeSerial, volumeSerial), (DriveInfo)drive);
					return;
				}

				//2. Check file contents
				int counter = 0;
				foreach (string filename in zipFilenames)
				{
					bool fileSuccessful = true;
					string absoluteFilename = Path.Combine(root, filename);
					string fileError = string.Empty;

					if (zipFileType[counter] == StickFactory.MainForm.FileDirType.Directory)
					{
						if (!Directory.Exists(absoluteFilename))
						{
							fileSuccessful = false;
							fileError = "Directory does not exist: " + absoluteFilename;
						}
						else
						{
							try
							{
								DirectoryInfo folder = new DirectoryInfo(absoluteFilename);
								if (folderAttributes.ContainsKey(filename.Replace('/', '\\').Trim(new char[] { '\\', '/' })))
								{
									if (folder.Attributes != folderAttributes[filename.Replace('/', '\\').Trim(new char[] { '\\', '/' })])
									{
										fileSuccessful = false;
										fileError = "Folder Attributes does not match: " + absoluteFilename;
									}
								}
								else
								{
									if (!(folder.Attributes == System.IO.FileAttributes.Normal || folder.Attributes == System.IO.FileAttributes.Archive || folder.Attributes == System.IO.FileAttributes.Directory))
									{
										fileSuccessful = false;
										fileError = "Folder Attributes does not match: " + absoluteFilename;
									}
								}
							}
							catch (FileNotFoundException) { OnSubThreadError("File not found! Probably the stick was removed...", (DriveInfo)drive); return; }
							catch (DirectoryNotFoundException) { OnSubThreadError("Directory not found! Probably the stick was removed...", (DriveInfo)drive); return; }
						}
					}
					else
					{
						if (!File.Exists(absoluteFilename))
						{
							fileSuccessful = false;
							fileError = "File does not exist: " + absoluteFilename;
						}
						else
						{
							try
							{
								FileInfo file = new FileInfo(absoluteFilename);
								if (file.Length != zipFileSizes[counter])
								{
									fileSuccessful = false;
									fileError = "File size does not correspond: " + absoluteFilename;
								}
								if (fileAttributes.ContainsKey(filename))
								{
									if (file.Attributes != fileAttributes[filename])
									{
										fileSuccessful = false;
										fileError = "File Attributes does not match: " + absoluteFilename;
									}
								}
								else
								{
									if (!(file.Attributes == System.IO.FileAttributes.Normal || file.Attributes == System.IO.FileAttributes.Archive))
									{
										fileSuccessful = false;
										fileError = "File Attributes does not match: " + absoluteFilename;
									}
								}
							}
							catch (FileNotFoundException) { OnSubThreadError("File not found! Probably the stick was removed...", (DriveInfo)drive); return; }
							catch (DirectoryNotFoundException) { OnSubThreadError("Directory not found! Probably the stick was removed...", (DriveInfo)drive); return; }
						}
					}
					if (!fileSuccessful)
					{
						OnSubThreadError(fileError, (DriveInfo)drive);
						return; //cancel checking process
					}

					counter++;
					if (counter % 25 == 0)
						OnSubThreadWriting(counter, zipFilenames.Count, (DriveInfo)drive);
				}

				//3. Check for any other (unwanted) files/directories
				int filecount = 0;
				int directorycount = 0;
				List<string> imagedirs = new List<string>();
				counter = 0;
				foreach (string filename in zipFilenames)
				{
					if (zipFileType[counter] == StickFactory.MainForm.FileDirType.Directory)
					{
						directorycount++;
						imagedirs.Add(filename);
					}
					else
						filecount++;

					++counter;
				}

				DirectoryInfo driveRoot = ((DriveInfo)drive).RootDirectory;
				FileInfo[] foundfiles = driveRoot.GetFiles("*", SearchOption.AllDirectories);
				int foundfilecount = foundfiles.Length;
				DirectoryInfo[] founddirs = driveRoot.GetDirectories("*", SearchOption.AllDirectories);
				int founddirectorycount = founddirs.Length;

				if (foundfilecount != filecount)
				{
					OnSubThreadError(string.Format("File count wrong: expected {0}, actually {1}",
						filecount, foundfilecount), (DriveInfo)drive);
					return;
				}

				if (founddirectorycount != directorycount)
				{
					bool overrideFailded = true;
					foreach (DirectoryInfo info in founddirs)
					{
						string pathToCheck = info.FullName.Remove(0, 3).Replace(@"\", @"/") + @"/";
						if (!imagedirs.Contains(pathToCheck))
						{
							if (info.GetFiles("*", SearchOption.TopDirectoryOnly).Length != 0)
							{
								overrideFailded = false;
								break;
							}
						}
					}

					if (!overrideFailded)
					{
						OnSubThreadError(string.Format("Directory count wrong: expected {0}, actually {1}",
							directorycount, founddirectorycount), (DriveInfo)drive);
						return;
					}
				}
			}
			#endregion

			Thread.Sleep(ejectionDelay);

			EjectDrive(root);

			OnSubThreadFinished(drive);
		}

		/// <summary>
		/// Gets the volume serial.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev02, 2008-09-16</remarks>
		private string GetVolumeSerial(DriveInfo drive)
		{
			try
			{
				string driveLetter = drive.Name.Substring(0, 1);
				ManagementObject diskObj = new ManagementObject(string.Format("win32_logicaldisk.deviceid=\"{0}:\"", driveLetter));
				diskObj.Get();
				return diskObj["VolumeSerialNumber"].ToString();
			}
			catch
			{ }

			return string.Empty;
		}

		/// <summary>
		/// Formats the Drive.
		/// </summary>
		/// <param name="Drive">The Drive.</param>
		/// <remarks>Documented by Dev05, 2008-10-09</remarks>
		private void FormatDrive(string drive)
		{
			Process prc = new Process();
			prc.StartInfo = new ProcessStartInfo("cmd", string.Format("/c format {0}: /FS:FAT /q /x /y /v:{1}", drive[0], StickName));
			prc.StartInfo.CreateNoWindow = true;
			prc.StartInfo.UseShellExecute = false;
			prc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			prc.Start();

			while (!prc.HasExited) Thread.Sleep(10);
		}

		/// <summary>
		/// Sets the volume id.
		/// </summary>
		/// <param name="Drive">The Drive.</param>
		/// <param name="volumeId">The volume id (D4FC-44F7).</param>
		/// <remarks>Documented by Dev05, 2008-10-09</remarks>
		private void SetVolumeId(string drive, string volumeId)
		{
			Process prc = new Process();
			prc.StartInfo = new ProcessStartInfo(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "vid.exe"), drive[0] + ": " + volumeId);
			prc.StartInfo.CreateNoWindow = true;
			prc.StartInfo.UseShellExecute = true;
			prc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			prc.Start();

			while (!prc.HasExited) Thread.Sleep(10);
		}

		/// <summary>
		/// Ejects the Drive.
		/// </summary>
		/// <param name="Drive">The Drive.</param>
		/// <remarks>Documented by Dev05, 2008-10-09</remarks>
		private void EjectDrive(string drive)
		{
			Process ecj = new Process();
			ecj.StartInfo = new ProcessStartInfo(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "usbe.exe"), "/REMOVELETTER " + drive[0]);
			ecj.StartInfo.CreateNoWindow = true;
			ecj.StartInfo.UseShellExecute = true;
			ecj.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			ecj.Start();
		}

		/// <summary>
		/// Called when the sub thread is writing a file/folder.
		/// </summary>
		/// <param name="current">The current.</param>
		/// <param name="total">The total.</param>
		/// <param name="drive">The drive.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void OnSubThreadWriting(int current, int total, DriveInfo drive)
		{
			if (UsbStickWriteProcess != null)
				UsbStickWriteProcess(new UsbProgress(current, total, drive), EventArgs.Empty);
		}
		/// <summary>
		/// Called when the sub thread has an error.
		/// </summary>
		/// <param name="error">The error.</param>
		/// <param name="drive">The drive.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void OnSubThreadError(string error, DriveInfo drive)
		{
			if (UsbStickWriteError != null)
				UsbStickWriteError(this, new UsbStickWriteMessageEventArgs(error, drive));
		}
		/// <summary>
		/// Called when the sub thread is finished with formatting.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void OnSubThreadFormattingFinish(DriveInfo drive)
		{
			if (UsbStickFormattingFinish != null)
				UsbStickFormattingFinish(this, new UsbStickWriteMessageEventArgs(string.Empty, drive));
		}
		/// <summary>
		/// Called when the sub thread set the id.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void OnSubThreadIdSet(DriveInfo drive)
		{
			if (UsbStickIdSet != null)
				UsbStickIdSet(this, new UsbStickWriteMessageEventArgs(string.Empty, drive));
		}
		/// <summary>
		/// Called when the sub thread has the content written.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void OnSubThreadContentWritten(DriveInfo drive)
		{
			if (UsbStickContentWritten != null)
				UsbStickContentWritten(this, new UsbStickWriteMessageEventArgs(string.Empty, drive));
		}
		/// <summary>
		/// Called when the sub thread has a status message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="drive">The drive.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void OnSubThreadStatusMessage(string message, DriveInfo drive)
		{
			if (UsbStickWriteStatusMessage != null)
				UsbStickWriteStatusMessage(this, new UsbStickWriteMessageEventArgs(message, drive));
		}
		/// <summary>
		/// Called when a sub thread begins working.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>CFI, 2012-02-24</remarks>
		private void OnSubThreadBegin(object drive)
		{
			if (UsbStickWriteBegin != null)
				UsbStickWriteBegin(drive, EventArgs.Empty);
		}
		/// <summary>
		/// Called when the sub thread finished.
		/// </summary>
		/// <param name="drive">The drive.</param>
		/// <remarks>
		/// CFI, 2012-02-24
		/// </remarks>
		private void OnSubThreadFinished(object drive)
		{
			if (UsbStickWriteFinish != null)
				UsbStickWriteFinish(drive, EventArgs.Empty);
		}
	}

	public struct UsbProgress
	{
		public int Current;
		public int Total;
		public DriveInfo Drive;

		public UsbProgress(int current, int total, DriveInfo drive)
		{
			Current = current;
			Total = total;
			Drive = drive;
		}
	}

	public class UsbStickWriteMessageEventArgs : EventArgs
	{
		public string Message;
		public DriveInfo Drive;

		public UsbStickWriteMessageEventArgs(string error, DriveInfo drive)
		{
			this.Message = error;
			this.Drive = drive;
		}
	}
}
