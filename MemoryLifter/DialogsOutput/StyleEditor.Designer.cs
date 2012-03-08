namespace MLifter
{
    partial class StyleEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StyleEditor));
			this.btnSave = new System.Windows.Forms.Button();
			this.selectStylesheet = new System.Windows.Forms.ComboBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.rbCorrect = new System.Windows.Forms.RadioButton();
			this.rbWrong = new System.Windows.Forms.RadioButton();
			this.radioButtonQuestion = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.xslStyleEdit = new MLifter.Controls.XSLStyleEdit();
			this.MainHelp = new System.Windows.Forms.HelpProvider();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSave
			// 
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.Name = "btnSave";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnRestore_Click);
			// 
			// selectStylesheet
			// 
			resources.ApplyResources(this.selectStylesheet, "selectStylesheet");
			this.selectStylesheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.selectStylesheet.FormattingEnabled = true;
			this.selectStylesheet.Name = "selectStylesheet";
			this.selectStylesheet.SelectedIndexChanged += new System.EventHandler(this.selectStylesheet_SelectedIndexChanged);
			// 
			// btnClose
			// 
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Name = "btnClose";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// rbCorrect
			// 
			resources.ApplyResources(this.rbCorrect, "rbCorrect");
			this.rbCorrect.Name = "rbCorrect";
			this.rbCorrect.UseVisualStyleBackColor = true;
			this.rbCorrect.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// rbWrong
			// 
			resources.ApplyResources(this.rbWrong, "rbWrong");
			this.rbWrong.Name = "rbWrong";
			this.rbWrong.UseVisualStyleBackColor = true;
			// 
			// radioButtonQuestion
			// 
			resources.ApplyResources(this.radioButtonQuestion, "radioButtonQuestion");
			this.radioButtonQuestion.Checked = true;
			this.radioButtonQuestion.Name = "radioButtonQuestion";
			this.radioButtonQuestion.TabStop = true;
			this.radioButtonQuestion.UseVisualStyleBackColor = true;
			this.radioButtonQuestion.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonQuestion);
			this.groupBox1.Controls.Add(this.rbWrong);
			this.groupBox1.Controls.Add(this.rbCorrect);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// xslStyleEdit
			// 
			resources.ApplyResources(this.xslStyleEdit, "xslStyleEdit");
			this.xslStyleEdit.Name = "xslStyleEdit";
			// 
			// StyleEditor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.CancelButton = this.btnClose;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.xslStyleEdit);
			this.Controls.Add(this.selectStylesheet);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
			this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StyleEditor";
			this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
			this.ShowInTaskbar = false;
			this.Shown += new System.EventHandler(this.StyleEditor_Shown);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.XSLPreview_FormClosed);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox selectStylesheet;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RadioButton rbCorrect;
        private System.Windows.Forms.RadioButton rbWrong;
        private System.Windows.Forms.RadioButton radioButtonQuestion;
        private MLifter.Controls.XSLStyleEdit xslStyleEdit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.HelpProvider MainHelp;
    }
}