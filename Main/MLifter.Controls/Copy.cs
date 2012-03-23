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
using System.ComponentModel;
using System.Windows.Forms;
using MLifter.Controls.Properties;

namespace MLifter.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Documented by Dev09, 2009-03-06</remarks>
    public class Copy
    {
        MLifter.Controls.LoadStatusMessage loadStatusMessage = new MLifter.Controls.LoadStatusMessage(string.Empty, 100, true);

        /// <summary>
        /// Saves a copy of a file to the given target path.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <remarks>Documented by Dev09, 2009-03-06</remarks>
        public void SaveCopyAs(string sourcePath, string targetPath)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                loadStatusMessage.SetProgress(0);
                loadStatusMessage.InfoMessage = Resources.PROGRESS_MESSAGE_SAVE_AS;
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
