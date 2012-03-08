// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace SyncApplication
{
    partial class ConflictForm
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
            this.groupBoxRemoteChange = new System.Windows.Forms.GroupBox();
            this.dataGridServerChange = new System.Windows.Forms.DataGridView();
            this.groupBoxLocalChange = new System.Windows.Forms.GroupBox();
            this.dataGridClientChange = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxConflictType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxError = new System.Windows.Forms.TextBox();
            this.buttonRetry = new System.Windows.Forms.Button();
            this.buttonForceWrite = new System.Windows.Forms.Button();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSyncStage = new System.Windows.Forms.TextBox();
            this.richTextBoxHelp = new System.Windows.Forms.RichTextBox();
            this.buttonRetryNextSync = new System.Windows.Forms.Button();
            this.groupBoxRemoteChange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridServerChange)).BeginInit();
            this.groupBoxLocalChange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridClientChange)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxRemoteChange
            // 
            this.groupBoxRemoteChange.Controls.Add(this.dataGridServerChange);
            this.groupBoxRemoteChange.Location = new System.Drawing.Point(12, 123);
            this.groupBoxRemoteChange.Name = "groupBoxRemoteChange";
            this.groupBoxRemoteChange.Size = new System.Drawing.Size(679, 145);
            this.groupBoxRemoteChange.TabIndex = 0;
            this.groupBoxRemoteChange.TabStop = false;
            this.groupBoxRemoteChange.Text = "Remote Change";
            // 
            // dataGridServerChange
            // 
            this.dataGridServerChange.AllowUserToAddRows = false;
            this.dataGridServerChange.AllowUserToDeleteRows = false;
            this.dataGridServerChange.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridServerChange.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridServerChange.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridServerChange.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dataGridServerChange.Location = new System.Drawing.Point(21, 30);
            this.dataGridServerChange.Name = "dataGridServerChange";
            this.dataGridServerChange.Size = new System.Drawing.Size(636, 97);
            this.dataGridServerChange.TabIndex = 1;
            // 
            // groupBoxLocalChange
            // 
            this.groupBoxLocalChange.Controls.Add(this.dataGridClientChange);
            this.groupBoxLocalChange.Location = new System.Drawing.Point(12, 284);
            this.groupBoxLocalChange.Name = "groupBoxLocalChange";
            this.groupBoxLocalChange.Size = new System.Drawing.Size(679, 145);
            this.groupBoxLocalChange.TabIndex = 1;
            this.groupBoxLocalChange.TabStop = false;
            this.groupBoxLocalChange.Text = "Local Change";
            // 
            // dataGridClientChange
            // 
            this.dataGridClientChange.AllowUserToAddRows = false;
            this.dataGridClientChange.AllowUserToDeleteRows = false;
            this.dataGridClientChange.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridClientChange.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridClientChange.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridClientChange.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dataGridClientChange.Location = new System.Drawing.Point(21, 29);
            this.dataGridClientChange.Name = "dataGridClientChange";
            this.dataGridClientChange.Size = new System.Drawing.Size(636, 97);
            this.dataGridClientChange.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Conflict Type";
            // 
            // textBoxConflictType
            // 
            this.textBoxConflictType.Location = new System.Drawing.Point(98, 36);
            this.textBoxConflictType.Name = "textBoxConflictType";
            this.textBoxConflictType.ReadOnly = true;
            this.textBoxConflictType.Size = new System.Drawing.Size(229, 20);
            this.textBoxConflictType.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Error Message";
            // 
            // textBoxError
            // 
            this.textBoxError.Location = new System.Drawing.Point(98, 67);
            this.textBoxError.Multiline = true;
            this.textBoxError.Name = "textBoxError";
            this.textBoxError.ReadOnly = true;
            this.textBoxError.Size = new System.Drawing.Size(593, 48);
            this.textBoxError.TabIndex = 5;
            // 
            // buttonRetry
            // 
            this.buttonRetry.Location = new System.Drawing.Point(503, 534);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(188, 23);
            this.buttonRetry.TabIndex = 6;
            this.buttonRetry.Text = "Retr&y";
            this.buttonRetry.UseVisualStyleBackColor = true;
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            // 
            // buttonForceWrite
            // 
            this.buttonForceWrite.Location = new System.Drawing.Point(503, 473);
            this.buttonForceWrite.Name = "buttonForceWrite";
            this.buttonForceWrite.Size = new System.Drawing.Size(188, 23);
            this.buttonForceWrite.TabIndex = 7;
            this.buttonForceWrite.Text = "&Force Write [Remote Wins]";
            this.buttonForceWrite.UseVisualStyleBackColor = true;
            this.buttonForceWrite.Click += new System.EventHandler(this.buttonForceWrite_Click);
            // 
            // buttonContinue
            // 
            this.buttonContinue.Location = new System.Drawing.Point(503, 443);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(188, 23);
            this.buttonContinue.TabIndex = 8;
            this.buttonContinue.Text = "&Continue [Local Wins]";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Sync Stage";
            // 
            // textBoxSyncStage
            // 
            this.textBoxSyncStage.Location = new System.Drawing.Point(98, 6);
            this.textBoxSyncStage.Name = "textBoxSyncStage";
            this.textBoxSyncStage.ReadOnly = true;
            this.textBoxSyncStage.Size = new System.Drawing.Size(229, 20);
            this.textBoxSyncStage.TabIndex = 11;
            // 
            // richTextBoxHelp
            // 
            this.richTextBoxHelp.Location = new System.Drawing.Point(12, 445);
            this.richTextBoxHelp.Name = "richTextBoxHelp";
            this.richTextBoxHelp.ReadOnly = true;
            this.richTextBoxHelp.Size = new System.Drawing.Size(475, 112);
            this.richTextBoxHelp.TabIndex = 12;
            this.richTextBoxHelp.Text = "";
            // 
            // buttonRetryNextSync
            // 
            this.buttonRetryNextSync.Location = new System.Drawing.Point(503, 504);
            this.buttonRetryNextSync.Name = "buttonRetryNextSync";
            this.buttonRetryNextSync.Size = new System.Drawing.Size(188, 23);
            this.buttonRetryNextSync.TabIndex = 13;
            this.buttonRetryNextSync.Text = "&Retry Next Sync";
            this.buttonRetryNextSync.UseVisualStyleBackColor = true;
            this.buttonRetryNextSync.Click += new System.EventHandler(this.buttonRetryNextSync_Click);
            // 
            // ConflictForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 569);
            this.Controls.Add(this.buttonRetryNextSync);
            this.Controls.Add(this.richTextBoxHelp);
            this.Controls.Add(this.textBoxSyncStage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.buttonForceWrite);
            this.Controls.Add(this.buttonRetry);
            this.Controls.Add(this.textBoxError);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxConflictType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBoxLocalChange);
            this.Controls.Add(this.groupBoxRemoteChange);
            this.Name = "ConflictForm";
            this.groupBoxRemoteChange.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridServerChange)).EndInit();
            this.groupBoxLocalChange.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridClientChange)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxRemoteChange;
        private System.Windows.Forms.GroupBox groupBoxLocalChange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxConflictType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxError;
        private System.Windows.Forms.DataGridView dataGridServerChange;
        private System.Windows.Forms.DataGridView dataGridClientChange;
        private System.Windows.Forms.Button buttonRetry;
        private System.Windows.Forms.Button buttonForceWrite;
        private System.Windows.Forms.Button buttonContinue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSyncStage;
        private System.Windows.Forms.RichTextBox richTextBoxHelp;
        private System.Windows.Forms.Button buttonRetryNextSync;
    }
}
