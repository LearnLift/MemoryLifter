namespace MLifter.Controls.LearningWindow
{
    partial class StatisticsPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticsPanel));
            this.gradientPanelStatisticsPanel = new MLifter.Components.GradientPanel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxCards = new System.Windows.Forms.PictureBox();
            this.pictureBoxRight = new System.Windows.Forms.PictureBox();
            this.labelRight = new System.Windows.Forms.Label();
            this.pictureBoxWrong = new System.Windows.Forms.PictureBox();
            this.labelWrong = new System.Windows.Forms.Label();
            this.labelBoxNumber = new System.Windows.Forms.Label();
            this.pictureBoxTimer = new System.Windows.Forms.PictureBox();
            this.labelTimer = new System.Windows.Forms.Label();
            this.labelUser = new System.Windows.Forms.Label();
            this.labelChapter = new System.Windows.Forms.Label();
            this.pictureBoxUser = new System.Windows.Forms.PictureBox();
            this.pictureBoxChapters = new System.Windows.Forms.PictureBox();
            this.scoreControlKnown = new MLifter.Controls.LearningWindow.ScoreControl();
            this.gradientPanelStatisticsPanel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCards)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWrong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTimer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxChapters)).BeginInit();
            this.SuspendLayout();
            // 
            // gradientPanelStatisticsPanel
            // 
            this.gradientPanelStatisticsPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.gradientPanelStatisticsPanel.Controls.Add(this.tableLayoutPanel);
            resources.ApplyResources(this.gradientPanelStatisticsPanel, "gradientPanelStatisticsPanel");
            this.gradientPanelStatisticsPanel.Gradient = null;
            this.gradientPanelStatisticsPanel.LayoutSuspended = false;
            this.gradientPanelStatisticsPanel.Name = "gradientPanelStatisticsPanel";
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel.Controls.Add(this.pictureBoxCards, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.pictureBoxRight, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.labelRight, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.pictureBoxWrong, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.labelWrong, 5, 0);
            this.tableLayoutPanel.Controls.Add(this.labelBoxNumber, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.scoreControlKnown, 6, 0);
            this.tableLayoutPanel.Controls.Add(this.pictureBoxTimer, 8, 0);
            this.tableLayoutPanel.Controls.Add(this.labelTimer, 9, 0);
            this.tableLayoutPanel.Controls.Add(this.labelUser, 13, 0);
            this.tableLayoutPanel.Controls.Add(this.labelChapter, 11, 0);
            this.tableLayoutPanel.Controls.Add(this.pictureBoxUser, 12, 0);
            this.tableLayoutPanel.Controls.Add(this.pictureBoxChapters, 10, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // pictureBoxCards
            // 
            resources.ApplyResources(this.pictureBoxCards, "pictureBoxCards");
            this.pictureBoxCards.Image = global::MLifter.Controls.Properties.Resources.systemFileManager16;
            this.pictureBoxCards.Name = "pictureBoxCards";
            this.pictureBoxCards.TabStop = false;
            // 
            // pictureBoxRight
            // 
            resources.ApplyResources(this.pictureBoxRight, "pictureBoxRight");
            this.pictureBoxRight.Image = global::MLifter.Controls.Properties.Resources.face_smile_22;
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.TabStop = false;
            // 
            // labelRight
            // 
            resources.ApplyResources(this.labelRight, "labelRight");
            this.labelRight.MaximumSize = new System.Drawing.Size(40, 0);
            this.labelRight.MinimumSize = new System.Drawing.Size(20, 0);
            this.labelRight.Name = "labelRight";
            // 
            // pictureBoxWrong
            // 
            resources.ApplyResources(this.pictureBoxWrong, "pictureBoxWrong");
            this.pictureBoxWrong.Image = global::MLifter.Controls.Properties.Resources.face_sad_22;
            this.pictureBoxWrong.Name = "pictureBoxWrong";
            this.pictureBoxWrong.TabStop = false;
            // 
            // labelWrong
            // 
            resources.ApplyResources(this.labelWrong, "labelWrong");
            this.labelWrong.MaximumSize = new System.Drawing.Size(40, 0);
            this.labelWrong.MinimumSize = new System.Drawing.Size(20, 0);
            this.labelWrong.Name = "labelWrong";
            // 
            // labelBoxNumber
            // 
            resources.ApplyResources(this.labelBoxNumber, "labelBoxNumber");
            this.labelBoxNumber.AutoEllipsis = true;
            this.labelBoxNumber.Name = "labelBoxNumber";
            // 
            // pictureBoxTimer
            // 
            resources.ApplyResources(this.pictureBoxTimer, "pictureBoxTimer");
            this.pictureBoxTimer.Image = global::MLifter.Controls.Properties.Resources.stop_watch;
            this.pictureBoxTimer.Name = "pictureBoxTimer";
            this.pictureBoxTimer.TabStop = false;
            // 
            // labelTimer
            // 
            resources.ApplyResources(this.labelTimer, "labelTimer");
            this.labelTimer.MaximumSize = new System.Drawing.Size(40, 0);
            this.labelTimer.MinimumSize = new System.Drawing.Size(20, 0);
            this.labelTimer.Name = "labelTimer";
            // 
            // labelUser
            // 
            resources.ApplyResources(this.labelUser, "labelUser");
            this.labelUser.AutoEllipsis = true;
            this.labelUser.MaximumSize = new System.Drawing.Size(120, 0);
            this.labelUser.Name = "labelUser";
            // 
            // labelChapter
            // 
            resources.ApplyResources(this.labelChapter, "labelChapter");
            this.labelChapter.AutoEllipsis = true;
            this.labelChapter.Name = "labelChapter";
            // 
            // pictureBoxUser
            // 
            resources.ApplyResources(this.pictureBoxUser, "pictureBoxUser");
            this.pictureBoxUser.Image = global::MLifter.Controls.Properties.Resources.user_22;
            this.pictureBoxUser.Name = "pictureBoxUser";
            this.pictureBoxUser.TabStop = false;
            // 
            // pictureBoxChapters
            // 
            resources.ApplyResources(this.pictureBoxChapters, "pictureBoxChapters");
            this.pictureBoxChapters.Image = global::MLifter.Controls.Properties.Resources.chapters_22;
            this.pictureBoxChapters.Name = "pictureBoxChapters";
            this.pictureBoxChapters.TabStop = false;
            // 
            // scoreControlKnown
            // 
            this.scoreControlKnown.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.scoreControlKnown, "scoreControlKnown");
            this.scoreControlKnown.Name = "scoreControlKnown";
            this.scoreControlKnown.Score = 0;
            this.scoreControlKnown.Load += new System.EventHandler(this.scoreControlKnown_Load);
            // 
            // StatisticsPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.gradientPanelStatisticsPanel);
            this.Name = "StatisticsPanel";
            resources.ApplyResources(this, "$this");
            this.gradientPanelStatisticsPanel.ResumeLayout(false);
            this.gradientPanelStatisticsPanel.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCards)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWrong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTimer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxChapters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MLifter.Components.GradientPanel gradientPanelStatisticsPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox pictureBoxCards;
        private System.Windows.Forms.PictureBox pictureBoxRight;
        private System.Windows.Forms.Label labelRight;
        private System.Windows.Forms.PictureBox pictureBoxWrong;
        private System.Windows.Forms.Label labelWrong;
        private System.Windows.Forms.Label labelBoxNumber;
        private System.Windows.Forms.PictureBox pictureBoxUser;
        private System.Windows.Forms.Label labelUser;
        private ScoreControl scoreControlKnown;
        private System.Windows.Forms.Label labelTimer;
        private System.Windows.Forms.PictureBox pictureBoxTimer;
        private System.Windows.Forms.Label labelChapter;
        private System.Windows.Forms.PictureBox pictureBoxChapters;


    }
}
