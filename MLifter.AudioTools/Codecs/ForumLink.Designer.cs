namespace MLifterAudioTools.Codecs
{
    partial class ForumLink
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
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // linkLabel
            // 
            this.linkLabel.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel.LinkArea = new System.Windows.Forms.LinkArea(10, 18);
            this.linkLabel.Location = new System.Drawing.Point(0, 0);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new System.Drawing.Size(300, 30);
            this.linkLabel.TabIndex = 0;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "Visit the MemoryLifter Forum for information and help.";
            this.linkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel.UseCompatibleTextRendering = true;
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // ForumLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabel);
            this.Name = "ForumLink";
            this.Size = new System.Drawing.Size(300, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel;
    }
}
