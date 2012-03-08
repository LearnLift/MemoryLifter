using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MLifterErrorHandler.Properties;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace MLifterErrorHandler.BusinessLayer
{
    /// <summary>
    /// Sends error reports to the webservice.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-07-15</remarks>
    internal static class ErrorReportSender
    {
        /// <summary>
        /// Gets the unsent path.
        /// </summary>
        /// <value>The unsent path.</value>
        /// <remarks>Documented by Dev02, 2009-07-15</remarks>
        static string UnsentPath
        {
            get { return Path.Combine(Program.AppdataPath, Settings.Default.AppDataFolderErrorReportsUnsent); }
        }

        /// <summary>
        /// Gets the archive path.
        /// </summary>
        /// <value>The archive path.</value>
        /// <remarks>Documented by Dev02, 2009-07-15</remarks>
        static string ArchivePath
        {
            get { return Path.Combine(Program.AppdataPath, Settings.Default.AppDataFolderErrorReportsArchive); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sending canceled by user].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [sending canceled by user]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2009-07-15</remarks>
        static bool SendingCanceledByUser
        {
            get;
            set;
        }

        /// <summary>
        /// This is to avoid multiple sending instances.
        /// </summary>
        static object SendingLock = new object();

        /// <summary>
        /// Sends the pending (previously not sendable) reports.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-07-15</remarks>
        internal static void SendPendingReports()
        {
            try
            {
                //send any unsent error reports
                DirectoryInfo unsent = new DirectoryInfo(UnsentPath);
                if (unsent.Exists)
                {
                    foreach (FileInfo unsentErrorReport in unsent.GetFiles())
                    {
                        if (unsentErrorReport.Extension.ToLowerInvariant() == Resources.ERRORFILE_EXTENSION.ToLowerInvariant())
                        {
                            if (DateTime.Now - unsentErrorReport.LastWriteTime <= new TimeSpan(Settings.Default.Transfer_TryDays, 0, 0, 0))
                                Transfer(unsentErrorReport.FullName, false);
                            else
                                ArchiveReport(unsentErrorReport.FullName);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Trace.WriteLine("Exception during sending of pending error reports:");
                Trace.WriteLine(exp.ToString());
            }
        }

        /// <summary>
        /// Adds a new report to the send list and tries to send it.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks>Documented by Dev02, 2009-07-15</remarks>
        internal static void SendReport(string path, bool showMessages)
        {
            if (!Directory.Exists(UnsentPath))
                Directory.CreateDirectory(UnsentPath);

            string newpath = Path.Combine(UnsentPath, Path.GetFileName(path));
            if (path != newpath)
            {
                File.Move(path, newpath);
                Transfer(newpath, showMessages);
            }
        }

        /// <summary>
        /// Archives an error report (directly - without sending it).
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks>Documented by Dev02, 2009-07-15</remarks>
        internal static void ArchiveReport(string path)
        {
            if (!Directory.Exists(ArchivePath))
                Directory.CreateDirectory(ArchivePath);

            string newpath = Path.Combine(ArchivePath, Path.GetFileName(path));
            try
            {
                if (path != newpath)
                    File.Move(path, newpath);
            }
            catch { }
        }

        /// <summary>
        /// Contains a list of pending threads.
        /// </summary>
        private static List<Thread> pendingThreads = new List<Thread>();

        /// <summary>
        /// Sleeps the current thread until all pending threads are finished.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-09-09</remarks>
        public static void WaitForThreads()
        {
            while (true)
            {
                lock (pendingThreads)
                    if (pendingThreads.Count < 1)
                        return;

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Transfers the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="showMessages">if set to <c>true</c> [show messages].</param>
        /// <remarks>Documented by Dev02, 2009-07-15</remarks>
        private static void Transfer(string file, bool showMessages)
        {
            Thread sendingFormThread = new Thread(new ThreadStart(delegate()
                {
                    try
                    {
                        lock (pendingThreads)
                            pendingThreads.Add(Thread.CurrentThread);

                        lock (SendingLock)
                        {
                            if (SendingCanceledByUser)
                                return;

                            ErrorReportSenderForm errorReportForm = new ErrorReportSenderForm(file, showMessages);
                            Application.Run(errorReportForm);
                            SendingCanceledByUser = errorReportForm.SendingCanceled;

                            if (errorReportForm.SendingSuccess)
                                ArchiveReport(errorReportForm.ReportFile);
                        }
                    }
                    catch (Exception exp)
                    {
                        Trace.WriteLine("Error in Error report sending form thread: " + Environment.NewLine + exp.ToString());
                    }
                    finally
                    {
                        lock (pendingThreads)
                            pendingThreads.Remove(Thread.CurrentThread);
                    }
                }));
            sendingFormThread.Name = "Error Report Sending Form Thread";
            sendingFormThread.IsBackground = false;
            sendingFormThread.Start();
        }
    }
}
