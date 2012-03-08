namespace MLifter.Controls.Wizards.Startup
{
    partial class RegisterPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterPage));
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelMail = new System.Windows.Forms.Label();
            this.textBoxMail = new System.Windows.Forms.TextBox();
            this.checkBoxNewsletter = new System.Windows.Forms.CheckBox();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.labelComment = new System.Windows.Forms.Label();
            this.labelRegister = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            // 
            // labelMail
            // 
            resources.ApplyResources(this.labelMail, "labelMail");
            this.labelMail.Name = "labelMail";
            // 
            // textBoxMail
            // 
            resources.ApplyResources(this.textBoxMail, "textBoxMail");
            this.textBoxMail.Name = "textBoxMail";
            // 
            // checkBoxNewsletter
            // 
            this.checkBoxNewsletter.Checked = true;
            this.checkBoxNewsletter.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.checkBoxNewsletter, "checkBoxNewsletter");
            this.checkBoxNewsletter.Name = "checkBoxNewsletter";
            this.checkBoxNewsletter.UseVisualStyleBackColor = true;
            // 
            // textBoxComment
            // 
            this.textBoxComment.AcceptsReturn = true;
            resources.ApplyResources(this.textBoxComment, "textBoxComment");
            this.textBoxComment.Name = "textBoxComment";
            // 
            // labelComment
            // 
            resources.ApplyResources(this.labelComment, "labelComment");
            this.labelComment.Name = "labelComment";
            // 
            // labelRegister
            // 
            resources.ApplyResources(this.labelRegister, "labelRegister");
            this.labelRegister.Name = "labelRegister";
            // 
            // RegisterPage
            // 
            this.Controls.Add(this.labelRegister);
            this.Controls.Add(this.textBoxComment);
            this.Controls.Add(this.checkBoxNewsletter);
            this.Controls.Add(this.textBoxMail);
            this.Controls.Add(this.labelMail);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.labelComment);
            resources.ApplyResources(this, "$this");
            this.HelpAvailable = true;
            this.LeftImage = global::MLifter.Controls.Properties.Resources.setup;
            this.Name = "RegisterPage";
            this.Load += new System.EventHandler(this.RegisterPage_Load);
            this.Controls.SetChildIndex(this.labelComment, 0);
            this.Controls.SetChildIndex(this.textBoxName, 0);
            this.Controls.SetChildIndex(this.labelName, 0);
            this.Controls.SetChildIndex(this.labelMail, 0);
            this.Controls.SetChildIndex(this.textBoxMail, 0);
            this.Controls.SetChildIndex(this.checkBoxNewsletter, 0);
            this.Controls.SetChildIndex(this.textBoxComment, 0);
            this.Controls.SetChildIndex(this.labelRegister, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelMail;
        private System.Windows.Forms.TextBox textBoxMail;
        private System.Windows.Forms.CheckBox checkBoxNewsletter;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.Label labelComment;
        private System.Windows.Forms.Label labelRegister;
    }
}
