namespace MLifter.ImportExport
{
    partial class ExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportForm));
            this.cmbFileFormat = new System.Windows.Forms.ComboBox();
            this.lblselfileformat = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ExportCardLabel = new System.Windows.Forms.Label();
            this.ExportChapterLabel = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.exportProgressBar = new MLifter.Components.ColorProgressBar();
            this.exportFieldsFrm = new MLifter.Controls.FieldsFrame();
            this.exportChapterFrm = new MLifter.Controls.ChapterFrame();
            this.mainHelp = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // cmbFileFormat
            // 
            this.cmbFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFileFormat.FormattingEnabled = true;
            resources.ApplyResources(this.cmbFileFormat, "cmbFileFormat");
            this.cmbFileFormat.Name = "cmbFileFormat";
            // 
            // lblselfileformat
            // 
            resources.ApplyResources(this.lblselfileformat, "lblselfileformat");
            this.lblselfileformat.Name = "lblselfileformat";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ExportCardLabel
            // 
            resources.ApplyResources(this.ExportCardLabel, "ExportCardLabel");
            this.ExportCardLabel.Name = "ExportCardLabel";
            // 
            // ExportChapterLabel
            // 
            resources.ApplyResources(this.ExportChapterLabel, "ExportChapterLabel");
            this.ExportChapterLabel.Name = "ExportChapterLabel";
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.Name = "btnExport";
            this.btnExport.TabStop = false;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // exportProgressBar
            // 
            this.exportProgressBar.BarColor = System.Drawing.Color.SteelBlue;
            this.exportProgressBar.BorderColor = System.Drawing.Color.Black;
            this.exportProgressBar.FillStyle = MLifter.Components.ColorProgressBar.FillStyles.Solid;
            resources.ApplyResources(this.exportProgressBar, "exportProgressBar");
            this.exportProgressBar.Maximum = 100;
            this.exportProgressBar.Minimum = 0;
            this.exportProgressBar.Name = "exportProgressBar";
            this.exportProgressBar.Step = 1;
            this.exportProgressBar.Value = 0;
            // 
            // exportFieldsFrm
            // 
            resources.ApplyResources(this.exportFieldsFrm, "exportFieldsFrm");
            this.exportFieldsFrm.Name = "exportFieldsFrm";
            // 
            // exportChapterFrm
            // 
            resources.ApplyResources(this.exportChapterFrm, "exportChapterFrm");
            this.exportChapterFrm.Name = "exportChapterFrm";
            // 
            // ExportForm
            // 
            this.AcceptButton = this.btnExport;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.exportChapterFrm);
            this.Controls.Add(this.cmbFileFormat);
            this.Controls.Add(this.lblselfileformat);
            this.Controls.Add(this.exportProgressBar);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.ExportCardLabel);
            this.Controls.Add(this.ExportChapterLabel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.exportFieldsFrm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.mainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.mainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportForm";
            this.mainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ExportForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbFileFormat;
        private System.Windows.Forms.Label lblselfileformat;
        private Components.ColorProgressBar exportProgressBar;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label ExportCardLabel;
        private System.Windows.Forms.Label ExportChapterLabel;
        private System.Windows.Forms.Button btnExport;
        private MLifter.Controls.FieldsFrame exportFieldsFrm;
        private MLifter.Controls.ChapterFrame exportChapterFrm;
        private System.Windows.Forms.HelpProvider mainHelp;
    }
}