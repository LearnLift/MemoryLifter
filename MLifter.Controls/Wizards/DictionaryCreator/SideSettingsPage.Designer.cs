namespace MLifter.Controls.Wizards.DictionaryCreator
{
    partial class SideSettingsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SideSettingsPage));
            this.dictionaryCaptions = new MLifter.Controls.DictionaryCaptions();
            this.SuspendLayout();
            // 
            // dictionaryCaptions
            // 
            this.dictionaryCaptions.AccessibleDescription = null;
            this.dictionaryCaptions.AccessibleName = null;
            resources.ApplyResources(this.dictionaryCaptions, "dictionaryCaptions");
            this.dictionaryCaptions.BackgroundImage = null;
            this.dictionaryCaptions.Font = null;
            this.dictionaryCaptions.Name = "dictionaryCaptions";
            // 
            // SideSettingsPage
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.dictionaryCaptions);
            this.HelpAvailable = true;
            this.Name = "SideSettingsPage";
            this.TopImage = global::MLifter.Controls.Properties.Resources.banner;
            this.Controls.SetChildIndex(this.dictionaryCaptions, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private DictionaryCaptions dictionaryCaptions;




    }
}
