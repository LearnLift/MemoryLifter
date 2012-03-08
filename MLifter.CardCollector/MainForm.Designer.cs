namespace MLifter.CardCollector
{
    partial class CollectorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if ((dictionary != null) && finallyClose) dictionary.Dispose();
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectorForm));
            this.buttonStart = new System.Windows.Forms.Button();
            this.timerWatch = new System.Windows.Forms.Timer(this.components);
            this.tableCards = new XPTable.Models.Table();
            this.columnModelCards = new XPTable.Models.ColumnModel();
            this.tableModelCards = new XPTable.Models.TableModel();
            this.checkBoxBeep = new System.Windows.Forms.CheckBox();
            this.linkLabelEditColumns = new System.Windows.Forms.LinkLabel();
            this.checkBoxRemove = new System.Windows.Forms.CheckBox();
            this.cardEdit = new MLifter.Controls.CardEdit();
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            ((System.ComponentModel.ISupportInitialize)(this.tableCards)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            resources.ApplyResources(this.buttonStart, "buttonStart");
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // timerWatch
            // 
            this.timerWatch.Tick += new System.EventHandler(this.timerWatch_Tick);
            // 
            // tableCards
            // 
            this.tableCards.AlternatingRowColor = System.Drawing.Color.LemonChiffon;
            this.tableCards.ColumnModel = this.columnModelCards;
            this.tableCards.EnableHeaderContextMenu = false;
            this.tableCards.FullRowSelect = true;
            this.tableCards.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableCards.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            resources.ApplyResources(this.tableCards, "tableCards");
            this.tableCards.Name = "tableCards";
            this.tableCards.NoItemsText = "There are no items in this view. Please copy some Text to the clipboard.";
            this.tableCards.SelectionStyle = XPTable.Models.SelectionStyle.Grid;
            this.tableCards.TableModel = this.tableModelCards;
            this.tableCards.CellPropertyChanged += new XPTable.Events.CellEventHandler(this.tableCards_CellPropertyChanged);
            this.tableCards.SelectionChanged += new XPTable.Events.SelectionEventHandler(this.tableCards_SelectionChanged);
            this.tableCards.CellButtonClicked += new XPTable.Events.CellButtonEventHandler(this.tableCards_CellButtonClicked);
            // 
            // checkBoxBeep
            // 
            resources.ApplyResources(this.checkBoxBeep, "checkBoxBeep");
            this.checkBoxBeep.Checked = true;
            this.checkBoxBeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBeep.Name = "checkBoxBeep";
            this.checkBoxBeep.UseVisualStyleBackColor = true;
            // 
            // linkLabelEditColumns
            // 
            resources.ApplyResources(this.linkLabelEditColumns, "linkLabelEditColumns");
            this.linkLabelEditColumns.Name = "linkLabelEditColumns";
            this.linkLabelEditColumns.TabStop = true;
            this.linkLabelEditColumns.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEditColumns_LinkClicked);
            // 
            // checkBoxRemove
            // 
            resources.ApplyResources(this.checkBoxRemove, "checkBoxRemove");
            this.checkBoxRemove.Name = "checkBoxRemove";
            this.checkBoxRemove.UseVisualStyleBackColor = true;
            // 
            // cardEdit
            // 
            this.cardEdit.AllowDrop = true;
            resources.ApplyResources(this.cardEdit, "cardEdit");
            this.cardEdit.HelpNamespace = "";
            this.cardEdit.Modified = false;
            this.cardEdit.Multiselect = false;
            this.cardEdit.Name = "cardEdit";
            this.cardEdit.Add += new System.EventHandler(this.cardEdit_Add);
            // 
            // CollectorForm
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableCards);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.checkBoxRemove);
            this.Controls.Add(this.checkBoxBeep);
            this.Controls.Add(this.cardEdit);
            this.Controls.Add(this.linkLabelEditColumns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.Name = "CollectorForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.tableCards)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Timer timerWatch;
        private XPTable.Models.Table tableCards;
        private XPTable.Models.ColumnModel columnModelCards;
        private XPTable.Models.TableModel tableModelCards;
        private MLifter.Controls.CardEdit cardEdit;
        private System.Windows.Forms.CheckBox checkBoxBeep;
        private System.Windows.Forms.LinkLabel linkLabelEditColumns;
        private System.Windows.Forms.CheckBox checkBoxRemove;
        private System.Windows.Forms.HelpProvider MainHelp;
    }
}

