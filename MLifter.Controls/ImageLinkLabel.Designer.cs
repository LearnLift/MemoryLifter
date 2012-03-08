namespace MLifter.Controls
{
    partial class ImageLinkLabel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageLinkLabel));
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.panelPlaceHolder = new System.Windows.Forms.Panel();
            this.pictureBoxLinkLabel = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLinkLabel)).BeginInit();
            this.SuspendLayout();
            // 
            // linkLabel
            // 
            resources.ApplyResources(this.linkLabel, "linkLabel");
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.TabStop = true;
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // panelPlaceHolder
            // 
            resources.ApplyResources(this.panelPlaceHolder, "panelPlaceHolder");
            this.panelPlaceHolder.Name = "panelPlaceHolder";
            // 
            // pictureBoxLinkLabel
            // 
            this.pictureBoxLinkLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pictureBoxLinkLabel, "pictureBoxLinkLabel");
            this.pictureBoxLinkLabel.Name = "pictureBoxLinkLabel";
            this.pictureBoxLinkLabel.TabStop = false;
            this.pictureBoxLinkLabel.Click += new System.EventHandler(this.pictureBoxLinkLabel_Click);
            // 
            // ImageLinkLabel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pictureBoxLinkLabel);
            this.Controls.Add(this.panelPlaceHolder);
            this.Controls.Add(this.linkLabel);
            this.Name = "ImageLinkLabel";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLinkLabel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Panel panelPlaceHolder;
        private System.Windows.Forms.PictureBox pictureBoxLinkLabel;


    }
}
