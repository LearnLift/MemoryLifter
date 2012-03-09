namespace MLifter.Controls.Wizards.DictionaryCreator
{
    partial class SourceSelectionPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceSelectionPage));
            this.labelWelcome = new System.Windows.Forms.Label();
            this.listViewSources = new System.Windows.Forms.ListView();
            this.ColumnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelWelcome
            // 
            resources.ApplyResources(this.labelWelcome, "labelWelcome");
            this.labelWelcome.Name = "labelWelcome";
            // 
            // listViewSources
            // 
            this.listViewSources.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderName});
            resources.ApplyResources(this.listViewSources, "listViewSources");
            this.listViewSources.FullRowSelect = true;
            this.listViewSources.HideSelection = false;
            this.listViewSources.MultiSelect = false;
            this.listViewSources.Name = "listViewSources";
            this.listViewSources.ShowItemToolTips = true;
            this.listViewSources.UseCompatibleStateImageBehavior = false;
            this.listViewSources.View = System.Windows.Forms.View.Details;
            this.listViewSources.SelectedIndexChanged += new System.EventHandler(this.listViewSources_SelectedIndexChanged);
            this.listViewSources.DoubleClick += new System.EventHandler(this.listViewSources_DoubleClick);
            // 
            // ColumnHeaderName
            // 
            resources.ApplyResources(this.ColumnHeaderName, "ColumnHeaderName");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // SourceSelectionPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewSources);
            this.Controls.Add(this.labelWelcome);
            this.LeftImage = global::MLifter.Controls.Properties.Resources.setup;
            this.Name = "SourceSelectionPage";
            this.NextAllowed = false;
            resources.ApplyResources(this, "$this");
            this.Load += new System.EventHandler(this.SourceSelection_Load);
            this.Controls.SetChildIndex(this.labelWelcome, 0);
            this.Controls.SetChildIndex(this.listViewSources, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelWelcome;
        private System.Windows.Forms.ListView listViewSources;
        private System.Windows.Forms.ColumnHeader ColumnHeaderName;
        private System.Windows.Forms.Label label1;
    }
}
