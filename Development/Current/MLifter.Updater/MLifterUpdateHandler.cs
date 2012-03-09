using System;
using System.Collections.Generic;
using System.Text;
using MLifterUpdateHandler;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MLifterUpdater
{
    [UpdateHandler]
    public class MLifterUpdateHandler : IUpdateHandler
    {
        private string downloadPath;
        private string localPath;

        public MLifterUpdateHandler()
        {
            FileDownloaded += new EventHandler(MLifterUpdateHandler_FileDownloaded);
        }

        #region IUpdateHandler Members

        public void StartUpdateProcess(Version newVersion)
        {
            localPath = Path.Combine(Path.GetTempPath(), "MLifter" + newVersion.Major + newVersion.Minor + "Setup.exe");
            downloadPath = @"http://services.memorylifter.com/update/program/" + newVersion.ToString(3) + "/MemoryLifter-Setup.exe";

            UpdateForm updateForm = new UpdateForm(this, newVersion);
            updateForm.ShowDialog();
        }

        #endregion

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);
        [Flags]
        enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 0x00000001,
            MOVEFILE_COPY_ALLOWED = 0x00000002,
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
            MOVEFILE_WRITE_THROUGH = 0x00000008,
            MOVEFILE_CREATE_HARDLINK = 0x00000010,
            MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x00000020
        }

        public delegate void ProgressStatusUpdate(DownloadProgressChangedEventArgs e);
        /// <summary>
        /// Occurs when file downloaded.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-07-15</remarks>
        public event EventHandler FileDownloaded;
        /// <summary>
        /// Raises the <see cref="E:FileDownloaded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-07-15</remarks>
        protected virtual void OnFileDownloaded(EventArgs e)
        {
            if (FileDownloaded != null)
                FileDownloaded(this, e);
        }

        private ProgressStatusUpdate currentUpdateDelegate;
        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="updateDelegate">The update delegate.</param>
        /// <remarks>Documented by Dev05, 2009-07-15</remarks>
        public void DownloadFile(ProgressStatusUpdate updateDelegate)
        {
            if (File.Exists(localPath))
            {
                int i = 1;
                while (File.Exists(Path.ChangeExtension(localPath, "_" + i + ".exe"))) i++;
                localPath = Path.ChangeExtension(localPath, "_" + i + ".exe");
            }

            Uri url = new Uri(downloadPath);

            currentUpdateDelegate = updateDelegate;
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(url, localPath);

            MoveFileEx(localPath, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            currentUpdateDelegate.Invoke(e);
        }
        private void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            OnFileDownloaded(EventArgs.Empty);
        }

        /// <summary>
        /// Handles the FileDownloaded event of the MLifterUpdateHandler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-07-15</remarks>
        private void MLifterUpdateHandler_FileDownloaded(object sender, EventArgs e)
        {
            Process.Start(localPath);
            Environment.Exit(-1);
        }

        /// <summary>
        /// Cancels the update.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-07-15</remarks>
        public void CancelUpdate()
        {
            Process.Start(Path.Combine(Application.StartupPath, "MemoryLifter.exe"), "--no-autoupdate");
            Environment.Exit(-1);
        }
    }
}
