namespace StickFactory
{
    partial class DriveStatusControl
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
            this.labelDrive = new System.Windows.Forms.Label();
            this.checkBoxFormat = new System.Windows.Forms.CheckBox();
            this.checkBoxContent = new System.Windows.Forms.CheckBox();
            this.checkBoxStickID = new System.Windows.Forms.CheckBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressBarStatus = new System.Windows.Forms.ProgressBar();
            this.labelTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelDrive
            // 
            this.labelDrive.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDrive.Location = new System.Drawing.Point(-7, -5);
            this.labelDrive.Name = "labelDrive";
            this.labelDrive.Size = new System.Drawing.Size(64, 54);
            this.labelDrive.TabIndex = 0;
            this.labelDrive.Text = "C:";
            this.labelDrive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBoxFormat
            // 
            this.checkBoxFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFormat.Enabled = false;
            this.checkBoxFormat.Location = new System.Drawing.Point(48, 0);
            this.checkBoxFormat.Name = "checkBoxFormat";
            this.checkBoxFormat.Size = new System.Drawing.Size(132, 20);
            this.checkBoxFormat.TabIndex = 1;
            this.checkBoxFormat.Text = "Formatting finish";
            this.checkBoxFormat.UseVisualStyleBackColor = true;
            // 
            // checkBoxContent
            // 
            this.checkBoxContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxContent.Enabled = false;
            this.checkBoxContent.Location = new System.Drawing.Point(48, 17);
            this.checkBoxContent.Name = "checkBoxContent";
            this.checkBoxContent.Size = new System.Drawing.Size(132, 17);
            this.checkBoxContent.TabIndex = 2;
            this.checkBoxContent.Text = "Content written";
            this.checkBoxContent.UseVisualStyleBackColor = true;
            // 
            // checkBoxStickID
            // 
            this.checkBoxStickID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxStickID.Enabled = false;
            this.checkBoxStickID.Location = new System.Drawing.Point(48, 32);
            this.checkBoxStickID.Name = "checkBoxStickID";
            this.checkBoxStickID.Size = new System.Drawing.Size(132, 17);
            this.checkBoxStickID.TabIndex = 3;
            this.checkBoxStickID.Text = "ID set";
            this.checkBoxStickID.UseVisualStyleBackColor = true;
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.Location = new System.Drawing.Point(3, 46);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(174, 18);
            this.labelStatus.TabIndex = 4;
            this.labelStatus.Text = "Waiting for Stick...";
            // 
            // progressBarStatus
            // 
            this.progressBarStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarStatus.Location = new System.Drawing.Point(3, 61);
            this.progressBarStatus.Name = "progressBarStatus";
            this.progressBarStatus.Size = new System.Drawing.Size(174, 24);
            this.progressBarStatus.Step = 1;
            this.progressBarStatus.TabIndex = 5;
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTime.Location = new System.Drawing.Point(105, 32);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(75, 14);
            this.labelTime.TabIndex = 6;
            this.labelTime.Text = "~MM:SS left";
            this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelTime.Visible = false;
            // 
            // DriveStatusControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.progressBarStatus);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.checkBoxStickID);
            this.Controls.Add(this.checkBoxContent);
            this.Controls.Add(this.checkBoxFormat);
            this.Controls.Add(this.labelDrive);
            this.Name = "DriveStatusControl";
            this.Size = new System.Drawing.Size(180, 88);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelDrive;
        private System.Windows.Forms.CheckBox checkBoxFormat;
        private System.Windows.Forms.CheckBox checkBoxContent;
        private System.Windows.Forms.CheckBox checkBoxStickID;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ProgressBar progressBarStatus;
        private System.Windows.Forms.Label labelTime;
    }
}
