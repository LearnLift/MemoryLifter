namespace MLifterAudioBookGenerator
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
			this.textBoxLearningModule = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxOptions = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.radioButtonStereo = new System.Windows.Forms.RadioButton();
			this.radioButtonMono = new System.Windows.Forms.RadioButton();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.groupBoxPlaybackSequence = new System.Windows.Forms.GroupBox();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.listViewPlaybackSequence = new System.Windows.Forms.ListView();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBoxAvailable = new System.Windows.Forms.GroupBox();
			this.listViewAvailableFields = new System.Windows.Forms.ListView();
			this.buttonStart = new System.Windows.Forms.Button();
			this.buttonBrowseAudiobook = new System.Windows.Forms.Button();
			this.textBoxAudiobook = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonBrowseLearningModule = new System.Windows.Forms.Button();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.howToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBoxOptions.SuspendLayout();
			this.groupBoxPlaybackSequence.SuspendLayout();
			this.groupBoxAvailable.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxLearningModule
			// 
			this.textBoxLearningModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLearningModule.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.textBoxLearningModule.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.textBoxLearningModule.Location = new System.Drawing.Point(121, 20);
			this.textBoxLearningModule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxLearningModule.Name = "textBoxLearningModule";
			this.textBoxLearningModule.Size = new System.Drawing.Size(531, 23);
			this.textBoxLearningModule.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(108, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Learning module:";
			// 
			// groupBoxOptions
			// 
			this.groupBoxOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxOptions.Controls.Add(this.label4);
			this.groupBoxOptions.Controls.Add(this.radioButtonStereo);
			this.groupBoxOptions.Controls.Add(this.radioButtonMono);
			this.groupBoxOptions.Controls.Add(this.buttonAdd);
			this.groupBoxOptions.Controls.Add(this.groupBoxPlaybackSequence);
			this.groupBoxOptions.Controls.Add(this.label3);
			this.groupBoxOptions.Controls.Add(this.groupBoxAvailable);
			this.groupBoxOptions.Controls.Add(this.buttonStart);
			this.groupBoxOptions.Controls.Add(this.buttonBrowseAudiobook);
			this.groupBoxOptions.Controls.Add(this.textBoxAudiobook);
			this.groupBoxOptions.Controls.Add(this.label2);
			this.groupBoxOptions.Controls.Add(this.buttonBrowseLearningModule);
			this.groupBoxOptions.Controls.Add(this.label1);
			this.groupBoxOptions.Controls.Add(this.textBoxLearningModule);
			this.groupBoxOptions.Location = new System.Drawing.Point(14, 28);
			this.groupBoxOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBoxOptions.Name = "groupBoxOptions";
			this.groupBoxOptions.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBoxOptions.Size = new System.Drawing.Size(737, 411);
			this.groupBoxOptions.TabIndex = 2;
			this.groupBoxOptions.TabStop = false;
			this.groupBoxOptions.Text = "Audiobook options";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(10, 379);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(150, 16);
			this.label4.TabIndex = 12;
			this.label4.Text = "Desired output channels:";
			// 
			// radioButtonStereo
			// 
			this.radioButtonStereo.AutoSize = true;
			this.radioButtonStereo.Checked = true;
			this.radioButtonStereo.Location = new System.Drawing.Point(229, 377);
			this.radioButtonStereo.Name = "radioButtonStereo";
			this.radioButtonStereo.Size = new System.Drawing.Size(56, 17);
			this.radioButtonStereo.TabIndex = 11;
			this.radioButtonStereo.TabStop = true;
			this.radioButtonStereo.Text = "Stereo";
			this.radioButtonStereo.UseVisualStyleBackColor = true;
			// 
			// radioButtonMono
			// 
			this.radioButtonMono.AutoSize = true;
			this.radioButtonMono.Location = new System.Drawing.Point(166, 377);
			this.radioButtonMono.Name = "radioButtonMono";
			this.radioButtonMono.Size = new System.Drawing.Size(52, 17);
			this.radioButtonMono.TabIndex = 10;
			this.radioButtonMono.Text = "Mono";
			this.radioButtonMono.UseVisualStyleBackColor = true;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Enabled = false;
			this.buttonAdd.Image = global::MLifterAudioBookGenerator.Properties.Resources.goNext;
			this.buttonAdd.Location = new System.Drawing.Point(356, 231);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(50, 50);
			this.buttonAdd.TabIndex = 9;
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// groupBoxPlaybackSequence
			// 
			this.groupBoxPlaybackSequence.Controls.Add(this.buttonDelete);
			this.groupBoxPlaybackSequence.Controls.Add(this.listViewPlaybackSequence);
			this.groupBoxPlaybackSequence.Location = new System.Drawing.Point(412, 134);
			this.groupBoxPlaybackSequence.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBoxPlaybackSequence.Name = "groupBoxPlaybackSequence";
			this.groupBoxPlaybackSequence.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBoxPlaybackSequence.Size = new System.Drawing.Size(319, 232);
			this.groupBoxPlaybackSequence.TabIndex = 8;
			this.groupBoxPlaybackSequence.TabStop = false;
			this.groupBoxPlaybackSequence.Text = "Field playback sequence for each card";
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDelete.Enabled = false;
			this.buttonDelete.Image = global::MLifterAudioBookGenerator.Properties.Resources.delete;
			this.buttonDelete.Location = new System.Drawing.Point(288, 200);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(25, 25);
			this.buttonDelete.TabIndex = 10;
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// listViewPlaybackSequence
			// 
			this.listViewPlaybackSequence.AllowDrop = true;
			this.listViewPlaybackSequence.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewPlaybackSequence.FullRowSelect = true;
			this.listViewPlaybackSequence.GridLines = true;
			this.listViewPlaybackSequence.HideSelection = false;
			this.listViewPlaybackSequence.Location = new System.Drawing.Point(3, 20);
			this.listViewPlaybackSequence.Name = "listViewPlaybackSequence";
			this.listViewPlaybackSequence.Size = new System.Drawing.Size(313, 208);
			this.listViewPlaybackSequence.TabIndex = 0;
			this.listViewPlaybackSequence.UseCompatibleStateImageBehavior = false;
			this.listViewPlaybackSequence.View = System.Windows.Forms.View.List;
			this.listViewPlaybackSequence.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewPlaybackSequence_ItemDrag);
			this.listViewPlaybackSequence.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewPlaybackSequence_ItemSelectionChanged);
			this.listViewPlaybackSequence.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewPlaybackSequence_DragDrop);
			this.listViewPlaybackSequence.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewPlaybackSequence_DragEnter);
			this.listViewPlaybackSequence.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewPlaybackSequence_MouseDoubleClick);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 109);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(437, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Please select the required fields and the desired playback sequence below:";
			// 
			// groupBoxAvailable
			// 
			this.groupBoxAvailable.Controls.Add(this.listViewAvailableFields);
			this.groupBoxAvailable.Location = new System.Drawing.Point(10, 134);
			this.groupBoxAvailable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBoxAvailable.Name = "groupBoxAvailable";
			this.groupBoxAvailable.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBoxAvailable.Size = new System.Drawing.Size(340, 232);
			this.groupBoxAvailable.TabIndex = 7;
			this.groupBoxAvailable.TabStop = false;
			this.groupBoxAvailable.Text = "Available fields";
			// 
			// listViewAvailableFields
			// 
			this.listViewAvailableFields.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewAvailableFields.FullRowSelect = true;
			this.listViewAvailableFields.GridLines = true;
			this.listViewAvailableFields.HideSelection = false;
			this.listViewAvailableFields.Location = new System.Drawing.Point(3, 20);
			this.listViewAvailableFields.Name = "listViewAvailableFields";
			this.listViewAvailableFields.Size = new System.Drawing.Size(334, 208);
			this.listViewAvailableFields.TabIndex = 0;
			this.listViewAvailableFields.UseCompatibleStateImageBehavior = false;
			this.listViewAvailableFields.View = System.Windows.Forms.View.List;
			this.listViewAvailableFields.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewAvailableFields_ItemSelectionChanged);
			this.listViewAvailableFields.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewAvailableFields_MouseDoubleClick);
			// 
			// buttonStart
			// 
			this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStart.Location = new System.Drawing.Point(536, 373);
			this.buttonStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(194, 28);
			this.buttonStart.TabIndex = 6;
			this.buttonStart.Text = "Create audiobook";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// buttonBrowseAudiobook
			// 
			this.buttonBrowseAudiobook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBrowseAudiobook.Image = global::MLifterAudioBookGenerator.Properties.Resources.document_open;
			this.buttonBrowseAudiobook.Location = new System.Drawing.Point(658, 58);
			this.buttonBrowseAudiobook.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonBrowseAudiobook.Name = "buttonBrowseAudiobook";
			this.buttonBrowseAudiobook.Size = new System.Drawing.Size(72, 28);
			this.buttonBrowseAudiobook.TabIndex = 5;
			this.buttonBrowseAudiobook.UseVisualStyleBackColor = true;
			this.buttonBrowseAudiobook.Click += new System.EventHandler(this.buttonBrowseAudiobook_Click);
			// 
			// textBoxAudiobook
			// 
			this.textBoxAudiobook.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxAudiobook.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.textBoxAudiobook.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.textBoxAudiobook.Location = new System.Drawing.Point(121, 60);
			this.textBoxAudiobook.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxAudiobook.Name = "textBoxAudiobook";
			this.textBoxAudiobook.Size = new System.Drawing.Size(531, 23);
			this.textBoxAudiobook.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Audio output file:";
			// 
			// buttonBrowseLearningModule
			// 
			this.buttonBrowseLearningModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBrowseLearningModule.Image = global::MLifterAudioBookGenerator.Properties.Resources.document_open;
			this.buttonBrowseLearningModule.Location = new System.Drawing.Point(658, 16);
			this.buttonBrowseLearningModule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonBrowseLearningModule.Name = "buttonBrowseLearningModule";
			this.buttonBrowseLearningModule.Size = new System.Drawing.Size(72, 28);
			this.buttonBrowseLearningModule.TabIndex = 2;
			this.buttonBrowseLearningModule.UseVisualStyleBackColor = true;
			this.buttonBrowseLearningModule.Click += new System.EventHandler(this.buttonBrowseLearningModule_Click);
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(14, 447);
			this.textBoxLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(737, 182);
			this.textBoxLog.TabIndex = 3;
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(12, 636);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(739, 23);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar.TabIndex = 12;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(765, 24);
			this.menuStrip1.TabIndex = 13;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.settingsToolStripMenuItem.Text = "Codec Settings...";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(159, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.howToToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// howToToolStripMenuItem
			// 
			this.howToToolStripMenuItem.Name = "howToToolStripMenuItem";
			this.howToToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.howToToolStripMenuItem.Text = "How To...";
			this.howToToolStripMenuItem.Click += new System.EventHandler(this.howToToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.aboutToolStripMenuItem.Text = "About...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AcceptButton = this.buttonStart;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(765, 663);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.groupBoxOptions);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MinimumSize = new System.Drawing.Size(700, 600);
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MemoryLifter Audiobook Generator";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.groupBoxOptions.ResumeLayout(false);
			this.groupBoxOptions.PerformLayout();
			this.groupBoxPlaybackSequence.ResumeLayout(false);
			this.groupBoxAvailable.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLearningModule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonBrowseAudiobook;
        private System.Windows.Forms.TextBox textBoxAudiobook;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBrowseLearningModule;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.GroupBox groupBoxAvailable;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listViewAvailableFields;
        private System.Windows.Forms.GroupBox groupBoxPlaybackSequence;
        private System.Windows.Forms.ListView listViewPlaybackSequence;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.RadioButton radioButtonStereo;
        private System.Windows.Forms.RadioButton radioButtonMono;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem howToToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

