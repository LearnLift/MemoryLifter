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
using System.IO;
using System.Threading;
using MLifterErrorHandler.Properties;
using System.Reflection;
using System.Diagnostics;

namespace MLifterErrorHandler.BusinessLayer
{
	public partial class ErrorReportSenderForm : Form
	{
		string reportfile = null;
		bool showMessages = false;

		NotifyIcon trayicon = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorReportSenderObject"/> class.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <param name="showMessages">if set to <c>true</c> [show messages].</param>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		public ErrorReportSenderForm(string file, bool showMessages)
		{
			if (!File.Exists(file))
				throw new FileNotFoundException();

			this.reportfile = file;
			this.showMessages = showMessages;

			this.Shown += new EventHandler(ErrorReportSenderForm_Shown);
			this.FormClosing += new FormClosingEventHandler(ErrorReportSenderForm_FormClosing);

			this.Region = new Region(new Rectangle(0, 0, 0, 0));

			InitializeComponent();
		}

		/// <summary>
		/// Handles the Shown event of the ErrorReportSenderForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		void ErrorReportSenderForm_Shown(object sender, EventArgs e)
		{
			trayicon = TrayIconCreate();

			Thread sendThread = new Thread(new ThreadStart(Send));
			sendThread.IsBackground = true;
			sendThread.Name = "Error Report Sending Data Thread";
			sendThread.Start();
		}

		/// <summary>
		/// Handles the FormClosing event of the ErrorReportSenderForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		void ErrorReportSenderForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (trayicon != null)
				TrayIconDispose(trayicon);
		}

		/// <summary>
		/// Gets the associated error report file.
		/// </summary>
		/// <value>The file.</value>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		public String ReportFile
		{
			get { return reportfile; }
		}

		/// <summary>
		/// Gets the sending process was canceled by the user.
		/// </summary>
		/// <value>The sending canceled.</value>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		public bool SendingCanceled
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [sending success].
		/// </summary>
		/// <value><c>true</c> if [sending success]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		public bool SendingSuccess
		{
			get;
			private set;
		}

		/// <summary>
		/// Sends this instance.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-07-15</remarks>
		private void Send()
		{
			TrayIconSetText(trayicon, Properties.Resources.ERROR_TRANSMIT_SENDING, showMessages);

			//begin with chunking and transfer
			ErrorReportService.ErrorReportService errorReportService = null;
			bool success = false;
			try
			{
				if (!SendingCanceled)
				{
					//read out report data
					byte[] buffer = File.ReadAllBytes(reportfile);
					string filename = Path.GetFileName(reportfile);

					ErrorReportHandler handler = new ErrorReportHandler(new FileInfo(reportfile));
					string email = String.Empty, description = String.Empty, stacktrace = String.Empty;
					try
					{
						email = handler.GetValue(Resources.ERRORREPORTPATH_USEREMAIL);
						description = handler.GetValue(Resources.ERRORREPORTPATH_USERDESCRIPTION);
					}
					catch (Exception exp)
					{
						Trace.WriteLine("Reading user data from ErrorReport Exception: " + exp.ToString());
					}
					try
					{
						stacktrace = handler.GetValue(Resources.ERRORREPORTPATH_MESSAGE) + Environment.NewLine +
							handler.GetValue(Resources.ERRORREPORTPATH_STACKTRACE);
					}
					catch (Exception exp)
					{
						Trace.WriteLine("Reading error data from ErrorReport Exception: " + exp.ToString());
					}

					//try to connect to webservice
					errorReportService = new ErrorReportService.ErrorReportService();
					errorReportService.Url = Properties.Settings.Default.MLifterErrorHandler_ErrorReportService_ErrorReportService;
					errorReportService.CookieContainer = new System.Net.CookieContainer();

					int chunksize = Settings.Default.Transfer_Chunksize;
					int partcount = Convert.ToInt32(Math.Ceiling(buffer.Length * 1.0 / chunksize));
					if (success = errorReportService.TransferStart(filename, buffer.Length, chunksize, email, description, stacktrace))
					{
						byte[] chunk = new byte[chunksize];
						for (int partno = 0; partno < partcount; partno++)
						{
							if (SendingCanceled)
							{
								success = false;
								break;
							}

							int length = partno == partcount - 1 ? buffer.Length % chunksize : chunksize;
							Buffer.BlockCopy(buffer, partno * chunksize, chunk, 0, length);

							if (!(success = errorReportService.TransferChunk(filename, chunk, partcount)))
							{
								break;
							}
							else
							{
								TrayIconSetText(trayicon, string.Format(Properties.Resources.ERROR_TRANSMIT_SENDSTAT, 1.0 * partno / partcount), false);
								TrayIconPercent(trayicon, 1.0 * partno / partcount);
							}
						}
						if (success)
						{
							success = errorReportService.TransferFinish(filename, partcount);
						}
					}
				}

				if (!success)
				{
					TrayIconSetText(trayicon, Properties.Resources.ERROR_TRANSMIT_SERVERERROR, showMessages);
				}
			}
			catch
			{
				success = false;
				TrayIconSetText(trayicon, Properties.Resources.ERROR_TRANSMIT_ERROR, showMessages);
			}
			finally
			{
				if (errorReportService != null)
					errorReportService.Abort();
			}

			SendingSuccess = success;

			if (!SendingSuccess && !SendingCanceled && showMessages) //in case an error message needs to be shown, delay the thread termination
			{
				//set the events to enable close form on click
				this.Invoke((MethodInvoker)delegate()
				{
					if (!success && showMessages)
						SetTrayIconClickCloseForm(trayicon);

				});

				//wait before closing the form anyway
				System.Threading.Thread.Sleep(10000);
			}

			this.Invoke((MethodInvoker)delegate()
			{
				if (this.IsHandleCreated && !this.IsDisposed)
					this.Close();
			});
		}

		#region TrayIcon functions

		/// <summary>
		/// Gets the assembly title.
		/// </summary>
		/// <value>The assembly title.</value>
		/// <remarks>Documented by Dev02, 2009-07-20</remarks>
		private string AssemblyTitle
		{
			get
			{
				return ((AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute))).Title;
			}
		}

		/// <summary>
		/// Gets the assembly description.
		/// </summary>
		/// <value>The assembly description.</value>
		/// <remarks>Documented by Dev02, 2009-07-20</remarks>
		private string AssemblyDescription
		{
			get
			{
				return ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute))).Description;
			}
		}

		/// <summary>
		/// Creates and shows a new trayicon.
		/// </summary>
		/// <returns>The created trayicon.</returns>
		/// <remarks>Documented by Dev02, 2007-11-21</remarks>
		private NotifyIcon TrayIconCreate()
		{
			NotifyIcon trayicon = new NotifyIcon();

			ContextMenuStrip contextMenu = new ContextMenuStrip();
			contextMenu.Tag = trayicon;
			contextMenu.Opening += new CancelEventHandler(contextMenu_Opening);
			trayicon.ContextMenuStrip = contextMenu;

			trayicon.BalloonTipTitle = AssemblyTitle;

			//trayicon must be visible and Text and percent must be set for the menu to be correctly displayed
			TrayIconSetText(trayicon, trayicon.BalloonTipTitle, false);
			TrayIconPercent(trayicon, 0);

			//fill the contextmenu for the first time
			contextMenu_Opening(trayicon.ContextMenuStrip, new CancelEventArgs(false));

			return trayicon;
		}

		/// <summary>
		/// Sets the tray icon to close the form on click.
		/// </summary>
		/// <param name="trayicon">The trayicon.</param>
		/// <remarks>Documented by Dev02, 2009-07-20</remarks>
		void SetTrayIconClickCloseForm(NotifyIcon trayicon)
		{
			trayicon.BalloonTipClicked += new EventHandler(TrayIconClicked);
			trayicon.Click += new EventHandler(TrayIconClicked);
		}

		/// <summary>
		/// Checks what to do in case the tray icon is clicked.
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-07-20</remarks>
		void TrayIconClicked(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Handles the Opening event of the contextMenu control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-11-21</remarks>
		void contextMenu_Opening(object sender, CancelEventArgs e)
		{
			//refreshes the entries in the contextmenu
			ContextMenuStrip contextMenu = sender as ContextMenuStrip;
			contextMenu.Items.Clear();

			ToolStripMenuItem info = new ToolStripMenuItem();
			info.Text = (contextMenu.Tag as NotifyIcon).BalloonTipText;
			info.Enabled = false;
			info.Image = Resources.MLIcon48;
			contextMenu.Items.Add(info);

			string cancelAllUploadsText = Properties.Resources.ERROR_TRANSMIT_CANCELUPLOAD;

			ToolStripMenuItem cancelAllUploads = new ToolStripMenuItem();
			cancelAllUploads.Text = cancelAllUploadsText;
			cancelAllUploads.Click += new EventHandler(cancelUpload_Click);
			cancelAllUploads.Image = Resources.trayicon_stop;
			contextMenu.Items.Add(cancelAllUploads);
		}

		/// <summary>
		/// Handles the Click event of the cancelUpload control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		/// <remarks>Documented by Dev02, 2007-11-21</remarks>
		void cancelUpload_Click(object sender, EventArgs e)
		{
			//signal all pending threads to cancel
			SendingCanceled = true;
		}

		/// <summary>
		/// Disposes the trayicon.
		/// </summary>
		/// <param name="trayicon">The trayicon.</param>
		/// <remarks>Documented by Dev02, 2007-11-21</remarks>
		private void TrayIconDispose(NotifyIcon trayicon)
		{
			if (trayicon != null)
			{
				trayicon.Icon = null;
				trayicon.Visible = false;
				trayicon.Dispose();
			}
			return;
		}

		/// <summary>
		/// Sets the tray icon Text and baloontiptext.
		/// </summary>
		/// <param name="trayicon">The trayicon.</param>
		/// <param name="Text">The Text.</param>
		/// <param name="showBaloon">if set to <c>true</c> [show baloon].</param>
		/// <remarks>Documented by Dev02, 2007-11-21</remarks>
		private void TrayIconSetText(NotifyIcon trayicon, string text, bool showBaloon)
		{
			this.Invoke((MethodInvoker)delegate()
			{
				if (trayicon != null && !string.IsNullOrEmpty(text))
				{
					if (text.Length < 60)
						trayicon.Text = text;
					else
						trayicon.Text = text.Substring(0, 60);
					trayicon.BalloonTipText = text;
					if (showBaloon)
						trayicon.ShowBalloonTip(10000);
				}
			});
		}

		/// <summary>
		/// Sets the tray icon progress state.
		/// </summary>
		/// <param name="trayicon">The trayicon.</param>
		/// <param name="percent">The progress percent.</param>
		/// <remarks>Documented by Dev02, 2007-11-21</remarks>
		private void TrayIconPercent(NotifyIcon trayicon, double percentdouble)
		{
			this.Invoke((MethodInvoker)delegate()
			{
				if (trayicon != null)
				{
					if (percentdouble < 0 || percentdouble > 1)
						return;

					using (Bitmap border = Resources.trayicon_frame)
					{
						using (Bitmap filling = new Bitmap(border.Width, border.Height))
						{
							Graphics g = Graphics.FromImage(filling);

							//draw filling rectangle
							Brush fillBrush = new SolidBrush(Settings.Default.NotifyIcon_FillColor);
							int fillOffset = 2;
							int fillHeight = Convert.ToInt32(1D * (border.Height - 2 * fillOffset) * percentdouble) + fillOffset;
							g.FillRectangle(fillBrush, 0, filling.Height - fillHeight, filling.Width, fillHeight);

							//overlay border
							g.DrawImageUnscaled(border, 0, 0);

							//overlay percentage text
							/*string percentText = Convert.ToString(Math.Round(percentdouble * 100, 0));
							Font percentFont = new Font("Arial", 7, FontStyle.Regular);
							SizeF percentSize = g.MeasureString(percentText, percentFont);
							g.DrawString(percentText, percentFont, Brushes.White,
								(filling.Width - percentSize.Width) / 2, (filling.Height - percentSize.Height) / 2);*/

							//apply as tray icon
							trayicon.Icon = Icon.FromHandle(filling.GetHicon());

							if (!trayicon.Visible)
								trayicon.Visible = true;
						}
					}
				}
			});
		}

		#endregion
	}
}
