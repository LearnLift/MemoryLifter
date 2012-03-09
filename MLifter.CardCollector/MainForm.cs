using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using MLifter;
using MLifter.BusinessLayer;
using MLifter.CardCollector.Properties;
using MLifter.DAL;
using MLifter.DAL.Interfaces;

using XPTable;
using XPTable.Models;

namespace MLifter.CardCollector
{
    public partial class CollectorForm : Form
    {
        bool selectingCard = false;
        bool finallyClose = true;   //finally close the dictionary
        int position = 50;
        int lastVisibleColumn = 50;
        Row currentItem;
        Dictionary dictionary;
        System.Windows.Forms.Timer selectionTimer = new System.Windows.Forms.Timer();

        Controls.Wizards.CardCollector.SettingsPage settingsPage = new MLifter.Controls.Wizards.CardCollector.SettingsPage();

        protected delegate void emptyDelegate();

        /// <summary>
        /// Gets or sets the help file.
        /// </summary>
        /// <value>The help file.</value>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public string HelpFile
        {
            get { return MainHelp.HelpNamespace; }
            set { MainHelp.HelpNamespace = value; if (cardEdit != null) cardEdit.HelpNamespace = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorForm"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        public CollectorForm()
        {
            InitializeComponent();
            cardEdit.HelpNamespace = this.HelpFile;

            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Dictionary|*.odx";
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                if (dictionary != null) dictionary.Dispose();
                MLifter.DAL.Interfaces.IUser user = UserFactory.Create((GetLoginInformation)MLifter.Controls.LoginForm.OpenLoginForm,
                    new ConnectionStringStruct(DatabaseType.Xml, openDlg.FileName, true),
                    (DataAccessErrorDelegate)delegate(object sender, Exception e) { MessageBox.Show(e.ToString()); }, this);
                dictionary = new Dictionary(user.Open(), null);
            }
            else
                Close();

            tableCards.NoItemsText = Properties.Resources.TABLECARDS_NOITEMSTEXT;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorForm"/> class.
        /// </summary>
        /// <param name="Dictionary">The dictionary.</param>
        /// <remarks>Documented by Dev05, 2007-10-02</remarks>
        public CollectorForm(string Dictionary)
        {
            ShowInTaskbar = false;
            InitializeComponent();
            cardEdit.HelpNamespace = this.HelpFile;

            if (dictionary != null) dictionary.Dispose();
            MLifter.DAL.Interfaces.IUser user = UserFactory.Create((GetLoginInformation)MLifter.Controls.LoginForm.OpenLoginForm,
                 new ConnectionStringStruct(DatabaseType.Xml, Dictionary, true),
                    (DataAccessErrorDelegate)delegate(object sender, Exception e) { MessageBox.Show(e.ToString()); }, this);
            dictionary = new Dictionary(user.Open(), null);

            tableCards.NoItemsText = Properties.Resources.TABLECARDS_NOITEMSTEXT;
        }

        ///<summary>
        ///Initializes a new instance of the <see cref="CollectorForm"/> class.
        ///</summary>
        ///<param name="Dictionary">The dictionary.</param>
        ///<remarks>Documented by Dev05, 2007-10-02</remarks>
        public CollectorForm(Dictionary Dictionary)
        {
            ShowInTaskbar = false;
            finallyClose = false;
            InitializeComponent();
            cardEdit.HelpNamespace = this.HelpFile;
            dictionary = Dictionary;
            cardEdit.CardCollector = true;

            tableCards.NoItemsText = Properties.Resources.TABLECARDS_NOITEMSTEXT;
        }

        /// <summary>
        /// Handles the Load event of the CollectorForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void MainForm_Load(object sender, EventArgs e)
        {
            selectionTimer.Interval = 50;
            selectionTimer.Tick += new EventHandler(selectionTimer_Tick);

            buttonStart.Text = Resources.BUTTON_START;

            cardEdit.LoadNewCard(dictionary);
            cardEdit.Modified = false;

            //tableCards.HeaderRenderer = new XPTable.Renderers.GradientHeaderRenderer();

            tableCards.ColumnModel.Columns.Clear();
            tableCards.ColumnModel.Columns.Add(new ButtonColumn("", 20));
            tableCards.ColumnModel.Columns[0].Sortable = false;

            foreach (ListViewItem item in settingsPage.listViewElements.Items)
                tableCards.ColumnModel.Columns.Add(new TextColumn(item.Text, (tableCards.Width - 38) / settingsPage.listViewElements.Items.Count));

            settingsPage.listViewElements.ItemChecked += new ItemCheckedEventHandler(listViewElements_ItemChecked);
            if (ShowWizard() == DialogResult.Cancel)
            {
                Opacity = 0;
                Close();
            }
            else
                buttonStart.PerformClick();

            //fix for [ML-1335]  Problems with bidirectional Text (RTL languages)
            cardEdit.RightToLeftQuestion = dictionary.QuestionCulture.TextInfo.IsRightToLeft;
            cardEdit.RightToLeftAnswer = dictionary.AnswerCulture.TextInfo.IsRightToLeft;
        }

        /// <summary>
        /// Shows the wizard.
        /// </summary>
        /// <returns>The Dialog result of the wizard.</returns>
        /// <remarks>Documented by Dev05, 2007-12-14</remarks>
        private DialogResult ShowWizard()
        {
            CreateListViewSnapshot(settingsPage.listViewElements);
            Wizard wizard = new Wizard();
            wizard.HelpFile = HelpFile;
            wizard.Text = Resources.WIZARD_HEADER;
            wizard.Pages.Add(settingsPage);
            wizard.ShowInTaskbar = false;
            wizard.ShowDialog();
            if (wizard.DialogResult == DialogResult.Cancel)
            {
                RestoreListViewSnapshot(settingsPage.listViewElements);
                RefreshSettingsPageListView();
            }
            return wizard.DialogResult;
        }

        /// <summary>
        /// Handles the ItemChecked event of the listViewElements control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ItemCheckedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void listViewElements_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //sometimes this eventhandler gets called when the listview is not complete yet, this is to avoid null reference exceptions
            foreach (ListViewItem item in settingsPage.listViewElements.Items)
            {
                if (item == null || item.Text == string.Empty)
                    return;
            }

            RefreshSettingsPageListView();
        }

        /// <summary>
        /// Refreshes the settings page list view.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-22</remarks>
        private void RefreshSettingsPageListView()
        {
            for (int i = 1; i < tableCards.ColumnModel.Columns.Count; i++)
            {
                Column col = tableCards.ColumnModel.Columns[i];
                col.Visible = false;
                col.Text = settingsPage.listViewElements.Items[i - 1].Text;

                if ((DAL.Helper.CardFields)settingsPage.listViewElements.Items[i - 1].Tag == Helper.CardFields.Question && dictionary.QuestionCulture.TextInfo.IsRightToLeft)
                {
                    col.Renderer = new MLifter.Controls.RtlTextRenderer();
                    (col.Renderer as MLifter.Controls.RtlTextRenderer).FormatFlags |= TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis;
                }
                else if ((DAL.Helper.CardFields)settingsPage.listViewElements.Items[i - 1].Tag == Helper.CardFields.Answer && dictionary.AnswerCulture.TextInfo.IsRightToLeft)
                {
                    col.Renderer = new MLifter.Controls.RtlTextRenderer();
                    (col.Renderer as MLifter.Controls.RtlTextRenderer).FormatFlags |= TextFormatFlags.RightToLeft | TextFormatFlags.EndEllipsis;
                }
                else
                    col.Renderer = new XPTable.Renderers.TextCellRenderer();
            }
            foreach (ListViewItem item in settingsPage.listViewElements.CheckedItems)
            {
                tableCards.ColumnModel.Columns[item.Index + 1].Visible = true;
                tableCards.ColumnModel.Columns[item.Index + 1].Width = (tableCards.Width - 40) / settingsPage.listViewElements.CheckedItems.Count;
                lastVisibleColumn = item.Index + 1;
            }
            tableCards.ColumnModel.Columns[0].Visible = true;
        }

        /// <summary>
        /// Creates the list view snapshot.
        /// </summary>
        /// <param name="listview">The listview.</param>
        /// <remarks>Documented by Dev02, 2008-01-22</remarks>
        private void CreateListViewSnapshot(ListView listview)
        {
            List<ListViewItem> items = new List<ListViewItem>();
            foreach (ListViewItem item in listview.Items)
                items.Add(item.Clone() as ListViewItem);
            listview.Tag = items;
        }

        /// <summary>
        /// Restores the list view snapshot.
        /// </summary>
        /// <param name="listview">The listview.</param>
        /// <remarks>Documented by Dev02, 2008-01-22</remarks>
        private void RestoreListViewSnapshot(ListView listview)
        {
            if (listview.Tag is List<ListViewItem>)
            {
                listview.BeginUpdate();
                listview.Items.Clear();
                foreach (ListViewItem item in (listview.Tag) as List<ListViewItem>)
                    listview.Items.Add(item);
                listview.Tag = null;
                listview.EndUpdate();
            }
        }

        /// <summary>
        /// Handles the Click event of the pictureBoxAdd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-14</remarks>
        private void pictureBoxAdd_Click(object sender, EventArgs e)
        {
            ShowWizard();
        }

        /// <summary>
        /// Handles the SizeChanged event of the CollectorForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-01</remarks>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            Screen currentScreen = Screen.FromControl(this);
            if (currentScreen.WorkingArea.Height - Location.Y < Height)
                Top = currentScreen.WorkingArea.Height - Height - 20;
        }

        /// <summary>
        /// Handles the Click event of the buttonStart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (buttonStart.Text == Resources.BUTTON_START)
            {
                Clipboard.Clear();
                //tableCards.FullRowSelect = false;

                buttonStart.Text = Resources.BUTTON_STOP;
                timerWatch.Enabled = true;
                timerWatch.Start();
            }
            else
            {
                timerWatch.Enabled = false;
                timerWatch.Stop();

                buttonStart.Text = Resources.BUTTON_START;

                //if (tableCards.TableModel.Rows.Count > 0)
                //{
                //    tableCards.TableModel.Selections.Clear();
                //    tableCards.FocusedCell = new CellPos(0, 1);
                //    tableCards.TableModel.Selections.AddCell(0, 1);
                //    //tableCards.FullRowSelect = true;
                //    LoadCardForEditing();
                //}
            }
        }

        /// <summary>
        /// Handles the FormClosing event of the CollectorForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.tableCards.RowCount > 0)
            {
                if (MessageBox.Show(Properties.Resources.COLLECTOR_NOT_EMPTY_TEXT, Properties.Resources.COLLECTOR_NOT_EMPTY_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            timerWatch.Stop();
            cardEdit.StopPlayingVideos();
        }

        /// <summary>
        /// Handles the Tick event of the timerWatch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void timerWatch_Tick(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText(TextDataFormat.UnicodeText);
                Clipboard.Clear();

                AddText(text);
            }
            else if (Clipboard.ContainsFileDropList())
            {
                foreach (string file in Clipboard.GetFileDropList())
                    AddText(file);
                Clipboard.Clear();
            }
        }

        /// <summary>
        /// Adds the Text.
        /// </summary>
        /// <param name="Text">The Text.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void AddText(string text)
        {
            if (!text.Contains("\t") && !text.Contains(";"))
                AddWord(text);
            else
            {
                string[] lines = text.Split(new string[] { "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                bool statusmessageenabled = lines.Length > 100;
                MLifter.Controls.LoadStatusMessage message = new MLifter.Controls.LoadStatusMessage(Properties.Resources.COLLECTOR_STATUSMESSAGE, lines.Length, true);
                if (statusmessageenabled) message.Show();
                int linecount = 0;
                foreach (string line in lines)
                {
                    linecount++;
                    if (linecount % 10 == 0)
                        message.SetProgress(linecount);
                    AddLine(line);
                }
                message.Close();
                if (checkBoxBeep.Checked)
                    Beep();
            }
        }

        /// <summary>
        /// Adds the word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void AddWord(string word)
        {
            if (checkBoxRemove.Checked)
            {
                Regex regEx = new Regex(@"[({\[].*[)}\]]");
                word = regEx.Replace(word, "");
            }

            position++;
            if (position >= tableCards.ColumnModel.Columns.Count || currentItem == null)
            {
                tableCards.GridLines = GridLines.Both;

                currentItem = new Row();
                currentItem.Cells.Add(new Cell("x"));

                currentItem.Cells[0].CellStyle = new CellStyle();
                currentItem.Cells[0].CellStyle.Padding = new CellPadding(0, 0, 0, 0);
                currentItem.Cells[0].CellStyle.Font = new Font(new FontFamily("Arial"), 8, FontStyle.Bold);

                position = 1;

                while (!tableCards.ColumnModel.Columns[position].Visible && position < tableCards.ColumnModel.Columns.Count)
                {
                    tableCards.Invoke((emptyDelegate)delegate() { currentItem.Cells.Add(new Cell("")); });
                    position++;
                }

                currentItem.Cells.Add(new Cell(word));
                tableCards.Invoke((emptyDelegate)delegate() { tableCards.TableModel.Rows.Add(currentItem); });
            }
            else
            {
                while (!tableCards.ColumnModel.Columns[position].Visible && position < tableCards.ColumnModel.Columns.Count)
                {
                    tableCards.Invoke((emptyDelegate)delegate() { currentItem.Cells.Add(new Cell("")); });
                    position++;
                }

                tableCards.Invoke((emptyDelegate)delegate() { currentItem.Cells.Add(new Cell(word)); });
            }

            if (checkBoxBeep.Checked && position == lastVisibleColumn)
                Beep();
            if (position == lastVisibleColumn)
                position = 50;

            tableCards.EnsureVisible(tableCards.TableModel.Rows.Count - 1, currentItem.Cells.Count - 1);
        }

        /// <summary>
        /// Beeps.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        private static void Beep()
        {
            //Console.Beep();
            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// Adds the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void AddLine(string line)
        {
            tableCards.GridLines = GridLines.Both;

            currentItem = null;
            position = 0;
            bool oldBeepValue = checkBoxBeep.Checked;
            checkBoxBeep.Checked = false;

            int i = 0;
            string[] words = line.Split(new char[] { '\t', ';' });

            while (position <= 12 && i < words.Length)
            {
                AddWord(words[i]);
                i++;
            }

            checkBoxBeep.Checked = oldBeepValue;

            currentItem = null;
        }

        /// <summary>
        /// Handles the CellButtonClicked event of the tableCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="XPTable.Events.CellButtonEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void tableCards_CellButtonClicked(object sender, XPTable.Events.CellButtonEventArgs e)
        {
            tableCards.TableModel.Rows.RemoveAt(e.Cell.Row.Index);

            if (tableCards.TableModel.Rows.Count == 0)
                tableCards.GridLines = GridLines.None;
            if (e.Cell.Row == currentItem)
                position = 50;
        }

        /// <summary>
        /// Handles the DragEnter event of the CollectorForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (!buttonStart.Enabled && (
                e.Data.GetDataPresent(DataFormats.Bitmap) ||
                e.Data.GetDataPresent(DataFormats.Text) ||
                e.Data.GetDataPresent(DataFormats.FileDrop) ||
                e.Data.GetDataPresent(DataFormats.CommaSeparatedValue)))
                e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Handles the DragDrop event of the CollectorForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                AddText((string)e.Data.GetData(DataFormats.Text));
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop))
                    AddWord(file);
            else if (e.Data.GetDataPresent(DataFormats.CommaSeparatedValue))
            {
                string csv = (string)e.Data.GetData(DataFormats.CommaSeparatedValue);
                AddText(csv);
            }
            else if (e.Data.GetDataPresent(DataFormats.Bitmap))
                MessageBox.Show("Not Implemented");
        }

        /// <summary>
        /// Handles the SelectionChanged event of the tableCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="XPTable.Events.SelectionEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-01</remarks>
        private void tableCards_SelectionChanged(object sender, XPTable.Events.SelectionEventArgs e)
        {
            if (!selectingCard)
                selectionTimer.Start();
        }

        /// <summary>
        /// Handles the Tick event of the selectionTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-23</remarks>
        void selectionTimer_Tick(object sender, EventArgs e)
        {
            selectingCard = true;
            tableCards.TableModel.Selections.Clear();
            if (tableCards.FocusedCell.Row < tableCards.TableModel.Rows.Count)
                tableCards.TableModel.Selections.AddCell(tableCards.FocusedCell.Row, 0);
            selectingCard = false;

            if (tableCards.TableModel.Rows.Count > 0 && (tableCards.FocusedCell.Row < tableCards.TableModel.Rows.Count))
                LoadCardForEditing();
            else
                cardEdit.LoadNewCard(dictionary);

            selectionTimer.Stop();
        }

        /// <summary>
        /// Handles the CellPropertyChanged event of the tableCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="XPTable.Events.CellEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-01</remarks>
        private void tableCards_CellPropertyChanged(object sender, XPTable.Events.CellEventArgs e)
        {
            if (tableCards.TableModel.Rows.Count > 0 && buttonStart.Enabled)
                LoadCardForEditing();
        }

        /// <summary>
        /// Loads the card for editing.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-27</remarks>
        private void LoadCardForEditing()
        {
            int row = tableCards.FocusedCell.Row;
            //[ML-849] Error while collecting cards 
            if ((row < 0) && (tableCards.TableModel.Selections.SelectedItems.Length > 0))
                row = tableCards.TableModel.Selections.SelectedItems[0].Index;
            if (row < 0) return;

            string question = string.Empty;
            string questionExample = string.Empty;
            string answer = string.Empty;
            string answerExample = string.Empty;
            string questionImage = string.Empty;
            string answerImage = string.Empty;
            string questionAudio = string.Empty;
            string questionExampleAudio = string.Empty;
            string answerAudio = string.Empty;
            string answerExampleAudio = string.Empty;
            string questionVideo = string.Empty;
            string answerVideo = string.Empty;

            for (int pos = 1; pos < tableCards.ColumnModel.Columns.Count; pos++)
            {
                ListViewItem item = settingsPage.listViewElements.Items[pos - 1];

                if (tableCards.TableModel.Rows[row].Cells.Count > pos)
                {
                    if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.Question)
                        question = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.QuestionExample)
                        questionExample = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.Answer)
                        answer = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.AnswerExample)
                        answerExample = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.QuestionImage)
                        questionImage = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.AnswerImage)
                        answerImage = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.QuestionAudio)
                        questionAudio = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.QuestionExampleAudio)
                        questionExampleAudio = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.AnswerAudio)
                        answerAudio = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.AnswerExampleAudio)
                        answerExampleAudio = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.QuestionVideo)
                        questionVideo = tableCards.TableModel.Rows[row].Cells[pos].Text;
                    else if ((DAL.Helper.CardFields)item.Tag == Helper.CardFields.AnswerVideo)
                        answerVideo = tableCards.TableModel.Rows[row].Cells[pos].Text;
                }
            }

            //fix for [ML-543] collector crash when capturing image paths and adding this card to the dictionary
            questionImage = StripInvalidFileNameChars(questionImage);
            answerImage = StripInvalidFileNameChars(answerImage);
            questionAudio = StripInvalidFileNameChars(questionAudio);
            answerAudio = StripInvalidFileNameChars(answerAudio);
            questionExampleAudio = StripInvalidFileNameChars(questionExampleAudio);
            answerExampleAudio = StripInvalidFileNameChars(answerExampleAudio);
            questionVideo = StripInvalidFileNameChars(questionVideo);
            answerVideo = StripInvalidFileNameChars(answerVideo);

            cardEdit.LoadNewCard(dictionary, question, questionExample, answer, answerExample, questionImage, answerImage, questionAudio, questionExampleAudio, questionVideo,
                answerAudio, answerExampleAudio, answerVideo);

            //[ML-630] Collect cards --> confirm with <Enter> (ML crashes) 
            cardEdit.LoadingCard = false;
        }

        /// <summary>
        /// Strips the invalid file path characters.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-02-18</remarks>
        private string StripInvalidFileNameChars(string path)
        {
            foreach (char invalidchar in System.IO.Path.GetInvalidPathChars())
                path = path.Replace(invalidchar.ToString(), string.Empty);
            path = path.Trim();
            //check if the file path exists
            try
            {
                System.IO.FileInfo fileinfo = new System.IO.FileInfo(path);
                if (!fileinfo.Exists)
                    return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
            return path;
        }

        /// <summary>
        /// Handles the Add event of the cardEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-15</remarks>
        private void cardEdit_Add(object sender, EventArgs e)
        {
            AddNext();
        }

        /// <summary>
        /// Handles the Click event of the buttonAddNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-01</remarks>
        private void AddNext()
        {
            //[ML-849] Error while collecting cards 
            if ((tableCards.FocusedCell.Row < 0) && (tableCards.TableModel.Selections.SelectedItems.Length > 0))
                tableCards.TableModel.Rows.RemoveAt(tableCards.TableModel.Selections.SelectedItems[0].Index);
            else
                tableCards.TableModel.Rows.RemoveAt(tableCards.FocusedCell.Row);
            tableCards_SelectionChanged(null, null);
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkLabelEditColumns control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-17</remarks>
        private void linkLabelEditColumns_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowWizard();
        }
    }
}