namespace MLifter.Controls.Wizards.ServerConnector
{
    partial class LMSelectionPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LMSelectionPage));
            this.labelConnectionString = new System.Windows.Forms.Label();
            this.listViewLMs = new System.Windows.Forms.ListView();
            this.columnHeaderTitle = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderAuthor = new System.Windows.Forms.ColumnHeader();
            this.buttonCreateNew = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelConnectionString
            // 
            resources.ApplyResources(this.labelConnectionString, "labelConnectionString");
            this.labelConnectionString.Name = "labelConnectionString";
            // 
            // listViewLMs
            // 
            this.listViewLMs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTitle,
            this.columnHeaderAuthor});
            this.listViewLMs.FullRowSelect = true;
            this.listViewLMs.HideSelection = false;
            resources.ApplyResources(this.listViewLMs, "listViewLMs");
            this.listViewLMs.Name = "listViewLMs";
            this.listViewLMs.UseCompatibleStateImageBehavior = false;
            this.listViewLMs.View = System.Windows.Forms.View.Details;
            this.listViewLMs.SelectedIndexChanged += new System.EventHandler(this.listViewLMs_SelectedIndexChanged);
            // 
            // columnHeaderTitle
            // 
            resources.ApplyResources(this.columnHeaderTitle, "columnHeaderTitle");
            // 
            // columnHeaderAuthor
            // 
            resources.ApplyResources(this.columnHeaderAuthor, "columnHeaderAuthor");
            // 
            // buttonCreateNew
            // 
            this.buttonCreateNew.Image = global::MLifter.Controls.Properties.Resources.listAdd;
            resources.ApplyResources(this.buttonCreateNew, "buttonCreateNew");
            this.buttonCreateNew.Name = "buttonCreateNew";
            this.buttonCreateNew.UseVisualStyleBackColor = true;
            this.buttonCreateNew.Click += new System.EventHandler(this.buttonCreateNew_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Image = global::MLifter.Controls.Properties.Resources.listRemove;
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Image = global::MLifter.Controls.Properties.Resources.import;
            resources.ApplyResources(this.buttonImport, "buttonImport");
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Image = global::MLifter.Controls.Properties.Resources.export;
            resources.ApplyResources(this.buttonExport, "buttonExport");
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonImport);
            this.groupBox1.Controls.Add(this.buttonExport);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonCreateNew);
            this.groupBox2.Controls.Add(this.buttonDelete);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // LMSelectionPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listViewLMs);
            this.Controls.Add(this.labelConnectionString);
            resources.ApplyResources(this, "$this");
            this.Name = "LMSelectionPage";
            this.TopImage = global::MLifter.Controls.Properties.Resources.banner;
            this.Controls.SetChildIndex(this.labelConnectionString, 0);
            this.Controls.SetChildIndex(this.listViewLMs, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelConnectionString;
        private System.Windows.Forms.ListView listViewLMs;
        private System.Windows.Forms.ColumnHeader columnHeaderTitle;
        private System.Windows.Forms.ColumnHeader columnHeaderAuthor;
        private System.Windows.Forms.Button buttonCreateNew;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
