namespace MLifter.Controls
{
    partial class CardEdit
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
            StopPlayingVideos();

            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardEdit));
            this.groupBoxAnswer = new System.Windows.Forms.GroupBox();
            this.checkBoxCharacterMapAnswer = new System.Windows.Forms.CheckBox();
            this.checkBoxSynonymAnswer = new System.Windows.Forms.CheckBox();
            this.listViewAnswer = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStripSynonyms = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.distractorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whatsThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxResizeAnswer = new System.Windows.Forms.CheckBox();
            this.buttonAnswerImage = new System.Windows.Forms.Button();
            this.checkBoxSamePicture = new System.Windows.Forms.CheckBox();
            this.buttonAnswerVideo = new System.Windows.Forms.Button();
            this.buttonAnswerAudio = new System.Windows.Forms.Button();
            this.pictureBoxAnswer = new System.Windows.Forms.PictureBox();
            this.buttonAnswerExampleAudio = new System.Windows.Forms.Button();
            this.textBoxAnswerExample = new System.Windows.Forms.TextBox();
            this.textBoxAnswer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textColumnQuestion = new XPTable.Models.TextColumn();
            this.groupBoxQuestion = new System.Windows.Forms.GroupBox();
            this.checkBoxCharacterMapQuestion = new System.Windows.Forms.CheckBox();
            this.checkBoxSynonymQuestion = new System.Windows.Forms.CheckBox();
            this.listViewQuestion = new System.Windows.Forms.ListView();
            this.Main = new System.Windows.Forms.ColumnHeader();
            this.buttonQuestionImage = new System.Windows.Forms.Button();
            this.checkBoxResizeQuestion = new System.Windows.Forms.CheckBox();
            this.buttonQuestionVideo = new System.Windows.Forms.Button();
            this.buttonQuestionAudio = new System.Windows.Forms.Button();
            this.pictureBoxQuestion = new System.Windows.Forms.PictureBox();
            this.buttonQuestionExampleAudio = new System.Windows.Forms.Button();
            this.textBoxQuestionExample = new System.Windows.Forms.TextBox();
            this.textBoxQuestion = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.contextMenuStripMedia = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.comboBoxCardBox = new System.Windows.Forms.ComboBox();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.comboBoxChapter = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonAddEdit = new System.Windows.Forms.Button();
            this.panelVideoParker = new System.Windows.Forms.Panel();
            this.buttonStyle = new System.Windows.Forms.Button();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.characterMapComponent = new MLifter.Controls.LearningWindow.CharacterMapComponent(this.components);
            this.groupBoxAnswer.SuspendLayout();
            this.contextMenuStripSynonyms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnswer)).BeginInit();
            this.groupBoxQuestion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQuestion)).BeginInit();
            this.contextMenuStripMedia.SuspendLayout();
            this.groupBoxGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxAnswer
            // 
            this.groupBoxAnswer.Controls.Add(this.checkBoxCharacterMapAnswer);
            this.groupBoxAnswer.Controls.Add(this.checkBoxSynonymAnswer);
            this.groupBoxAnswer.Controls.Add(this.listViewAnswer);
            this.groupBoxAnswer.Controls.Add(this.checkBoxResizeAnswer);
            this.groupBoxAnswer.Controls.Add(this.buttonAnswerImage);
            this.groupBoxAnswer.Controls.Add(this.checkBoxSamePicture);
            this.groupBoxAnswer.Controls.Add(this.buttonAnswerVideo);
            this.groupBoxAnswer.Controls.Add(this.buttonAnswerAudio);
            this.groupBoxAnswer.Controls.Add(this.pictureBoxAnswer);
            this.groupBoxAnswer.Controls.Add(this.buttonAnswerExampleAudio);
            this.groupBoxAnswer.Controls.Add(this.textBoxAnswerExample);
            this.groupBoxAnswer.Controls.Add(this.textBoxAnswer);
            this.groupBoxAnswer.Controls.Add(this.label3);
            this.groupBoxAnswer.Controls.Add(this.label4);
            this.groupBoxAnswer.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBoxAnswer, "groupBoxAnswer");
            this.groupBoxAnswer.Name = "groupBoxAnswer";
            this.groupBoxAnswer.TabStop = false;
            // 
            // checkBoxCharacterMapAnswer
            // 
            resources.ApplyResources(this.checkBoxCharacterMapAnswer, "checkBoxCharacterMapAnswer");
            this.checkBoxCharacterMapAnswer.Image = global::MLifter.Controls.Properties.Resources.charactermap;
            this.checkBoxCharacterMapAnswer.Name = "checkBoxCharacterMapAnswer";
            this.checkBoxCharacterMapAnswer.TabStop = false;
            this.checkBoxCharacterMapAnswer.UseVisualStyleBackColor = true;
            this.checkBoxCharacterMapAnswer.CheckedChanged += new System.EventHandler(this.checkBoxCharacterMap_CheckedChanged);
            // 
            // checkBoxSynonymAnswer
            // 
            resources.ApplyResources(this.checkBoxSynonymAnswer, "checkBoxSynonymAnswer");
            this.checkBoxSynonymAnswer.Image = global::MLifter.Controls.Properties.Resources.synonym_mode;
            this.checkBoxSynonymAnswer.Name = "checkBoxSynonymAnswer";
            this.checkBoxSynonymAnswer.TabStop = false;
            this.checkBoxSynonymAnswer.UseVisualStyleBackColor = true;
            this.checkBoxSynonymAnswer.CheckedChanged += new System.EventHandler(this.checkBoxSynonym_CheckedChanged);
            // 
            // listViewAnswer
            // 
            this.listViewAnswer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewAnswer.ContextMenuStrip = this.contextMenuStripSynonyms;
            resources.ApplyResources(this.listViewAnswer, "listViewAnswer");
            this.listViewAnswer.FullRowSelect = true;
            this.listViewAnswer.GridLines = true;
            this.listViewAnswer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewAnswer.LabelEdit = true;
            this.listViewAnswer.Name = "listViewAnswer";
            this.listViewAnswer.ShowItemToolTips = true;
            this.listViewAnswer.UseCompatibleStateImageBehavior = false;
            this.listViewAnswer.View = System.Windows.Forms.View.Details;
            this.listViewAnswer.ClientSizeChanged += new System.EventHandler(this.listView_ClientSizeChanged);
            this.listViewAnswer.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView_AfterLabelEdit);
            this.listViewAnswer.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listViewAnswer.Enter += new System.EventHandler(this.listView_Enter);
            this.listViewAnswer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listView_KeyPress);
            this.listViewAnswer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // contextMenuStripSynonyms
            // 
            this.contextMenuStripSynonyms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.distractorToolStripMenuItem,
            this.whatsThisToolStripMenuItem});
            this.contextMenuStripSynonyms.Name = "contextMenuStripSynonyms";
            resources.ApplyResources(this.contextMenuStripSynonyms, "contextMenuStripSynonyms");
            this.contextMenuStripSynonyms.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripSynonyms_Opening);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = global::MLifter.Controls.Properties.Resources.edit_cut;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::MLifter.Controls.Properties.Resources.edit_copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::MLifter.Controls.Properties.Resources.edit_paste;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::MLifter.Controls.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // distractorToolStripMenuItem
            // 
            this.distractorToolStripMenuItem.Name = "distractorToolStripMenuItem";
            resources.ApplyResources(this.distractorToolStripMenuItem, "distractorToolStripMenuItem");
            this.distractorToolStripMenuItem.Click += new System.EventHandler(this.distractorToolStripMenuItem_Click);
            // 
            // whatsThisToolStripMenuItem
            // 
            this.whatsThisToolStripMenuItem.Name = "whatsThisToolStripMenuItem";
            resources.ApplyResources(this.whatsThisToolStripMenuItem, "whatsThisToolStripMenuItem");
            this.whatsThisToolStripMenuItem.Click += new System.EventHandler(this.whatsThisToolStripMenuItem_Click);
            // 
            // checkBoxResizeAnswer
            // 
            resources.ApplyResources(this.checkBoxResizeAnswer, "checkBoxResizeAnswer");
            this.checkBoxResizeAnswer.Image = global::MLifter.Controls.Properties.Resources.resize;
            this.checkBoxResizeAnswer.Name = "checkBoxResizeAnswer";
            this.checkBoxResizeAnswer.TabStop = false;
            this.checkBoxResizeAnswer.UseVisualStyleBackColor = true;
            this.checkBoxResizeAnswer.CheckedChanged += new System.EventHandler(this.checkBoxResizeAnswer_CheckedChanged);
            // 
            // buttonAnswerImage
            // 
            this.buttonAnswerImage.Image = global::MLifter.Controls.Properties.Resources.cameraPhoto;
            resources.ApplyResources(this.buttonAnswerImage, "buttonAnswerImage");
            this.buttonAnswerImage.Name = "buttonAnswerImage";
            this.buttonAnswerImage.TabStop = false;
            this.buttonAnswerImage.UseVisualStyleBackColor = true;
            this.buttonAnswerImage.Click += new System.EventHandler(this.buttonMedia_Click);
            // 
            // checkBoxSamePicture
            // 
            resources.ApplyResources(this.checkBoxSamePicture, "checkBoxSamePicture");
            this.checkBoxSamePicture.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxSamePicture.Name = "checkBoxSamePicture";
            this.checkBoxSamePicture.TabStop = false;
            this.checkBoxSamePicture.UseVisualStyleBackColor = false;
            this.checkBoxSamePicture.CheckedChanged += new System.EventHandler(this.checkBoxSamePicture_CheckedChanged);
            // 
            // buttonAnswerVideo
            // 
            this.buttonAnswerVideo.Image = global::MLifter.Controls.Properties.Resources.Video;
            resources.ApplyResources(this.buttonAnswerVideo, "buttonAnswerVideo");
            this.buttonAnswerVideo.Name = "buttonAnswerVideo";
            this.buttonAnswerVideo.TabStop = false;
            this.buttonAnswerVideo.UseVisualStyleBackColor = true;
            this.buttonAnswerVideo.Click += new System.EventHandler(this.buttonMedia_Click);
            // 
            // buttonAnswerAudio
            // 
            this.buttonAnswerAudio.Image = global::MLifter.Controls.Properties.Resources.AudioAvailable;
            resources.ApplyResources(this.buttonAnswerAudio, "buttonAnswerAudio");
            this.buttonAnswerAudio.Name = "buttonAnswerAudio";
            this.buttonAnswerAudio.TabStop = false;
            this.buttonAnswerAudio.UseVisualStyleBackColor = true;
            this.buttonAnswerAudio.Click += new System.EventHandler(this.buttonMedia_Click);
            // 
            // pictureBoxAnswer
            // 
            this.pictureBoxAnswer.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBoxAnswer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBoxAnswer, "pictureBoxAnswer");
            this.pictureBoxAnswer.Name = "pictureBoxAnswer";
            this.pictureBoxAnswer.TabStop = false;
            this.pictureBoxAnswer.DoubleClick += new System.EventHandler(this.pictureBoxAnswer_Click);
            this.pictureBoxAnswer.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragDrop);
            this.pictureBoxAnswer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxAnswer_MouseDown);
            this.pictureBoxAnswer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxAnswer_MouseUp);
            this.pictureBoxAnswer.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragEnter);
            // 
            // buttonAnswerExampleAudio
            // 
            this.buttonAnswerExampleAudio.Image = global::MLifter.Controls.Properties.Resources.Audio;
            resources.ApplyResources(this.buttonAnswerExampleAudio, "buttonAnswerExampleAudio");
            this.buttonAnswerExampleAudio.Name = "buttonAnswerExampleAudio";
            this.buttonAnswerExampleAudio.TabStop = false;
            this.buttonAnswerExampleAudio.UseVisualStyleBackColor = true;
            this.buttonAnswerExampleAudio.Click += new System.EventHandler(this.buttonMedia_Click);
            // 
            // textBoxAnswerExample
            // 
            resources.ApplyResources(this.textBoxAnswerExample, "textBoxAnswerExample");
            this.textBoxAnswerExample.Name = "textBoxAnswerExample";
            this.textBoxAnswerExample.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxAnswer
            // 
            resources.ApplyResources(this.textBoxAnswer, "textBoxAnswer");
            this.textBoxAnswer.Name = "textBoxAnswer";
            this.textBoxAnswer.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // textColumnQuestion
            // 
            this.textColumnQuestion.Width = 230;
            // 
            // groupBoxQuestion
            // 
            this.groupBoxQuestion.Controls.Add(this.checkBoxCharacterMapQuestion);
            this.groupBoxQuestion.Controls.Add(this.checkBoxSynonymQuestion);
            this.groupBoxQuestion.Controls.Add(this.listViewQuestion);
            this.groupBoxQuestion.Controls.Add(this.buttonQuestionImage);
            this.groupBoxQuestion.Controls.Add(this.checkBoxResizeQuestion);
            this.groupBoxQuestion.Controls.Add(this.buttonQuestionVideo);
            this.groupBoxQuestion.Controls.Add(this.buttonQuestionAudio);
            this.groupBoxQuestion.Controls.Add(this.pictureBoxQuestion);
            this.groupBoxQuestion.Controls.Add(this.buttonQuestionExampleAudio);
            this.groupBoxQuestion.Controls.Add(this.textBoxQuestionExample);
            this.groupBoxQuestion.Controls.Add(this.textBoxQuestion);
            this.groupBoxQuestion.Controls.Add(this.label1);
            this.groupBoxQuestion.Controls.Add(this.label2);
            this.groupBoxQuestion.Controls.Add(this.label5);
            resources.ApplyResources(this.groupBoxQuestion, "groupBoxQuestion");
            this.groupBoxQuestion.Name = "groupBoxQuestion";
            this.groupBoxQuestion.TabStop = false;
            // 
            // checkBoxCharacterMapQuestion
            // 
            resources.ApplyResources(this.checkBoxCharacterMapQuestion, "checkBoxCharacterMapQuestion");
            this.checkBoxCharacterMapQuestion.Image = global::MLifter.Controls.Properties.Resources.charactermap;
            this.checkBoxCharacterMapQuestion.Name = "checkBoxCharacterMapQuestion";
            this.checkBoxCharacterMapQuestion.TabStop = false;
            this.checkBoxCharacterMapQuestion.UseVisualStyleBackColor = true;
            this.checkBoxCharacterMapQuestion.CheckedChanged += new System.EventHandler(this.checkBoxCharacterMap_CheckedChanged);
            // 
            // checkBoxSynonymQuestion
            // 
            resources.ApplyResources(this.checkBoxSynonymQuestion, "checkBoxSynonymQuestion");
            this.checkBoxSynonymQuestion.Image = global::MLifter.Controls.Properties.Resources.synonym_mode;
            this.checkBoxSynonymQuestion.Name = "checkBoxSynonymQuestion";
            this.checkBoxSynonymQuestion.TabStop = false;
            this.checkBoxSynonymQuestion.UseVisualStyleBackColor = true;
            this.checkBoxSynonymQuestion.CheckedChanged += new System.EventHandler(this.checkBoxSynonym_CheckedChanged);
            // 
            // listViewQuestion
            // 
            this.listViewQuestion.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Main});
            this.listViewQuestion.ContextMenuStrip = this.contextMenuStripSynonyms;
            resources.ApplyResources(this.listViewQuestion, "listViewQuestion");
            this.listViewQuestion.FullRowSelect = true;
            this.listViewQuestion.GridLines = true;
            this.listViewQuestion.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewQuestion.LabelEdit = true;
            this.listViewQuestion.Name = "listViewQuestion";
            this.listViewQuestion.ShowItemToolTips = true;
            this.listViewQuestion.UseCompatibleStateImageBehavior = false;
            this.listViewQuestion.View = System.Windows.Forms.View.Details;
            this.listViewQuestion.ClientSizeChanged += new System.EventHandler(this.listView_ClientSizeChanged);
            this.listViewQuestion.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView_AfterLabelEdit);
            this.listViewQuestion.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listViewQuestion.Enter += new System.EventHandler(this.listView_Enter);
            this.listViewQuestion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listView_KeyPress);
            this.listViewQuestion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
            // 
            // Main
            // 
            resources.ApplyResources(this.Main, "Main");
            // 
            // buttonQuestionImage
            // 
            this.buttonQuestionImage.Image = global::MLifter.Controls.Properties.Resources.cameraPhoto;
            resources.ApplyResources(this.buttonQuestionImage, "buttonQuestionImage");
            this.buttonQuestionImage.Name = "buttonQuestionImage";
            this.buttonQuestionImage.TabStop = false;
            this.buttonQuestionImage.UseVisualStyleBackColor = true;
            this.buttonQuestionImage.Click += new System.EventHandler(this.buttonMedia_Click);
            // 
            // checkBoxResizeQuestion
            // 
            resources.ApplyResources(this.checkBoxResizeQuestion, "checkBoxResizeQuestion");
            this.checkBoxResizeQuestion.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxResizeQuestion.Image = global::MLifter.Controls.Properties.Resources.resize;
            this.checkBoxResizeQuestion.Name = "checkBoxResizeQuestion";
            this.checkBoxResizeQuestion.TabStop = false;
            this.checkBoxResizeQuestion.UseVisualStyleBackColor = false;
            this.checkBoxResizeQuestion.CheckedChanged += new System.EventHandler(this.checkBoxResizeQuestion_CheckedChanged);
            // 
            // buttonQuestionVideo
            // 
            this.buttonQuestionVideo.Image = global::MLifter.Controls.Properties.Resources.VideoAvailable;
            resources.ApplyResources(this.buttonQuestionVideo, "buttonQuestionVideo");
            this.buttonQuestionVideo.Name = "buttonQuestionVideo";
            this.buttonQuestionVideo.TabStop = false;
            this.buttonQuestionVideo.UseVisualStyleBackColor = true;
            this.buttonQuestionVideo.Click += new System.EventHandler(this.buttonMedia_Click);
            // 
            // buttonQuestionAudio
            // 
            this.buttonQuestionAudio.Image = global::MLifter.Controls.Properties.Resources.AudioAvailable;
            resources.ApplyResources(this.buttonQuestionAudio, "buttonQuestionAudio");
            this.buttonQuestionAudio.Name = "buttonQuestionAudio";
            this.buttonQuestionAudio.TabStop = false;
            this.buttonQuestionAudio.UseVisualStyleBackColor = true;
            this.buttonQuestionAudio.Click += new System.EventHandler(this.buttonMedia_Click);
            // 
            // pictureBoxQuestion
            // 
            this.pictureBoxQuestion.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBoxQuestion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBoxQuestion, "pictureBoxQuestion");
            this.pictureBoxQuestion.Name = "pictureBoxQuestion";
            this.pictureBoxQuestion.TabStop = false;
            this.pictureBoxQuestion.DoubleClick += new System.EventHandler(this.pictureBoxQuestion_Click);
            this.pictureBoxQuestion.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragDrop);
            this.pictureBoxQuestion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxQuestion_MouseDown);
            this.pictureBoxQuestion.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxQuestion_MouseUp);
            this.pictureBoxQuestion.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragEnter);
            // 
            // buttonQuestionExampleAudio
            // 
            this.buttonQuestionExampleAudio.Image = global::MLifter.Controls.Properties.Resources.Audio;
            resources.ApplyResources(this.buttonQuestionExampleAudio, "buttonQuestionExampleAudio");
            this.buttonQuestionExampleAudio.Name = "buttonQuestionExampleAudio";
            this.buttonQuestionExampleAudio.TabStop = false;
            this.buttonQuestionExampleAudio.UseVisualStyleBackColor = true;
            this.buttonQuestionExampleAudio.Click += new System.EventHandler(this.buttonMedia_Click);
            // 
            // textBoxQuestionExample
            // 
            resources.ApplyResources(this.textBoxQuestionExample, "textBoxQuestionExample");
            this.textBoxQuestionExample.Name = "textBoxQuestionExample";
            this.textBoxQuestionExample.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxQuestion
            // 
            resources.ApplyResources(this.textBoxQuestion, "textBoxQuestion");
            this.textBoxQuestion.Name = "textBoxQuestion";
            this.textBoxQuestion.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // contextMenuStripMedia
            // 
            this.contextMenuStripMedia.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.browseToolStripMenuItem});
            this.contextMenuStripMedia.Name = "contextMenuStripMedia";
            resources.ApplyResources(this.contextMenuStripMedia, "contextMenuStripMedia");
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Image = global::MLifter.Controls.Properties.Resources.mediaplayback_start;
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            resources.ApplyResources(this.playToolStripMenuItem, "playToolStripMenuItem");
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::MLifter.Controls.Properties.Resources.delete;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // browseToolStripMenuItem
            // 
            resources.ApplyResources(this.browseToolStripMenuItem, "browseToolStripMenuItem");
            this.browseToolStripMenuItem.Name = "browseToolStripMenuItem";
            this.browseToolStripMenuItem.Click += new System.EventHandler(this.browseToolStripMenuItem_Click);
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Controls.Add(this.comboBoxCardBox);
            this.groupBoxGeneral.Controls.Add(this.checkBoxActive);
            this.groupBoxGeneral.Controls.Add(this.comboBoxChapter);
            this.groupBoxGeneral.Controls.Add(this.label9);
            this.groupBoxGeneral.Controls.Add(this.label7);
            resources.ApplyResources(this.groupBoxGeneral, "groupBoxGeneral");
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.TabStop = false;
            // 
            // comboBoxCardBox
            // 
            this.comboBoxCardBox.Items.AddRange(new object[] {
            resources.GetString("comboBoxCardBox.Items"),
            resources.GetString("comboBoxCardBox.Items1"),
            resources.GetString("comboBoxCardBox.Items2"),
            resources.GetString("comboBoxCardBox.Items3"),
            resources.GetString("comboBoxCardBox.Items4"),
            resources.GetString("comboBoxCardBox.Items5"),
            resources.GetString("comboBoxCardBox.Items6"),
            resources.GetString("comboBoxCardBox.Items7"),
            resources.GetString("comboBoxCardBox.Items8"),
            resources.GetString("comboBoxCardBox.Items9"),
            resources.GetString("comboBoxCardBox.Items10")});
            resources.ApplyResources(this.comboBoxCardBox, "comboBoxCardBox");
            this.comboBoxCardBox.Name = "comboBoxCardBox";
            this.comboBoxCardBox.TabStop = false;
            this.comboBoxCardBox.SelectedIndexChanged += new System.EventHandler(this.comboBoxCardBox_SelectedIndexChanged);
            // 
            // checkBoxActive
            // 
            resources.ApplyResources(this.checkBoxActive, "checkBoxActive");
            this.checkBoxActive.Name = "checkBoxActive";
            this.checkBoxActive.TabStop = false;
            this.checkBoxActive.UseVisualStyleBackColor = true;
            this.checkBoxActive.CheckedChanged += new System.EventHandler(this.checkBoxActive_CheckedChanged);
            // 
            // comboBoxChapter
            // 
            this.comboBoxChapter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChapter.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxChapter, "comboBoxChapter");
            this.comboBoxChapter.Name = "comboBoxChapter";
            this.comboBoxChapter.TabStop = false;
            this.comboBoxChapter.SelectedIndexChanged += new System.EventHandler(this.comboBoxChapter_SelectedIndexChanged);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // buttonAddEdit
            // 
            this.buttonAddEdit.Image = global::MLifter.Controls.Properties.Resources.newDoc;
            resources.ApplyResources(this.buttonAddEdit, "buttonAddEdit");
            this.buttonAddEdit.Name = "buttonAddEdit";
            this.buttonAddEdit.UseVisualStyleBackColor = true;
            this.buttonAddEdit.Click += new System.EventHandler(this.buttonAddEdit_Click);
            // 
            // panelVideoParker
            // 
            resources.ApplyResources(this.panelVideoParker, "panelVideoParker");
            this.panelVideoParker.Name = "panelVideoParker";
            // 
            // buttonStyle
            // 
            resources.ApplyResources(this.buttonStyle, "buttonStyle");
            this.buttonStyle.Name = "buttonStyle";
            this.buttonStyle.TabStop = false;
            this.buttonStyle.UseVisualStyleBackColor = true;
            this.buttonStyle.Click += new System.EventHandler(this.buttonStyle_Click);
            // 
            // buttonPreview
            // 
            this.buttonPreview.Image = global::MLifter.Controls.Properties.Resources.edit_find;
            resources.ApplyResources(this.buttonPreview, "buttonPreview");
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.TabStop = false;
            this.buttonPreview.UseVisualStyleBackColor = true;
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
            // 
            // CardEdit
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.buttonStyle);
            this.Controls.Add(this.buttonPreview);
            this.Controls.Add(this.panelVideoParker);
            this.Controls.Add(this.groupBoxAnswer);
            this.Controls.Add(this.groupBoxQuestion);
            this.Controls.Add(this.buttonAddEdit);
            this.Controls.Add(this.groupBoxGeneral);
            resources.ApplyResources(this, "$this");
            this.Name = "CardEdit";
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CardEdit_DragEnter);
            this.groupBoxAnswer.ResumeLayout(false);
            this.groupBoxAnswer.PerformLayout();
            this.contextMenuStripSynonyms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnswer)).EndInit();
            this.groupBoxQuestion.ResumeLayout(false);
            this.groupBoxQuestion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQuestion)).EndInit();
            this.contextMenuStripMedia.ResumeLayout(false);
            this.groupBoxGeneral.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAnswer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBoxQuestion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMedia;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxGeneral;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panelVideoParker;
        protected System.Windows.Forms.Button buttonAnswerExampleAudio;
        protected System.Windows.Forms.Button buttonAnswerVideo;
        protected System.Windows.Forms.Button buttonAnswerAudio;
        protected System.Windows.Forms.PictureBox pictureBoxAnswer;
        protected System.Windows.Forms.TextBox textBoxAnswerExample;
        protected System.Windows.Forms.Button buttonQuestionExampleAudio;
        protected System.Windows.Forms.Button buttonQuestionVideo;
        protected System.Windows.Forms.Button buttonQuestionAudio;
        protected System.Windows.Forms.PictureBox pictureBoxQuestion;
        protected System.Windows.Forms.TextBox textBoxQuestionExample;
        protected System.Windows.Forms.ComboBox comboBoxChapter;
        protected System.Windows.Forms.CheckBox checkBoxActive;
        protected System.Windows.Forms.Button buttonAddEdit;
        private XPTable.Models.TextColumn textColumnQuestion;
        private System.Windows.Forms.Button buttonStyle;
        private System.Windows.Forms.CheckBox checkBoxResizeQuestion;
        protected System.Windows.Forms.Button buttonAnswerImage;
        protected System.Windows.Forms.Button buttonQuestionImage;
        protected System.Windows.Forms.ComboBox comboBoxCardBox;
        private System.Windows.Forms.CheckBox checkBoxResizeAnswer;
        private System.Windows.Forms.ColumnHeader Main;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        internal System.Windows.Forms.ListView listViewQuestion;
        internal System.Windows.Forms.ListView listViewAnswer;
        private System.Windows.Forms.Button buttonPreview;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSynonyms;
        private System.Windows.Forms.ToolStripMenuItem distractorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem whatsThisToolStripMenuItem;
        protected System.Windows.Forms.CheckBox checkBoxSamePicture;
        private System.Windows.Forms.CheckBox checkBoxSynonymQuestion;
        private System.Windows.Forms.CheckBox checkBoxSynonymAnswer;
        protected System.Windows.Forms.TextBox textBoxQuestion;
        protected System.Windows.Forms.TextBox textBoxAnswer;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.CheckBox checkBoxCharacterMapAnswer;
        private System.Windows.Forms.CheckBox checkBoxCharacterMapQuestion;
        private MLifter.Controls.LearningWindow.CharacterMapComponent characterMapComponent;
    }
}
