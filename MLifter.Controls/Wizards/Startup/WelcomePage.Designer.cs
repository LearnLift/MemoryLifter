namespace MLifter.Controls.Wizards.Startup
{
    partial class WelcomePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomePage));
            this.labelWelcome = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.checkBoxRegister = new System.Windows.Forms.CheckBox();
            this.checkBoxShowHelp = new System.Windows.Forms.CheckBox();
            this.labelLastNews = new System.Windows.Forms.Label();
            this.webBrowserNews = new MLifter.Components.MLifterWebBrowser();
            this.linkLabelEnlargeNews = new System.Windows.Forms.LinkLabel();
            this.panelWebContainer = new System.Windows.Forms.Panel();
            this.panelWebContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelWelcome
            // 
            resources.ApplyResources(this.labelWelcome, "labelWelcome");
            this.labelWelcome.Name = "labelWelcome";
            // 
            // labelDescription
            // 
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.labelDescription.Name = "labelDescription";
            // 
            // checkBoxRegister
            // 
            resources.ApplyResources(this.checkBoxRegister, "checkBoxRegister");
            this.checkBoxRegister.Checked = true;
            this.checkBoxRegister.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRegister.Name = "checkBoxRegister";
            this.checkBoxRegister.UseVisualStyleBackColor = true;
            this.checkBoxRegister.CheckedChanged += new System.EventHandler(this.checkBoxRegister_CheckedChanged);
            // 
            // checkBoxShowHelp
            // 
            resources.ApplyResources(this.checkBoxShowHelp, "checkBoxShowHelp");
            this.checkBoxShowHelp.Name = "checkBoxShowHelp";
            this.checkBoxShowHelp.UseVisualStyleBackColor = true;
            // 
            // labelLastNews
            // 
            resources.ApplyResources(this.labelLastNews, "labelLastNews");
            this.labelLastNews.Name = "labelLastNews";
            // 
            // webBrowserNews
            // 
            this.webBrowserNews.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.webBrowserNews, "webBrowserNews");
            this.webBrowserNews.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserNews.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserNews.Name = "webBrowserNews";
            // 
            // linkLabelEnlargeNews
            // 
            resources.ApplyResources(this.linkLabelEnlargeNews, "linkLabelEnlargeNews");
            this.linkLabelEnlargeNews.Name = "linkLabelEnlargeNews";
            this.linkLabelEnlargeNews.TabStop = true;
            this.linkLabelEnlargeNews.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEnlargeNews_LinkClicked);
            // 
            // panelWebContainer
            // 
            this.panelWebContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelWebContainer.Controls.Add(this.webBrowserNews);
            resources.ApplyResources(this.panelWebContainer, "panelWebContainer");
            this.panelWebContainer.Name = "panelWebContainer";
            // 
            // WelcomePage
            // 
            this.CancelAllowed = false;
            this.Controls.Add(this.linkLabelEnlargeNews);
            this.Controls.Add(this.labelLastNews);
            this.Controls.Add(this.checkBoxShowHelp);
            this.Controls.Add(this.checkBoxRegister);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.labelWelcome);
            this.Controls.Add(this.panelWebContainer);
            this.HelpAvailable = true;
            this.LeftImage = global::MLifter.Controls.Properties.Resources.setup;
            this.Name = "WelcomePage";
            this.Load += new System.EventHandler(this.WelcomePage_Load);
            this.Controls.SetChildIndex(this.panelWebContainer, 0);
            this.Controls.SetChildIndex(this.labelWelcome, 0);
            this.Controls.SetChildIndex(this.labelDescription, 0);
            this.Controls.SetChildIndex(this.checkBoxRegister, 0);
            this.Controls.SetChildIndex(this.checkBoxShowHelp, 0);
            this.Controls.SetChildIndex(this.labelLastNews, 0);
            this.Controls.SetChildIndex(this.linkLabelEnlargeNews, 0);
            this.panelWebContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWelcome;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.CheckBox checkBoxRegister;
        private System.Windows.Forms.CheckBox checkBoxShowHelp;
        private System.Windows.Forms.Label labelLastNews;
        private MLifter.Components.MLifterWebBrowser webBrowserNews;
        private System.Windows.Forms.LinkLabel linkLabelEnlargeNews;
        private System.Windows.Forms.Panel panelWebContainer;
    }
}
