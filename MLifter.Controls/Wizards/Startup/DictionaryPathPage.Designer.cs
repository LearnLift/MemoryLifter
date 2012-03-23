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
namespace MLifter.Controls.Wizards.Startup
{
    partial class DictionaryPathPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryPathPage));
            this.checkBoxDemoDic = new System.Windows.Forms.CheckBox();
            this.labelDictsInPathText = new System.Windows.Forms.Label();
            this.labelTotalSpaceText = new System.Windows.Forms.Label();
            this.labelFreeSpaceText = new System.Windows.Forms.Label();
            this.labelDictionariesInPath = new System.Windows.Forms.Label();
            this.labelTotalSpace = new System.Windows.Forms.Label();
            this.labelFreeSpace = new System.Windows.Forms.Label();
            this.labelSelectFolder = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxDemoDic
            // 
            this.checkBoxDemoDic.Checked = true;
            this.checkBoxDemoDic.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.checkBoxDemoDic, "checkBoxDemoDic");
            this.checkBoxDemoDic.Name = "checkBoxDemoDic";
            this.checkBoxDemoDic.UseVisualStyleBackColor = true;
            // 
            // labelDictsInPathText
            // 
            resources.ApplyResources(this.labelDictsInPathText, "labelDictsInPathText");
            this.labelDictsInPathText.Name = "labelDictsInPathText";
            // 
            // labelTotalSpaceText
            // 
            resources.ApplyResources(this.labelTotalSpaceText, "labelTotalSpaceText");
            this.labelTotalSpaceText.Name = "labelTotalSpaceText";
            // 
            // labelFreeSpaceText
            // 
            resources.ApplyResources(this.labelFreeSpaceText, "labelFreeSpaceText");
            this.labelFreeSpaceText.Name = "labelFreeSpaceText";
            // 
            // labelDictionariesInPath
            // 
            resources.ApplyResources(this.labelDictionariesInPath, "labelDictionariesInPath");
            this.labelDictionariesInPath.Name = "labelDictionariesInPath";
            // 
            // labelTotalSpace
            // 
            resources.ApplyResources(this.labelTotalSpace, "labelTotalSpace");
            this.labelTotalSpace.Name = "labelTotalSpace";
            // 
            // labelFreeSpace
            // 
            resources.ApplyResources(this.labelFreeSpace, "labelFreeSpace");
            this.labelFreeSpace.Name = "labelFreeSpace";
            // 
            // labelSelectFolder
            // 
            resources.ApplyResources(this.labelSelectFolder, "labelSelectFolder");
            this.labelSelectFolder.Name = "labelSelectFolder";
            // 
            // textBoxPath
            // 
            this.textBoxPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBoxPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            resources.ApplyResources(this.textBoxPath, "textBoxPath");
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.TextChanged += new System.EventHandler(this.textBoxPath_TextChanged);
            // 
            // buttonBrowse
            // 
            resources.ApplyResources(this.buttonBrowse, "buttonBrowse");
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // DictionaryPathPage
            // 
            this.CancelAllowed = false;
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.labelSelectFolder);
            this.Controls.Add(this.labelFreeSpace);
            this.Controls.Add(this.labelTotalSpace);
            this.Controls.Add(this.labelDictionariesInPath);
            this.Controls.Add(this.labelFreeSpaceText);
            this.Controls.Add(this.labelTotalSpaceText);
            this.Controls.Add(this.labelDictsInPathText);
            this.Controls.Add(this.checkBoxDemoDic);
            resources.ApplyResources(this, "$this");
            this.HelpAvailable = true;
            this.Name = "DictionaryPathPage";
            this.TopImage = global::MLifter.Controls.Properties.Resources.banner;
            this.Load += new System.EventHandler(this.DictionaryPathPage_Load);
            this.Controls.SetChildIndex(this.checkBoxDemoDic, 0);
            this.Controls.SetChildIndex(this.labelDictsInPathText, 0);
            this.Controls.SetChildIndex(this.labelTotalSpaceText, 0);
            this.Controls.SetChildIndex(this.labelFreeSpaceText, 0);
            this.Controls.SetChildIndex(this.labelDictionariesInPath, 0);
            this.Controls.SetChildIndex(this.labelTotalSpace, 0);
            this.Controls.SetChildIndex(this.labelFreeSpace, 0);
            this.Controls.SetChildIndex(this.labelSelectFolder, 0);
            this.Controls.SetChildIndex(this.textBoxPath, 0);
            this.Controls.SetChildIndex(this.buttonBrowse, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxDemoDic;
        private System.Windows.Forms.Label labelDictsInPathText;
        private System.Windows.Forms.Label labelTotalSpaceText;
        private System.Windows.Forms.Label labelFreeSpaceText;
        private System.Windows.Forms.Label labelDictionariesInPath;
        private System.Windows.Forms.Label labelTotalSpace;
        private System.Windows.Forms.Label labelFreeSpace;
        private System.Windows.Forms.Label labelSelectFolder;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button buttonBrowse;

    }
}
