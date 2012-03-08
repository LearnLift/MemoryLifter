namespace MLifter.Controls
{
    partial class DictionaryProperties
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryProperties));
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.textBoxLocation = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxAuthor = new System.Windows.Forms.TextBox();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.comboBoxCategory = new System.Windows.Forms.ComboBox();
			this.textBoxTitle = new System.Windows.Forms.TextBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonBrowse
			// 
			resources.ApplyResources(this.buttonBrowse, "buttonBrowse");
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.UseVisualStyleBackColor = true;
			this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
			// 
			// textBoxLocation
			// 
			resources.ApplyResources(this.textBoxLocation, "textBoxLocation");
			this.textBoxLocation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.textBoxLocation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.textBoxLocation.Name = "textBoxLocation";
			this.textBoxLocation.ReadOnly = true;
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// textBoxAuthor
			// 
			resources.ApplyResources(this.textBoxAuthor, "textBoxAuthor");
			this.textBoxAuthor.Name = "textBoxAuthor";
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.AcceptsReturn = true;
			resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
			this.textBoxDescription.Name = "textBoxDescription";
			// 
			// comboBoxCategory
			// 
			resources.ApplyResources(this.comboBoxCategory, "comboBoxCategory");
			this.comboBoxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCategory.FormattingEnabled = true;
			this.comboBoxCategory.Name = "comboBoxCategory";
			// 
			// textBoxTitle
			// 
			resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
			this.textBoxTitle.Name = "textBoxTitle";
			this.textBoxTitle.Enter += new System.EventHandler(this.textBoxTitle_Enter);
			this.textBoxTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textBoxTitle_MouseUp);
			// 
			// labelDescription
			// 
			resources.ApplyResources(this.labelDescription, "labelDescription");
			this.labelDescription.Name = "labelDescription";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// DictionaryProperties
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.buttonBrowse);
			this.Controls.Add(this.textBoxLocation);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxAuthor);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this.comboBoxCategory);
			this.Controls.Add(this.textBoxTitle);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "DictionaryProperties";
			resources.ApplyResources(this, "$this");
			this.Load += new System.EventHandler(this.DictionaryProperties_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxLocation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAuthor;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
