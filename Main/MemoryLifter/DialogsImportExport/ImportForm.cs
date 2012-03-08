using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

using MLifter.DAL;
using MLifter.DAL.ImportExport;
using MLifter.Properties;
using MLifter.DAL.Interfaces;

namespace MLifter.ImportExport
{
    public partial class ImportForm : Form
    {
        string fileName = string.Empty;
        DirectoryInfo filePath = null;
        DateTime start;
        List<List<string>> data;

        private MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportForm"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        public ImportForm()
        {
            InitializeComponent();
            MLifter.Classes.Help.SetHelpNameSpace(mainHelp);
        }

        /// <summary>
        /// Handles the Load event of the ImportForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void ImportForm_Load(object sender, EventArgs e)
        {
            frmFields.PrepareChapter(Dictionary);

            IChapter[] chapterArray = new IChapter[Dictionary.Chapters.Chapters.Count];
            Dictionary.Chapters.Chapters.CopyTo(chapterArray, 0);
            cmbSelectChapter.Items.AddRange(chapterArray);
            if (cmbSelectChapter.Items.Count > 0)
                cmbSelectChapter.SelectedIndex = 0;

            cmbFileFormat.Items.Add(Resources.FILETYPE_COMMA);
            cmbFileFormat.Items.Add(Resources.FILETYPE_SEMICOLON);
            cmbFileFormat.Items.Add(Resources.FILETYPE_TAB);
            cmbFileFormat.SelectedIndex = 0;

            comboBoxCharset.Items.Clear();
            foreach (EncodingInfo encoding in Encoding.GetEncodings())
            {
                EncodingWrapper encodingwrapper = new EncodingWrapper(encoding);
                comboBoxCharset.Items.Add(encodingwrapper);
                if (encodingwrapper.Encoding.CodePage == Encoding.Default.CodePage) //the encodings are not equal, so the codepages get compared
                    comboBoxCharset.SelectedItem = encodingwrapper;
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the FormClosed event of the ImportForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void ImportForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the btnLoadFromFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void btnLoadFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgImportFile = new OpenFileDialog();
            dlgImportFile.Title = Resources.IMPORT_OPENDIALOG_TITLE;
            dlgImportFile.Filter = Resources.IMPORT_FILETYPE;
            dlgImportFile.AddExtension = true;
            dlgImportFile.CheckFileExists = true;
            dlgImportFile.CheckPathExists = true;
            dlgImportFile.Multiselect = false;

            if (dlgImportFile.ShowDialog() == DialogResult.OK)
            {
                Text = "Importer - " + dlgImportFile.FileName;

                fileName = Path.GetTempFileName();
                filePath = new FileInfo(dlgImportFile.FileName).Directory;
                File.Copy(dlgImportFile.FileName, fileName, true);

                LoadFileForPreview();
            }
            this.AcceptButton = btnImport;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cmbFileFormat control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void cmbFileFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFileForPreview();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void checkBoxHeader_CheckedChanged(object sender, EventArgs e)
        {
            LoadFileForPreview();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxCharset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void checkBoxCharset_CheckedChanged(object sender, EventArgs e)
        {
            LoadFileForPreview();
        }

        /// <summary>
        /// On update of the frmFields object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void frmFields_OnUpdate(object sender)
        {
            UpdateColumns();
        }

        /// <summary>
        /// Handles the Click event of the btnImport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (frmFields.ContainsMedia)
            {
                DirectoryInfo dictionarydirectory = new FileInfo(Dictionary.DictionaryPath).Directory;
                int result = MLifter.Controls.TaskDialog.ShowCommandBox(Resources.IMPORTER_MEDIA_WARNING_TITLE, Resources.IMPORTER_MEDIA_WARNING_TEXT,
                    Resources.IMPORTER_MEDIA_WARNING_CONTENT, string.Empty, string.Empty, string.Empty, string.Format(Properties.Resources.IMPORTER_MEDIA_WARNING_COMMANDS, filePath.FullName, dictionarydirectory.FullName), false,
                    MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Question);
                switch (result)
                {
                    case 0:
                        Environment.CurrentDirectory = filePath.FullName;
                        Import();
                        break;
                    case 1:
                        Environment.CurrentDirectory = dictionarydirectory.FullName;
                        Import();
                        break;
                    default:
                        break;
                }
            }
            else
                Import();
        }

        /// <summary>
        /// Loads the file for preview.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void LoadFileForPreview()
        {
            if (fileName != null && fileName.Length > 0)
            {
                bool exceptionhappened = false;
                DateTime start = DateTime.Now;

                List<List<string>> data;
                try
                {
                    data = MLifter.DAL.ImportExport.Importer.ReadCSV(fileName, GenerateFileSchema(), 50);
                }
                catch (Exception exc)
                {
                    data = new List<List<string>>();
                    List<string> row = new List<string>();
                    row.Add(exc.Message);
                    data.Add(row);
                    exceptionhappened = true;
                }

                listViewPreview.Items.Clear();
                listViewPreview.Columns.Clear();

                UpdateColumns();

                List<ListViewItem> items = new List<ListViewItem>();   // Use List<>+AddRange for better performance
                for (int i = 0; i < data.Count; i++)
                {
                    ListViewItem item = new ListViewItem(data[i][0]);

                    for (int j = 1; j < data[i].Count; j++)
                        item.SubItems.Add(data[i][j]);

                    items.Add(item);
                }
                listViewPreview.Items.AddRange(items.ToArray());

                TimeSpan time = DateTime.Now - start;
                toolStripStatusLabelStatusMessage.Text = string.Format(Resources.IMPORTER_FILE_LOADED, Math.Round(time.TotalSeconds, 3), data.Count);

                btnImport.Enabled = !exceptionhappened && listViewPreview.Items.Count > 0;
            }
        }

        /// <summary>
        /// Generates the file schema for the csv parser.
        /// </summary>
        /// <returns>The Fileschema.</returns>
        /// <remarks>Documented by Dev02, 2007-12-14</remarks>
        private FileSchema GenerateFileSchema()
        {
            FileSchema fileSchema = new FileSchema();
            fileSchema.Encoding = ((EncodingWrapper)comboBoxCharset.SelectedItem).Encoding;
            fileSchema.hasHeaders = checkBoxHeader.Checked;

            char delimiter;
            switch (cmbFileFormat.SelectedIndex)
            {
                case 1:
                    delimiter = ';';
                    break;
                case 2:
                    delimiter = '\t';
                    break;
                case 3:
                default:
                    delimiter = ',';
                    break;
            }
            fileSchema.Delimiter = delimiter;

            return fileSchema;
        }

        /// <summary>
        /// Updates the columns.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void UpdateColumns()
        {
            listViewPreview.SuspendLayout();
            listViewPreview.Columns.Clear();
            listViewPreview.RightToLeftEnabledColumnIndexes.Clear();

            int headerWidth = (listViewPreview.Width - 21) / (frmFields.StrOrder.Length > 0 ? frmFields.StrOrder.Length : 1);

            foreach (string header in frmFields.StrOrder)
            {
                listViewPreview.Columns.Add(header, headerWidth);

                //fix for [ML-1335] Problems with bidirectional Text (RTL languages)
                if (Dictionary.QuestionCulture.TextInfo.IsRightToLeft &&
                    isSameField(header, Resources.LISTBOXFIELDS_QUESTION_TEXT) ||
                    isSameField(header, Resources.LISTBOXFIELDS_EXQUESTION_TEXT))
                    listViewPreview.RightToLeftEnabledColumnIndexes.Add(listViewPreview.Columns.Count - 1);

                if (Dictionary.AnswerCulture.TextInfo.IsRightToLeft &&
                    isSameField(header, Resources.LISTBOXFIELDS_ANSWER_TEXT) ||
                    isSameField(header, Resources.LISTBOXFIELDS_EXANSWER_TEXT))
                    listViewPreview.RightToLeftEnabledColumnIndexes.Add(listViewPreview.Columns.Count - 1);

            }

            listViewPreview.ResumeLayout();
            listViewPreview.Invalidate(); //make sure to redraw
        }

        /// <summary>
        /// Imports the cards.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-30</remarks>
        private void Import()
        {
            start = DateTime.Now;
            Properties.Settings.Default.LastImportTimestamp = start;

            data = MLifter.DAL.ImportExport.Importer.ReadCSV(fileName, GenerateFileSchema());

            Dictionary<Field, int> fields = new Dictionary<Field, int>();

            for (int i = 0; i < listViewPreview.Columns.Count; i++)
            {
                string caption = listViewPreview.Columns[i].Text;

                if (isSameField(caption, Resources.LISTBOXFIELDS_ANSWER_TEXT))
                    fields.Add(Field.Answer, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_EXANSWER_TEXT))
                    fields.Add(Field.AnswerExample, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_EXQUESTION_TEXT))
                    fields.Add(Field.QuestionExample, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_ANSWER_DISTRACTORS_TEXT))
                    fields.Add(Field.AnswerDistractors, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_QUESTION_DISTRACTORS_TEXT))
                    fields.Add(Field.QuestionDistractors, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_IMAGE_ANSWER_TEXT))
                    fields.Add(Field.AnswerImage, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_IMAGE_QUESTION_TEXT))
                    fields.Add(Field.QuestionImage, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_QUESTION_TEXT))
                    fields.Add(Field.Question, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_SOUND_ANSWER_TEXT))
                    fields.Add(Field.AnswerSound, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_SOUND_QUESTION_TEXT))
                    fields.Add(Field.QuestionSound, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_SOUND_EXANSWER_TEXT))
                    fields.Add(Field.AnswerExampleSound, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_SOUND_EXQUESTION_TEXT))
                    fields.Add(Field.QuestionExampleSound, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_VIDEO_ANSWER_TEXT))
                    fields.Add(Field.AnswerVideo, i);
                else if (isSameField(caption, Resources.LISTBOXFIELDS_VIDEO_QUESTION_TEXT))
                    fields.Add(Field.QuestionVideo, i);
                else if (caption == Resources.LISTBOXFIELDS_CHAPTER)
                    fields.Add(Field.Chapter, i);
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync(new object[] { Dictionary.DictionaryDAL, data, cmbSelectChapter.SelectedItem, fields });
        }

        /// <summary>
        /// Determines whether two strings are equal, where the second string gets a replacement with the 
        /// corresponding caption of the dictionary (question- and answercaption).
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="resourceString">The resource string.</param>
        /// <returns>
        /// 	<c>true</c> if they match; otherwise, <c>false</c>.
        /// </returns>
        /// Documented by MatBre,  07.09.2009
        private Boolean isSameField(string field, string resourceString)
        {
            string caption;

            if (resourceString.Equals(Resources.LISTBOXFIELDS_QUESTION_DISTRACTORS_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_QUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_EXQUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_IMAGE_QUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_SOUND_EXQUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_SOUND_QUESTION_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_VIDEO_QUESTION_TEXT))
                caption = MainForm.LearnLogic.Dictionary.QuestionCaption;
            else if (resourceString.Equals(Resources.LISTBOXFIELDS_ANSWER_DISTRACTORS_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_ANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_EXANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_IMAGE_ANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_SOUND_ANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_SOUND_EXANSWER_TEXT) ||
                resourceString.Equals(Resources.LISTBOXFIELDS_VIDEO_ANSWER_TEXT))
                caption = MainForm.LearnLogic.Dictionary.AnswerCaption;
            else
                caption = "";

            string finalResourceString = String.Format(resourceString, caption);

            return field.Equals(finalResourceString);
        }

        /// <summary>
        /// Handles the DoWork event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-31</remarks>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Start();

                object[] arguments = (object[])e.Argument;
                DAL.Interfaces.IDictionary dictionary = (DAL.Interfaces.IDictionary)arguments[0];
                List<List<string>> data = (List<List<string>>)arguments[1];
                IChapter chapter = (IChapter)arguments[2];
                Dictionary<Field, int> fields = (Dictionary<Field, int>)arguments[3];
                BackgroundWorker worker = (BackgroundWorker)sender;

                MLifter.DAL.ImportExport.Importer.ImportToDictionary(dictionary, data, chapter, fields, worker, start, checkBoxSplitSynonyms.Checked);
            }
            catch (Exception exp)
            {
                ((BackgroundWorker)sender).RunWorkerCompleted -= new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                System.Diagnostics.Debug.WriteLine("Import failed: " + Environment.NewLine + exp.ToString());
                Finished(false);
            }
        }
        /// <summary>
        /// Handles the ProgressChanged event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-31</remarks>
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBarStatus.Value = e.ProgressPercentage;
            toolStripProgressBarStatus.Invalidate();
            toolStripStatusLabelStatusMessage.Text = string.Format(Resources.IMPORTER_IMPORTED,
                (int)((double)e.ProgressPercentage / (double)100 * (double)data.Count), data.Count);
        }
        /// <summary>
        /// Handles the RunWorkerCompleted event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-31</remarks>
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Finished(true);
        }

        private delegate void emptyDelegate();
        /// <summary>
        /// Starts the import.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-08-31</remarks>
        private void Start()
        {
            if (!InvokeRequired)
            {
                toolStripProgressBarStatus.Visible = true;
                Enabled = false;
            }
            else
                Invoke((emptyDelegate)Start);
        }

        private delegate void FinishedDelegate(bool successful);

        /// <summary>
        /// Finishes the import.
        /// </summary>
        /// <param name="successful">if set to <c>true</c> [successful].</param>
        /// <remarks>Documented by Dev02, 2008-02-27</remarks>
        private void Finished(bool successful)
        {
            if (!InvokeRequired)
            {
                if (successful)
                {
                    TimeSpan time = DateTime.Now - start;
                    toolStripStatusLabelStatusMessage.Text = string.Format(Resources.IMPORTER_IMPORTED_END, data.Count, Math.Round(time.TotalSeconds, 3));
                    MessageBox.Show(string.Format(Properties.Resources.IMPORT_SUCCESS_TEXT, data.Count), Properties.Resources.IMPORT_SUCCESS_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show(Properties.Resources.IMPORT_FAILURE_TEXT, Properties.Resources.IMPORT_FAILURE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Enabled = true;
                toolStripProgressBarStatus.Visible = false;
            }
            else
                Invoke((FinishedDelegate)Finished, successful);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBoxCharset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-13</remarks>
        private void comboBoxCharset_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFileForPreview();
        }
    }

    /// <summary>
    /// Holds an Encoding object and provides a ToString() function.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-02-13</remarks>
    class EncodingWrapper
    {
        private Encoding encoding = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingWrapper"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <remarks>Documented by Dev02, 2008-02-13</remarks>
        public EncodingWrapper(Encoding encoding)
        {
            this.encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingWrapper"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <remarks>Documented by Dev02, 2008-02-13</remarks>
        public EncodingWrapper(EncodingInfo encoding)
        {
            this.encoding = encoding.GetEncoding();
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        /// <remarks>Documented by Dev02, 2008-02-13</remarks>
        public Encoding Encoding
        {
            get { return this.encoding; }
            set { this.encoding = value; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        /// <remarks>Documented by Dev02, 2008-02-13</remarks>
        public override string ToString()
        {
            return string.Format(Resources.IMPORT_CHARSET_FORMAT, this.encoding.EncodingName, this.encoding.CodePage);
        }
    }
}