namespace MLifter
{
    partial class WizardPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardPage));
            this.pictureBoxLeft = new System.Windows.Forms.PictureBox();
            this.pictureBoxTop = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBoxLeft, "pictureBoxLeft");
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.TabStop = false;
            // 
            // pictureBoxTop
            // 
            this.pictureBoxTop.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBoxTop, "pictureBoxTop");
            this.pictureBoxTop.Name = "pictureBoxTop";
            this.pictureBoxTop.TabStop = false;
            this.pictureBoxTop.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxTop_Paint);
            // 
            // WizardPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.pictureBoxTop);
            this.Controls.Add(this.pictureBoxLeft);
            resources.ApplyResources(this, "$this");
            this.Name = "WizardPage";
            this.Load += new System.EventHandler(this.WizardPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLeft;
        private System.Windows.Forms.PictureBox pictureBoxTop;
    }
}
