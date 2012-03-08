using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MLifterErrorHandler.Properties;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using MLifterErrorHandler.BusinessLayer;

namespace MLifterErrorHandler
{
    /// <summary>
    /// This form shows some information about the error, the 
    /// error report and the options which a user have if
    /// an error occured
    /// </summary>
    /// <remarks>Documented by Dev07, 2009-07-15</remarks>
    public partial class ErrorHandlerForm : Form
    {
        /// <summary>
        /// height of the form if detailed information are shown
        /// </summary>
        private int ExpandedSize = 535;
        private bool Fatal = false;
        private FileInfo report = null;
        private ErrorReportHandler errorReportHandler = null;
        /// <summary>
        /// height of the form if no detailed information are shown
        /// </summary>
        private int NotExpandedSize = 345;
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlerForm"/> class.
        /// </summary>
        /// <remarks>Documented by Dev07, 2009-07-15</remarks>
        public ErrorHandlerForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlerForm"/> class.
        /// </summary>
        /// <param name="fatal">if set to <c>true</c> [fatal].</param>
        /// <param name="report">The report.</param>
        /// <remarks>Documented by Dev07, 2009-07-16</remarks>
        public ErrorHandlerForm(bool fatal, FileInfo report)
        {
            this.Fatal = fatal;
            this.report = report;
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the ErrorHandlerForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-07-15</remarks>
        private void ErrorHandlerForm_Load(object sender, EventArgs e)
        {
            this.Height = NotExpandedSize;

            // Load the appropriate text in the header
            if (Fatal)
                labelErrorOccuredInformationMessage.Text = Resources.ERROR_OCCURED_INFORMATION_MESSAGE_FATAL;
            else
                labelErrorOccuredInformationMessage.Text = Resources.ERROR_OCCURED_INFORMATION_MESSAGE_NOT_FATAL;

            //restart MemoryLifter
            checkBoxRestartMemoryLifter.Checked = Fatal;

            // Load the zip file and display the content in the list view
            using (Stream stream = report.Open(FileMode.Open))
            {
                if (stream != null)
                {
                    foreach (ZipEntry i in GetZipContent(stream))
                    {
                        ListViewItem listViewItem = new ListViewItem();
                        listViewItem.Tag = i.Name;
                        listViewItem.ToolTipText = i.Name;
                        listViewItem.Text = string.Format("{0} ({1:0.0} KB)", i.Name, i.Size / 1024D);
                        listViewItem.Checked = i.Name.StartsWith("ErrorReport.") ? true : false;
                        listViewFiles.Items.Add(listViewItem);
                    }
                }
            }

            errorReportHandler = new ErrorReportHandler(report);
            try
            {
                labelErrorMessage.Text = errorReportHandler.GetValue(Resources.ERRORREPORTPATH_MESSAGE);
            }
            catch (Exception exp)
            {
                Trace.WriteLine("Reading error data from ErrorReport Exception: " + exp.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonSend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-07-15</remarks>
        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                PrepareErrorReport();
                BusinessLayer.ErrorReportSender.SendReport(report.FullName, true);
            }
            catch (Exception ex)
            {
                InternalErrorHandler(ex, false);
            }
            finally
            {
                Close();
                RestartMemoryLifter();
            }
        }

        /// <summary>
        /// Prepares the error report.
        /// (Expands it with the user's information and removes unchecked files.)
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-07-16</remarks>
        private void PrepareErrorReport()
        {
            try
            {
                errorReportHandler.SetValue(Resources.ERRORREPORTPATH_USEREMAIL, textBoxEmailAddress.Text);
                errorReportHandler.SetValue(Resources.ERRORREPORTPATH_USERDESCRIPTION, textBoxAdditionalInformation.Text);
                errorReportHandler.CommitUpdates();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error during adding user fields to error report:");
                Trace.WriteLine(ex.ToString());
            }

            try
            {
                //remove unchecked files
                foreach (ListViewItem item in listViewFiles.Items)
                {
                    string filename = item.Tag as string;

                    if (!item.Checked && !String.IsNullOrEmpty(filename))
                        errorReportHandler.RemoveFile(filename);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error during removing error report files:");
                Trace.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonDontSend control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-07-16</remarks>
        private void buttonDontSend_Click(object sender, EventArgs e)
        {
            try
            {
                BusinessLayer.ErrorReportSender.ArchiveReport(report.FullName);
            }
            catch (Exception ex)
            {
                InternalErrorHandler(ex, false);
            }
            finally
            {
                Close();
                RestartMemoryLifter();
            }
        }

        /// <summary>
        /// Restarts the memory lifter.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-07-16</remarks>
        private void RestartMemoryLifter()
        {
            try
            {
                if (checkBoxRestartMemoryLifter.Checked)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Resources.MLIFTER_MAINPROGRAM);
                    if (File.Exists(psi.FileName))
                        Process.Start(psi);
                }
            }
            catch (Exception exp)
            {
                Trace.WriteLine("Exception during restart of MemoryLifter:");
                Trace.WriteLine(exp.ToString());
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the listViewAdditionalInformation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev07, 2009-07-16</remarks>
        private void listViewAdditionalInformation_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //double click opens the program associated to the selected file
            if (e.Button == MouseButtons.Left)
            {
                //undo the change of the checkstate (results from doubleclicking an item)
                listViewFiles.SelectedItems[0].Checked = !listViewFiles.SelectedItems[0].Checked;

                try
                {
                    string name = listViewFiles.SelectedItems[0].Tag as string;
                    string ext = Path.GetExtension(name);
                    PreviewForm preview = new PreviewForm(ext == ".xml" || ext == ".xsl" ? PreviewType.HTML :
                        ext == ".mlcfg" || ext == ".txt" ? PreviewType.Text :
                        PreviewType.Image,
                        errorReportHandler.GetEntry(content.Find(i => i.Name == name)));
                    preview.Text = name;
                    preview.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    InternalErrorHandler(ex, false);
                }
            }
        }
        /// <summary>
        /// Displays a messagebox for internal exceptions.
        /// </summary>
        /// <param name="ex">The exeption.</param>
        /// <param name="closeform">If set to <c>true</c> closes the form.</param>
        /// <remarks>Documented by Dev02, 2007-11-09</remarks>
        public void InternalErrorHandler(Exception exception, bool closeform)
        {
            MessageBox.Show(string.Format(Properties.Resources.ERROR_INTERNALERROR, Environment.NewLine + Environment.NewLine, exception.Message + exception.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            try
            {
                Clipboard.SetDataObject(exception.ToString(), true);
            }
            catch { }
            if (closeform)
                this.Close();
        }
        List<ZipEntry> content = null;
        /// <summary>
        /// Gets the content of the zip.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev07, 2009-07-16</remarks>
        public List<ZipEntry> GetZipContent(Stream zipFile)
        {
            content = new List<ZipEntry>();
            ZipEntry zipEntry;
            int nBytes = 2048;
            byte[] data = new byte[nBytes];
            try
            {
                using (ZipInputStream zipStream = new ZipInputStream(zipFile))
                {
                    while ((zipEntry = zipStream.GetNextEntry()) != null)
                    {
                        if (zipEntry.IsFile)
                        {
                            content.Add(zipEntry);
                        }
                    }
                }
            }
            catch (Exception ze)
            { Trace.WriteLine("Zip Exception: " + ze.ToString()); }
            return content;
        }

        private void listViewFiles_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Tag.ToString().ToLower() == Resources.ERRORFILE_NAME.ToLower())
            {
                if (!e.Item.Checked)
                    e.Item.Checked = !e.Item.Checked;
            }
        }

        /// <summary>
        /// Handles the Click event of the linkLabelErrorReportDetails control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-07-17</remarks>
        private void linkLabelErrorReportDetails_Click(object sender, EventArgs e)
        {
            if (this.Height == NotExpandedSize)
            {
                this.Height = ExpandedSize;
                labelErrorMessage.Visible = true;
                linkLabelErrorReportDetails.Image = Resources.arrow_up_bw;
            }
            else if (this.Height == ExpandedSize)
            {
                this.Height = NotExpandedSize;
                labelErrorMessage.Visible = false;
                linkLabelErrorReportDetails.Image = Resources.arrow_down_bw;
            }
        }

    }
}
