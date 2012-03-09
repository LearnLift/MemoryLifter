namespace MLifter.AudioTools
{
    partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.labelWordSentence = new System.Windows.Forms.Label();
			this.labelCard = new System.Windows.Forms.Label();
			this.groupBoxFontSize = new System.Windows.Forms.GroupBox();
			this.radioButtonFontSizeAutomatic = new System.Windows.Forms.RadioButton();
			this.radioButtonFontSizeLarge = new System.Windows.Forms.RadioButton();
			this.radioButtonFontSizeMedium = new System.Windows.Forms.RadioButton();
			this.radioButtonFontSizeSmall = new System.Windows.Forms.RadioButton();
			this.progressBarStatus = new System.Windows.Forms.ProgressBar();
			this.buttonAllCards = new System.Windows.Forms.Button();
			this.buttonNoCards = new System.Windows.Forms.Button();
			this.buttonSelectInverse = new System.Windows.Forms.Button();
			this.buttonSelectIncomplete = new System.Windows.Forms.Button();
			this.treeViewCards = new System.Windows.Forms.TreeView();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemHowTo = new System.Windows.Forms.ToolStripMenuItem();
			this.codecInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStripMain = new System.Windows.Forms.MenuStrip();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelAction = new System.Windows.Forms.ToolStripStatusLabel();
			this.groupBoxPosition = new System.Windows.Forms.GroupBox();
			this.groupBoxSelect = new System.Windows.Forms.GroupBox();
			this.buttonAdvancedMode = new System.Windows.Forms.Button();
			this.numPadControl = new MLifter.AudioTools.NumPadControl();
			this.loadingControl = new MLifter.AudioTools.LoadingControl();
			this.groupBoxFontSize.SuspendLayout();
			this.menuStripMain.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.groupBoxPosition.SuspendLayout();
			this.groupBoxSelect.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelWordSentence
			// 
			resources.ApplyResources(this.labelWordSentence, "labelWordSentence");
			this.labelWordSentence.Name = "labelWordSentence";
			this.labelWordSentence.Paint += new System.Windows.Forms.PaintEventHandler(this.labelWordSentence_Paint);
			// 
			// labelCard
			// 
			resources.ApplyResources(this.labelCard, "labelCard");
			this.labelCard.Name = "labelCard";
			// 
			// groupBoxFontSize
			// 
			this.groupBoxFontSize.Controls.Add(this.radioButtonFontSizeAutomatic);
			this.groupBoxFontSize.Controls.Add(this.radioButtonFontSizeLarge);
			this.groupBoxFontSize.Controls.Add(this.radioButtonFontSizeMedium);
			this.groupBoxFontSize.Controls.Add(this.radioButtonFontSizeSmall);
			resources.ApplyResources(this.groupBoxFontSize, "groupBoxFontSize");
			this.groupBoxFontSize.Name = "groupBoxFontSize";
			this.groupBoxFontSize.TabStop = false;
			// 
			// radioButtonFontSizeAutomatic
			// 
			resources.ApplyResources(this.radioButtonFontSizeAutomatic, "radioButtonFontSizeAutomatic");
			this.radioButtonFontSizeAutomatic.Name = "radioButtonFontSizeAutomatic";
			this.radioButtonFontSizeAutomatic.UseVisualStyleBackColor = true;
			this.radioButtonFontSizeAutomatic.CheckedChanged += new System.EventHandler(this.radioButtonFontSize_CheckedChanged);
			this.radioButtonFontSizeAutomatic.Enter += new System.EventHandler(this.radioButtonFontSize_Enter);
			// 
			// radioButtonFontSizeLarge
			// 
			resources.ApplyResources(this.radioButtonFontSizeLarge, "radioButtonFontSizeLarge");
			this.radioButtonFontSizeLarge.Name = "radioButtonFontSizeLarge";
			this.radioButtonFontSizeLarge.UseVisualStyleBackColor = true;
			this.radioButtonFontSizeLarge.CheckedChanged += new System.EventHandler(this.radioButtonFontSize_CheckedChanged);
			this.radioButtonFontSizeLarge.Enter += new System.EventHandler(this.radioButtonFontSize_Enter);
			// 
			// radioButtonFontSizeMedium
			// 
			resources.ApplyResources(this.radioButtonFontSizeMedium, "radioButtonFontSizeMedium");
			this.radioButtonFontSizeMedium.Checked = true;
			this.radioButtonFontSizeMedium.Name = "radioButtonFontSizeMedium";
			this.radioButtonFontSizeMedium.TabStop = true;
			this.radioButtonFontSizeMedium.UseVisualStyleBackColor = true;
			this.radioButtonFontSizeMedium.CheckedChanged += new System.EventHandler(this.radioButtonFontSize_CheckedChanged);
			this.radioButtonFontSizeMedium.Enter += new System.EventHandler(this.radioButtonFontSize_Enter);
			// 
			// radioButtonFontSizeSmall
			// 
			resources.ApplyResources(this.radioButtonFontSizeSmall, "radioButtonFontSizeSmall");
			this.radioButtonFontSizeSmall.Name = "radioButtonFontSizeSmall";
			this.radioButtonFontSizeSmall.UseVisualStyleBackColor = true;
			this.radioButtonFontSizeSmall.CheckedChanged += new System.EventHandler(this.radioButtonFontSize_CheckedChanged);
			this.radioButtonFontSizeSmall.Enter += new System.EventHandler(this.radioButtonFontSize_Enter);
			// 
			// progressBarStatus
			// 
			resources.ApplyResources(this.progressBarStatus, "progressBarStatus");
			this.progressBarStatus.Name = "progressBarStatus";
			this.progressBarStatus.Value = 50;
			// 
			// buttonAllCards
			// 
			resources.ApplyResources(this.buttonAllCards, "buttonAllCards");
			this.buttonAllCards.Name = "buttonAllCards";
			this.buttonAllCards.UseVisualStyleBackColor = true;
			this.buttonAllCards.Click += new System.EventHandler(this.buttonAllCards_Click);
			this.buttonAllCards.Enter += new System.EventHandler(this.buttons_FocusEnter);
			// 
			// buttonNoCards
			// 
			resources.ApplyResources(this.buttonNoCards, "buttonNoCards");
			this.buttonNoCards.Name = "buttonNoCards";
			this.buttonNoCards.UseVisualStyleBackColor = true;
			this.buttonNoCards.Click += new System.EventHandler(this.buttonNoCards_Click);
			this.buttonNoCards.Enter += new System.EventHandler(this.buttons_FocusEnter);
			// 
			// buttonSelectInverse
			// 
			resources.ApplyResources(this.buttonSelectInverse, "buttonSelectInverse");
			this.buttonSelectInverse.Name = "buttonSelectInverse";
			this.buttonSelectInverse.UseVisualStyleBackColor = true;
			this.buttonSelectInverse.Click += new System.EventHandler(this.buttonSelectInverse_Click);
			this.buttonSelectInverse.Enter += new System.EventHandler(this.buttons_FocusEnter);
			// 
			// buttonSelectIncomplete
			// 
			resources.ApplyResources(this.buttonSelectIncomplete, "buttonSelectIncomplete");
			this.buttonSelectIncomplete.Name = "buttonSelectIncomplete";
			this.buttonSelectIncomplete.UseVisualStyleBackColor = true;
			this.buttonSelectIncomplete.Click += new System.EventHandler(this.buttonSelectIncomplete_Click);
			this.buttonSelectIncomplete.Enter += new System.EventHandler(this.buttons_FocusEnter);
			// 
			// treeViewCards
			// 
			resources.ApplyResources(this.treeViewCards, "treeViewCards");
			this.treeViewCards.CheckBoxes = true;
			this.treeViewCards.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
			this.treeViewCards.ForeColor = System.Drawing.SystemColors.WindowText;
			this.treeViewCards.FullRowSelect = true;
			this.treeViewCards.HideSelection = false;
			this.treeViewCards.Name = "treeViewCards";
			this.treeViewCards.ShowLines = false;
			this.treeViewCards.ShowPlusMinus = false;
			this.treeViewCards.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewCards_BeforeCheck);
			this.treeViewCards.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCards_AfterCheck);
			this.treeViewCards.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewCards_BeforeCollapse);
			this.treeViewCards.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewCards_BeforeExpand);
			this.treeViewCards.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeViewCards_DrawNode);
			this.treeViewCards.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewCards_BeforeSelect);
			this.treeViewCards.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCards_AfterSelect);
			this.treeViewCards.Click += new System.EventHandler(this.treeViewCards_Click);
			this.treeViewCards.DoubleClick += new System.EventHandler(this.treeViewCards_DoubleClick);
			this.treeViewCards.Enter += new System.EventHandler(this.treeViewCards_Enter);
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDictionaryToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
			// 
			// openDictionaryToolStripMenuItem
			// 
			this.openDictionaryToolStripMenuItem.Name = "openDictionaryToolStripMenuItem";
			resources.ApplyResources(this.openDictionaryToolStripMenuItem, "openDictionaryToolStripMenuItem");
			this.openDictionaryToolStripMenuItem.Click += new System.EventHandler(this.openDictionaryToolStripMenuItem_Click);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemHowTo,
            this.codecInformationToolStripMenuItem,
            this.toolStripSeparator1,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
			// 
			// toolStripMenuItemHowTo
			// 
			this.toolStripMenuItemHowTo.Name = "toolStripMenuItemHowTo";
			resources.ApplyResources(this.toolStripMenuItemHowTo, "toolStripMenuItemHowTo");
			this.toolStripMenuItemHowTo.Click += new System.EventHandler(this.toolStripMenuItemHowTo_Click);
			// 
			// codecInformationToolStripMenuItem
			// 
			this.codecInformationToolStripMenuItem.Name = "codecInformationToolStripMenuItem";
			resources.ApplyResources(this.codecInformationToolStripMenuItem, "codecInformationToolStripMenuItem");
			this.codecInformationToolStripMenuItem.Click += new System.EventHandler(this.codecInformationToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// menuStripMain
			// 
			this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			resources.ApplyResources(this.menuStripMain, "menuStripMain");
			this.menuStripMain.Name = "menuStripMain";
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelAction});
			resources.ApplyResources(this.statusStrip, "statusStrip");
			this.statusStrip.Name = "statusStrip";
			// 
			// toolStripStatusLabelAction
			// 
			this.toolStripStatusLabelAction.Name = "toolStripStatusLabelAction";
			resources.ApplyResources(this.toolStripStatusLabelAction, "toolStripStatusLabelAction");
			// 
			// groupBoxPosition
			// 
			this.groupBoxPosition.Controls.Add(this.progressBarStatus);
			this.groupBoxPosition.Controls.Add(this.labelCard);
			resources.ApplyResources(this.groupBoxPosition, "groupBoxPosition");
			this.groupBoxPosition.Name = "groupBoxPosition";
			this.groupBoxPosition.TabStop = false;
			// 
			// groupBoxSelect
			// 
			resources.ApplyResources(this.groupBoxSelect, "groupBoxSelect");
			this.groupBoxSelect.Controls.Add(this.buttonAllCards);
			this.groupBoxSelect.Controls.Add(this.buttonNoCards);
			this.groupBoxSelect.Controls.Add(this.buttonSelectIncomplete);
			this.groupBoxSelect.Controls.Add(this.buttonSelectInverse);
			this.groupBoxSelect.Controls.Add(this.treeViewCards);
			this.groupBoxSelect.Name = "groupBoxSelect";
			this.groupBoxSelect.TabStop = false;
			// 
			// buttonAdvancedMode
			// 
			resources.ApplyResources(this.buttonAdvancedMode, "buttonAdvancedMode");
			this.buttonAdvancedMode.Name = "buttonAdvancedMode";
			this.buttonAdvancedMode.UseVisualStyleBackColor = true;
			this.buttonAdvancedMode.Click += new System.EventHandler(this.buttonAdvancedMode_Click);
			this.buttonAdvancedMode.Enter += new System.EventHandler(this.buttons_FocusEnter);
			// 
			// numPadControl
			// 
			resources.ApplyResources(this.numPadControl, "numPadControl");
			this.numPadControl.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			this.numPadControl.BackColor = System.Drawing.SystemColors.Control;
			this.numPadControl.ButtonBackImage = global::MLifter.Recorder.Properties.Resources.backward_active_expertmode;
			this.numPadControl.ButtonBackImageNotAvailable = global::MLifter.Recorder.Properties.Resources.backward_inactive_expertmode;
			this.numPadControl.ButtonNextImage = global::MLifter.Recorder.Properties.Resources.forward_active_expertmode;
			this.numPadControl.ButtonNextImageNotAvailable = global::MLifter.Recorder.Properties.Resources.forward_inactive_expertmode;
			this.numPadControl.ButtonPlayImage = global::MLifter.Recorder.Properties.Resources.play_active_expertmode;
			this.numPadControl.ButtonPlayImageNotAvailable = global::MLifter.Recorder.Properties.Resources.play_inactive_expertmode;
			this.numPadControl.ButtonRecordImage = global::MLifter.Recorder.Properties.Resources.rec_active_expertmode;
			this.numPadControl.ButtonRecordImageNotAvailable = global::MLifter.Recorder.Properties.Resources.rec_inactive_expertmode;
			this.numPadControl.ButtonStopImage = global::MLifter.Recorder.Properties.Resources.stop_active_expertmode;
			this.numPadControl.ButtonSwitchImage = global::MLifter.Recorder.Properties.Resources.simplemode;
			this.numPadControl.MaximumSize = new System.Drawing.Size(540, 600);
			this.numPadControl.MinimumSize = new System.Drawing.Size(360, 450);
			this.numPadControl.Name = "numPadControl";
			this.numPadControl.PlayImage = ((System.Drawing.Image)(resources.GetObject("numPadControl.PlayImage")));
			this.numPadControl.RecordImage = global::MLifter.Recorder.Properties.Resources.rec_active_simplemode;
			this.numPadControl.StopImage = global::MLifter.Recorder.Properties.Resources.stop_active_simplemode;
			this.numPadControl.Record += new System.EventHandler(this.numPadControl_Record);
			this.numPadControl.Play += new System.EventHandler(this.numPadControl_Play);
			this.numPadControl.StopRecord += new System.EventHandler(this.numPadControl_Stop);
			this.numPadControl.StopPlay += new System.EventHandler(this.numPadControl_Stop);
			this.numPadControl.Next += new System.EventHandler(this.numPadControl_Next);
			this.numPadControl.Previous += new System.EventHandler(this.numPadControl_Previous);
			this.numPadControl.ViewChanged += new System.EventHandler(this.numPadControl_ViewChanged);
			this.numPadControl.Enter += new System.EventHandler(this.numPadControl_Enter);
			// 
			// loadingControl
			// 
			resources.ApplyResources(this.loadingControl, "loadingControl");
			this.loadingControl.Message = "LoadingControl";
			this.loadingControl.Name = "loadingControl";
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.groupBoxPosition);
			this.Controls.Add(this.groupBoxFontSize);
			this.Controls.Add(this.menuStripMain);
			this.Controls.Add(this.labelWordSentence);
			this.Controls.Add(this.buttonAdvancedMode);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.groupBoxSelect);
			this.Controls.Add(this.numPadControl);
			this.Controls.Add(this.loadingControl);
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStripMain;
			this.Name = "MainForm";
			this.Activated += new System.EventHandler(this.MainForm_Activated);
			this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.groupBoxFontSize.ResumeLayout(false);
			this.groupBoxFontSize.PerformLayout();
			this.menuStripMain.ResumeLayout(false);
			this.menuStripMain.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.groupBoxPosition.ResumeLayout(false);
			this.groupBoxSelect.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWordSentence;
        private NumPadControl numPadControl;
        private System.Windows.Forms.Label labelCard;
        private System.Windows.Forms.GroupBox groupBoxFontSize;
        private System.Windows.Forms.RadioButton radioButtonFontSizeAutomatic;
        private System.Windows.Forms.RadioButton radioButtonFontSizeLarge;
        private System.Windows.Forms.RadioButton radioButtonFontSizeMedium;
        private System.Windows.Forms.RadioButton radioButtonFontSizeSmall;
        private System.Windows.Forms.ProgressBar progressBarStatus;
        private System.Windows.Forms.Button buttonAllCards;
        private System.Windows.Forms.Button buttonNoCards;
        private System.Windows.Forms.Button buttonSelectInverse;
        private System.Windows.Forms.Button buttonSelectIncomplete;
        private System.Windows.Forms.TreeView treeViewCards;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDictionaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelAction;
        private System.Windows.Forms.GroupBox groupBoxPosition;
        private System.Windows.Forms.GroupBox groupBoxSelect;
        private LoadingControl loadingControl;
        private System.Windows.Forms.Button buttonAdvancedMode;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHowTo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem codecInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

