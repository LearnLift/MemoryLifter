namespace MLifter.Controls
{
    partial class MultipleChoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultipleChoice));
            this.flowLayoutPanelMultipleChoice = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flowLayoutPanelMultipleChoice
            // 
            resources.ApplyResources(this.flowLayoutPanelMultipleChoice, "flowLayoutPanelMultipleChoice");
            this.flowLayoutPanelMultipleChoice.Name = "flowLayoutPanelMultipleChoice";
            this.flowLayoutPanelMultipleChoice.Scroll += new System.Windows.Forms.ScrollEventHandler(this.flowLayoutPanelMultipleChoice_Scroll);
            this.flowLayoutPanelMultipleChoice.Resize += new System.EventHandler(this.flowLayoutPanelMultipleChoice_Resize);
            // 
            // MultipleChoice
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.flowLayoutPanelMultipleChoice);
            resources.ApplyResources(this, "$this");
            this.Name = "MultipleChoice";
            this.Load += new System.EventHandler(this.MultipleChoice_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMultipleChoice;
    }
}
