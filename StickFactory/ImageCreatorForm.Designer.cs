/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
namespace StickFactory
{
    partial class ImageCreatorForm
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
            this.groupBoxInput = new System.Windows.Forms.GroupBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxInputFilePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.textBoxStickID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxStickName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxOutPut = new System.Windows.Forms.GroupBox();
            this.buttonBrowseOutput = new System.Windows.Forms.Button();
            this.textBoxOutputFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.statusStripStatus = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBarStatus = new System.Windows.Forms.ToolStripProgressBar();
            this.groupBoxInput.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.groupBoxOutPut.SuspendLayout();
            this.statusStripStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxInput
            // 
            this.groupBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInput.Controls.Add(this.buttonBrowse);
            this.groupBoxInput.Controls.Add(this.textBoxInputFilePath);
            this.groupBoxInput.Controls.Add(this.label1);
            this.groupBoxInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxInput.Location = new System.Drawing.Point(12, 12);
            this.groupBoxInput.Name = "groupBoxInput";
            this.groupBoxInput.Size = new System.Drawing.Size(465, 79);
            this.groupBoxInput.TabIndex = 1;
            this.groupBoxInput.TabStop = false;
            this.groupBoxInput.Text = "Input";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowse.Location = new System.Drawing.Point(359, 43);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(100, 27);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxInputFilePath
            // 
            this.textBoxInputFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInputFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxInputFilePath.Location = new System.Drawing.Point(130, 15);
            this.textBoxInputFilePath.Name = "textBoxInputFilePath";
            this.textBoxInputFilePath.Size = new System.Drawing.Size(329, 22);
            this.textBoxInputFilePath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a folder:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.textBoxStickID);
            this.groupBoxSettings.Controls.Add(this.label3);
            this.groupBoxSettings.Controls.Add(this.textBoxStickName);
            this.groupBoxSettings.Controls.Add(this.label2);
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 97);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(465, 79);
            this.groupBoxSettings.TabIndex = 2;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Image Settings";
            // 
            // textBoxStickID
            // 
            this.textBoxStickID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStickID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStickID.Location = new System.Drawing.Point(130, 47);
            this.textBoxStickID.Name = "textBoxStickID";
            this.textBoxStickID.Size = new System.Drawing.Size(329, 22);
            this.textBoxStickID.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Stick ID:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxStickName
            // 
            this.textBoxStickName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStickName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxStickName.Location = new System.Drawing.Point(130, 19);
            this.textBoxStickName.Name = "textBoxStickName";
            this.textBoxStickName.Size = new System.Drawing.Size(329, 22);
            this.textBoxStickName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Stick Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonCreate
            // 
            this.buttonCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCreate.Location = new System.Drawing.Point(12, 267);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(176, 23);
            this.buttonCreate.TabIndex = 3;
            this.buttonCreate.Text = "Create Image...";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(301, 267);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(176, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBoxOutPut
            // 
            this.groupBoxOutPut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOutPut.Controls.Add(this.buttonBrowseOutput);
            this.groupBoxOutPut.Controls.Add(this.textBoxOutputFile);
            this.groupBoxOutPut.Controls.Add(this.label4);
            this.groupBoxOutPut.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxOutPut.Location = new System.Drawing.Point(12, 182);
            this.groupBoxOutPut.Name = "groupBoxOutPut";
            this.groupBoxOutPut.Size = new System.Drawing.Size(465, 79);
            this.groupBoxOutPut.TabIndex = 3;
            this.groupBoxOutPut.TabStop = false;
            this.groupBoxOutPut.Text = "Output";
            // 
            // buttonBrowseOutput
            // 
            this.buttonBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowseOutput.Location = new System.Drawing.Point(359, 43);
            this.buttonBrowseOutput.Name = "buttonBrowseOutput";
            this.buttonBrowseOutput.Size = new System.Drawing.Size(100, 27);
            this.buttonBrowseOutput.TabIndex = 2;
            this.buttonBrowseOutput.Text = "Browse...";
            this.buttonBrowseOutput.UseVisualStyleBackColor = true;
            this.buttonBrowseOutput.Click += new System.EventHandler(this.buttonBrowseOutput_Click);
            // 
            // textBoxOutputFile
            // 
            this.textBoxOutputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutputFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOutputFile.Location = new System.Drawing.Point(9, 45);
            this.textBoxOutputFile.Name = "textBoxOutputFile";
            this.textBoxOutputFile.Size = new System.Drawing.Size(344, 22);
            this.textBoxOutputFile.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(249, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Select a file to store the image:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStripStatus
            // 
            this.statusStripStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelMessage,
            this.toolStripProgressBarStatus});
            this.statusStripStatus.Location = new System.Drawing.Point(0, 299);
            this.statusStripStatus.Name = "statusStripStatus";
            this.statusStripStatus.Size = new System.Drawing.Size(489, 22);
            this.statusStripStatus.TabIndex = 5;
            // 
            // toolStripStatusLabelMessage
            // 
            this.toolStripStatusLabelMessage.AutoSize = false;
            this.toolStripStatusLabelMessage.Name = "toolStripStatusLabelMessage";
            this.toolStripStatusLabelMessage.Size = new System.Drawing.Size(350, 17);
            this.toolStripStatusLabelMessage.Text = "Ready...";
            this.toolStripStatusLabelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBarStatus
            // 
            this.toolStripProgressBarStatus.Name = "toolStripProgressBarStatus";
            this.toolStripProgressBarStatus.Size = new System.Drawing.Size(100, 16);
            // 
            // ImageCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 321);
            this.Controls.Add(this.statusStripStatus);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.groupBoxInput);
            this.Controls.Add(this.groupBoxOutPut);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImageCreatorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create new Stick image...";
            this.groupBoxInput.ResumeLayout(false);
            this.groupBoxInput.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.groupBoxOutPut.ResumeLayout(false);
            this.groupBoxOutPut.PerformLayout();
            this.statusStripStatus.ResumeLayout(false);
            this.statusStripStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxInput;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxInputFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.TextBox textBoxStickID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxStickName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxOutPut;
        private System.Windows.Forms.Button buttonBrowseOutput;
        private System.Windows.Forms.TextBox textBoxOutputFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.StatusStrip statusStripStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMessage;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarStatus;

    }
}
