using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace MLifter.Classes
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Documented by Dev09, 2009-03-06</remarks>
    internal class Copy
    {
        MLifter.Controls.LoadStatusMessage loadStatusMessage = new MLifter.Controls.LoadStatusMessage(string.Empty, 100, true);

        /// <summary>
        /// Saves a copy of a file to the given target path.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <remarks>Documented by Dev09, 2009-03-06</remarks>
        internal void SaveCopyAs(string sourcePath, string targetPath)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                loadStatusMessage.SetProgress(0);
                loadStatusMessage.InfoMessage = MLifter.Properties.Resources.PROGRESS_MESSAGE_SAVE_AS;
                loadStatusMessage.Show();

                MLifter.DAL.Helper.SaveCopyToProgressChanged += new MLifter.DAL.Tools.StatusMessageEventHandler(Helper_SaveCopyToProgressChanged);
                MLifter.DAL.Helper.SaveCopyTo(sourcePath, targetPath, true);
            }
            finally
            {
                loadStatusMessage.Close();
                Cursor.Current = Cursors.Default;
            }
        }

        void Helper_SaveCopyToProgressChanged(object sender, MLifter.DAL.Tools.StatusMessageEventArgs args)
        {
            loadStatusMessage.SetProgress(args.ProgressPercentage);
        }
    }
}
