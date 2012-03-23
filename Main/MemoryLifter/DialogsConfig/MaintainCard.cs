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
using System.Data;
using System.Collections.Generic;
using System.Threading;
using System.IO;

using MLifter.DAL.Interfaces;
using MLifter.Components;
using MLifter.Properties;
using MLifter.BusinessLayer;
using MLifter.Controls;
using MLifter.DAL;
using MLifter.DAL.Interfaces.DB;

namespace MLifter
{
    /// <summary>
    /// The CardManager represents the menu item Cards - Maintain.
    /// It allows the user to organize the cards in the dictionary, record sounds, ... 
    /// </summary>	
    public class MaintainCardForm : System.Windows.Forms.Form
    {
        private MaintainCardDB CardDB = null;
        private int VisibleCardID = -1;

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.GroupBox groupBoxDictionaryOverview;
        private System.Windows.Forms.Label labelShowChapter;
        private System.Windows.Forms.Label labelSelectedCards;
        private System.Windows.Forms.Label labelSelectedCardsCount;
        private IContainer components;

        private System.Data.DataTable cards_table;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataSet CardsDataSet;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonNewCard;
        private ToolTip ToolTipControl;
        private ColumnRightToLeftListView listViewCards;
        private ColumnHeader colQuestion;
        private ColumnHeader colAnswer;
        private ColumnHeader colBox;
        private ColumnHeader colChapter;
        private ColumnHeader colID;
        private MLifter.Controls.CardMultiEdit cardMultiEdit;
        private System.Windows.Forms.ComboBox comboBoxShowChapter;
        private System.Windows.Forms.Timer selectionTimer = new System.Windows.Forms.Timer();
        private ContextMenuStrip contextMenuStripCards;
        private ToolStripMenuItem cardsSelectedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem moveToToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem chapterToolStripMenuItem;
        private ToolStripMenuItem boxToolStripMenuItem;
        private ToolStripMenuItem deactivateCardToolStripMenuItem;
        private System.Windows.Forms.Timer ToolTipTimer;
        private System.Windows.Forms.Timer AddCardTimer = new System.Windows.Forms.Timer();
        private HelpProvider MainHelp;
        private SearchTextbox SearchBox;

        private int selectedIndicesCountPre = 0;
        private LoadStatusMessage loadStatusMessage = new LoadStatusMessage(Properties.Resources.MAINTAIN_CARDS_LOAD_CARDS, 100, true);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="VisibleCardID">ID of card that is visible</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        public MaintainCardForm(int VisibleCard)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //[ML-925] Adapt the controls for localization 
            comboBoxShowChapter.Left = labelShowChapter.Left + labelShowChapter.Width + 5;

            selectionTimer.Interval = 50;
            selectionTimer.Tick += new EventHandler(selectionTimer_Tick);

            AddCardTimer.Interval = 100;
            AddCardTimer.Tick += new EventHandler(AddCardTimer_Tick);

            buttonDelete.Enabled = false;
            MLifter.Classes.Help.SetHelpNameSpace(MainHelp);
            this.cardMultiEdit.HelpNamespace = MLifter.Classes.Help.HelpPath;
            this.VisibleCardID = VisibleCard;
        }

        private static MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>        
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaintainCardForm));
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBoxDictionaryOverview = new System.Windows.Forms.GroupBox();
            this.SearchBox = new MLifter.Controls.SearchTextbox();
            this.listViewCards = new MLifter.Components.ColumnRightToLeftListView();
            this.colID = new System.Windows.Forms.ColumnHeader();
            this.colQuestion = new System.Windows.Forms.ColumnHeader();
            this.colAnswer = new System.Windows.Forms.ColumnHeader();
            this.colBox = new System.Windows.Forms.ColumnHeader();
            this.colChapter = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStripCards = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cardsSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chapterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.boxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deactivateCardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelShowChapter = new System.Windows.Forms.Label();
            this.comboBoxShowChapter = new System.Windows.Forms.ComboBox();
            this.labelSelectedCardsCount = new System.Windows.Forms.Label();
            this.labelSelectedCards = new System.Windows.Forms.Label();
            this.buttonNewCard = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.CardsDataSet = new System.Data.DataSet();
            this.cards_table = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.ToolTipControl = new System.Windows.Forms.ToolTip(this.components);
            this.ToolTipTimer = new System.Windows.Forms.Timer(this.components);
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.cardMultiEdit = new MLifter.Controls.CardMultiEdit();
            this.groupBoxDictionaryOverview.SuspendLayout();
            this.contextMenuStripCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CardsDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cards_table)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Name = "buttonClose";
            this.MainHelp.SetShowHelp(this.buttonClose, ((bool)(resources.GetObject("buttonClose.ShowHelp"))));
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // groupBoxDictionaryOverview
            // 
            resources.ApplyResources(this.groupBoxDictionaryOverview, "groupBoxDictionaryOverview");
            this.groupBoxDictionaryOverview.Controls.Add(this.SearchBox);
            this.groupBoxDictionaryOverview.Controls.Add(this.listViewCards);
            this.groupBoxDictionaryOverview.Controls.Add(this.labelShowChapter);
            this.groupBoxDictionaryOverview.Controls.Add(this.comboBoxShowChapter);
            this.groupBoxDictionaryOverview.Name = "groupBoxDictionaryOverview";
            this.MainHelp.SetShowHelp(this.groupBoxDictionaryOverview, ((bool)(resources.GetObject("groupBoxDictionaryOverview.ShowHelp"))));
            this.groupBoxDictionaryOverview.TabStop = false;
            // 
            // SearchBox
            // 
            resources.ApplyResources(this.SearchBox, "SearchBox");
            this.SearchBox.Name = "SearchBox";
            this.MainHelp.SetShowHelp(this.SearchBox, ((bool)(resources.GetObject("SearchBox.ShowHelp"))));
            this.SearchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // listViewCards
            // 
            this.listViewCards.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colQuestion,
            this.colAnswer,
            this.colBox,
            this.colChapter});
            this.listViewCards.ContextMenuStrip = this.contextMenuStripCards;
            this.listViewCards.FullRowSelect = true;
            this.listViewCards.GridLines = true;
            this.listViewCards.HideSelection = false;
            resources.ApplyResources(this.listViewCards, "listViewCards");
            this.listViewCards.Name = "listViewCards";
            this.listViewCards.OwnerDraw = true;
            this.MainHelp.SetShowHelp(this.listViewCards, ((bool)(resources.GetObject("listViewCards.ShowHelp"))));
            this.listViewCards.UseCompatibleStateImageBehavior = false;
            this.listViewCards.View = System.Windows.Forms.View.Details;
            this.listViewCards.SelectedIndexChanged += new System.EventHandler(this.listViewCards_SelectedIndexChanged);
            this.listViewCards.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewCards_ColumnClick);
            this.listViewCards.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listViewCards_MouseMove);
            this.listViewCards.VScrollChanged += new System.Windows.Forms.ScrollEventHandler(this.listViewCards_VScrollChanged);
            this.listViewCards.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewCards_KeyDown);
            this.listViewCards.MouseLeave += new System.EventHandler(this.listViewCards_MouseLeave);
            // 
            // colID
            // 
            resources.ApplyResources(this.colID, "colID");
            // 
            // colQuestion
            // 
            resources.ApplyResources(this.colQuestion, "colQuestion");
            // 
            // colAnswer
            // 
            resources.ApplyResources(this.colAnswer, "colAnswer");
            // 
            // colBox
            // 
            resources.ApplyResources(this.colBox, "colBox");
            // 
            // colChapter
            // 
            resources.ApplyResources(this.colChapter, "colChapter");
            // 
            // contextMenuStripCards
            // 
            this.contextMenuStripCards.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cardsSelectedToolStripMenuItem,
            this.toolStripSeparator1,
            this.moveToToolStripMenuItem,
            this.deactivateCardToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStripCards.Name = "contextMenuStripCards";
            this.MainHelp.SetShowHelp(this.contextMenuStripCards, ((bool)(resources.GetObject("contextMenuStripCards.ShowHelp"))));
            resources.ApplyResources(this.contextMenuStripCards, "contextMenuStripCards");
            this.contextMenuStripCards.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripCards_Opening);
            // 
            // cardsSelectedToolStripMenuItem
            // 
            this.cardsSelectedToolStripMenuItem.Name = "cardsSelectedToolStripMenuItem";
            resources.ApplyResources(this.cardsSelectedToolStripMenuItem, "cardsSelectedToolStripMenuItem");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // moveToToolStripMenuItem
            // 
            this.moveToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chapterToolStripMenuItem,
            this.boxToolStripMenuItem});
            this.moveToToolStripMenuItem.Image = global::MLifter.Properties.Resources.folder16;
            this.moveToToolStripMenuItem.Name = "moveToToolStripMenuItem";
            resources.ApplyResources(this.moveToToolStripMenuItem, "moveToToolStripMenuItem");
            // 
            // chapterToolStripMenuItem
            // 
            this.chapterToolStripMenuItem.Name = "chapterToolStripMenuItem";
            resources.ApplyResources(this.chapterToolStripMenuItem, "chapterToolStripMenuItem");
            // 
            // boxToolStripMenuItem
            // 
            this.boxToolStripMenuItem.Name = "boxToolStripMenuItem";
            resources.ApplyResources(this.boxToolStripMenuItem, "boxToolStripMenuItem");
            // 
            // deactivateCardToolStripMenuItem
            // 
            this.deactivateCardToolStripMenuItem.Image = global::MLifter.Properties.Resources.viewRefresh;
            this.deactivateCardToolStripMenuItem.Name = "deactivateCardToolStripMenuItem";
            resources.ApplyResources(this.deactivateCardToolStripMenuItem, "deactivateCardToolStripMenuItem");
            this.deactivateCardToolStripMenuItem.Click += new System.EventHandler(this.deactivateCardToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::MLifter.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // labelShowChapter
            // 
            resources.ApplyResources(this.labelShowChapter, "labelShowChapter");
            this.labelShowChapter.Name = "labelShowChapter";
            this.MainHelp.SetShowHelp(this.labelShowChapter, ((bool)(resources.GetObject("labelShowChapter.ShowHelp"))));
            // 
            // comboBoxShowChapter
            // 
            this.comboBoxShowChapter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxShowChapter.Items.AddRange(new object[] {
            resources.GetString("comboBoxShowChapter.Items")});
            resources.ApplyResources(this.comboBoxShowChapter, "comboBoxShowChapter");
            this.comboBoxShowChapter.Name = "comboBoxShowChapter";
            this.MainHelp.SetShowHelp(this.comboBoxShowChapter, ((bool)(resources.GetObject("comboBoxShowChapter.ShowHelp"))));
            this.comboBoxShowChapter.TabStop = false;
            this.comboBoxShowChapter.SelectedIndexChanged += new System.EventHandler(this.comboBoxShowChapter_SelectedIndexChanged);
            // 
            // labelSelectedCardsCount
            // 
            resources.ApplyResources(this.labelSelectedCardsCount, "labelSelectedCardsCount");
            this.labelSelectedCardsCount.Name = "labelSelectedCardsCount";
            this.MainHelp.SetShowHelp(this.labelSelectedCardsCount, ((bool)(resources.GetObject("labelSelectedCardsCount.ShowHelp"))));
            // 
            // labelSelectedCards
            // 
            resources.ApplyResources(this.labelSelectedCards, "labelSelectedCards");
            this.labelSelectedCards.Name = "labelSelectedCards";
            this.MainHelp.SetShowHelp(this.labelSelectedCards, ((bool)(resources.GetObject("labelSelectedCards.ShowHelp"))));
            // 
            // buttonNewCard
            // 
            this.buttonNewCard.Image = global::MLifter.Properties.Resources.newDoc;
            resources.ApplyResources(this.buttonNewCard, "buttonNewCard");
            this.buttonNewCard.Name = "buttonNewCard";
            this.MainHelp.SetShowHelp(this.buttonNewCard, ((bool)(resources.GetObject("buttonNewCard.ShowHelp"))));
            this.buttonNewCard.Click += new System.EventHandler(this.buttonNewCard_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Image = global::MLifter.Properties.Resources.delete;
            this.buttonDelete.Name = "buttonDelete";
            this.MainHelp.SetShowHelp(this.buttonDelete, ((bool)(resources.GetObject("buttonDelete.ShowHelp"))));
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // CardsDataSet
            // 
            this.CardsDataSet.DataSetName = "ML Cards";
            this.CardsDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.cards_table});
            // 
            // cards_table
            // 
            this.cards_table.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5});
            this.cards_table.TableName = "Cards";
            // 
            // dataColumn1
            // 
            this.dataColumn1.AllowDBNull = false;
            this.dataColumn1.Caption = "#";
            this.dataColumn1.ColumnName = "ID";
            this.dataColumn1.DataType = typeof(int);
            this.dataColumn1.ReadOnly = true;
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "CurrentQuestion";
            this.dataColumn2.ReadOnly = true;
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "CurrentAnswer";
            this.dataColumn3.ReadOnly = true;
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "Box";
            this.dataColumn4.ReadOnly = true;
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "Chapter";
            this.dataColumn5.DataType = typeof(int);
            this.dataColumn5.ReadOnly = true;
            // 
            // ToolTipControl
            // 
            this.ToolTipControl.AutomaticDelay = 1000;
            // 
            // ToolTipTimer
            // 
            this.ToolTipTimer.Interval = 500;
            this.ToolTipTimer.Tick += new System.EventHandler(this.ToolTipTimer_Tick);
            // 
            // cardMultiEdit
            // 
            this.cardMultiEdit.AllowDrop = true;
            resources.ApplyResources(this.cardMultiEdit, "cardMultiEdit");
            this.cardMultiEdit.Modified = false;
            this.cardMultiEdit.Multiselect = true;
            this.cardMultiEdit.Name = "cardMultiEdit";
            this.cardMultiEdit.RightToLeftAnswer = true;
            this.cardMultiEdit.RightToLeftQuestion = true;
            this.MainHelp.SetShowHelp(this.cardMultiEdit, ((bool)(resources.GetObject("cardMultiEdit.ShowHelp"))));
            this.cardMultiEdit.Edit += new System.EventHandler(this.cardMultiEdit_OnEdit);
            this.cardMultiEdit.AddStyle += new System.EventHandler(this.cardMultiEdit_AddStyle);
            this.cardMultiEdit.Add += new System.EventHandler(this.cardMultiEdit_OnAdd);
            this.cardMultiEdit.MultiEdit += new System.EventHandler(this.cardMultiEdit_OnMultiEdit);
            // 
            // MaintainCardForm
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.buttonNewCard);
            this.Controls.Add(this.cardMultiEdit);
            this.Controls.Add(this.groupBoxDictionaryOverview);
            this.Controls.Add(this.labelSelectedCardsCount);
            this.Controls.Add(this.labelSelectedCards);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonDelete);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MaintainCardForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.MaintainCardForm_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MaintainCardForm_Closing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MaintainCardForm_FormClosing);
            this.groupBoxDictionaryOverview.ResumeLayout(false);
            this.groupBoxDictionaryOverview.PerformLayout();
            this.contextMenuStripCards.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CardsDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cards_table)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Checks whether cards is pending, saves Dictionary and closes
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>        
        private void MaintainCardForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check whether card is pending 
            if (CardDB.Dictionary != null && CardDB.Dictionary.IsActive) //[ML-749] don't check modified in stick mode when the stick was pulled
                cardMultiEdit.CheckModified();

            cardMultiEdit.StopPlayingVideos();
            // Save 'n' Close 
            CardDB.Save();

            if (CardDB.Dictionary != null && CardDB.Dictionary.IsActive)
                CardDB.Dictionary.DictionaryDAL.ClearUnusedMedia();
        }

        /// <summary>
        /// Loads MaintainCard-Form
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exception cref="ex"></exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>        
        private void MaintainCardForm_Load(object sender, System.EventArgs e)
        {
            if (Dictionary.Chapters.Chapters.Count == 0)
            {
                MessageBox.Show(Resources.NO_CHAPTERS_TEXT, Resources.NO_CHAPTERS_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            //CardDB initialization
            loadStatusMessage.Show();
            CardDB = new MaintainCardDB(loadStatusMessage);
            loadStatusMessage.Hide();

            cardMultiEdit.Modified = false;
            cardMultiEdit.LoadNewCard(CardDB.Dictionary);

            //Control initializations
            this.colQuestion.Text = CardDB.Dictionary.QuestionCaption;
            this.colAnswer.Text = CardDB.Dictionary.AnswerCaption;

            ToolTipControl.SetToolTip(buttonDelete, Resources.TOOLTIP_MAINTAIN_DELETE);
            ToolTipControl.SetToolTip(buttonNewCard, Resources.TOOLTIP_MAINTAIN_ADD);

            //fix for [ML-1335]  Problems with bidirectional Text (RTL languages)
            if (cardMultiEdit.RightToLeftQuestion = Dictionary.QuestionCulture.TextInfo.IsRightToLeft)
                listViewCards.RightToLeftEnabledColumnIndexes.Add(1);

            if (cardMultiEdit.RightToLeftAnswer = Dictionary.AnswerCulture.TextInfo.IsRightToLeft)
                listViewCards.RightToLeftEnabledColumnIndexes.Add(2);

            //Add all chapters to Show and Change combo boxes
            comboBoxShowChapter.Items.Clear();
            comboBoxShowChapter.Items.Add(Resources.MAINTAIN_DICTOVERVIEW_TEXT);
            comboBoxShowChapter.Items.Add(Resources.ACTIVE_CARDS);
            comboBoxShowChapter.Items.Add(Resources.INACTIVE_CARDS);
            IChapter[] chapterArray = new IChapter[CardDB.Dictionary.Chapters.Chapters.Count];
            CardDB.Dictionary.Chapters.Chapters.CopyTo(chapterArray, 0);
            comboBoxShowChapter.Items.AddRange(chapterArray);
            comboBoxShowChapter.SelectedIndex = 0; //Changed SelectionIndex triggers the population of the listView

            chapterToolStripMenuItem.DropDownItems.Clear();
            foreach (IChapter chapter in CardDB.Dictionary.Chapters.Chapters)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = chapter.ToString();
                item.Tag = chapter;
                item.Click += new EventHandler(item_Click);
                chapterToolStripMenuItem.DropDownItems.Add(item);
            }

            boxToolStripMenuItem.DropDownItems.Clear();
            foreach (IBox box in CardDB.Dictionary.Boxes)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = box.Id != 0 ? Convert.ToString(box.Id) : Properties.Resources.MAINTAIN_POOL;
                item.Tag = box;
                item.Click += new EventHandler(item_Click);
                boxToolStripMenuItem.DropDownItems.Add(item);
            }

            //check if the visible card id is valid
            bool VisibleCardValid = true;
            try { CardDB.IndexValidateID(VisibleCardID); }
            catch (MaintainCardDB.IDNotValidException) { VisibleCardValid = false; }

            //select the supplied visible card or generate a new one
            if (VisibleCardValid)
            {
                CardDB.SelectCard(listViewCards, VisibleCardID);
            }
            else
            {
                //Load a new card - clicking on the button here would not work because the form isn't visible yet
                AddCardTimer.Enabled = true;
                AddCardTimer.Start();
            }
        }

        /// <summary>
        /// Handles the Tick event of the AddCardTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-22</remarks>
        void AddCardTimer_Tick(object sender, EventArgs e)
        {
            AddCardTimer.Stop();
            AddCardTimer.Enabled = false;

            buttonNewCard_Click(this, EventArgs.Empty);
            cardMultiEdit.BeginEdit();
        }

        /// <summary>
        /// Redraws the listView with the current values.
        /// </summary>
        /// <param name="preserveSelection">if set to <c>true</c> [preserve selection].</param>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void RefreshListView(bool preserveSelection)
        {
            Cursor.Current = Cursors.WaitCursor;
            MaintainCardFilter filter = new MaintainCardFilter();
            filter.Chapter = (comboBoxShowChapter.SelectedItem is IChapter) ? ((IChapter)comboBoxShowChapter.SelectedItem).Id : -1;
            filter.CardActive = (comboBoxShowChapter.SelectedItem is string) ?
                comboBoxShowChapter.SelectedItem as string == Resources.ACTIVE_CARDS ? true : comboBoxShowChapter.SelectedItem as string == Resources.INACTIVE_CARDS ? false : (bool?)null : (bool?)null;
            filter.SearchString = SearchBox.Text;

            CardDB.RefreshListView(listViewCards, filter, preserveSelection);
            labelSelectedCardsCount.Text = CardDB.TotalCardCount.ToString();
            listViewCards.Sort();
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the comboBoxShowChapter control.
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void comboBoxShowChapter_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            RefreshListView(false);
            cardMultiEdit.LoadNewCard(CardDB.Dictionary);
            UpdateSelectedChapter();
        }

        /// <summary>
        /// Updates the selected chapter to the card edit.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-02-07</remarks>
        private void UpdateSelectedChapter()
        {
            bool modified = cardMultiEdit.Modified;
            cardMultiEdit.SelectedChapter = (comboBoxShowChapter.SelectedItem is IChapter) ? ((IChapter)comboBoxShowChapter.SelectedItem).Id : -1;
            cardMultiEdit.Modified = modified;
        }

        /// <summary>
        /// Shows selected card, enables and disables botton delete.
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void listViewCards_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCards.SelectedIndices.Count > selectedIndicesCountPre || listViewCards.SelectedIndices.Count == 0) //[ML-640]  Maintain cards: Modified cards do not get saved - only start selectiontimer for selected cards, not for deselected
            {
                if (listViewCards.SelectedIndices.Count > 0)
                {
                    buttonDelete.Enabled = true;
                    if (!selectionTimer.Enabled && !SearchBox.Focused)
                    {
                        selectionTimer.Enabled = true;
                        selectionTimer.Start();
                    }
                }
                else
                {
                    buttonDelete.Enabled = false;
                }
            }

            selectedIndicesCountPre = listViewCards.SelectedIndices.Count;
        }

        /// <summary>
        /// Handles the KeyDown event of the listViewCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-20</remarks>
        private void listViewCards_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.KeyCode == Keys.A && e.Control)
                {
                    selectionTimer.Stop();
                    e.Handled = true;
                    CardDB.SelectAllCards(listViewCards);
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the selectionTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-08</remarks>
        void selectionTimer_Tick(object sender, EventArgs e)
        {
            selectionTimer.Stop();
            selectionTimer.Enabled = false;

            LoadSelectedCards();
        }

        /// <summary>
        /// Loads the selected cards into the CardEdit control.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-22</remarks>
        private void LoadSelectedCards()
        {
            if (listViewCards.SelectedItems.Count > 0)
            {
                if (listViewCards.SelectedItems.Count == 1)
                {
                    cardMultiEdit.LoadCardForEditing(CardDB.Dictionary, ((ICard)listViewCards.SelectedItems[0].Tag).Id);
                }
                else
                {
                    List<int> IDs = new List<int>();
                    foreach (ListViewItem item in listViewCards.SelectedItems)
                        IDs.Add(((ICard)item.Tag).Id);

                    cardMultiEdit.LoadMultipleCards(IDs.ToArray());
                }
            }
            else
            {
                cardMultiEdit.LoadNewCard(CardDB.Dictionary);
                cardMultiEdit.BeginEdit();
            }
        }

        /// <summary>
        /// When multiple cards are changed...
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <remarks>Documented by Dev05, 2007-09-18</remarks>
        private void cardMultiEdit_OnMultiEdit(object sender, EventArgs e)
        {
            foreach (int ID in cardMultiEdit.SelectedCards)
                CardDB.CardRefresh(ID);

            RefreshListView(true);
        }

        /// <summary>
        /// Handles the OnAdd event of the cardMultiEdit control.
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void cardMultiEdit_OnAdd(object sender, EventArgs e)
        {
            CardDB.CardRefresh(cardMultiEdit.CardID);
            RefreshListView(false);
            CardDB.SelectCard(listViewCards, cardMultiEdit.CardID);

            CardDB.Dictionary.PoolEmptyMessageShown = false;
        }

        /// <summary>
        /// Handles the OnEdit event of the cardMultiEdit control.
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void cardMultiEdit_OnEdit(object sender, EventArgs e)
        {
            CardDB.CardRefresh(cardMultiEdit.CardID);
            RefreshListView(true);
        }

        /// <summary>
        /// Handles the Click event of the buttonDelete control.
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void buttonDelete_Click(object sender, System.EventArgs e)
        {
            TaskDialog.ShowTaskDialogBox(Resources.MAINTAIN_DELETE_CAPTION, Resources.MAINTAIN_DELETE_TEXT, string.Format(Resources.MAINTAIN_DELETE_CONTENT, listViewCards.SelectedItems.Count),
                string.Empty, Resources.MAINTAIN_DELETE_WARNING, string.Empty, string.Empty, Resources.MAINTAIN_DELETE_TEXT_OPTION_YES + "|" + Resources.MAINTAIN_DELETE_TEXT_OPTION_NO,
                TaskDialogButtons.None, TaskDialogIcons.Question, TaskDialogIcons.Warning);
            if (TaskDialog.CommandButtonResult == 0)
            {
                cardMultiEdit.Modified = false; // [ML-1125] CardEdit.SetCardValues() still has some problems

                Cursor.Current = Cursors.WaitCursor;
                CardDB.DeleteCardItems(listViewCards.SelectedItems);

                RefreshListView(false);
                AddCardTimer.Enabled = true;
                AddCardTimer.Start();
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Shows an empty flashcard and allows modification and adds card.
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void buttonNewCard_Click(object sender, System.EventArgs e)
        {
            cardMultiEdit.LoadNewCard(CardDB.Dictionary);
            UpdateSelectedChapter();
            cardMultiEdit.BeginEdit();
        }

        /// <summary>
        /// Sorts selected column
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        private void listViewCards_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listViewCards.ListViewItemSorter = new ListViewItemComparer(e.Column);
        }

        /// <summary>
        /// Handles the VScrollChanged event of the listViewCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ScrollEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-09-17</remarks>
        private void listViewCards_VScrollChanged(object sender, ScrollEventArgs e)
        {
            listViewCards.Refresh();
        }

        /// <summary>
        /// Handles the TextChanged event of the SearchBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2007-12-17</remarks>
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            RefreshListView(false);
        }

        /// <summary>
        /// Handles the Click event of the deleteToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonDelete.PerformClick();
        }

        /// <summary>
        /// Handles the Opening event of the contextMenuStripCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        private void contextMenuStripCards_Opening(object sender, CancelEventArgs e)
        {
            int selected = listViewCards.SelectedIndices.Count;
            if (selected < 1)
                e.Cancel = true;
            else
            {
                int activated = 0;
                int deactivated = 0;
                foreach (ListViewItem lvi in listViewCards.SelectedItems)
                {
                    if ((lvi.Tag as ICard).Active)
                        activated++;
                    else
                        deactivated++;
                }
                deactivateCardToolStripMenuItem.Text = activated >= deactivated ? Resources.MAINTAIN_DEACTIVATE : Resources.MAINTAIN_ACTIVATE;
                deactivateCardToolStripMenuItem.Tag = activated >= deactivated ? false : true;

                cardsSelectedToolStripMenuItem.Text = selected == 1 ? Resources.MAINTAIN_SELECTEDCARD : string.Format(Resources.MAINTAIN_SELECTEDCARDS, selected);
            }
        }

        /// <summary>
        /// Handles the Click event of the item control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        private void item_Click(object sender, EventArgs e)
        {
            List<ICard> selectedCards = new List<ICard>();
            foreach (ListViewItem lvi in listViewCards.SelectedItems)
                selectedCards.Add(lvi.Tag as ICard);

            if (sender is ToolStripMenuItem && selectedCards.Count > 0)
            {
                if ((sender as ToolStripMenuItem).Tag is IBox)
                {
                    int boxid = ((sender as ToolStripMenuItem).Tag as IBox).Id;
                    CardDB.ChangeCardBoxes(selectedCards, boxid);
                }
                else if ((sender as ToolStripMenuItem).Tag is IChapter)
                {
                    IChapter selectedchapter = ((sender as ToolStripMenuItem).Tag as IChapter);
                    CardDB.ChangeCardChapters(selectedCards, selectedchapter.Id);
                }
                RefreshListView(true);
            }
        }

        /// <summary>
        /// Handles the Click event of the deactivateCardToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        private void deactivateCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ICard> selectedCards = new List<ICard>();
            foreach (ListViewItem lvi in listViewCards.SelectedItems)
                selectedCards.Add(lvi.Tag as ICard);

            if (sender is ToolStripMenuItem && (sender as ToolStripMenuItem).Tag is bool && selectedCards.Count > 0)
            {
                CardDB.ChangeCardActive(selectedCards, (bool)(sender as ToolStripMenuItem).Tag);
                RefreshListView(true);
            }
        }

        /// <summary>
        /// Handles the Tick event of the ToolTipTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-28</remarks>
        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            //this event is needed to display a tooltip over the listviewsubitems, not just over the listviewitems
            if (ToolTipTimer.Tag is ListViewItem)
            {
                ListViewItem item = ToolTipTimer.Tag as ListViewItem;
                Point mouseposition = new Point(MousePosition.X, MousePosition.Y);
                if (listViewCards.ClientRectangle.Contains(listViewCards.PointToClient(mouseposition)))
                    ToolTipControl.Show(item.ToolTipText, this, this.PointToClient(mouseposition), 5000);
                ToolTipTimer.Stop();
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the listViewCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-28</remarks>
        private void listViewCards_MouseMove(object sender, MouseEventArgs e)
        {
            //this event is needed to display a tooltip over the listviewsubitems, not just over the listviewitems
            ListView listview = sender as ListView;
            ListViewItem lvi = listview.GetItemAt(e.X, e.Y);
            if (lvi != null && !string.IsNullOrEmpty(lvi.ToolTipText))
            {
                if (ToolTipControl.Tag == null || (ToolTipControl.Tag is ListViewItem && (ToolTipControl.Tag as ListViewItem) != lvi))
                {
                    ToolTipControl.Tag = lvi;
                    ToolTipTimer.Tag = lvi;
                    ToolTipControl.Hide(this);
                    ToolTipTimer.Stop();
                    ToolTipTimer.Start();
                }
            }
            else
            {
                ToolTipControl.Hide(this);
                ToolTipControl.Tag = null;
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the listViewCards control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-28</remarks>
        private void listViewCards_MouseLeave(object sender, EventArgs e)
        {
            ToolTipControl.Hide(this);
        }

        /// <summary>
        /// Handles the AddStyle event of the cardMultiEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-03-25</remarks>
        private void cardMultiEdit_AddStyle(object sender, EventArgs e)
        {
            Dictionary.UseDictionaryStyleSheets = true;
        }

        /// <summary>
        /// Handles the FormClosing event of the MaintainCardForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-03-25</remarks>
        private void MaintainCardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Handles the Click event of the buttonClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-03-25</remarks>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    /// <summary>
    /// Class for centralizing dictionary access from the Maintain Cards form.
    /// </summary>
    /// <remarks>Documented by Dev02, 2007-12-17</remarks>
    class MaintainCardDB
    {
        private Dictionary dictionary = MainForm.LearnLogic.Dictionary;
        private Dictionary<int, IChapter> chapters = new Dictionary<int, IChapter>();

        //the index consists of the following lists
        private Dictionary<int, ListViewItem> cards = new Dictionary<int, ListViewItem>();
        private List<string> indexStrings = new List<string>();
        private List<int> indexIDs = new List<int>();
        private List<int> indexChapters = new List<int>();

        #region Constructor
        /// <summary>
        /// Constructor for the Class for centralizing dictionary access in the Maintain Cards form.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-12-17</remarks>
        public MaintainCardDB(LoadStatusMessage statusMessage)
        {
            Dictionary.PreloadCardCache();
            //populate chapters dictionary
            foreach (IChapter chapter in Dictionary.Chapters.Chapters)
                chapters.Add(chapter.Id, chapter);

            //populate cards dictionary
            int counter = 0;
            int count = Dictionary.Cards.Cards.Count;
            foreach (ICard card in Dictionary.Cards.Cards)
            {
                if (counter++ % 10 == 0)
                    statusMessage.SetProgress(Convert.ToInt32((counter * 1.0 / count) * 100));
                IndexAdd(card);
            }
            statusMessage.SetProgress(100);
        }
        #endregion

        #region Index Operations
        /// <summary>
        /// Adds a card to the index.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>The new index position.</returns>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        private int IndexAdd(ICard card)
        {
            return IndexAdd(card, -1);
        }

        /// <summary>
        /// Indexes the add.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="indexPosition">The index position (negative for adding at the end).</param>
        /// <returns>The new index position.</returns>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        private int IndexAdd(ICard card, int indexPosition)
        {
            if (indexPosition < 0 || indexPosition > indexStrings.Count)
            {
                indexStrings.Add(card.ToString());
                indexIDs.Add(card.Id);
                indexChapters.Add(card.Chapter);
                cards.Add(card.Id, GenerateListViewItem(card));
            }
            else
            {
                indexStrings.Insert(indexPosition, card.ToString());
                indexIDs.Insert(indexPosition, card.Id);
                indexChapters.Insert(indexPosition, card.Chapter);
                cards.Add(card.Id, GenerateListViewItem(card));
            }

            return indexIDs.IndexOf(card.Id);
        }

        /// <summary>
        /// Removed a card from the index.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns>The position of the card in the index. (For later adding at the same position).</returns>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        private int IndexRemove(int ID)
        {
            IndexValidateID(ID);

            int i = indexIDs.IndexOf(ID);
            if (i >= 0)
            {
                indexStrings.RemoveAt(i);
                indexIDs.RemoveAt(i);
                indexChapters.RemoveAt(i);
                cards.Remove(ID);
            }
            return i;
        }

        /// <summary>
        /// Removed a card from the index.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>The position of the card in the index. (For later adding at the same position).</returns>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        private int IndexRemove(ICard card)
        {
            return IndexRemove(card.Id);
        }

        /// <summary>
        /// Validates that the supplied ID is in the index properly.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <remarks>Documented by Dev02, 2007-12-19</remarks>
        /// <exception cref="IndexCountMissmatchException">Can occur when there is an internal missmatch in the index arrays.</exception>
        /// <exception cref="IDNotValidException">Occurs when the supplied ID is not in the index properly.</exception>
        public void IndexValidateID(int ID)
        {
            if (!((cards.Count == indexStrings.Count) && (cards.Count == indexIDs.Count) && (cards.Count == indexChapters.Count)))
            {
                throw new IndexCountMissmatchException();
            }

            bool valid = false;
            if (indexIDs.Contains(ID) && cards.ContainsKey(ID))
            {
                int index = indexIDs.IndexOf(ID);
                if (index < indexStrings.Count && index < indexChapters.Count && cards[ID].Tag != null)
                    valid = true;
            }

            if (!valid)
                throw new IDNotValidException();

            return;
        }


        /// <summary>
        /// Gets thrown when a supplied ID was not valid.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-12-19</remarks>
        [global::System.Serializable]
        public class IDNotValidException : Exception
        {
            public IDNotValidException() { }
            public IDNotValidException(string message) : base(message) { }
            public IDNotValidException(string message, Exception inner) : base(message, inner) { }
            protected IDNotValidException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }


        /// <summary>
        /// Gets thrown when the counter values of the index variables are not matching.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-12-19</remarks>
        [global::System.Serializable]
        public class IndexCountMissmatchException : Exception
        {
            public IndexCountMissmatchException() { }
            public IndexCountMissmatchException(string message) : base(message) { }
            public IndexCountMissmatchException(string message, Exception inner) : base(message, inner) { }
            protected IndexCountMissmatchException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        #endregion

        #region Card Operations
        /// <summary>
        /// Deletes a card.
        /// </summary>
        /// <param name="lvi">The lvi.</param>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        internal void CardDelete(ListViewItem lvi)
        {
            ICard card = lvi.Tag as ICard;
            CardDelete(card.Id);
        }

        /// <summary>
        /// Deletes a card.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        internal void CardDelete(int ID)
        {
            IndexValidateID(ID);

            dictionary.Cards.DeleteCardByID(ID);
            IndexRemove(ID);
        }

        /// <summary>
        /// Deletes the card items.
        /// </summary>
        /// <param name="cardItems">The card items.</param>
        /// <remarks>Documented by Dev03, 2008-01-04</remarks>
        internal void DeleteCardItems(ListView.SelectedListViewItemCollection cardItems)
        {
            List<int> cardIds = new List<int>();
            foreach (ListViewItem cardItem in cardItems)
            {
                int id = ((ICard)cardItem.Tag).Id;
                if (id < 0)
                    throw new ArgumentOutOfRangeException();
                IndexValidateID(id);
                IndexRemove(id);
                cardIds.Add(id);
            }
            dictionary.Cards.DeleteCards(cardIds);
        }

        /// <summary>
        /// Refreshes a card from the dictionary.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        public void CardRefresh(int ID)
        {
            int indexPosition = -1;

            if (cards.ContainsKey(ID))
            {
                try
                {
                    indexPosition = IndexRemove(ID);
                }
                catch (IDNotValidException) { }
            }

            IndexAdd(dictionary.Cards.GetCardByID(ID).BaseCard, indexPosition);
        }

        /// <summary>
        /// Changes the card boxes.
        /// </summary>
        /// <param name="cards">The cards.</param>
        /// <param name="newBox">The new box.</param>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        internal void ChangeCardBoxes(List<ICard> cards, int newBox)
        {
            foreach (ICard card in cards)
            {
                if (card.Box != newBox && newBox >= -1 && newBox < dictionary.Boxes.Count)
                {
                    LearnLogStruct lls = new LearnLogStruct();
                    lls.CardsID = card.Id;
                    lls.SessionID = Log.LastSessionID;
                    lls.MoveType = MoveType.Manual;
                    lls.OldBox = dictionary.Cards.GetCardByID(card.Id).BaseCard.Box;
                    lls.NewBox = newBox;
                    //Dummy values:
                    lls.TimeStamp = DateTime.Now;
                    lls.Duration = 0;
                    lls.CaseSensitive = false;
                    lls.CorrectOnTheFly = false;
                    lls.Direction = EQueryDirection.Question2Answer;
                    lls.LearnMode = EQueryType.Word;
                    Log.CreateLearnLogEntry(lls, dictionary.DictionaryDAL.Parent);

                    dictionary.Cards.GetCardByID(card.Id).BaseCard.Box = newBox;
                    CardRefresh(card.Id);
                }
            }
        }

        /// <summary>
        /// Changes the activate cards.
        /// </summary>
        /// <param name="cards">The cards.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        internal void ChangeCardActive(List<ICard> cards, bool active)
        {
            foreach (ICard card in cards)
            {
                if (card.Active != active)
                {
                    dictionary.Cards.GetCardByID(card.Id).BaseCard.Active = active;
                    CardRefresh(card.Id);
                }
            }
        }

        /// <summary>
        /// Changes the card chapters.
        /// </summary>
        /// <param name="cards">The cards.</param>
        /// <param name="newChapter">The new chapter.</param>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        internal void ChangeCardChapters(List<ICard> cards, int newChapter)
        {
            foreach (ICard card in cards)
            {
                if (card.Chapter != newChapter)
                {
                    dictionary.Cards.GetCardByID(card.Id).BaseCard.Chapter = newChapter;
                    CardRefresh(card.Id);
                }
            }
        }

        /// <summary>
        /// Gets the total amount of cards in the index/dictionary.
        /// </summary>
        /// <value>The total amount of cards.</value>
        /// <remarks>Documented by Dev02, 2007-12-19</remarks>
        public int TotalCardCount
        {
            get
            {
                return indexIDs.Count;
            }
        }

        #endregion

        #region ListView Operations
        /// <summary>
        /// Fills or refreshes the supplied listview with the current cards and based on the supplied filter.
        /// </summary>
        /// <param name="listView">The list view.</param>
        /// <param name="filter">The card filter.</param>
        /// <param name="preserveSelection">if set to <c>true</c> [preserve selection].</param>
        /// <remarks>Documented by Dev02, 2007-12-17</remarks>
        public void RefreshListView(ListView listView, MaintainCardFilter filter, bool preserveSelection)
        {
            List<int> selectedIDs = null;
            int topIndex = -1;

            listView.BeginUpdate();
            //save selection
            if (preserveSelection)
            {
                selectedIDs = new List<int>();
                foreach (ListViewItem lvi in listView.SelectedItems)
                    selectedIDs.Add((lvi.Tag as ICard).Id);
                //preserve scroll position
                if (listView.TopItem != null)
                    topIndex = listView.TopItem.Index;
            }
            else
            {
                //[ML-770] Clear selection when it should not be preserved
                listView.SelectedIndices.Clear();
            }

            //add the items
            listView.Items.Clear();
            listView.Items.AddRange(GetCards(filter).ToArray());

            //restore selection
            if (selectedIDs != null && selectedIDs.Count > 0)
            {
                if (selectedIDs.Count < indexIDs.Count / 4)
                {
                    //faster for a low amount of selected items
                    foreach (int ID in selectedIDs)
                    {
                        ListViewItem lvi = cards[ID];
                        if (listView.Items.Contains(lvi))
                        {
                            listView.Items[listView.Items.IndexOf(lvi)].Selected = true;
                        }
                    }
                }
                else
                {
                    //faster for a high amount of selected items
                    foreach (ListViewItem lvi in listView.Items)
                    {
                        if (selectedIDs.Contains((lvi.Tag as ICard).Id))
                            lvi.Selected = true;
                    }
                }
            }

            //ensure the first selected item to be visible
            if (preserveSelection)
            {
                if (topIndex > 0 && topIndex < listView.Items.Count)
                    listView.TopItem = listView.Items[topIndex];
                else if (listView.SelectedItems.Count > 0)
                    listView.SelectedItems[0].EnsureVisible();
            }
            listView.EndUpdate();
        }

        /// <summary>
        /// Sets a desired card in the listView to selected and ensures it is visible.
        /// </summary>
        /// <param name="listView">The list view.</param>
        /// <param name="cardID">The card ID.</param>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        public void SelectCard(ListView listView, int cardID)
        {
            try
            {
                IndexValidateID(cardID);
            }
            catch (IDNotValidException) { return; }

            listView.SelectedItems.Clear();
            ListViewItem card = cards[cardID];

            if (listView.Items.Contains(card))
            {
                listView.Items[listView.Items.IndexOf(card)].Selected = true;
                listView.Items[listView.Items.IndexOf(card)].EnsureVisible();
            }
        }

        /// <summary>
        /// Selects all cards in the list view.
        /// </summary>
        /// <param name="listView">The list view.</param>
        /// <remarks>Documented by Dev02, 2007-12-20</remarks>
        public void SelectAllCards(ListView listView)
        {
            listView.BeginUpdate();
            listView.SelectedIndices.Clear();

            foreach (ListViewItem lvi in listView.Items)
                lvi.Selected = true;

            listView.EndUpdate();
        }

        /// <summary>
        /// Deselects all cards in the list view.
        /// </summary>
        /// <param name="listView">The list view.</param>
        /// <remarks>Documented by Dev02, 2008-01-16</remarks>
        public void DeselectAllCards(ListView listView)
        {
            listView.SelectedIndices.Clear();
        }

        /// <summary>
        /// Gets the cards matching the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>List of matching cards.</returns>
        /// <remarks>Documented by Dev02, 2007-12-17</remarks>
        public List<ListViewItem> GetCards(MaintainCardFilter filter)
        {
            List<ListViewItem> getCards = new List<ListViewItem>();
            List<int> foundIndezes = new List<int>();

            if (filter.SearchString.Length > 0)
            {
                if (filter.CaseSensitive)
                {
                    for (int i = 0; i < indexStrings.Count; i++)
                    {
                        if (indexStrings[i].Contains(filter.SearchString))
                            foundIndezes.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < indexStrings.Count; i++)
                    {
                        if (indexStrings[i].ToLower().Contains(filter.SearchString.ToLower()))
                            foundIndezes.Add(i);
                    }
                }

                foreach (int index in foundIndezes)
                {
                    if ((filter.Chapter == -1 || filter.Chapter == indexChapters[index]) && (!filter.CardActive.HasValue || (cards[indexIDs[index]].Tag as ICard).Active == filter.CardActive.Value))
                        getCards.Add(cards[indexIDs[index]]);
                }
            }
            else
            {
                for (int index = 0; index < indexStrings.Count; index++)
                {
                    if ((filter.Chapter == -1 || filter.Chapter == indexChapters[index]) && (!filter.CardActive.HasValue || (cards[indexIDs[index]].Tag as ICard).Active == filter.CardActive.Value))
                        getCards.Add(cards[indexIDs[index]]);
                }
            }

            return getCards;
        }

        /// <summary>
        /// Generates the list view item from an ICard.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        private ListViewItem GenerateListViewItem(ICard card)
        {
            ListViewItem lvi;
            lvi = new ListViewItem();
            lvi.Text = card.ToString(); //while not visible, this Text is important for the listView.TopItem to work properly!
            lvi.SubItems.Add(card.Question.Words.Count > 0 ? card.Question.ToString() : Resources.MAINTAIN_NA);
            lvi.SubItems.Add(card.Answer.Words.Count > 0 ? card.Answer.ToString() : Resources.MAINTAIN_NA);
            lvi.SubItems.Add(!card.Active ? Resources.MAINTAIN_NONE : (card.Box == 0 ? Resources.MAINTAIN_POOL : card.Box.ToString()));
            lvi.SubItems.Add(chapters.ContainsKey(card.Chapter) ? chapters[card.Chapter].ToString() : Resources.MAINTAIN_NA);
            lvi.Tag = card;
            //the datetime from the settings is only exact to the second
            if (((TimeSpan)card.Timestamp.Subtract(Properties.Settings.Default.LastImportTimestamp)).Duration() < new TimeSpan(0, 0, 1))
            {
                lvi.BackColor = Properties.Settings.Default.LastImportColor;
                lvi.ToolTipText = Properties.Resources.MAINTAIN_TOOLTIP_IMPORTED;
            }
            return lvi;
        }
        #endregion

        /// <summary>
        /// Saves the dictionary.
        /// </summary>
        /// <remarks>Documented by Dev02, 2007-12-17</remarks>
        public void Save()
        {
            if (Dictionary != null && Dictionary.IsActive)
                Dictionary.Save();
        }

        /// <summary>
        /// Gets the used dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev02, 2007-12-18</remarks>
        public Dictionary Dictionary
        {
            get
            {
                return dictionary;
            }
        }
    }

    /// <summary>
    /// Defines a filter for the list population of the maintain cards.
    /// </summary>
    /// <remarks>Documented by Dev02, 2007-12-17</remarks>
    class MaintainCardFilter
    {
        public string SearchString = string.Empty;
        public bool CaseSensitive = false;
        public bool? CardActive = null;
        public int Chapter = -1; //all chapters

        public MaintainCardFilter() { }
    }

    /// <summary>
    /// Implements the manual sorting of items by column
    /// </summary>    
    class ListViewItemComparer : IComparer
    {
        private int col;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        public ListViewItemComparer()
        {
            col = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="column">Number of column</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>

        public ListViewItemComparer(int column)
        {
            col = column;
        }

        /// <summary>
        /// Compares two different objects and returns an indication of their relative sort order
        /// </summary>
        /// <param name="x">Object x that should be compared</param>
        /// <param name="y">Object y that should be compared</param>
        /// <returns>Indication of relative sort order</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>

        public int Compare(object x, object y)
        {
            return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
        }
    }
}
