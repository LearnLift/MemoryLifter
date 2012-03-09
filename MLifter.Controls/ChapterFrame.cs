using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using MLifter.DAL.Interfaces;
using MLifter.Controls.Properties;
using MLifter.BusinessLayer;

namespace MLifter.Controls
{
    /// <summary>
    /// This frame is used in various parts of the program. It gives the user the
    /// possibility to select given chapters, arrange their order, etc.
    /// </summary>
    /// <remarks>Documented by Dev03, 2007-07-18</remarks>
    public class ChapterFrame : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Label LblRemaining;
        private System.Windows.Forms.Label LblSelected;
        private System.Windows.Forms.Label LblRemainingCount;
        private System.Windows.Forms.Label LblSelectedCount;
        private System.Windows.Forms.Button SBAdd;
        private System.Windows.Forms.Button SBAddAll;
        private System.Windows.Forms.Button SBDel;
        private IContainer components;

        private ToolTip ToolTipControl;
        private ImageList TangoCollection16;
        private ImageList OldButtonCollection16;
        private DragNDrop.DragAndDropListView DBChapters;
        private DragNDrop.DragAndDropListView LBChapters;
        private Button SBDelAll;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader1;

        public delegate void FieldEventHandler(object sender);
        public event FieldEventHandler OnUpdate;

        public ChapterFrame()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChapterFrame));
            this.LblRemaining = new System.Windows.Forms.Label();
            this.LblSelected = new System.Windows.Forms.Label();
            this.LblRemainingCount = new System.Windows.Forms.Label();
            this.LblSelectedCount = new System.Windows.Forms.Label();
            this.SBAdd = new System.Windows.Forms.Button();
            this.TangoCollection16 = new System.Windows.Forms.ImageList(this.components);
            this.SBAddAll = new System.Windows.Forms.Button();
            this.SBDel = new System.Windows.Forms.Button();
            this.ToolTipControl = new System.Windows.Forms.ToolTip(this.components);
            this.SBDelAll = new System.Windows.Forms.Button();
            this.OldButtonCollection16 = new System.Windows.Forms.ImageList(this.components);
            this.LBChapters = new DragNDrop.DragAndDropListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.DBChapters = new DragNDrop.DragAndDropListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // LblRemaining
            // 
            resources.ApplyResources(this.LblRemaining, "LblRemaining");
            this.LblRemaining.Name = "LblRemaining";
            // 
            // LblSelected
            // 
            resources.ApplyResources(this.LblSelected, "LblSelected");
            this.LblSelected.Name = "LblSelected";
            // 
            // LblRemainingCount
            // 
            resources.ApplyResources(this.LblRemainingCount, "LblRemainingCount");
            this.LblRemainingCount.Name = "LblRemainingCount";
            // 
            // LblSelectedCount
            // 
            resources.ApplyResources(this.LblSelectedCount, "LblSelectedCount");
            this.LblSelectedCount.Name = "LblSelectedCount";
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
            // SBDelAll
            // 
            resources.ApplyResources(this.SBDelAll, "SBDelAll");
            this.SBDelAll.ImageList = this.TangoCollection16;
            this.SBDelAll.Name = "SBDelAll";
            this.SBDelAll.Click += new System.EventHandler(this.SBDelAll_Click);
            // 
            // OldButtonCollection16
            // 
            this.OldButtonCollection16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("OldButtonCollection16.ImageStream")));
            this.OldButtonCollection16.TransparentColor = System.Drawing.Color.Transparent;
            this.OldButtonCollection16.Images.SetKeyName(0, "add.gif");
            this.OldButtonCollection16.Images.SetKeyName(1, "addall.gif");
            this.OldButtonCollection16.Images.SetKeyName(2, "del.gif");
            this.OldButtonCollection16.Images.SetKeyName(3, "delete2.gif");
            // 
            // LBChapters
            // 
            this.LBChapters.AllowDrop = true;
            this.LBChapters.AllowReorder = true;
            resources.ApplyResources(this.LBChapters, "LBChapters");
            this.LBChapters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.LBChapters.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.LBChapters.HideSelection = false;
            this.LBChapters.LineColor = System.Drawing.Color.YellowGreen;
            this.LBChapters.Name = "LBChapters";
            this.LBChapters.TileSize = new System.Drawing.Size(156, 16);
            this.LBChapters.UseCompatibleStateImageBehavior = false;
            this.LBChapters.View = System.Windows.Forms.View.Details;
            this.LBChapters.DoubleClick += new System.EventHandler(this.LBChapters_DoubleClick);
            this.LBChapters.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_DragDrop);
            this.LBChapters.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListItem_MouseMove);
            this.LBChapters.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LBChapters_KeyDown);
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
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
            this.DBChapters.Name = "DBChapters";
            this.DBChapters.TileSize = new System.Drawing.Size(156, 16);
            this.DBChapters.UseCompatibleStateImageBehavior = false;
            this.DBChapters.View = System.Windows.Forms.View.Details;
            this.DBChapters.DoubleClick += new System.EventHandler(this.DBChapters_DoubleClick);
            this.DBChapters.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_DragDrop);
            this.DBChapters.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListItem_MouseMove);
            this.DBChapters.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DBChapters_KeyDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // ChapterFrame
            // 
            this.Controls.Add(this.LblRemainingCount);
            this.Controls.Add(this.LblSelected);
            this.Controls.Add(this.LBChapters);
            this.Controls.Add(this.DBChapters);
            this.Controls.Add(this.SBDelAll);
            this.Controls.Add(this.SBDel);
            this.Controls.Add(this.SBAddAll);
            this.Controls.Add(this.SBAdd);
            this.Controls.Add(this.LblRemaining);
            this.Controls.Add(this.LblSelectedCount);
            this.Name = "ChapterFrame";
            resources.ApplyResources(this, "$this");
            this.Resize += new System.EventHandler(this.ChapterFrame_Resize);
            this.ResumeLayout(false);

        }
        #endregion


        #region Properties
        /// <summary>
        /// Total number of cards in selected chapters
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        public int Total
        {
            get
            {
                return Convert.ToInt32(LblSelectedCount.Text);
            }
        }

        /// <summary>
        /// Gets the IDs of the selected chapters.
        /// </summary>
        /// <value>The IDs of the selected chapters.</value>
        /// <remarks>Documented by Dev02, 2007-11-14</remarks>
        public List<int> SelChapters
        {
            get
            {
                List<int> selChapters = new List<int>();
                foreach (ListViewItem lvichapter in LBChapters.Items)
                {
                    selChapters.Add((lvichapter.Tag as IChapter).Id);
                }
                return selChapters;
            }
        }
        #endregion

        /// <summary>
        /// Calculates the amount of the currently selected and target cards and updates the labels.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        /// <remarks>Documented by Dev02, 2007-11-14</remarks>
        private void RefreshLabels()
        {
            int selectedCards = 0;

            foreach (ListViewItem lviChapter in LBChapters.Items)
            {
                selectedCards += (lviChapter.Tag as IChapter).Size;
            }
            LblSelectedCount.Text = selectedCards.ToString();
            //this.Invoke((MethodInvoker)delegate { LblSelectedCount.Text = selectedCards.ToString(); });

            int remainingCards = 0;
            foreach (ListViewItem lviChapter in DBChapters.Items)
            {
                remainingCards += (lviChapter.Tag as IChapter).Size;
            }
            LblRemainingCount.Text = remainingCards.ToString();
            //this.Invoke((MethodInvoker)delegate { LblRemainingCount.Text = remainingCards.ToString(); });

            if (OnUpdate != null)
                OnUpdate(this);
        }

        /// <summary>
        /// Adds the selected chapters from DBChapters to LBChapters
        /// </summary>
        /// <param name="sender">Button object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        /// <remarks>Documented by Dev02, 2007-11-14</remarks>
        private void SBAdd_Click(object sender, System.EventArgs e)
        {
            // Add: move from DBChapters to LBChapters
            foreach (ListViewItem lviChapter in DBChapters.SelectedItems)
            {
                DBChapters.Items.Remove(lviChapter);
                LBChapters.Items.Add(lviChapter);
            }
            RefreshLabels();
        }

        /// <summary>
        /// Adds all chapters to LBChapters
        /// </summary>
        /// <param name="sender">Button object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        /// <remarks>Documented by Dev02, 2007-11-14</remarks>
        private void SBAddAll_Click(object sender, System.EventArgs e)
        {
            // Select all chapters and then perform add
            foreach (ListViewItem lviChapter in DBChapters.Items)
                lviChapter.Selected = true;

            SBAdd_Click(sender, e);
        }

        /// <summary>
        /// Removes the selected chapters from LBChapters
        /// </summary>
        /// <param name="sender">Button object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        /// <remarks>Documented by Dev02, 2007-11-14</remarks>
        private void SBDel_Click(object sender, System.EventArgs e)
        {
            // Remove: move from LBChapters to DBChapters
            foreach (ListViewItem lviChapter in LBChapters.SelectedItems)
            {
                LBChapters.Items.Remove(lviChapter);
                DBChapters.Items.Add(lviChapter);
            }

            RefreshLabels();
        }

        /// <summary>
        /// Removes all chapters from LBChapters
        /// </summary>
        /// <param name="sender">Button object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        /// <remarks>Documented by Dev02, 2007-11-14</remarks>
        private void SBDelAll_Click(object sender, System.EventArgs e)
        {
            // Select all chapters and then perform remove
            foreach (ListViewItem lviChapter in LBChapters.Items)
                lviChapter.Selected = true;

            SBDel_Click(sender, e);
        }

        /// <summary>
        /// Adds the selected chapters to LBChapters when arrayList double click on DBChapters occurs.
        /// </summary>
        /// <param name="sender">DragAndDropListView object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        private void DBChapters_DoubleClick(object sender, System.EventArgs e)
        {
            SBAdd_Click(sender, e);
        }

        /// <summary>
        /// Forwards the event to SBAdd_Click whenever the arrow-right key is pressed.
        /// </summary>
        /// <param name="sender">DragAndDropListView object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        private void DBChapters_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                SBAdd_Click(sender, null);
        }

        /// <summary>
        /// Forwards the event to SBDel_Click whenever the arrow-left or delete key is pressed.
        /// </summary>
        /// <param name="sender">DragAndDropListView object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        private void LBChapters_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left)
                SBDel_Click(sender, null);
        }

        /// <summary>
        /// Removes the selected chapters to LBChapters when arrayList double click on DBChapters occurs.
        /// </summary>
        /// <param name="sender">DragAndDropListView object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        private void LBChapters_DoubleClick(object sender, System.EventArgs e)
        {
            SBDel_Click(sender, e);
        }

        /// <summary>
        /// Populates the chapter list objects.
        /// </summary>
        /// <param name="wordbase">The currently used dictionary</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        public void Prepare(Dictionary dictionary)
        {
            this.Prepare(dictionary, true);
        }

        /// <summary>
        /// Populates the chapter list objects.
        /// </summary>
        /// <param name="wordbase">The currently used dictionary</param>
        /// <param name="fill_right_side">True if the rigth side should be populated</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        /// <remarks>Documented by Dev02, 2007-11-14</remarks>
        public void Prepare(Dictionary dictionary, bool fill_right_side)
        {
            IList<IChapter> chapters = dictionary.Chapters.Chapters;
            this.setToolTips();
            DBChapters.Items.Clear();
            LBChapters.Items.Clear();

            // Get left side from available chapters
            foreach (IChapter chapter in chapters)
            {
                ListViewItem lviChapter = new ListViewItem();
                lviChapter.Name = chapter.Id.ToString();
                lviChapter.Text = chapter.Title;
                string description = chapter.Description;
                if (string.IsNullOrEmpty(description))
                    description = Properties.Resources.CHAPTER_FRAME_NODESCRIPTION;
                lviChapter.ToolTipText = string.Format(Properties.Resources.CHAPTER_FRAME_TOOLTIP, chapter.Size, description);
                lviChapter.Tag = chapter;
                DBChapters.Items.Add(lviChapter);
            }

            // move selected chapters to the right side
            if (fill_right_side)
            {
                foreach (int id in dictionary.QueryChapters)
                {
                    ListViewItem[] lviChapters = DBChapters.Items.Find(id.ToString(), false);
                    foreach (ListViewItem lviChapter in lviChapters)
                        lviChapter.Selected = true;
                }

                SBAdd_Click(this, new EventArgs());
            }
        }

        /// <summary>
        /// Counts the number of selected chapters in LBChapters.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        public int GetItemCount()
        {
            return LBChapters.Items.Count;
        }

        /// <summary>
        /// Sets the tool tips for the controls.
        /// </summary>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        private void setToolTips()
        {
            ToolTipControl.SetToolTip(SBAdd, Resources.TOOLTIP_FRMCHAPTER_SBADD);
            ToolTipControl.SetToolTip(SBAddAll, Resources.TOOLTIP_FRMCHAPTER_SBADDALL);
            ToolTipControl.SetToolTip(SBDel, Resources.TOOLTIP_FRMCHAPTER_SBDEL);
            ToolTipControl.SetToolTip(SBDelAll, Resources.TOOLTIP_FRMCHAPTER_SBDELALL);
            ToolTipControl.SetToolTip(LBChapters, Resources.TOOLTIP_FRMCHAPTER_LBCHAPTERS);
        }

        /// <summary>
        /// Display arrayList hint whenever the cursor moves over arrayList list item.
        /// </summary>
        /// <param name="sender">DragAndDropListView object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        private void ListItem_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem current = (sender as DragNDrop.DragAndDropListView).GetItemAt(e.X, e.Y);
            if (current != null)
                ToolTipControl.SetToolTip((sender as DragNDrop.DragAndDropListView), current.ToolTipText);
            else
                ToolTipControl.SetToolTip((sender as DragNDrop.DragAndDropListView), string.Empty);
        }


        /// <summary>
        /// Calls CalcChapters whenever arrayList drag-and-drop event occurs for arrayList ListView control.
        /// </summary>
        /// <param name="sender">DragAndDropListView object which raised the event.</param>
        /// <param name="e">System.EventArgs that contains the event data.</param>
        /// <remarks>Documented by Dev03, 2007-07-18</remarks>
        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            RefreshLabels();
        }

        /// <summary>
        /// Handles the Resize event of the ChapterFrame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-30</remarks>
        private void ChapterFrame_Resize(object sender, EventArgs e)
        {
            DBChapters.Width = Width / 2 - 20;
            LBChapters.Width = DBChapters.Width;
            LBChapters.Left = Width - LBChapters.Width;

            //[ML-1143] Resize columns according to listview width (the 25 are so that no horicontal scrollbar appears)
            if (DBChapters.Columns.Count > 0)
                DBChapters.Columns[0].Width = DBChapters.Width - 25;
            if (LBChapters.Columns.Count > 0)
                LBChapters.Columns[0].Width = LBChapters.Width - 25;

            LblRemainingCount.Left = DBChapters.Width - LblRemainingCount.Width;
            LblSelected.Left = Width / 2 + 20;
        }

    }
}