namespace MLifter.Controls
{
    partial class AudioDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioDialog));
            this.labelText = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelText
            // 
            this.tableLayoutPanel.SetColumnSpan(this.labelText, 4);
            resources.ApplyResources(this.labelText, "labelText");
            this.labelText.Name = "labelText";
            // 
            // buttonBrowse
            // 
            resources.ApplyResources(this.buttonBrowse, "buttonBrowse");
            this.buttonBrowse.Image = global::MLifter.Controls.Properties.Resources.documentOpen;
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonRecord
            // 
            resources.ApplyResources(this.buttonRecord, "buttonRecord");
            this.buttonRecord.Image = global::MLifter.Controls.Properties.Resources.mediaRecord;
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.UseVisualStyleBackColor = true;
            this.buttonRecord.Click += new System.EventHandler(this.buttonRecord_Click);
            // 
            // buttonPlay
            // 
            resources.ApplyResources(this.buttonPlay, "buttonPlay");
            this.buttonPlay.Image = global::MLifter.Controls.Properties.Resources.mediaPlaybackStart;
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.buttonRemove, 3, 1);
            this.tableLayoutPanel.Controls.Add(this.labelText, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonRecord, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonPlay, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonBrowse, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.buttonClose, 3, 2);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // buttonRemove
            // 
            resources.ApplyResources(this.buttonRemove, "buttonRemove");
            this.buttonRemove.Image = global::MLifter.Controls.Properties.Resources.process_stop;
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.TabStop = false;
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // AudioDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AudioDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AudioDialog_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonRecord;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonClose;
    }
}