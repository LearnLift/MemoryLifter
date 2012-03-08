namespace MLifter.AudioTools
{
    partial class LayoutSelectorForm
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
            this.pictureBoxNumPad = new System.Windows.Forms.PictureBox();
            this.pictureBoxKeyboard = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonNumPad = new System.Windows.Forms.RadioButton();
            this.radioButtonKeyboard = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkBoxAskAgain = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNumPad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxKeyboard)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxNumPad
            // 
            this.pictureBoxNumPad.Image = global::MLifter.AudioTools.Properties.Resources.numpad_layout;
            this.pictureBoxNumPad.Location = new System.Drawing.Point(12, 51);
            this.pictureBoxNumPad.Name = "pictureBoxNumPad";
            this.pictureBoxNumPad.Size = new System.Drawing.Size(250, 295);
            this.pictureBoxNumPad.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxNumPad.TabIndex = 0;
            this.pictureBoxNumPad.TabStop = false;
            // 
            // pictureBoxKeyboard
            // 
            this.pictureBoxKeyboard.Image = global::MLifter.AudioTools.Properties.Resources.keyboard_layout;
            this.pictureBoxKeyboard.Location = new System.Drawing.Point(268, 184);
            this.pictureBoxKeyboard.Name = "pictureBoxKeyboard";
            this.pictureBoxKeyboard.Size = new System.Drawing.Size(533, 162);
            this.pictureBoxKeyboard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxKeyboard.TabIndex = 1;
            this.pictureBoxKeyboard.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(517, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Which Layout would you like to use? Please choose your favorite...";
            // 
            // radioButtonNumPad
            // 
            this.radioButtonNumPad.AutoSize = true;
            this.radioButtonNumPad.Checked = true;
            this.radioButtonNumPad.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonNumPad.Location = new System.Drawing.Point(74, 352);
            this.radioButtonNumPad.Name = "radioButtonNumPad";
            this.radioButtonNumPad.Size = new System.Drawing.Size(127, 20);
            this.radioButtonNumPad.TabIndex = 3;
            this.radioButtonNumPad.TabStop = true;
            this.radioButtonNumPad.Text = "NumPad-Layout";
            this.radioButtonNumPad.UseVisualStyleBackColor = true;
            // 
            // radioButtonKeyboard
            // 
            this.radioButtonKeyboard.AutoSize = true;
            this.radioButtonKeyboard.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonKeyboard.Location = new System.Drawing.Point(467, 352);
            this.radioButtonKeyboard.Name = "radioButtonKeyboard";
            this.radioButtonKeyboard.Size = new System.Drawing.Size(134, 20);
            this.radioButtonKeyboard.TabIndex = 4;
            this.radioButtonKeyboard.Text = "Keyboard-Layout";
            this.radioButtonKeyboard.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(726, 430);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // checkBoxAskAgain
            // 
            this.checkBoxAskAgain.AutoSize = true;
            this.checkBoxAskAgain.Location = new System.Drawing.Point(12, 436);
            this.checkBoxAskAgain.Name = "checkBoxAskAgain";
            this.checkBoxAskAgain.Size = new System.Drawing.Size(100, 17);
            this.checkBoxAskAgain.TabIndex = 6;
            this.checkBoxAskAgain.Text = "Don\'t ask again";
            this.checkBoxAskAgain.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 396);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(608, 19);
            this.label2.TabIndex = 7;
            this.label2.Text = "Use TAB, BACKSPACE or NUM to switch between the expert and simple mode.";
            // 
            // LayoutSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 465);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxAskAgain);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.radioButtonKeyboard);
            this.Controls.Add(this.radioButtonNumPad);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxKeyboard);
            this.Controls.Add(this.pictureBoxNumPad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LayoutSelectorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Layout";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNumPad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxKeyboard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxNumPad;
        private System.Windows.Forms.PictureBox pictureBoxKeyboard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonNumPad;
        private System.Windows.Forms.RadioButton radioButtonKeyboard;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkBoxAskAgain;
        private System.Windows.Forms.Label label2;
    }
}