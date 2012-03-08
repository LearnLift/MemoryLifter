namespace MLifter.Controls.LearningModulesPage
{
    partial class LearningModulePreviewControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearningModulePreviewControl));
            this.pictureBoxScreenShot = new System.Windows.Forms.PictureBox();
            this.richTextBoxDescription = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenShot)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxScreenShot
            // 
            resources.ApplyResources(this.pictureBoxScreenShot, "pictureBoxScreenShot");
            this.pictureBoxScreenShot.Name = "pictureBoxScreenShot";
            this.pictureBoxScreenShot.TabStop = false;
            // 
            // richTextBoxDescription
            // 
            resources.ApplyResources(this.richTextBoxDescription, "richTextBoxDescription");
            this.richTextBoxDescription.BackColor = System.Drawing.Color.White;
            this.richTextBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxDescription.Name = "richTextBoxDescription";
            this.richTextBoxDescription.ReadOnly = true;
            // 
            // LearningModulePreviewControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.richTextBoxDescription);
            this.Controls.Add(this.pictureBoxScreenShot);
            this.MinimumSize = new System.Drawing.Size(267, 150);
            this.Name = "LearningModulePreviewControl";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenShot)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxScreenShot;
        private System.Windows.Forms.RichTextBox richTextBoxDescription;
    }
}
