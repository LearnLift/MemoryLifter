namespace MLifter.Components
{
    partial class DockingButton
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
            this.linkLabelText = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // linkLabelText
            // 
            this.linkLabelText.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.linkLabelText.AutoSize = true;
            this.linkLabelText.Location = new System.Drawing.Point(50, 4);
            this.linkLabelText.Name = "linkLabelText";
            this.linkLabelText.Size = new System.Drawing.Size(82, 13);
            this.linkLabelText.TabIndex = 0;
            this.linkLabelText.TabStop = true;
            this.linkLabelText.Text = "dockingButton1";
            this.linkLabelText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelText_LinkClicked);
            // 
            // DockingButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabelText);
            this.Name = "DockingButton";
            this.Size = new System.Drawing.Size(180, 20);
            this.Resize += new System.EventHandler(this.DockingButton_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabelText;
    }
}
