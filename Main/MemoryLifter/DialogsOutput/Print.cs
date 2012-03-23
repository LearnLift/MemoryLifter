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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

using MLifter.Properties;
using MLifter.DAL.Interfaces;
using MLifter.Controls;
using MLifter.Components;

namespace MLifter
{
    /// <summary>
    /// Allows the user to select chapters and print their content with or without multimedia (uses Internet Explorer).
    /// </summary>
    public class PrintForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox GBChapters;
        private System.Windows.Forms.Button btnCancel;
        private MLifter.Controls.ChapterFrame Chapters;
        private WebBrowser Browser;
        private Button btnPreview;
        private ComboBox comboBoxPrintStyle;
        private ComboBox comboBoxCardOrder;
        private Label lblPrintStyle;
        private ListView listViewBoxesSelection;
        private CheckBox checkBoxAllBoxes;
        private LoadStatusMessage statusDialog;
        private GroupBox GBBoxes;
        private CheckBox cbReverseOrder;
        private Label lblCardOrder;
        private Label lblCurSelVal;
        private GroupBox GBPrintOptions;
        private System.Windows.Forms.Timer timerUpdateStatistics;
        private Button buttonWizard;
        private HelpProvider MainHelp;
        private IContainer components;

        public PrintForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            //
            MLifter.Classes.Help.SetHelpNameSpace(MainHelp);
        }

        private MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintForm));
            this.GBChapters = new System.Windows.Forms.GroupBox();
            this.Chapters = new MLifter.Controls.ChapterFrame();
            this.btnCancel = new System.Windows.Forms.Button();
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.btnPreview = new System.Windows.Forms.Button();
            this.comboBoxPrintStyle = new System.Windows.Forms.ComboBox();
            this.comboBoxCardOrder = new System.Windows.Forms.ComboBox();
            this.lblPrintStyle = new System.Windows.Forms.Label();
            this.listViewBoxesSelection = new System.Windows.Forms.ListView();
            this.checkBoxAllBoxes = new System.Windows.Forms.CheckBox();
            this.lblCardOrder = new System.Windows.Forms.Label();
            this.GBBoxes = new System.Windows.Forms.GroupBox();
            this.cbReverseOrder = new System.Windows.Forms.CheckBox();
            this.lblCurSelVal = new System.Windows.Forms.Label();
            this.GBPrintOptions = new System.Windows.Forms.GroupBox();
            this.timerUpdateStatistics = new System.Windows.Forms.Timer(this.components);
            this.buttonWizard = new System.Windows.Forms.Button();
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.GBChapters.SuspendLayout();
            this.GBBoxes.SuspendLayout();
            this.GBPrintOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // GBChapters
            // 
            this.GBChapters.Controls.Add(this.Chapters);
            resources.ApplyResources(this.GBChapters, "GBChapters");
            this.GBChapters.Name = "GBChapters";
            this.GBChapters.TabStop = false;
            // 
            // Chapters
            // 
            resources.ApplyResources(this.Chapters, "Chapters");
            this.Chapters.Name = "Chapters";
            this.Chapters.OnUpdate += new MLifter.Controls.ChapterFrame.FieldEventHandler(this.Chapters_OnUpdate);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            // 
            // Browser
            // 
            resources.ApplyResources(this.Browser, "Browser");
            this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.Browser.Name = "Browser";
            this.Browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Browser_DocumentCompleted);
            // 
            // btnPreview
            // 
            resources.ApplyResources(this.btnPreview, "btnPreview");
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // comboBoxPrintStyle
            // 
            this.comboBoxPrintStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPrintStyle.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxPrintStyle, "comboBoxPrintStyle");
            this.comboBoxPrintStyle.Name = "comboBoxPrintStyle";
            // 
            // comboBoxCardOrder
            // 
            this.comboBoxCardOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCardOrder.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxCardOrder, "comboBoxCardOrder");
            this.comboBoxCardOrder.Name = "comboBoxCardOrder";
            this.comboBoxCardOrder.SelectedIndexChanged += new System.EventHandler(this.comboBoxCardOrder_SelectedIndexChanged);
            // 
            // lblPrintStyle
            // 
            resources.ApplyResources(this.lblPrintStyle, "lblPrintStyle");
            this.lblPrintStyle.Name = "lblPrintStyle";
            // 
            // listViewBoxesSelection
            // 
            this.listViewBoxesSelection.BackColor = System.Drawing.SystemColors.Control;
            this.listViewBoxesSelection.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewBoxesSelection.CheckBoxes = true;
            resources.ApplyResources(this.listViewBoxesSelection, "listViewBoxesSelection");
            this.listViewBoxesSelection.GridLines = true;
            this.listViewBoxesSelection.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewBoxesSelection.MultiSelect = false;
            this.listViewBoxesSelection.Name = "listViewBoxesSelection";
            this.listViewBoxesSelection.Scrollable = false;
            this.listViewBoxesSelection.ShowItemToolTips = true;
            this.listViewBoxesSelection.UseCompatibleStateImageBehavior = false;
            this.listViewBoxesSelection.View = System.Windows.Forms.View.List;
            this.listViewBoxesSelection.ItemActivate += new System.EventHandler(this.listViewBoxesSelection_ItemActivate);
            this.listViewBoxesSelection.EnabledChanged += new System.EventHandler(this.listViewBoxesSelection_EnabledChanged);
            this.listViewBoxesSelection.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewBoxesSelection_ItemChecked);
            // 
            // checkBoxAllBoxes
            // 
            resources.ApplyResources(this.checkBoxAllBoxes, "checkBoxAllBoxes");
            this.checkBoxAllBoxes.Checked = true;
            this.checkBoxAllBoxes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAllBoxes.Name = "checkBoxAllBoxes";
            this.checkBoxAllBoxes.UseVisualStyleBackColor = true;
            this.checkBoxAllBoxes.CheckedChanged += new System.EventHandler(this.checkBoxAllBoxes_CheckedChanged);
            // 
            // lblCardOrder
            // 
            resources.ApplyResources(this.lblCardOrder, "lblCardOrder");
            this.lblCardOrder.Name = "lblCardOrder";
            // 
            // GBBoxes
            // 
            this.GBBoxes.Controls.Add(this.listViewBoxesSelection);
            resources.ApplyResources(this.GBBoxes, "GBBoxes");
            this.GBBoxes.Name = "GBBoxes";
            this.GBBoxes.TabStop = false;
            // 
            // cbReverseOrder
            // 
            resources.ApplyResources(this.cbReverseOrder, "cbReverseOrder");
            this.cbReverseOrder.Name = "cbReverseOrder";
            this.cbReverseOrder.UseVisualStyleBackColor = true;
            // 
            // lblCurSelVal
            // 
            resources.ApplyResources(this.lblCurSelVal, "lblCurSelVal");
            this.lblCurSelVal.Name = "lblCurSelVal";
            // 
            // GBPrintOptions
            // 
            this.GBPrintOptions.Controls.Add(this.lblCardOrder);
            this.GBPrintOptions.Controls.Add(this.comboBoxPrintStyle);
            this.GBPrintOptions.Controls.Add(this.lblPrintStyle);
            this.GBPrintOptions.Controls.Add(this.cbReverseOrder);
            this.GBPrintOptions.Controls.Add(this.comboBoxCardOrder);
            resources.ApplyResources(this.GBPrintOptions, "GBPrintOptions");
            this.GBPrintOptions.Name = "GBPrintOptions";
            this.GBPrintOptions.TabStop = false;
            // 
            // timerUpdateStatistics
            // 
            this.timerUpdateStatistics.Interval = 250;
            this.timerUpdateStatistics.Tick += new System.EventHandler(this.timerUpdateStatistics_Tick);
            // 
            // buttonWizard
            // 
            resources.ApplyResources(this.buttonWizard, "buttonWizard");
            this.buttonWizard.Name = "buttonWizard";
            this.buttonWizard.UseVisualStyleBackColor = true;
            this.buttonWizard.Click += new System.EventHandler(this.buttonWizard_Click);
            // 
            // PrintForm
            // 
            this.AcceptButton = this.btnPreview;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.checkBoxAllBoxes);
            this.Controls.Add(this.buttonWizard);
            this.Controls.Add(this.GBPrintOptions);
            this.Controls.Add(this.lblCurSelVal);
            this.Controls.Add(this.GBBoxes);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.GBChapters);
            this.Controls.Add(this.Browser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.TPrintForm_Load);
            this.GBChapters.ResumeLayout(false);
            this.GBBoxes.ResumeLayout(false);
            this.GBPrintOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        public readonly string[] QueryOrders = new string[5]
        {
            Properties.Resources.PRINT_QUERYORDER_Question,
            Properties.Resources.PRINT_QUERYORDER_Answer,
            Properties.Resources.PRINT_QUERYORDER_Box,
            Properties.Resources.PRINT_QUERYORDER_Chapter,
            Properties.Resources.PRINT_QUERYORDER_None
        };
        public readonly DAL.Interfaces.QueryOrder[] QueryOrdersEnum = new DAL.Interfaces.QueryOrder[5]
        {
            QueryOrder.Question,
            QueryOrder.Answer,
            QueryOrder.Box,
            QueryOrder.Chapter,
            QueryOrder.None
        };

        /// <summary>
        /// Prepares the chapter selector on startup.
        /// </summary>
        /// <param name="sender">Event Sender, unused.</param>
        /// <param name="e">Event Arguments, unused.</param>
        /// <remarks>Documented by Dev01, 2007-07-19</remarks>
        /// <remarks>Documented by Dev02, 2007-11-22</remarks>
        private void TPrintForm_Load(object sender, System.EventArgs e)
        {
            Browser.Visible = false;

            //add chapters
            Chapters.Prepare(Dictionary);

            //add boxes
            for (int index = 0; index < Dictionary.Boxes.Count; index++)
            {
                ListViewItem box = new ListViewItem();
                box.Text = (index == 0 ? Properties.Resources.PRINT_BOXNO_0 : string.Format(Properties.Resources.PRINT_BOXNO, index));
                box.ToolTipText = string.Format(Properties.Resources.PRINT_BOX_TOOLTIP, Dictionary.Boxes[index].Size);
                box.Checked = false;
                listViewBoxesSelection.Items.Add(box);
            }

            //add queryorders
            foreach (string queryOrder in QueryOrders)
            {
                comboBoxCardOrder.Items.Add(queryOrder);
            }

            //add printstyles
            comboBoxPrintStyle.Items.Clear();
            foreach (StyleInfo styleinfo in MainForm.styleHandler.CurrentStyle.PrintStyles)
                comboBoxPrintStyle.Items.Add(styleinfo);

            //select default values
            comboBoxPrintStyle.SelectedIndex = 0;
            comboBoxCardOrder.SelectedIndex = 0;
            checkBoxAllBoxes_CheckedChanged(this, new EventArgs());
            listViewBoxesSelection_EnabledChanged(this, new EventArgs());
        }

        /// <summary>
        /// Checks the chapter and multimedia (stylesheet) selection, generates the HTML page by arrayList function call and loads the page.
        /// </summary>
        /// <param name="sender">Event Sender, unused.</param>
        /// <param name="e">Event Arguments, unused.</param>
        /// <remarks>Documented by Dev01, 2007-07-19</remarks>
        /// <remarks>Documented by Dev02, 2007-11-23</remarks>
        private void btnPreview_Click(object sender, EventArgs e)
        {
            // Only start the query when at least one chapter and box has been selected 
            if (Chapters.GetItemCount() < 1 || (listViewBoxesSelection.CheckedItems.Count < 1 && !checkBoxAllBoxes.Checked))
            {
                MessageBox.Show(Resources.PRINT_SELECT_TEXT, Resources.PRINT_SELECT_CAPTION,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.Enabled = false;

                statusDialog = new LoadStatusMessage(Properties.Resources.PRINT_STATUS_FETCHINGCARDS, 100, false);
                statusDialog.Show();
                statusDialog.SetProgress(0);

                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);

                BackgroundWorker prepareworker = new BackgroundWorker();
                prepareworker.WorkerReportsProgress = true;
                prepareworker.ProgressChanged += new ProgressChangedEventHandler(prepareworker_ProgressChanged);

                System.Collections.Generic.List<int> boxes;
                QueryOrder cardorder;
                QueryOrderDir cardorderdir;
                QueryCardState cardstate;
                GetGUISelection(out boxes, out cardorder, out cardorderdir, out cardstate);

                List<QueryStruct> querystructs = GetQueryStructs(Chapters.SelChapters, boxes, cardstate);

                string stylesheet = ((StyleInfo)comboBoxPrintStyle.SelectedItem).Path;

                List<ICard> cards = Dictionary.GetPrintOutCards(querystructs, cardorder, cardorderdir);

                string htmlContent = Dictionary.GeneratePrintOut(
                    cards,
                    stylesheet,
                    worker,
                    prepareworker);

                statusDialog.InfoMessage = Resources.PRINT_STATUS_RENDERING;
                statusDialog.EnableProgressbar = false;

                Browser.DocumentText = htmlContent;
            }
        }

        /// <summary>
        /// Gets the query structs.
        /// </summary>
        /// <param name="boxes">The boxes.</param>
        /// <param name="cardstate">The cardstate.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-01-03</remarks>
        private List<MLifter.DAL.Interfaces.QueryStruct> GetQueryStructs(List<int> chapters, List<int> boxes, QueryCardState cardstate)
        {
            if (chapters.Count < 1 || boxes.Count < 1)
                return null;

            //get cards with desired selection and ordering
            List<QueryStruct> querystructs = new List<QueryStruct>();
            foreach (int chapter in chapters)
                foreach (int box in boxes)
                    querystructs.Add(new QueryStruct(chapter, box, cardstate));

            return querystructs;
        }

        /// <summary>
        /// Gets the selection of the form controls.
        /// </summary>
        /// <param name="boxes">The boxes.</param>
        /// <param name="cardorder">The cardorder.</param>
        /// <param name="cardorderdir">The cardorderdir.</param>
        /// <param name="cardstate">The cardstate.</param>
        /// <remarks>Documented by Dev02, 2007-11-26</remarks>
        private void GetGUISelection(out List<int> boxes, out QueryOrder cardorder, out QueryOrderDir cardorderdir, out QueryCardState cardstate)
        {
            boxes = new System.Collections.Generic.List<int>();
            if (checkBoxAllBoxes.Checked)
            {
                for (int index = 0; index <= Dictionary.Boxes.Count; index++)
                    boxes.Add(index);
            }
            else
            {
                foreach (int index in listViewBoxesSelection.CheckedIndices)
                    boxes.Add(index);
            }

            cardorder = QueryOrdersEnum[comboBoxCardOrder.SelectedIndex < 0 ? 0 : comboBoxCardOrder.SelectedIndex];
            cardorderdir = cbReverseOrder.Checked ? QueryOrderDir.Descending : QueryOrderDir.Ascending;
            cardstate = QueryCardState.Active;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-11-23</remarks>
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (statusDialog != null)
            {
                if (statusDialog.InfoMessage != Properties.Resources.PRINT_STATUS_PAGES)
                {
                    statusDialog.InfoMessage = Properties.Resources.PRINT_STATUS_PAGES;
                    statusDialog.EnableProgressbar = true;
                    statusDialog.SetProgress(0);
                }
                statusDialog.SetProgress(e.ProgressPercentage);
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the prepareworker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        void prepareworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (statusDialog != null)
            {
                if (statusDialog.InfoMessage != Properties.Resources.PRINT_STATUS_MEDIA)
                {
                    statusDialog.InfoMessage = Properties.Resources.PRINT_STATUS_MEDIA;
                    statusDialog.EnableProgressbar = true;
                    statusDialog.SetProgress(0);
                }
                statusDialog.SetProgress(e.ProgressPercentage);
            }
        }

        /// <summary>
        /// Shows the print preview dialog of Internet Explorer as soon as the document has been loaded in the (invisible) browser.
        /// </summary>
        /// <param name="sender">Event Sender, unused.</param>
        /// <param name="e">Event Arguments, unused.</param>
        /// <remarks>Documented by Dev01, 2007-07-19</remarks>
        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (statusDialog != null && statusDialog.Visible)
                statusDialog.Close();

            //little hack to resize the PreviewDialog Window - without that, it would be shown in the size of this dialog
            //if there is any other way, let me know ;-) AWE
            this.Hide();
            Size size = this.Size;
            this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            Browser.ShowPrintPreviewDialog();
            Application.DoEvents();
            this.WindowState = FormWindowState.Normal;
            this.Size = size;
            this.Show();

            this.Enabled = true;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxAllBoxes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-11-22</remarks>
        private void checkBoxAllBoxes_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem box in listViewBoxesSelection.Items)
                box.Checked = checkBoxAllBoxes.Checked;
            listViewBoxesSelection.Enabled = !checkBoxAllBoxes.Checked;
            UpdateSelectionStatistics();
        }

        /// <summary>
        /// Updates the displayed selection statistics.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-11-26</remarks>
        private void UpdateSelectionStatistics()
        {
            timerUpdateStatistics.Enabled = true;
        }

        /// <summary>
        /// Handles the ItemChecked event of the listViewBoxesSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ItemCheckedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-11-23</remarks>
        private void listViewBoxesSelection_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateSelectionStatistics();
        }

        /// <summary>
        /// Handles the OnUpdate event of the ChaptersFrm control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <remarks>Documented by Dev02, 2007-11-23</remarks>
        private void Chapters_OnUpdate(object sender)
        {
            UpdateSelectionStatistics();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the rbState controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-11-26</remarks>
        private void rbState_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSelectionStatistics();
        }

        /// <summary>
        /// Handles the EnabledChanged event of the listViewBoxesSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-11-26</remarks>
        private void listViewBoxesSelection_EnabledChanged(object sender, EventArgs e)
        {
            if (listViewBoxesSelection.Enabled)
            {
                listViewBoxesSelection.ForeColor = SystemColors.WindowText;
            }
            else
            {
                listViewBoxesSelection.ForeColor = SystemColors.GrayText;
            }
        }

        /// <summary>
        /// Handles the Tick event of the timerUpdateStatistics control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-11-27</remarks>
        private void timerUpdateStatistics_Tick(object sender, EventArgs e)
        {
            timerUpdateStatistics.Enabled = false;

            int selected = 0;
            int total = 0;

            System.Collections.Generic.List<int> boxes;
            QueryOrder cardorder;
            QueryOrderDir cardorderdir;
            QueryCardState cardstate;
            GetGUISelection(out boxes, out cardorder, out cardorderdir, out cardstate);

            List<QueryStruct> querystructs = GetQueryStructs(Chapters.SelChapters, boxes, cardstate);

            List<ICard> cards = Dictionary.GetPrintOutCards(querystructs, MLifter.DAL.Interfaces.QueryOrder.None, MLifter.DAL.Interfaces.QueryOrderDir.Ascending);

            if (cards != null && cards.Count > 0)
                selected = cards.Count;
            else
                selected = 0;

            total = Dictionary.Cards.ActiveCardsCount;

            lblCurSelVal.Text = string.Format(Properties.Resources.PRINT_SELECTED, selected, total);
        }

        /// <summary>
        /// Handles the ItemActivate event of the listViewBoxesSelection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-07</remarks>
        private void listViewBoxesSelection_ItemActivate(object sender, EventArgs e)
        {
            foreach (ListViewItem box in listViewBoxesSelection.SelectedItems)
                box.Checked = true;
        }

        /// <summary>
        /// Handles the Click event of the buttonWizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-04</remarks>
        private void buttonWizard_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            this.Close();
            return;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBoxCardOrder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-24</remarks>
        private void comboBoxCardOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryOrder cardorder;
            QueryOrderDir cardorderdir;
            QueryCardState cardstate;
            List<int> boxes;
            GetGUISelection(out boxes, out cardorder, out cardorderdir, out cardstate);
            cbReverseOrder.Enabled = cardorder != QueryOrder.None;

        }
    }
}

