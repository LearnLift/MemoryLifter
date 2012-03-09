using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MLifterErrorHandler.Properties;
using System.Windows.Forms;
using System.Threading;

namespace MLifterErrorHandler
{
    class Program
    {
        /// <summary>
        /// The mutex to ensure that only once instance runs at a time.
        /// </summary>
        private static Mutex errorHandlerMutex;

        /// <summary>
        /// The program entry point.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <remarks>Documented by Dev02, 2009-07-16</remarks>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            #region Only allow one instance [ML-2448]
            try
            {
                bool firstInstance;
                errorHandlerMutex = new Mutex(false, "Local\\MLifterErrorHandlerMutex", out firstInstance);
                if (!firstInstance)
                    return;
            }
            catch
            { }
            #endregion

            #region Parse command line arguments
            bool fatal = RunningFromStick = false;

            try
            {
                if (args.Length >= 2)
                {
                    fatal = Convert.ToBoolean(args[0]);
                    RunningFromStick = Convert.ToBoolean(args[1]);
                }
            }
            catch (System.FormatException)
            { }
            #endregion

            //check if there are new error reports to ask the user
            string newErrorReportPath = Path.Combine(AppdataPath, Settings.Default.AppDataFolderErrorReportsNew);
            bool newFound = false;
            if (Directory.Exists(newErrorReportPath))
            {
                DirectoryInfo container = new DirectoryInfo(newErrorReportPath);
                foreach (FileInfo report in container.GetFiles())
                {
                    if (report.Extension.ToLowerInvariant() == Resources.ERRORFILE_EXTENSION.ToLowerInvariant())
                    {
                        newFound = true;
                        ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(fatal, report);
                        Application.Run(errorHandlerForm);
                    }
                }
            }

            //else send old reports
            if (!newFound)
                BusinessLayer.ErrorReportSender.SendPendingReports();

            //wait for the pending threads before exiting main thread (to maintain mutex)
            BusinessLayer.ErrorReportSender.WaitForThreads();

        }

        /// <summary>
        /// Gets the appdata path, depending on location.
        /// </summary>
        /// <value>The appdata path.</value>
        /// <remarks>Documented by Dev02, 2009-07-15</remarks>
        internal static string AppdataPath
        {
            get
            {
                if (RunningFromStick)
                    return Application.StartupPath;
                else
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Settings.Default.AppDataFolder);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [running from stick].
        /// </summary>
        /// <value><c>true</c> if [running from stick]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2009-07-16</remarks>
        private static bool RunningFromStick { get; set; }
    }
}
