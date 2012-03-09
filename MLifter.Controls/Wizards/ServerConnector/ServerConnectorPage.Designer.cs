namespace MLifter.Controls.Wizards.ServerConnector
{
    partial class ServerConnectorPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerConnectorPage));
            this.textBoxConnectionString = new System.Windows.Forms.TextBox();
            this.labelConnectionString = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxConnectionString
            // 
            resources.ApplyResources(this.textBoxConnectionString, "textBoxConnectionString");
            this.textBoxConnectionString.Name = "textBoxConnectionString";
            // 
            // labelConnectionString
            // 
            resources.ApplyResources(this.labelConnectionString, "labelConnectionString");
            this.labelConnectionString.Name = "labelConnectionString";
            // 
            // ServerConnectorPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.labelConnectionString);
            this.Controls.Add(this.textBoxConnectionString);
            resources.ApplyResources(this, "$this");
            this.Name = "ServerConnectorPage";
            this.TopImage = global::MLifter.Controls.Properties.Resources.banner;
            this.Controls.SetChildIndex(this.textBoxConnectionString, 0);
            this.Controls.SetChildIndex(this.labelConnectionString, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxConnectionString;
        private System.Windows.Forms.Label labelConnectionString;
    }
}
