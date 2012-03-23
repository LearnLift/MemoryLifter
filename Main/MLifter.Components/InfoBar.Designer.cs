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
namespace MLifter.Components
{
    partial class InfoBar
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoBar));
			this.timerAnimation = new System.Windows.Forms.Timer(this.components);
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.buttonClose = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.checkBoxDontShowAgain = new System.Windows.Forms.CheckBox();
			this.labelInfo = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerAnimation
			// 
			this.timerAnimation.Interval = 10;
			this.timerAnimation.Tick += new System.EventHandler(this.timerFade_Tick);
			// 
			// buttonClose
			// 
			this.buttonClose.BackColor = System.Drawing.SystemColors.Control;
			this.buttonClose.BackgroundImage = global::MLifter.Components.Properties.Resources.button_close;
			resources.ApplyResources(this.buttonClose, "buttonClose");
			this.buttonClose.Name = "buttonClose";
			this.toolTip.SetToolTip(this.buttonClose, resources.GetString("buttonClose.ToolTip"));
			this.buttonClose.UseVisualStyleBackColor = false;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.buttonClose, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.checkBoxDontShowAgain, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.labelInfo, 0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// checkBoxDontShowAgain
			// 
			resources.ApplyResources(this.checkBoxDontShowAgain, "checkBoxDontShowAgain");
			this.checkBoxDontShowAgain.Name = "checkBoxDontShowAgain";
			this.checkBoxDontShowAgain.UseVisualStyleBackColor = true;
			this.checkBoxDontShowAgain.CheckedChanged += new System.EventHandler(this.checkBoxDontShowAgain_CheckedChanged);
			// 
			// labelInfo
			// 
			resources.ApplyResources(this.labelInfo, "labelInfo");
			this.labelInfo.Name = "labelInfo";
			// 
			// InfoBar
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Info;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "InfoBar";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Timer timerAnimation;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.CheckBox checkBoxDontShowAgain;
		private System.Windows.Forms.Label labelInfo;
    }
}
