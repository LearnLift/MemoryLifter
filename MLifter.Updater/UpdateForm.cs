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
using System.Threading;
using System.Net;
using MLifterUpdater.Properties;

namespace MLifterUpdater
{
    public partial class UpdateForm : Form
    {
        public MLifterUpdateHandler UpdateHandler { get; set; }

        public UpdateForm(MLifterUpdateHandler updateHandler, Version newVersion)
        {
            UpdateHandler = updateHandler;

            InitializeComponent();

            labelTitle.Text = string.Format(Resources.HEADER, newVersion.ToString(3));
            textBoxDescription.Rtf = Resources.DESCRIPTION;
            labelDownloadTitle.Text = string.Format(Resources.TITLE, newVersion.ToString(3));
            labelDownloadNote.Text = Resources.NOTE;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            textBoxDescription.Visible = false;
            buttonCancel.Enabled = false;
            buttonUpdate.Enabled = false;

            Thread updater = new Thread(new ThreadStart(delegate { UpdateHandler.DownloadFile(UpdateProgress); }));
            updater.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            updater.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            updater.IsBackground = true;
            updater.Name = "Download Update";
            updater.Priority = ThreadPriority.BelowNormal;
            updater.Start();
        }

        private void UpdateProgress(DownloadProgressChangedEventArgs e)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { UpdateProgress(e); });
            else
            {
                labelProgress.Text = string.Format("{0} of {1}", GetFileSize(e.BytesReceived, true), GetFileSize(e.TotalBytesToReceive, true));
                progressBarDownload.Value = e.ProgressPercentage;
            }
        }

        /// <summary>
        /// Gets the size of the file in a nice string.
        /// </summary>
        /// <param name="Bytes">The bytes.</param>
        /// <param name="showDecimalPlaces">if set to <c>true</c> show decimal places.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-26</remarks>
        public static string GetFileSize(long Bytes, bool showDecimalPlaces)
        {
            if (Bytes >= 1073741824)
            {
                Decimal size = Decimal.Divide(Bytes, 1073741824);
                return String.Format("{0:##" + (showDecimalPlaces ? ".##" : string.Empty) + "} GB", size);
            }
            else if (Bytes >= 1048576)
            {
                Decimal size = Decimal.Divide(Bytes, 1048576);
                return String.Format("{0:##" + (showDecimalPlaces ? ".##" : string.Empty) + "} MB", size);
            }
            else if (Bytes >= 1024)
            {
                Decimal size = Decimal.Divide(Bytes, 1024);
                return String.Format("{0:##" + (showDecimalPlaces ? ".##" : string.Empty) + "} KB", size);
            }
            else if (Bytes > 0)
            {
                Decimal size = Bytes;
                return showDecimalPlaces ? String.Format("{0:##" + (showDecimalPlaces ? ".##" : string.Empty) + "} Bytes", size) : "1 KB";
            }
            else
            {
                return showDecimalPlaces ? "0 Bytes" : "-";
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            UpdateHandler.CancelUpdate();
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {

        }
    }
}
