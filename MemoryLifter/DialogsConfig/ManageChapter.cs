using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.Properties;

namespace MLifter
{
    /// <summary>
    /// Used to add/remove/modify chapters.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-07-19</remarks>
    public class SetupChaptersForm : System.Windows.Forms.Form
    {
        ToolTip emptyTitleToolTip = new ToolTip();

        private System.Windows.Forms.GroupBox GBChapters;
        private System.Windows.Forms.Label LDescr;
        private System.Windows.Forms.GroupBox GBSetting;
        private System.Windows.Forms.Button btnAddChapter;
        private System.Windows.Forms.Button btnDelChapter;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label LblDesc;
        private System.Windows.Forms.TextBox EdtChapter;
        private System.Windows.Forms.TextBox MDescr;
        private System.Windows.Forms.Button btnOkay;
        private IContainer components;
        private DragNDrop.DragAndDropListView DBChapters;

        private ToolTip ToolTipControl;
        private Button buttonStyle;
        private HelpProvider MainHelp;
        private ColumnHeader columnHeader1;

        private MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SetupChaptersForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            //
            MLifter.Classes.Help.SetHelpNameSpace(MainHelp);

            emptyTitleToolTip.Active = false;

            // If there's no chapter yet, add arrayList default one 
            if (Dictionary.Chapters.Chapters.Count == 0)
                Dictionary.Chapters.AddChapter(Resources.MANAGECHAPTER_DEFAULT_CHAPTER_NAME, Resources.MANAGECHAPTER_DEFAULT_CHAPTER_DESC);

            UpdateChapters();
            DBChapters.SelectedIndices.Add(0);
            EdtChapter.Focus();
            EdtChapter.SelectAll();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupChaptersForm));
            this.GBChapters = new System.Windows.Forms.GroupBox();
            this.DBChapters = new DragNDrop.DragAndDropListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.btnDelChapter = new System.Windows.Forms.Button();
            this.btnAddChapter = new System.Windows.Forms.Button();
            this.LDescr = new System.Windows.Forms.Label();
            this.GBSetting = new System.Windows.Forms.GroupBox();
            this.buttonStyle = new System.Windows.Forms.Button();
            this.MDescr = new System.Windows.Forms.TextBox();
            this.EdtChapter = new System.Windows.Forms.TextBox();
            this.LblDesc = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.ToolTipControl = new System.Windows.Forms.ToolTip(this.components);
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.GBChapters.SuspendLayout();
            this.GBSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // GBChapters
            // 
            resources.ApplyResources(this.GBChapters, "GBChapters");
            this.GBChapters.Controls.Add(this.DBChapters);
            this.GBChapters.Controls.Add(this.btnDelChapter);
            this.GBChapters.Controls.Add(this.btnAddChapter);
            this.GBChapters.Controls.Add(this.LDescr);
            this.GBChapters.Name = "GBChapters";
            this.MainHelp.SetShowHelp(this.GBChapters, ((bool)(resources.GetObject("GBChapters.ShowHelp"))));
            this.GBChapters.TabStop = false;
            // 
            // DBChapters
            // 
            this.DBChapters.AllowDrop = true;
            this.DBChapters.AllowReorder = true;
            resources.ApplyResources(this.DBChapters, "DBChapters");
            this.DBChapters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.DBChapters.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.DBChapters.HideSelection = false;
            this.DBChapters.LineColor = System.Drawing.Color.YellowGreen;
            this.DBChapters.MultiSelect = false;
            this.DBChapters.Name = "DBChapters";
            this.MainHelp.SetShowHelp(this.DBChapters, ((bool)(resources.GetObject("DBChapters.ShowHelp"))));
            this.DBChapters.TileSize = new System.Drawing.Size(300, 16);
            this.DBChapters.UseCompatibleStateImageBehavior = false;
            this.DBChapters.View = System.Windows.Forms.View.Details;
            this.DBChapters.SelectedIndexChanged += new System.EventHandler(this.DBChapters_SelectedIndexChanged);
            this.DBChapters.DragDrop += new System.Windows.Forms.DragEventHandler(this.DBChapter_DragDrop);
            this.DBChapters.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListItem_MouseMove);
            this.DBChapters.Click += new System.EventHandler(this.DBChapters_Click);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // btnDelChapter
            // 
            resources.ApplyResources(this.btnDelChapter, "btnDelChapter");
            this.btnDelChapter.Name = "btnDelChapter";
            this.MainHelp.SetShowHelp(this.btnDelChapter, ((bool)(resources.GetObject("btnDelChapter.ShowHelp"))));
            this.btnDelChapter.Click += new System.EventHandler(this.btnDelChapter_Click);
            // 
            // btnAddChapter
            // 
            resources.ApplyResources(this.btnAddChapter, "btnAddChapter");
            this.btnAddChapter.Name = "btnAddChapter";
            this.MainHelp.SetShowHelp(this.btnAddChapter, ((bool)(resources.GetObject("btnAddChapter.ShowHelp"))));
            this.btnAddChapter.Click += new System.EventHandler(this.btnAddChapter_Click);
            // 
            // LDescr
            // 
            resources.ApplyResources(this.LDescr, "LDescr");
            this.LDescr.Name = "LDescr";
            this.MainHelp.SetShowHelp(this.LDescr, ((bool)(resources.GetObject("LDescr.ShowHelp"))));
            // 
            // GBSetting
            // 
            resources.ApplyResources(this.GBSetting, "GBSetting");
            this.GBSetting.Controls.Add(this.buttonStyle);
            this.GBSetting.Controls.Add(this.MDescr);
            this.GBSetting.Controls.Add(this.EdtChapter);
            this.GBSetting.Controls.Add(this.LblDesc);
            this.GBSetting.Controls.Add(this.lblTitle);
            this.GBSetting.Controls.Add(this.btnApply);
            this.GBSetting.Name = "GBSetting";
            this.MainHelp.SetShowHelp(this.GBSetting, ((bool)(resources.GetObject("GBSetting.ShowHelp"))));
            this.GBSetting.TabStop = false;
            // 
            // buttonStyle
            // 
            resources.ApplyResources(this.buttonStyle, "buttonStyle");
            this.buttonStyle.Name = "buttonStyle";
            this.MainHelp.SetShowHelp(this.buttonStyle, ((bool)(resources.GetObject("buttonStyle.ShowHelp"))));
            this.buttonStyle.Click += new System.EventHandler(this.buttonStyle_Click);
            // 
            // MDescr
            // 
            resources.ApplyResources(this.MDescr, "MDescr");
            this.MDescr.Name = "MDescr";
            this.MainHelp.SetShowHelp(this.MDescr, ((bool)(resources.GetObject("MDescr.ShowHelp"))));
            // 
            // EdtChapter
            // 
            resources.ApplyResources(this.EdtChapter, "EdtChapter");
            this.EdtChapter.Name = "EdtChapter";
            this.MainHelp.SetShowHelp(this.EdtChapter, ((bool)(resources.GetObject("EdtChapter.ShowHelp"))));
            // 
            // LblDesc
            // 
            resources.ApplyResources(this.LblDesc, "LblDesc");
            this.LblDesc.Name = "LblDesc";
            this.MainHelp.SetShowHelp(this.LblDesc, ((bool)(resources.GetObject("LblDesc.ShowHelp"))));
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.Name = "lblTitle";
            this.MainHelp.SetShowHelp(this.lblTitle, ((bool)(resources.GetObject("lblTitle.ShowHelp"))));
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.MainHelp.SetShowHelp(this.btnApply, ((bool)(resources.GetObject("btnApply.ShowHelp"))));
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnOkay
            // 
            resources.ApplyResources(this.btnOkay, "btnOkay");
            this.btnOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOkay.Name = "btnOkay";
            this.MainHelp.SetShowHelp(this.btnOkay, ((bool)(resources.GetObject("btnOkay.ShowHelp"))));
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // SetupChaptersForm
            // 
            this.AcceptButton = this.btnApply;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnOkay;
            this.ControlBox = false;
            this.Controls.Add(this.btnOkay);
            this.Controls.Add(this.GBSetting);
            this.Controls.Add(this.GBChapters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupChaptersForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.SetupChaptersForm_Closing);
            this.GBChapters.ResumeLayout(false);
            this.GBSetting.ResumeLayout(false);
            this.GBSetting.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Load chapter names from the currently loaded dictionary
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-07-19</remarks>
        private void UpdateChapters()
        {
            // preserve selected chapter
            int currently_selected = -1;
            if (DBChapters.SelectedIndices.Count != 0)
                currently_selected = DBChapters.SelectedIndices[0];

            // Get available chapter names
            DBChapters.Items.Clear();
            foreach (IChapter chapter in Dictionary.Chapters.Chapters)
            {
                ListViewItem item = new ListViewItem(chapter.Title);
                item.Tag = chapter;
                DBChapters.Items.Add(item);
            }

            // reselect selected chapter
            if ((currently_selected >= 0) && (currently_selected < DBChapters.Items.Count))
                DBChapters.SelectedIndices.Add(currently_selected);
        }

        /// <summary>
        /// Handles the Click event of the DBChapters control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void DBChapters_Click(object sender, EventArgs e)
        {
            EdtChapter.Focus();
            EdtChapter.SelectAll();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the DBChapters control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void DBChapters_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Update title and Description to reflect selected chapter
            if (DBChapters.SelectedIndices.Count == 0)
            {
                GBSetting.Enabled = false;
                EdtChapter.Text = String.Empty;
                MDescr.Text = String.Empty;
            }
            else if (DBChapters.SelectedIndices.Count == 1)
            {
                IChapter chapter = (IChapter)DBChapters.SelectedItems[0].Tag;
                GBSetting.Enabled = true;
                EdtChapter.Text = chapter.Title;
                MDescr.Text = chapter.Description;
            }
            else
            {
                GBSetting.Enabled = false;
                //EdtChapter.Text = "Click to edit multiple...";
                //MDescr.Text = "Click to edit multiple...";
            }
            EdtChapter.Focus();
            EdtChapter.SelectAll();
        }

        /// <summary>
        /// Handles the Closing event of the SetupChaptersForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void SetupChaptersForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dictionary.Save();
        }

        /// <summary>
        /// Handles the Click event of the btnAddChapter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void btnAddChapter_Click(object sender, System.EventArgs e)
        {
            // Add arrayList new empty chapter
            Dictionary.Chapters.AddChapter(Resources.MANAGECHAPTER_NEW_CHAPTER_NAME, Resources.MANAGECHAPTER_NEW_CHAPTER_DESC);
            UpdateChapters();
            DBChapters.SelectedIndices.Clear();
            DBChapters.SelectedIndices.Add(DBChapters.Items.Count - 1);
        }

        /// <summary>
        /// Handles the Click event of the btnDelChapter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void btnDelChapter_Click(object sender, System.EventArgs e)
        {
            foreach (ListViewItem item in DBChapters.SelectedItems)
            {
                IChapter chapterToDelete = (IChapter)item.Tag;
                bool chapterInUse = Dictionary.QueryChapters.Contains(chapterToDelete.Id);

                if (Dictionary.Chapters.Chapters.Count > 1)
                {
                    if (chapterToDelete.Size > 0)
                    {
                        MLifter.Controls.TaskDialog.ShowTaskDialogBox(Resources.MANAGECHAPTER_DELETE_CHAPTER_CONFIRM_CAPTION, Resources.MANAGECHAPTER_DELETE_CHAPTER_CONFIRM_TEXT,
                            string.Format(Resources.MANAGECHAPTER_DELETE_CHAPTER_CONTENT, chapterToDelete.Title, chapterToDelete.Size),
                            string.Empty, Resources.MANAGECHAPTER_DELETE_CHAPTER_WARNING, string.Empty, string.Empty, Resources.MANAGECHAPTER_DELETE_CHAPTER_OPTION_YES + "|" + Resources.MANAGECHAPTER_DELETE_CHAPTER_OPTION_NO,
                            MLifter.Controls.TaskDialogButtons.None, MLifter.Controls.TaskDialogIcons.Question, MLifter.Controls.TaskDialogIcons.Warning);
                    }

                    if (chapterToDelete.Size < 1 || MLifter.Controls.TaskDialog.CommandButtonResult == 0)
                    {
                        if (chapterInUse)
                            Dictionary.QueryChapters.Remove(chapterToDelete.Id);

                        Dictionary.Chapters.DeleteChapterByID(chapterToDelete.Id);
                    }

                    if (Dictionary.Chapters.Chapters.Count == 0)
                        Dictionary.Chapters.AddChapter(Resources.MANAGECHAPTER_DEFAULT_CHAPTER_NAME, Resources.MANAGECHAPTER_DEFAULT_CHAPTER_DESC);
                }
                else
                    MessageBox.Show(Resources.MANAGECHAPTER_LAST_CHAPTER_MESSAGE_TEXT,
                        Resources.MANAGECHAPTER_LAST_CHAPTER_MESSAGE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);

                UpdateChapters();
            }
            // this is so that the selection is updated - the event is not fired if an element is deleted
            DBChapters_SelectedIndexChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Click event of the btnApply control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void btnApply_Click(object sender, System.EventArgs e)
        {
            if (String.IsNullOrEmpty(EdtChapter.Text) || (EdtChapter.Text.Trim().Length == 0))
            {
                emptyTitleToolTip.Active = true;
                emptyTitleToolTip.Show(Resources.MANAGECHAPTER_EMPTY_TITLE, EdtChapter, 60, 20, 2000);
                return;
            }
            else
            {
                emptyTitleToolTip.Hide(EdtChapter);
                emptyTitleToolTip.Active = false;
            }
            for (int item = 0; item < DBChapters.SelectedIndices.Count; item++)
            {
                // Update chapter information 
                ((IChapter)DBChapters.SelectedItems[item].Tag).Title = EdtChapter.Text;
                ((IChapter)DBChapters.SelectedItems[item].Tag).Description = MDescr.Text;
            }
            UpdateChapters();
            EdtChapter.Focus();
            EdtChapter.SelectAll();
        }

        /// <summary>
        /// Handles the Click event of the btnOkay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void btnOkay_Click(object sender, System.EventArgs e)
        {
            btnApply_Click(sender, e);
            this.Close();
        }

        /// <summary>
        /// Handles the DragDrop event of the DBChapter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void DBChapter_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Use drag and drop to change order of chapters 
            for (int count = 0; count < DBChapters.SelectedItems.Count; count++)
            {
                IChapter draggedChapter = (IChapter)DBChapters.SelectedItems[count].Tag;
                IChapter swappedChapter = Dictionary.Chapters.Chapters[DBChapters.SelectedItems[count].Index];
                Dictionary.Chapters.MoveChapter(draggedChapter.Id, swappedChapter.Id);
            }
            UpdateChapters();
        }

        /// <summary>
        /// Set the ToolTips
        /// </summary>
        private void setToolTips()
        {
            ToolTipControl.SetToolTip(DBChapters, "");
        }

        /// <summary>
        /// Display arrayList hint, showing the full field title
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListItem_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem current = (sender as DragNDrop.DragAndDropListView).GetItemAt(e.X, e.Y);
            if (current != null && current.Tag as IChapter != null && !string.IsNullOrEmpty((current.Tag as IChapter).Description))
            {
                IChapter chapter = (IChapter)current.Tag;
                ToolTipControl.SetToolTip((sender as DragNDrop.DragAndDropListView), string.IsNullOrEmpty(chapter.Description) ? string.Empty : chapter.Title + " - " + chapter.Description);
            }
            else
                ToolTipControl.SetToolTip((sender as DragNDrop.DragAndDropListView), String.Empty);
        }

        /// <summary>
        /// Handles the Click event of the buttonStyle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-02-06</remarks>
        private void buttonStyle_Click(object sender, EventArgs e)
        {
            if (DBChapters.SelectedItems.Count == 0)
                return;
            ICard card;
            IChapter chapter = (IChapter)DBChapters.SelectedItems[0].Tag;
            System.Collections.Generic.List<ICard> cards = Dictionary.GetCardsFromChapter(chapter);
            if (cards.Count > 0)
            {
                card = cards[0];
            }
            else
            {
                card = new MLifter.DAL.Preview.PreviewCard(null);
                card.Chapter = chapter.Id;
                card.Answer.AddWord(card.Answer.CreateWord(Properties.Resources.EXAMPLE_CARD_ANSWER, WordType.Word, true));
                card.AnswerExample.AddWord(card.AnswerExample.CreateWord(Properties.Resources.EXAMPLE_CARD_ANSWER_EXAMPLE, WordType.Sentence, false));
                card.Question.AddWord(card.Question.CreateWord(Properties.Resources.EXAMPLE_CARD_QUESTION, WordType.Word, true));
                card.QuestionExample.AddWord(card.QuestionExample.CreateWord(Properties.Resources.EXAMPLE_CARD_QUESTION_EXAMPLE, WordType.Sentence, false));
            }

            if (((IChapter)DBChapters.SelectedItems[0].Tag).Settings == null)
                ((IChapter)DBChapters.SelectedItems[0].Tag).Settings = Dictionary.CreateSettings();
            if (((IChapter)DBChapters.SelectedItems[0].Tag).Settings.Style == null)
                ((IChapter)DBChapters.SelectedItems[0].Tag).Settings.Style = ((IChapter)DBChapters.SelectedItems[0].Tag).CreateCardStyle();

            MLifter.Controls.CardStyleEditor editor = new MLifter.Controls.CardStyleEditor();
            editor.HelpNamespace = MLifter.Classes.Help.HelpPath;
            editor.LoadStyle(card, ((IChapter)DBChapters.SelectedItems[0].Tag).Settings.Style, Dictionary, DBChapters.SelectedItems[0].Tag);
            switch (editor.ShowDialog())
            {
                case DialogResult.Abort:
                    ((IChapter)DBChapters.SelectedItems[0].Tag).Settings.Style = null;
                    break;
                case DialogResult.Cancel:
                    break;
                default:
                    Dictionary.UseDictionaryStyleSheets = true;
                    break;
            }
            //cleanup
            if (card != null) card.Dispose();
        }
    }
}
