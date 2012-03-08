namespace MLifter.Controls
{
    partial class SearchTextbox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchTextbox));
            this.textBoxSearchBox = new System.Windows.Forms.TextBox();
            this.pictureBoxSearchBoxClear = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearchBoxClear)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxSearchBox
            // 
            resources.ApplyResources(this.textBoxSearchBox, "textBoxSearchBox");
            this.textBoxSearchBox.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.textBoxSearchBox.Name = "textBoxSearchBox";
            this.textBoxSearchBox.TextChanged += new System.EventHandler(this.textBoxSearchBox_TextChanged);
            this.textBoxSearchBox.Leave += new System.EventHandler(this.textBoxSearchBox_Leave);
            this.textBoxSearchBox.Enter += new System.EventHandler(this.textBoxSearchBox_Enter);
            // 
            // pictureBoxSearchBoxClear
            // 
            resources.ApplyResources(this.pictureBoxSearchBoxClear, "pictureBoxSearchBoxClear");
            this.pictureBoxSearchBoxClear.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxSearchBoxClear.Image = global::MLifter.Controls.Properties.Resources.search;
            this.pictureBoxSearchBoxClear.Name = "pictureBoxSearchBoxClear";
            this.pictureBoxSearchBoxClear.TabStop = false;
            this.pictureBoxSearchBoxClear.Click += new System.EventHandler(this.pictureBoxSearchBoxClear_Click);
            // 
            // SearchTextbox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBoxSearchBoxClear);
            this.Controls.Add(this.textBoxSearchBox);
            this.Name = "SearchTextbox";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSearchBoxClear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSearchBox;
        private System.Windows.Forms.PictureBox pictureBoxSearchBoxClear;

    }
}
