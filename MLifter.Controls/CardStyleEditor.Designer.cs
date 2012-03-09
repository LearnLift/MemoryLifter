namespace MLifter.Controls
{
    partial class CardStyleEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardStyleEditor));
			this.buttonClose = new System.Windows.Forms.Button();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonClearStyles = new System.Windows.Forms.Button();
			this.MainHelp = new System.Windows.Forms.HelpProvider();
			this.cardStyleEdit = new MLifter.Controls.CardStyleEdit();
			this.SuspendLayout();
			// 
			// buttonClose
			// 
			resources.ApplyResources(this.buttonClose, "buttonClose");
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// buttonSave
			// 
			resources.ApplyResources(this.buttonSave, "buttonSave");
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonClearStyles
			// 
			resources.ApplyResources(this.buttonClearStyles, "buttonClearStyles");
			this.buttonClearStyles.Name = "buttonClearStyles";
			this.buttonClearStyles.UseVisualStyleBackColor = true;
			this.buttonClearStyles.Click += new System.EventHandler(this.buttonClearStyles_Click);
			// 
			// cardStyleEdit
			// 
			resources.ApplyResources(this.cardStyleEdit, "cardStyleEdit");
			this.cardStyleEdit.Name = "cardStyleEdit";
			this.MainHelp.SetShowHelp(this.cardStyleEdit, ((bool)(resources.GetObject("cardStyleEdit.ShowHelp"))));
			// 
			// CardStyleEditor
			// 
			this.AcceptButton = this.buttonSave;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.CancelButton = this.buttonClose;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.buttonClearStyles);
			this.Controls.Add(this.cardStyleEdit);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
			this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CardStyleEditor";
			this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
			this.ShowInTaskbar = false;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CardStyleEditor_FormClosing);
			this.Resize += new System.EventHandler(this.CardStyleEditor_Resize);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonSave;
        private CardStyleEdit cardStyleEdit;
        private System.Windows.Forms.Button buttonClearStyles;
        private System.Windows.Forms.HelpProvider MainHelp;
    }
}