using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MLifter.Controls.Properties;
using MLifter.Components;
using MLifter.BusinessLayer;

namespace MLifter.Controls
{
    /// <summary>
    /// This frame is used in various parts of the program. It gives the user the
    /// possibility to select given fieldIDs.
    /// </summary>
    /// <remarks>Documented by Dev04, 2007-07-20</remarks>
    public class FieldsFrame : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Button SBAdd;
        private System.Windows.Forms.Button SBAddAll;
        private System.Windows.Forms.Button SBDel;
        private IContainer components;

        public int[] Order = new int[0];
        public string[] StrOrder;
        private int maxKey;

        public const int FIELDSELEMENTS = 15;
        private ImageList TangoCollection16;
        private ImageList OldButtonCollection16;
        private DragNDrop.DragAndDropListView LBFields;
        private DragNDrop.DragAndDropListView LBSelFields;
        private ToolTip ToolTipControl;
        private Button SBDelAll;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader1;
        public string[] FullStr = new string[FIELDSELEMENTS];

        public delegate void FieldEventHandler(object sender);
        public event FieldEventHandler OnUpdate;

        /// <summary>
        /// Constructor of FieldsForm
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        public FieldsFrame()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FieldsFrame));
            this.SBAdd = new System.Windows.Forms.Button();
            this.TangoCollection16 = new System.Windows.Forms.ImageList(this.components);
            this.SBAddAll = new System.Windows.Forms.Button();
            this.SBDel = new System.Windows.Forms.Button();
            this.OldButtonCollection16 = new System.Windows.Forms.ImageList(this.components);
            this.ToolTipControl = new System.Windows.Forms.ToolTip(this.components);
            this.SBDelAll = new System.Windows.Forms.Button();
            this.LBSelFields = new DragNDrop.DragAndDropListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.LBFields = new DragNDrop.DragAndDropListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // SBAdd
            // 
            resources.ApplyResources(this.SBAdd, "SBAdd");
            this.SBAdd.ImageList = this.TangoCollection16;
            this.SBAdd.Name = "SBAdd";
            this.SBAdd.Click += new System.EventHandler(this.SBAdd_Click);
            // 
            // TangoCollection16
            // 
            this.TangoCollection16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TangoCollection16.ImageStream")));
            this.TangoCollection16.TransparentColor = System.Drawing.Color.Transparent;
            this.TangoCollection16.Images.SetKeyName(0, "go-next.png");
            this.TangoCollection16.Images.SetKeyName(1, "go-last.png");
            this.TangoCollection16.Images.SetKeyName(2, "go-previous.png");
            this.TangoCollection16.Images.SetKeyName(3, "go-first.png");
            // 
            // SBAddAll
            // 
            resources.ApplyResources(this.SBAddAll, "SBAddAll");
            this.SBAddAll.ImageList = this.TangoCollection16;
            this.SBAddAll.Name = "SBAddAll";
            this.SBAddAll.Click += new System.EventHandler(this.SBAddAll_Click);
            // 
            // SBDel
            // 
            resources.ApplyResources(this.SBDel, "SBDel");
            this.SBDel.ImageList = this.TangoCollection16;
            this.SBDel.Name = "SBDel";
            this.SBDel.Click += new System.EventHandler(this.SBDel_Click);
            // 
            // OldButtonCollection16
            // 
            this.OldButtonCollection16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("OldButtonCollection16.ImageStream")));
            this.OldButtonCollection16.TransparentColor = System.Drawing.Color.Transparent;
            this.OldButtonCollection16.Images.SetKeyName(0, "");
            this.OldButtonCollection16.Images.SetKeyName(1, "");
            this.OldButtonCollection16.Images.SetKeyName(2, "");
            this.OldButtonCollection16.Images.SetKeyName(3, "delete2.gif");
            // 
            // SBDelAll
            // 
            resources.ApplyResources(this.SBDelAll, "SBDelAll");
            this.SBDelAll.ImageList = this.TangoCollection16;
            this.SBDelAll.Name = "SBDelAll";
            this.SBDelAll.Click += new System.EventHandler(this.SBDelAll_Click);
            // 
            // LBSelFields
            // 
            this.LBSelFields.AllowDrop = true;
            this.LBSelFields.AllowReorder = true;
            this.LBSelFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.LBSelFields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.LBSelFields.LineColor = System.Drawing.Color.YellowGreen;
            resources.ApplyResources(this.LBSelFields, "LBSelFields");
            this.LBSelFields.Name = "LBSelFields";
            this.LBSelFields.TileSize = new System.Drawing.Size(156, 16);
            this.LBSelFields.UseCompatibleStateImageBehavior = false;
            this.LBSelFields.View = System.Windows.Forms.View.Details;
            this.LBSelFields.DoubleClick += new System.EventHandler(this.LBSelFields_DoubleClick);
            this.LBSelFields.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_DragDrop);
            this.LBSelFields.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListItem_MouseMove);
            this.LBSelFields.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LBSelFields_KeyDown);
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // LBFields
            // 
            this.LBFields.AllowDrop = true;
            this.LBFields.AllowReorder = true;
            this.LBFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.LBFields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.LBFields.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("LBFields.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("LBFields.Items1"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("LBFields.Items2"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("LBFields.Items3")))});
            this.LBFields.LineColor = System.Drawing.Color.YellowGreen;
            resources.ApplyResources(this.LBFields, "LBFields");
            this.LBFields.Name = "LBFields";
            this.LBFields.TileSize = new System.Drawing.Size(156, 16);
            this.LBFields.UseCompatibleStateImageBehavior = false;
            this.LBFields.View = System.Windows.Forms.View.Details;
            this.LBFields.DoubleClick += new System.EventHandler(this.LBFields_DoubleClick);
            this.LBFields.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_DragDrop);
            this.LBFields.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListItem_MouseMove);
            this.LBFields.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LBFields_KeyDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // FieldsFrame
            // 
            this.Controls.Add(this.LBSelFields);
            this.Controls.Add(this.LBFields);
            this.Controls.Add(this.SBDelAll);
            this.Controls.Add(this.SBDel);
            this.Controls.Add(this.SBAddAll);
            this.Controls.Add(this.SBAdd);
            this.Name = "FieldsFrame";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary> 
        /// Add: move from LBFields to LBSelFields
        /// </summary>
        /// <param name="e">not used</param>
        /// <param name="sender">not used</param>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void SBAdd_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i <= maxKey; i++)
                if (LBFields.SelectedItems.ContainsKey(Convert.ToString(i)))
                {
                    ListViewItem item = LBFields.Items[Convert.ToString(i)];
                    LBFields.Items[Convert.ToString(i)].Remove();
                    LBSelFields.Items.Add(item);
                }
            CalcOrder();
            LBFields.SelectedIndices.Clear();
            LBSelFields.SelectedIndices.Clear();
        }

        /// <summary> 
        /// Select all fieldIDs, then add them (with SBAdd_Click)
        /// </summary>
        /// <param name="e">used for starting SBAdd_Click event</param>
        /// <param name="sender">used for starting SBAdd_Click event</param>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void SBAddAll_Click(object sender, System.EventArgs e)
        {
            // Select all fieldIDs, then add them
            for (int i = 0; i < LBFields.Items.Count; i++)
                LBFields.SelectedIndices.Add(i);

            SBAdd_Click(sender, e);
        }

        /// <summary> 
        /// Remove: move from LBSelFields to LBFields
        /// </summary>
        /// <param name="e">not used</param>
        /// <param name="sender">not used</param>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void SBDel_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i <= maxKey; i++)
                if (LBSelFields.SelectedItems.ContainsKey(Convert.ToString(i)))
                {
                    ListViewItem item = LBSelFields.Items[Convert.ToString(i)];
                    LBSelFields.Items[Convert.ToString(i)].Remove();
                    LBFields.Items.Add(item);
                }
            CalcOrder();
            LBFields.SelectedIndices.Clear();
            LBSelFields.SelectedIndices.Clear();
        }

        /// <summary> 
        /// Select all fieldIDs, then remove them (with SBDel_Click)
        /// </summary>
        /// <param name="e">used for starting SBDel_Click event</param>
        /// <param name="sender">used for starting SBDel_Click event</param>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void SBDelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < LBSelFields.Items.Count; i++)
                LBSelFields.SelectedIndices.Add(i);

            SBDel_Click(sender, e);
        }

        /// <summary> 
        /// Deletes double-clicked field from LBSelFields
        /// </summary>
        /// <param name="e">used for starting SBDel_Click event</param>
        /// <param name="sender">used for starting SBDel_Click event</param>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void LBSelFields_DoubleClick(object sender, System.EventArgs e)
        {
            SBDel_Click(sender, e);
        }

        /// <summary> 
        /// Adds double-clicked field to LBSelFields
        /// </summary>
        /// <param name="e">used for starting SBAdd_Click event</param>
        /// <param name="sender">used for starting SBAdd_Click event</param>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void LBFields_DoubleClick(object sender, System.EventArgs e)
        {
            SBAdd_Click(sender, e);
        }

        /// <summary> 
        /// If the "right" key is pressed on the keyboard and the focus is on LBFields,
        /// it moves the selected item to LBSelFields.
        /// </summary>
        /// <param name="e">not used</param>
        /// <param name="sender">used for starting SBAdd_Click event</param>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void LBFields_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                SBAdd_Click(sender, null);
        }

        /// <summary> 
        /// If the "left" key or the "delete" key is pressed on the keyboard and the focus is on LBSelFields,
        /// it moves the selected item to LBFields.
        /// </summary>
        /// <param name="e">not used</param>
        /// <param name="sender">used for starting SBDel_Click event</param>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void LBSelFields_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left)
                SBDel_Click(sender, null);
        }

        /// <summary> 
        /// Prepares the form to have the Questions and Answers by default
        /// on the right side e.g. for the export.
        /// No Unit Test necessary.
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        public void Prepare(Dictionary dictionary)
        {
            this.setToolTips();

            string question_caption = dictionary.QuestionCaption;
            string answer_caption = dictionary.AnswerCaption;

            // Add the available fieldIDs to the left side }
            LBFields.Items.Clear();
            LBFields.SetLockCode("field");
            LBSelFields.Items.Clear();
            LBSelFields.SetLockCode("field");

            // Set standard field as selected
            FullStr[0] = String.Format(Resources.LISTBOXFIELDS_QUESTION_TEXT, question_caption);
            LBSelFields.Items.Add("0", FullStr[0], 0);
            FullStr[1] = String.Format(Resources.LISTBOXFIELDS_ANSWER_TEXT, answer_caption);
            LBSelFields.Items.Add("1", FullStr[1], 0);

            FullStr[2] = String.Format(Resources.LISTBOXFIELDS_EXQUESTION_TEXT, question_caption);
            LBFields.Items.Add("2", FullStr[2], 0);
            FullStr[3] = String.Format(Resources.LISTBOXFIELDS_EXANSWER_TEXT, answer_caption);
            LBFields.Items.Add("3", FullStr[3], 0);

            FullStr[4] = String.Format(Resources.LISTBOXFIELDS_QUESTION_DISTRACTORS_TEXT, question_caption);
            LBFields.Items.Add("4", FullStr[4], 0);
            FullStr[5] = String.Format(Resources.LISTBOXFIELDS_ANSWER_DISTRACTORS_TEXT, answer_caption);
            LBFields.Items.Add("5", FullStr[5], 0);

            maxKey = 5; // highest key when adding

            CalcOrder();
        }

        /// <summary> 
        /// Prepares the form to have the Media-File-Options on the left side.
        /// No Unit Test necessary.
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        public void PrepareImage(Dictionary dictionary)
        {
            Prepare(dictionary);
            string question_caption = dictionary.QuestionCaption;
            string answer_caption = dictionary.AnswerCaption;

            // Add sound and images 
            FullStr[6] = String.Format(Resources.LISTBOXFIELDS_SOUND_QUESTION_TEXT, question_caption);
            LBFields.Items.Add("6", FullStr[6], 0);
            FullStr[7] = String.Format(Resources.LISTBOXFIELDS_SOUND_ANSWER_TEXT, answer_caption);
            LBFields.Items.Add("7", FullStr[7], 0);
            FullStr[8] = String.Format(Resources.LISTBOXFIELDS_SOUND_EXQUESTION_TEXT, question_caption);
            LBFields.Items.Add("8", FullStr[8], 0);
            FullStr[9] = String.Format(Resources.LISTBOXFIELDS_SOUND_EXANSWER_TEXT, answer_caption);
            LBFields.Items.Add("9", FullStr[9], 0);
            FullStr[10] = String.Format(Resources.LISTBOXFIELDS_VIDEO_QUESTION_TEXT, question_caption);
            LBFields.Items.Add("10", FullStr[10], 0);
            FullStr[11] = String.Format(Resources.LISTBOXFIELDS_VIDEO_ANSWER_TEXT, answer_caption);
            LBFields.Items.Add("11", FullStr[11], 0);
            FullStr[12] = String.Format(Resources.LISTBOXFIELDS_IMAGE_QUESTION_TEXT, question_caption);
            LBFields.Items.Add("12", FullStr[12], 0);
            FullStr[13] = String.Format(Resources.LISTBOXFIELDS_IMAGE_ANSWER_TEXT, answer_caption);
            LBFields.Items.Add("13", FullStr[13], 0);

            maxKey = 13; // highest key when adding
        }

        /// <summary>
        /// Prepares the form to contain the Chapter-field.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        public void PrepareChapter(Dictionary dictionary)
        {
            PrepareImage(dictionary);

            //Add chapter
            LBFields.Items.Add("14", Resources.LISTBOXFIELDS_CHAPTER, 0);

            maxKey = 14; // highest key when adding
        }

        /// <summary>
        /// Gets a value indicating whether the current field selection [contains Media files].
        /// </summary>
        /// <value><c>true</c> if [contains Media]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-02-12</remarks>
        public bool ContainsMedia
        {
            get
            {
                bool containsMedia = false;
                foreach (string field in StrOrder)
                {
                    for (int i = 6; i <= 13; i++)
                        if (FullStr[i] == field)
                            containsMedia = true;
                }
                return containsMedia;
            }
        }

        /// <summary> 
        /// Based on the fieldIDs, calculate the Order array
        /// No Unit Test necessary.
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void CalcOrder()
        {
            Order = new int[FIELDSELEMENTS];
            int h = LBSelFields.Items.Count;
            StrOrder = new string[h];

            for (int i = 0; i < LBSelFields.Items.Count; i++)
            {
                Order[i] = Convert.ToInt32(LBSelFields.Items[i].Name);
                StrOrder[i] = LBSelFields.Items[i].Text;
            }

            if (OnUpdate != null)
                OnUpdate(this);
        }

        /// <summary> 
        /// Sets Tool Tips for the controls.
        /// No Unit Test necessary.
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void setToolTips()
        {
            ToolTipControl.SetToolTip(SBAdd, Resources.TOOLTIP_FRMFIELDS_ADD);
            ToolTipControl.SetToolTip(SBAddAll, Resources.TOOLTIP_FRMFIELDS_ADDALL);
            ToolTipControl.SetToolTip(SBDel, Resources.TOOLTIP_FRMFIELDS_DELETE);
            ToolTipControl.SetToolTip(SBDelAll, Resources.TOOLTIP_FRMFIELDS_DELETEALL);
            ToolTipControl.SetToolTip(LBSelFields, Resources.TOOLTIP_FRMFIELDS_LISTBOXSELFIELDS);
            ToolTipControl.SetToolTip(LBFields, Resources.TOOLTIP_FRMFIELDS_LISTBOXFIELDS);
        }

        /// <summary> 
        /// Display arrayList hint, showing the full field title.
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void ListItem_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem current = (sender as DragNDrop.DragAndDropListView).GetItemAt(e.X, e.Y);
            if (current != null)
                ToolTipControl.SetToolTip((sender as DragNDrop.DragAndDropListView), current.Text);
            else
                ToolTipControl.SetToolTip((sender as DragNDrop.DragAndDropListView), "");
        }

        /// <summary> 
        /// Recalculates the order after drag+drop of an item.
        /// </summary>
        /// <remarks>Documented by Dev04, 2007-07-20</remarks>
        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            CalcOrder();
        }

    }
}
