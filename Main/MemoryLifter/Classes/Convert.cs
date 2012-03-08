using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

using MLifter.DAL.Transformer.V17;
using MLifter.Properties;
using MLifter.Controls;
using MLifter.DAL;
using MLifter.BusinessLayer;
using MLifter.DAL.Interfaces;
using System.Threading;

namespace MLifter.Classes
{
    /// <summary>
    /// Convert encapsulates the workflow to convert a dictionary from version 1.7 or ODX.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-10-01</remarks>
    internal class Convert
    {
        #region�Fields�(4)

        private FolderBrowserDialog DirDialog = new FolderBrowserDialog();
        private static LoadStatusMessage loadStatusMessageImport = new LoadStatusMessage(Properties.Resources.COPYTO_STATUS_MESSAGE, 100, false);
        internal LoadStatusMessage m_LoadMsg = new LoadStatusMessage(String.Empty, 100, true);
        private string odxFile;
        private string edbFile;
        private bool cleanUpOdx = false;

        #endregion�Fields

        #region�Constructors�(1)

        internal Convert()
        {
            this.DirDialog.ShowNewFolderButton = true;
            this.DirDialog.Description = Resources.CONVERT_DIRDIALOG_DESCRIPTION;
        }

        #endregion�Constructors

        #region�Delegates�and�Events�(1)

        //�Events�(1)�

        public event EventHandler<ConvertingEventArgs> ConvertingFinished;

        #endregion�Delegates�and�Events

        #region�Methods�(10)

        //�Protected�Methods�(1)�

        protected virtual void OnConvertedFinish(ConvertingEventArgs e)
        {
            if (ConvertingFinished != null)
                ConvertingFinished(this, e);
        }
        //�Private�Methods�(7)�

        private void Convert_ProgressChanged(object sender, ProgressChangedEventArgs progressArgs)
        {
            m_LoadMsg.SetProgress(progressArgs.ProgressPercentage);
        }

        /// <summary>
        /// Called if a data access error occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="exp">The exp.</param>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        private void DataAccessError(object sender, Exception exp)
        {
            throw exp;
        }

        /// <summary>
        /// Hides the status message and activates the form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void HideStatusMessage()
        {
            loadStatusMessageImport.Invoke((MethodInvoker)delegate
            {
                LearnLogic.CopyToFinished -= new EventHandler(LearnLogic_CopyToFinished);
                loadStatusMessageImport.Hide();
            });
        }

        /// <summary>
        /// Handles the CopyToFinished event of the LearnLogic control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2008-12-15</remarks>
        void LearnLogic_CopyToFinished(object sender, EventArgs e)
        {
            HideStatusMessage();

            if (cleanUpOdx)
            {
                ConnectionStringStruct css = new ConnectionStringStruct(DatabaseType.Xml, Path.GetDirectoryName(odxFile), odxFile, true);
                IUser xmlUser = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
                      css, DataAccessError, this);
                xmlUser.List().Delete(css);
            }
            else
            {
                FileInfo fi = new FileInfo(odxFile);
                if (File.Exists(odxFile + ".bak"))
                {
                    try
                    {
                        File.Delete(odxFile + ".bak");
                    }
                    catch { }
                }
                try
                {
                    fi.MoveTo(odxFile + ".bak");
                }
                catch { }
            }

            OnConvertedFinish(new ConvertingEventArgs(edbFile));
        }

        private void Move_ProgressChanged(object sender, ProgressChangedEventArgs progressArgs)
        {
            m_LoadMsg.SetProgress(progressArgs.ProgressPercentage);
        }

        /// <summary>
        /// Shows the status message and deactivates the form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void ShowStatusMessage()
        {
            loadStatusMessageImport.InfoMessage = Properties.Resources.IMPORTING;
            loadStatusMessageImport.EnableProgressbar = true;
            loadStatusMessageImport.Show();
        }

        /// <summary>
        /// Updates the status message dialog with the current progress/infomessage.
        /// </summary>
        /// <param name="statusMessage">The status message.</param>
        /// <param name="currentPercentage">The current percentage.</param>
        /// <remarks>Documented by Dev02, 2008-09-29</remarks>
        private void UpdateStatusMessage(string statusMessage, double currentPercentage)
        {
            loadStatusMessageImport.Invoke((MethodInvoker)delegate
            {
                loadStatusMessageImport.InfoMessage = statusMessage;
                loadStatusMessageImport.SetProgress(System.Convert.ToInt32(currentPercentage));
            });
        }
        //�Internal�Methods�(2)�

        /// <summary>
        /// Converts an ODF to an ODX file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev03, 2007-10-01</remarks>
        internal string FromOdf(string filename)
        {
            string dictionaryPath = String.Empty;
            string temporaryPath = String.Empty;
            DAL.Interfaces.IDictionary dictionary = null;

            m_LoadMsg.InfoMessage = Resources.DIC_IMPORT_OLD_TEXT;
            m_LoadMsg.SetProgress(0);
            Cursor.Current = Cursors.WaitCursor;
            m_LoadMsg.Show();
            try
            {
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(Convert_ProgressChanged);
                Converter converter = new Converter(backgroundWorker);
                converter.ApplicationPath = Application.StartupPath;
                temporaryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + DAL.Helper.OdxExtension);
                converter.SourceEncoding = Encoding.Default;    //this not a 100% clean solution 
                converter.LoginCallback = (MLifter.DAL.GetLoginInformation)LoginForm.OpenLoginForm;
                dictionary = converter.Load(filename, temporaryPath);
            }
            catch (DAL.DictionaryFormatNotSupported)
            {
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(Resources.DIC_NOT_SUPPORTED_TEXT, Resources.DIC_NOT_SUPPORTED_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            catch (DAL.InvalidImportFormatException)
            {
                if (dictionary != null) dictionary.Dispose();
                MessageBox.Show(String.Format(Resources.DIC_INVALID_FORMAT_TEXT, filename), Resources.DIC_MEDIA_ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                if (dictionary != null) dictionary.Dispose();
                Cursor.Current = Cursors.Default;
                m_LoadMsg.Hide();
            }

            string dictionaryBasePath = Settings.Default.DicDir;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            // Ask user for target directory
            dictionaryPath = Path.Combine(dictionaryBasePath, fileNameWithoutExtension);

            try
            {
                if (!Directory.Exists(dictionaryPath))
                {
                    Directory.CreateDirectory(dictionaryPath);
                }
            }
            catch
            {
                dictionaryPath = dictionaryBasePath;
            }

            bool pathExists = false;
            bool overwrite = false;
            DirDialog.SelectedPath = dictionaryPath;
            do
            {
                if (DirDialog.ShowDialog(m_LoadMsg) == DialogResult.OK)
                {
                    dictionaryPath = DirDialog.SelectedPath;
                }
                else
                {
                    return null;
                }
                if (File.Exists(Path.Combine(dictionaryPath, fileNameWithoutExtension + DAL.Helper.OdxExtension)))
                {
                    pathExists = true;

                    DialogResult result = MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.CONVERT_DICTIONARY_EXISTS_MBX_CAPTION,
                        Resources.CONVERT_DICTIONARY_EXISTS_MBX_TEXT, Resources.CONVERT_DICTIONARY_EXISTS_MBX_CONTENT,
                        String.Empty, String.Empty, String.Empty, String.Empty,
                        String.Format("{0}|{1}|{2}", Resources.CONVERT_DICTIONARY_EXISTS_MBX_OPTION_YES, Resources.CONVERT_DICTIONARY_EXISTS_MBX_OPTION_NO, Resources.CONVERT_DICTIONARY_EXISTS_MBX_OPTION_CANCEL),
                        MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Warning);
                    switch (MLifter.Controls.TaskDialog.CommandButtonResult)
                    {
                        case 0:
                            break;
                        case 2:
                            continue;
                        case 1:
                        default:
                            return null;
                    }
                }
                else if (Directory.GetFiles(dictionaryPath, "*" + DAL.Helper.OdxExtension).Length > 0)
                {
                    pathExists = true;

                    DialogResult result = MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.CONVERT_DICTIONARY_FOLDER_EXISTS_MBX_CAPTION,
                        Resources.CONVERT_DICTIONARY_FOLDER_EXISTS_MBX_TEXT, Resources.CONVERT_DICTIONARY_FOLDER_EXISTS_MBX_CONTENT,
                        String.Empty, String.Empty, String.Empty, String.Empty,
                        String.Format("{0}|{1}|{2}", Resources.CONVERT_DICTIONARY_FOLDER_EXISTS_MBX_OPTION_YES, Resources.CONVERT_DICTIONARY_FOLDER_EXISTS_MBX_OPTION_NO, Resources.CONVERT_DICTIONARY_FOLDER_EXISTS_MBX_OPTION_CANCEL),
                        MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Warning, MLifter.Controls.TaskDialogIcons.Warning);
                    switch (MLifter.Controls.TaskDialog.CommandButtonResult)
                    {
                        case 0:
                            break;
                        case 2:
                            continue;
                        case 1:
                        default:
                            return null;
                    }
                }
                pathExists = false;
            }
            while (pathExists);

            dictionaryPath = Path.Combine(dictionaryPath, fileNameWithoutExtension + DAL.Helper.OdxExtension);

            m_LoadMsg.InfoMessage = Resources.CONVERT_TMOVEMSG;
            m_LoadMsg.SetProgress(0);
            Cursor.Current = Cursors.WaitCursor;
            m_LoadMsg.Show();
            try
            {
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(Move_ProgressChanged);
                dictionary.BackgroundWorker = backgroundWorker;
                dictionary.Move(dictionaryPath, overwrite);
            }
            finally
            {
                if (dictionary != null) dictionary.Dispose();
                Cursor.Current = Cursors.Default;
                m_LoadMsg.Hide();
            }

            return dictionaryPath;
        }

        /// <summary>
        /// Converts an ODX to an eDB file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="cleanup">if set to <c>true</c> [cleanup].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-03-02</remarks>
        internal string FromOdx(string filename, bool cleanup)
        {
            if (filename == (string)null) throw new ArgumentNullException("filename");
            if (filename.Length == 0) throw new ArgumentException("filename");
            try
            {
                ShowStatusMessage();
                odxFile = filename;
                cleanUpOdx = cleanup;
                IUser xmlUser = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
                    new ConnectionStringStruct(DatabaseType.Xml, Path.GetDirectoryName(filename), filename, true), DataAccessError, this);

                IDictionary oldDic = xmlUser.Open();

                IUser dbUser = UserFactory.Create((GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; },
                    new ConnectionStringStruct(DatabaseType.Unc, Path.GetDirectoryName(filename), Path.ChangeExtension(filename, Helper.EmbeddedDbExtension), true), DataAccessError, this);

                IDictionary newDic = dbUser.List().AddNew(oldDic.Category.Id > 5 ? 3 : oldDic.Category.Id, oldDic.Title);

                ConnectionStringStruct oldCon = new ConnectionStringStruct(DatabaseType.Xml, filename);
                ConnectionStringStruct newCon = new ConnectionStringStruct(DatabaseType.MsSqlCe, newDic.Connection, newDic.Id);
                edbFile = newCon.ConnectionString;

                oldDic.Dispose();
                newDic.Dispose();

                LearnLogic.CopyToFinished += new EventHandler(LearnLogic_CopyToFinished);
                LearnLogic.CopyLearningModule(oldCon, newCon, (GetLoginInformation)delegate(UserStruct u, ConnectionStringStruct c) { return u; }, UpdateStatusMessage, DataAccessError, null);

                return newCon.ConnectionString;
            }
            catch { HideStatusMessage(); throw; }
        }

        #endregion�Methods
    }

    /// <summary>
    /// Eventargs which delivers the filename of a converted file.
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-03</remarks>
    internal class ConvertingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the converted file.
        /// </summary>
        /// <value>The converted file.</value>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public string ConvertedFile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertingEventArgs"/> class.
        /// </summary>
        /// <param name="convertedFile">The converted file.</param>
        /// <remarks>Documented by Dev05, 2009-03-03</remarks>
        public ConvertingEventArgs(string convertedFile)
        {
            ConvertedFile = convertedFile;
        }
    }
}
