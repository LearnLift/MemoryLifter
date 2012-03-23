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
namespace MLifter.ImportExport
{
    partial class ImportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportForm));
            this.lblOrderHelp = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.cmbFileFormat = new System.Windows.Forms.ComboBox();
            this.lblchapter = new System.Windows.Forms.Label();
            this.cmbSelectChapter = new System.Windows.Forms.ComboBox();
            this.btnLoadFromFile = new System.Windows.Forms.Button();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.listViewPreview = new MLifter.Components.ColumnRightToLeftListView();
            this.lblselfileformat = new System.Windows.Forms.Label();
            this.checkBoxHeader = new System.Windows.Forms.CheckBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelStatusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBarStatus = new System.Windows.Forms.ToolStripProgressBar();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxCharset = new System.Windows.Forms.ComboBox();
            this.frmFields = new MLifter.Controls.FieldsFrame();
            this.checkBoxSplitSynonyms = new System.Windows.Forms.CheckBox();
            this.mainHelp = new System.Windows.Forms.HelpProvider();
            this.groupBoxPreview.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblOrderHelp
            // 
            resources.ApplyResources(this.lblOrderHelp, "lblOrderHelp");
            this.lblOrderHelp.Name = "lblOrderHelp";
            // 
            // btnImport
            // 
            resources.ApplyResources(this.btnImport, "btnImport");
            this.btnImport.Name = "btnImport";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // cmbFileFormat
            // 
            this.cmbFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFileFormat.FormattingEnabled = true;
            resources.ApplyResources(this.cmbFileFormat, "cmbFileFormat");
            this.cmbFileFormat.Name = "cmbFileFormat";
            this.cmbFileFormat.SelectedIndexChanged += new System.EventHandler(this.cmbFileFormat_SelectedIndexChanged);
            // 
            // lblchapter
            // 
            resources.ApplyResources(this.lblchapter, "lblchapter");
            this.lblchapter.Name = "lblchapter";
            // 
            // cmbSelectChapter
            // 
            this.cmbSelectChapter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectChapter.FormattingEnabled = true;
            resources.ApplyResources(this.cmbSelectChapter, "cmbSelectChapter");
            this.cmbSelectChapter.Name = "cmbSelectChapter";
            // 
            // btnLoadFromFile
            // 
            resources.ApplyResources(this.btnLoadFromFile, "btnLoadFromFile");
            this.btnLoadFromFile.Name = "btnLoadFromFile";
            this.btnLoadFromFile.UseVisualStyleBackColor = true;
            this.btnLoadFromFile.Click += new System.EventHandler(this.btnLoadFromFile_Click);
            // 
            // groupBoxPreview
            // 
            resources.ApplyResources(this.groupBoxPreview, "groupBoxPreview");
            this.groupBoxPreview.Controls.Add(this.listViewPreview);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.TabStop = false;
            // 
            // listViewPreview
            // 
            resources.ApplyResources(this.listViewPreview, "listViewPreview");
            this.listViewPreview.FullRowSelect = true;
            this.listViewPreview.Name = "listViewPreview";
            this.listViewPreview.TabStop = false;
            this.listViewPreview.UseCompatibleStateImageBehavior = false;
            this.listViewPreview.View = System.Windows.Forms.View.Details;
            // 
            // lblselfileformat
            // 
            resources.ApplyResources(this.lblselfileformat, "lblselfileformat");
            this.lblselfileformat.Name = "lblselfileformat";
            // 
            // checkBoxHeader
            // 
            resources.ApplyResources(this.checkBoxHeader, "checkBoxHeader");
            this.checkBoxHeader.Checked = true;
            this.checkBoxHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHeader.Name = "checkBoxHeader";
            this.checkBoxHeader.UseVisualStyleBackColor = true;
            this.checkBoxHeader.CheckedChanged += new System.EventHandler(this.checkBoxHeader_CheckedChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelStatusMessage,
            this.toolStripProgressBarStatus});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.SizingGrip = false;
            // 
            // toolStripStatusLabelStatusMessage
            // 
            resources.ApplyResources(this.toolStripStatusLabelStatusMessage, "toolStripStatusLabelStatusMessage");
            this.toolStripStatusLabelStatusMessage.Name = "toolStripStatusLabelStatusMessage";
            // 
            // toolStripProgressBarStatus
            // 
            this.toolStripProgressBarStatus.Name = "toolStripProgressBarStatus";
            resources.ApplyResources(this.toolStripProgressBarStatus, "toolStripProgressBarStatus");
            this.toolStripProgressBarStatus.Step = 1;
            this.toolStripProgressBarStatus.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxCharset
            // 
            this.comboBoxCharset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCharset.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxCharset, "comboBoxCharset");
            this.comboBoxCharset.Name = "comboBoxCharset";
            this.comboBoxCharset.SelectedIndexChanged += new System.EventHandler(this.comboBoxCharset_SelectedIndexChanged);
            // 
            // frmFields
            // 
            resources.ApplyResources(this.frmFields, "frmFields");
            this.frmFields.Name = "frmFields";
            this.frmFields.OnUpdate += new MLifter.Controls.FieldsFrame.FieldEventHandler(this.frmFields_OnUpdate);
            // 
            // checkBoxSplitSynonyms
            // 
            resources.ApplyResources(this.checkBoxSplitSynonyms, "checkBoxSplitSynonyms");
            this.checkBoxSplitSynonyms.Name = "checkBoxSplitSynonyms";
            this.checkBoxSplitSynonyms.UseVisualStyleBackColor = true;
            // 
            // ImportForm
            // 
            this.AcceptButton = this.btnLoadFromFile;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.buttonClose;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.checkBoxSplitSynonyms);
            this.Controls.Add(this.comboBoxCharset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.frmFields);
            this.Controls.Add(this.cmbSelectChapter);
            this.Controls.Add(this.cmbFileFormat);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.checkBoxHeader);
            this.Controls.Add(this.lblselfileformat);
            this.Controls.Add(this.groupBoxPreview);
            this.Controls.Add(this.lblOrderHelp);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lblchapter);
            this.Controls.Add(this.btnLoadFromFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.mainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.mainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportForm";
            this.mainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ImportForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportForm_FormClosed);
            this.groupBoxPreview.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOrderHelp;
        private MLifter.Controls.FieldsFrame frmFields;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.ComboBox cmbFileFormat;
        private System.Windows.Forms.Label lblchapter;
        private System.Windows.Forms.ComboBox cmbSelectChapter;
        private System.Windows.Forms.Button btnLoadFromFile;
        private System.Windows.Forms.GroupBox groupBoxPreview;
        private MLifter.Components.ColumnRightToLeftListView listViewPreview;
        private System.Windows.Forms.Label lblselfileformat;
        private System.Windows.Forms.CheckBox checkBoxHeader;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelStatusMessage;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBarStatus;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxCharset;
        private System.Windows.Forms.CheckBox checkBoxSplitSynonyms;
        private System.Windows.Forms.HelpProvider mainHelp;
    }
}
