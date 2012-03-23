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
namespace MLifterErrorHandler
{
    partial class ErrorHandlerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorHandlerForm));
            this.labelErrorOccuredInformationMessage = new System.Windows.Forms.Label();
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.checkBoxRestartMemoryLifter = new System.Windows.Forms.CheckBox();
            this.labelAdditionalInformation = new System.Windows.Forms.Label();
            this.textBoxEmailAddress = new System.Windows.Forms.TextBox();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.buttonDontSend = new System.Windows.Forms.Button();
            this.buttonSend = new System.Windows.Forms.Button();
            this.labelPleaseTellUs = new System.Windows.Forms.Label();
            this.labelPleaseTellUsDetailedInformation = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.linkLabelErrorReportDetails = new System.Windows.Forms.LinkLabel();
            this.textBoxAdditionalInformation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelErrorOccuredInformationMessage
            // 
            resources.ApplyResources(this.labelErrorOccuredInformationMessage, "labelErrorOccuredInformationMessage");
            this.labelErrorOccuredInformationMessage.Name = "labelErrorOccuredInformationMessage";
            // 
            // imgLogo
            // 
            this.imgLogo.Image = global::MLifterErrorHandler.Properties.Resources.MLIcon48;
            resources.ApplyResources(this.imgLogo, "imgLogo");
            this.imgLogo.Name = "imgLogo";
            this.imgLogo.TabStop = false;
            // 
            // checkBoxRestartMemoryLifter
            // 
            resources.ApplyResources(this.checkBoxRestartMemoryLifter, "checkBoxRestartMemoryLifter");
            this.checkBoxRestartMemoryLifter.Name = "checkBoxRestartMemoryLifter";
            this.checkBoxRestartMemoryLifter.UseVisualStyleBackColor = true;
            // 
            // labelAdditionalInformation
            // 
            resources.ApplyResources(this.labelAdditionalInformation, "labelAdditionalInformation");
            this.labelAdditionalInformation.Name = "labelAdditionalInformation";
            // 
            // textBoxEmailAddress
            // 
            resources.ApplyResources(this.textBoxEmailAddress, "textBoxEmailAddress");
            this.textBoxEmailAddress.Name = "textBoxEmailAddress";
            // 
            // listViewFiles
            // 
            this.listViewFiles.CheckBoxes = true;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            resources.ApplyResources(this.listViewFiles, "listViewFiles");
            this.listViewFiles.MultiSelect = false;
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.ShowItemToolTips = true;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.List;
            this.listViewFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewAdditionalInformation_MouseDoubleClick);
            this.listViewFiles.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewFiles_ItemChecked);
            // 
            // buttonDontSend
            // 
            resources.ApplyResources(this.buttonDontSend, "buttonDontSend");
            this.buttonDontSend.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonDontSend.Name = "buttonDontSend";
            this.buttonDontSend.Tag = "1";
            this.buttonDontSend.Click += new System.EventHandler(this.buttonDontSend_Click);
            // 
            // buttonSend
            // 
            resources.ApplyResources(this.buttonSend, "buttonSend");
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Tag = "0";
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // labelPleaseTellUs
            // 
            resources.ApplyResources(this.labelPleaseTellUs, "labelPleaseTellUs");
            this.labelPleaseTellUs.Name = "labelPleaseTellUs";
            // 
            // labelPleaseTellUsDetailedInformation
            // 
            resources.ApplyResources(this.labelPleaseTellUsDetailedInformation, "labelPleaseTellUsDetailedInformation");
            this.labelPleaseTellUsDetailedInformation.Name = "labelPleaseTellUsDetailedInformation";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MLifterErrorHandler.Properties.Resources.face_blushing;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // labelErrorMessage
            // 
            this.labelErrorMessage.BackColor = System.Drawing.SystemColors.Control;
            this.labelErrorMessage.ForeColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.labelErrorMessage, "labelErrorMessage");
            this.labelErrorMessage.Name = "labelErrorMessage";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.buttonSend);
            this.panel1.Controls.Add(this.buttonDontSend);
            this.panel1.Controls.Add(this.checkBoxRestartMemoryLifter);
            this.panel1.Name = "panel1";
            // 
            // linkLabelErrorReportDetails
            // 
            this.linkLabelErrorReportDetails.Image = global::MLifterErrorHandler.Properties.Resources.arrow_down_bw;
            resources.ApplyResources(this.linkLabelErrorReportDetails, "linkLabelErrorReportDetails");
            this.linkLabelErrorReportDetails.Name = "linkLabelErrorReportDetails";
            this.linkLabelErrorReportDetails.TabStop = true;
            this.linkLabelErrorReportDetails.Click += new System.EventHandler(this.linkLabelErrorReportDetails_Click);
            // 
            // textBoxAdditionalInformation
            // 
            this.textBoxAdditionalInformation.AcceptsReturn = true;
            resources.ApplyResources(this.textBoxAdditionalInformation, "textBoxAdditionalInformation");
            this.textBoxAdditionalInformation.Name = "textBoxAdditionalInformation";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ErrorHandlerForm
            // 
            this.AcceptButton = this.buttonSend;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.buttonDontSend;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxAdditionalInformation);
            this.Controls.Add(this.linkLabelErrorReportDetails);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.labelPleaseTellUsDetailedInformation);
            this.Controls.Add(this.labelPleaseTellUs);
            this.Controls.Add(this.imgLogo);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listViewFiles);
            this.Controls.Add(this.textBoxEmailAddress);
            this.Controls.Add(this.labelAdditionalInformation);
            this.Controls.Add(this.labelErrorOccuredInformationMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ErrorHandlerForm";
            this.Load += new System.EventHandler(this.ErrorHandlerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelErrorOccuredInformationMessage;
        private System.Windows.Forms.PictureBox imgLogo;
        private System.Windows.Forms.CheckBox checkBoxRestartMemoryLifter;
        private System.Windows.Forms.Label labelAdditionalInformation;
        private System.Windows.Forms.TextBox textBoxEmailAddress;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Button buttonDontSend;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label labelPleaseTellUs;
        private System.Windows.Forms.Label labelPleaseTellUsDetailedInformation;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelErrorMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabelErrorReportDetails;
        private System.Windows.Forms.TextBox textBoxAdditionalInformation;
        private System.Windows.Forms.Label label1;
    }
}
