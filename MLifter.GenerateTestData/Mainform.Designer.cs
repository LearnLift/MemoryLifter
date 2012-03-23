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
namespace MLifter.GenerateTestData
{
    partial class Mainform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.groupBoxCopyLM = new System.Windows.Forms.GroupBox();
            this.buttonSource = new System.Windows.Forms.Button();
            this.labelSourceLM = new System.Windows.Forms.Label();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.textBoxConnectionString = new System.Windows.Forms.TextBox();
            this.listViewLMs = new System.Windows.Forms.ListView();
            this.columnHeaderTitle = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderAuthor = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderNumber = new System.Windows.Forms.ColumnHeader();
            this.checkBoxCopyLM = new System.Windows.Forms.CheckBox();
            this.labelCurrentUser = new System.Windows.Forms.Label();
            this.textBoxCurrentUser = new System.Windows.Forms.TextBox();
            this.buttonChangeUser = new System.Windows.Forms.Button();
            this.textBoxSessionNum = new System.Windows.Forms.TextBox();
            this.labelSessionNum = new System.Windows.Forms.Label();
            this.textBoxCardNum = new System.Windows.Forms.TextBox();
            this.labelCardNum = new System.Windows.Forms.Label();
            this.groupBoxDB = new System.Windows.Forms.GroupBox();
            this.groupBoxCopyLM.SuspendLayout();
            this.groupBoxDB.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCopyLM
            // 
            resources.ApplyResources(this.groupBoxCopyLM, "groupBoxCopyLM");
            this.groupBoxCopyLM.Controls.Add(this.buttonSource);
            this.groupBoxCopyLM.Controls.Add(this.labelSourceLM);
            this.groupBoxCopyLM.Controls.Add(this.textBoxSource);
            this.groupBoxCopyLM.Name = "groupBoxCopyLM";
            this.groupBoxCopyLM.TabStop = false;
            // 
            // buttonSource
            // 
            resources.ApplyResources(this.buttonSource, "buttonSource");
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.buttonSource_Click);
            // 
            // labelSourceLM
            // 
            resources.ApplyResources(this.labelSourceLM, "labelSourceLM");
            this.labelSourceLM.Name = "labelSourceLM";
            // 
            // textBoxSource
            // 
            resources.ApplyResources(this.textBoxSource, "textBoxSource");
            this.textBoxSource.Name = "textBoxSource";
            // 
            // buttonGenerate
            // 
            resources.ApplyResources(this.buttonGenerate, "buttonGenerate");
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // textBoxConnectionString
            // 
            resources.ApplyResources(this.textBoxConnectionString, "textBoxConnectionString");
            this.textBoxConnectionString.Name = "textBoxConnectionString";
            this.textBoxConnectionString.TextChanged += new System.EventHandler(this.textBoxConnectionString_TextChanged);
            // 
            // listViewLMs
            // 
            resources.ApplyResources(this.listViewLMs, "listViewLMs");
            this.listViewLMs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTitle,
            this.columnHeaderAuthor,
            this.columnHeaderNumber});
            this.listViewLMs.GridLines = true;
            this.listViewLMs.HideSelection = false;
            this.listViewLMs.HoverSelection = true;
            this.listViewLMs.MultiSelect = false;
            this.listViewLMs.Name = "listViewLMs";
            this.listViewLMs.UseCompatibleStateImageBehavior = false;
            this.listViewLMs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderTitle
            // 
            resources.ApplyResources(this.columnHeaderTitle, "columnHeaderTitle");
            // 
            // columnHeaderAuthor
            // 
            resources.ApplyResources(this.columnHeaderAuthor, "columnHeaderAuthor");
            // 
            // columnHeaderNumber
            // 
            resources.ApplyResources(this.columnHeaderNumber, "columnHeaderNumber");
            // 
            // checkBoxCopyLM
            // 
            resources.ApplyResources(this.checkBoxCopyLM, "checkBoxCopyLM");
            this.checkBoxCopyLM.Name = "checkBoxCopyLM";
            this.checkBoxCopyLM.UseVisualStyleBackColor = true;
            this.checkBoxCopyLM.CheckedChanged += new System.EventHandler(this.checkBoxCopyLM_CheckedChanged);
            // 
            // labelCurrentUser
            // 
            resources.ApplyResources(this.labelCurrentUser, "labelCurrentUser");
            this.labelCurrentUser.Name = "labelCurrentUser";
            // 
            // textBoxCurrentUser
            // 
            resources.ApplyResources(this.textBoxCurrentUser, "textBoxCurrentUser");
            this.textBoxCurrentUser.Name = "textBoxCurrentUser";
            this.textBoxCurrentUser.ReadOnly = true;
            // 
            // buttonChangeUser
            // 
            resources.ApplyResources(this.buttonChangeUser, "buttonChangeUser");
            this.buttonChangeUser.Name = "buttonChangeUser";
            this.buttonChangeUser.UseVisualStyleBackColor = true;
            this.buttonChangeUser.Click += new System.EventHandler(this.buttonChangeUser_Click);
            // 
            // textBoxSessionNum
            // 
            resources.ApplyResources(this.textBoxSessionNum, "textBoxSessionNum");
            this.textBoxSessionNum.Name = "textBoxSessionNum";
            this.textBoxSessionNum.TextChanged += new System.EventHandler(this.textBoxSessionNum_TextChanged);
            // 
            // labelSessionNum
            // 
            resources.ApplyResources(this.labelSessionNum, "labelSessionNum");
            this.labelSessionNum.Name = "labelSessionNum";
            // 
            // textBoxCardNum
            // 
            resources.ApplyResources(this.textBoxCardNum, "textBoxCardNum");
            this.textBoxCardNum.Name = "textBoxCardNum";
            this.textBoxCardNum.TextChanged += new System.EventHandler(this.textBoxCardNum_TextChanged);
            // 
            // labelCardNum
            // 
            resources.ApplyResources(this.labelCardNum, "labelCardNum");
            this.labelCardNum.Name = "labelCardNum";
            // 
            // groupBoxDB
            // 
            resources.ApplyResources(this.groupBoxDB, "groupBoxDB");
            this.groupBoxDB.Controls.Add(this.listViewLMs);
            this.groupBoxDB.Controls.Add(this.textBoxConnectionString);
            this.groupBoxDB.Name = "groupBoxDB";
            this.groupBoxDB.TabStop = false;
            // 
            // Mainform
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxDB);
            this.Controls.Add(this.textBoxCardNum);
            this.Controls.Add(this.labelCardNum);
            this.Controls.Add(this.textBoxSessionNum);
            this.Controls.Add(this.labelSessionNum);
            this.Controls.Add(this.buttonChangeUser);
            this.Controls.Add(this.textBoxCurrentUser);
            this.Controls.Add(this.labelCurrentUser);
            this.Controls.Add(this.checkBoxCopyLM);
            this.Controls.Add(this.buttonGenerate);
            this.Controls.Add(this.groupBoxCopyLM);
            this.HelpButton = true;
            this.Name = "Mainform";
            this.Load += new System.EventHandler(this.Mainform_Load);
            this.groupBoxCopyLM.ResumeLayout(false);
            this.groupBoxCopyLM.PerformLayout();
            this.groupBoxDB.ResumeLayout(false);
            this.groupBoxDB.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCopyLM;
        private System.Windows.Forms.Label labelSourceLM;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.Button buttonSource;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.TextBox textBoxConnectionString;
        private System.Windows.Forms.ListView listViewLMs;
        private System.Windows.Forms.ColumnHeader columnHeaderTitle;
        private System.Windows.Forms.ColumnHeader columnHeaderAuthor;
        private System.Windows.Forms.ColumnHeader columnHeaderNumber;
        private System.Windows.Forms.CheckBox checkBoxCopyLM;
        private System.Windows.Forms.Label labelCurrentUser;
        private System.Windows.Forms.TextBox textBoxCurrentUser;
        private System.Windows.Forms.Button buttonChangeUser;
        private System.Windows.Forms.TextBox textBoxSessionNum;
        private System.Windows.Forms.Label labelSessionNum;
        private System.Windows.Forms.TextBox textBoxCardNum;
        private System.Windows.Forms.Label labelCardNum;
        private System.Windows.Forms.GroupBox groupBoxDB;
    }
}

