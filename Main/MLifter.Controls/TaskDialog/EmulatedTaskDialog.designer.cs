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
namespace MLifter.Controls
{
    partial class EmulatedTaskDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmulatedTaskDialog));
            this.imgMain = new System.Windows.Forms.PictureBox();
            this.lbContent = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.bt1 = new System.Windows.Forms.Button();
            this.bt2 = new System.Windows.Forms.Button();
            this.bt3 = new System.Windows.Forms.Button();
            this.cbVerify = new System.Windows.Forms.CheckBox();
            this.lbShowHideDetails = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lbFooter = new System.Windows.Forms.Label();
            this.imgFooter = new System.Windows.Forms.PictureBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pnlCommandButtons = new System.Windows.Forms.Panel();
            this.pnlMainInstruction = new System.Windows.Forms.Panel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlExpandedInfo = new System.Windows.Forms.Panel();
            this.lbExpandedInfo = new System.Windows.Forms.Label();
            this.pnlRadioButtons = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.imgMain)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgFooter)).BeginInit();
            this.pnlMainInstruction.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlExpandedInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgMain
            // 
            resources.ApplyResources(this.imgMain, "imgMain");
            this.imgMain.Name = "imgMain";
            this.imgMain.TabStop = false;
            // 
            // lbContent
            // 
            resources.ApplyResources(this.lbContent, "lbContent");
            this.lbContent.Name = "lbContent";
            // 
            // pnlButtons
            // 
            this.pnlButtons.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlButtons.Controls.Add(this.bt1);
            this.pnlButtons.Controls.Add(this.bt2);
            this.pnlButtons.Controls.Add(this.bt3);
            this.pnlButtons.Controls.Add(this.cbVerify);
            this.pnlButtons.Controls.Add(this.lbShowHideDetails);
            this.pnlButtons.Controls.Add(this.panel2);
            resources.ApplyResources(this.pnlButtons, "pnlButtons");
            this.pnlButtons.Name = "pnlButtons";
            // 
            // bt1
            // 
            resources.ApplyResources(this.bt1, "bt1");
            this.bt1.Name = "bt1";
            this.bt1.UseVisualStyleBackColor = true;
            // 
            // bt2
            // 
            resources.ApplyResources(this.bt2, "bt2");
            this.bt2.Name = "bt2";
            this.bt2.UseVisualStyleBackColor = true;
            // 
            // bt3
            // 
            resources.ApplyResources(this.bt3, "bt3");
            this.bt3.Name = "bt3";
            this.bt3.UseVisualStyleBackColor = true;
            // 
            // cbVerify
            // 
            resources.ApplyResources(this.cbVerify, "cbVerify");
            this.cbVerify.Name = "cbVerify";
            this.cbVerify.UseVisualStyleBackColor = true;
            // 
            // lbShowHideDetails
            // 
            this.lbShowHideDetails.Image = global::MLifter.Controls.Properties.Resources.arrow_down_bw;
            resources.ApplyResources(this.lbShowHideDetails, "lbShowHideDetails");
            this.lbShowHideDetails.Name = "lbShowHideDetails";
            this.lbShowHideDetails.MouseLeave += new System.EventHandler(this.lbDetails_MouseLeave);
            this.lbShowHideDetails.Click += new System.EventHandler(this.lbDetails_Click);
            this.lbShowHideDetails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbDetails_MouseDown);
            this.lbShowHideDetails.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbDetails_MouseUp);
            this.lbShowHideDetails.MouseEnter += new System.EventHandler(this.lbDetails_MouseEnter);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlFooter.Controls.Add(this.lbFooter);
            this.pnlFooter.Controls.Add(this.imgFooter);
            this.pnlFooter.Controls.Add(this.panel5);
            this.pnlFooter.Controls.Add(this.panel3);
            resources.ApplyResources(this.pnlFooter, "pnlFooter");
            this.pnlFooter.Name = "pnlFooter";
            // 
            // lbFooter
            // 
            resources.ApplyResources(this.lbFooter, "lbFooter");
            this.lbFooter.Name = "lbFooter";
            // 
            // imgFooter
            // 
            resources.ApplyResources(this.imgFooter, "imgFooter");
            this.imgFooter.Name = "imgFooter";
            this.imgFooter.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // pnlCommandButtons
            // 
            resources.ApplyResources(this.pnlCommandButtons, "pnlCommandButtons");
            this.pnlCommandButtons.Name = "pnlCommandButtons";
            // 
            // pnlMainInstruction
            // 
            this.pnlMainInstruction.Controls.Add(this.imgMain);
            resources.ApplyResources(this.pnlMainInstruction, "pnlMainInstruction");
            this.pnlMainInstruction.Name = "pnlMainInstruction";
            this.pnlMainInstruction.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlMainInstruction_Paint);
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.lbContent);
            resources.ApplyResources(this.pnlContent, "pnlContent");
            this.pnlContent.Name = "pnlContent";
            // 
            // pnlExpandedInfo
            // 
            this.pnlExpandedInfo.Controls.Add(this.lbExpandedInfo);
            resources.ApplyResources(this.pnlExpandedInfo, "pnlExpandedInfo");
            this.pnlExpandedInfo.Name = "pnlExpandedInfo";
            // 
            // lbExpandedInfo
            // 
            resources.ApplyResources(this.lbExpandedInfo, "lbExpandedInfo");
            this.lbExpandedInfo.Name = "lbExpandedInfo";
            // 
            // pnlRadioButtons
            // 
            resources.ApplyResources(this.pnlRadioButtons, "pnlRadioButtons");
            this.pnlRadioButtons.Name = "pnlRadioButtons";
            // 
            // EmulatedTaskDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlCommandButtons);
            this.Controls.Add(this.pnlRadioButtons);
            this.Controls.Add(this.pnlExpandedInfo);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlMainInstruction);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmulatedTaskDialog";
            this.ShowInTaskbar = false;
            this.Shown += new System.EventHandler(this.frmTaskDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.imgMain)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgFooter)).EndInit();
            this.pnlMainInstruction.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlExpandedInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imgMain;
        private System.Windows.Forms.Label lbContent;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.PictureBox imgFooter;
        private System.Windows.Forms.Label lbFooter;
        private System.Windows.Forms.Label lbShowHideDetails;
        private System.Windows.Forms.Panel pnlCommandButtons;
        private System.Windows.Forms.CheckBox cbVerify;
        private System.Windows.Forms.Panel pnlMainInstruction;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlExpandedInfo;
        private System.Windows.Forms.Label lbExpandedInfo;
        private System.Windows.Forms.Panel pnlRadioButtons;
        private System.Windows.Forms.Button bt1;
        private System.Windows.Forms.Button bt2;
        private System.Windows.Forms.Button bt3;
    }
}
