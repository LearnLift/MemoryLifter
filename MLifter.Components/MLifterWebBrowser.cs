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
using MLifter.Components.Properties;
using System.Diagnostics;
using System.Threading;

namespace MLifter.Components
{
    /// <summary>
    /// A customized webbrowser that catches new window events and opens them in the default browser instead.
    /// </summary>
    /// <remarks>Documented by Dev02, 2009-10-08</remarks>
    public class MLifterWebBrowser : WebBrowser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewWindowWebBrowser"/> class.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Windows.Forms.WebBrowser"/> control is hosted inside Internet Explorer.
        /// </exception>
        /// <remarks>Documented by Dev02, 2009-10-08</remarks>
        public MLifterWebBrowser()
            : base()
        { }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.HandleCreated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        /// <exception cref="T:System.Threading.ThreadStateException">
        /// The <see cref="P:System.Threading.Thread.ApartmentState"/> property of the application is not set to <see cref="F:System.Threading.ApartmentState.STA"/>.
        /// </exception>
        /// <remarks>Documented by Dev02, 2009-08-10</remarks>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.ActiveXInstance == null)
            {
                Trace.WriteLine(String.Format("Could not attach NewWindow event to webbrowser {0} because ActiveX instance is null.", this.Name));
                return;
            }

            //native com component access is necessary because the managed webbrowser wrapper does not provide the url for the newwindow event
            //code from: http://windowsteamblog.com/blogs/developers/archive/2009/07/12/windows-7-e-best-practices-for-isvs.aspx
            SHDocVw.WebBrowser webBrowser = (SHDocVw.WebBrowser)this.ActiveXInstance;
            webBrowser.NewWindow3 += new SHDocVw.DWebBrowserEvents2_NewWindow3EventHandler(webBrowser_NewWindow3);
        }

        private static Thread processStartThread = null;

        /// <summary>
        /// Handles the webbrowsers NewWindow3 event.
        /// </summary>
        /// <param name="ppDisp">The pp disp.</param>
        /// <param name="Cancel">if set to <c>true</c> [cancel].</param>
        /// <param name="dwFlags">The dw flags.</param>
        /// <param name="bstrUrlContext">The BSTR URL context.</param>
        /// <param name="bstrUrl">The BSTR URL.</param>
        /// <remarks>Documented by Dev02, 2009-08-10</remarks>
        private void webBrowser_NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            Cancel = true; //stop event from being processed by the browser itself

            //don't allow to start two processes simultaneously
            if (processStartThread != null && processStartThread.IsAlive)
                return;

            //start the browser process in a background thread to avoid application blocking
            processStartThread = new Thread(delegate()
            {
                try
                {
                    try
                    {
                        Process.Start(bstrUrl);
                    }
                    catch
                    {
                        //invoke is required for displaying the messagebox as modal form
                        if (this.Parent != null && this.Parent.IsHandleCreated)
                            this.Parent.Invoke((MethodInvoker)delegate()
                            {
                                MessageBox.Show(Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_TEXT, Resources.ERROR_LAUNCH_EXTERNAL_APPLICATION_CAPTION,
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            });
                    }
                }
                catch (Exception exp)
                {
                    Trace.WriteLine(String.Format("Exception in {0}: {1}", Thread.CurrentThread.Name, exp.ToString()));
                }
            });
            processStartThread.IsBackground = true;
            processStartThread.Name = "Browser process start thread";
            processStartThread.Start();
        }
    }
}
