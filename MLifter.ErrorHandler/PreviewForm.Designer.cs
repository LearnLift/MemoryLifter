namespace MLifterErrorHandler
{
    partial class PreviewForm
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
            this.webBrowserPreview = new System.Windows.Forms.WebBrowser();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.richTextBoxPreview = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowserPreview
            // 
            this.webBrowserPreview.AllowNavigation = false;
            this.webBrowserPreview.AllowWebBrowserDrop = false;
            this.webBrowserPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserPreview.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserPreview.Location = new System.Drawing.Point(0, 0);
            this.webBrowserPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserPreview.Name = "webBrowserPreview";
            this.webBrowserPreview.Size = new System.Drawing.Size(792, 597);
            this.webBrowserPreview.TabIndex = 0;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(792, 597);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 1;
            this.pictureBoxPreview.TabStop = false;
            // 
            // richTextBoxPreview
            // 
            this.richTextBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxPreview.Name = "richTextBoxPreview";
            this.richTextBoxPreview.Size = new System.Drawing.Size(792, 597);
            this.richTextBoxPreview.TabIndex = 2;
            this.richTextBoxPreview.Text = "";
            // 
            // PreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 597);
            this.Controls.Add(this.richTextBoxPreview);
            this.Controls.Add(this.webBrowserPreview);
            this.Controls.Add(this.pictureBoxPreview);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PreviewForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preview";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreviewForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowserPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.RichTextBox richTextBoxPreview;
    }
}