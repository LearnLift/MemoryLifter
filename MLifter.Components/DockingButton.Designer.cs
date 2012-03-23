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
    partial class DockingButton
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
            this.linkLabelText = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // linkLabelText
            // 
            this.linkLabelText.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.linkLabelText.AutoSize = true;
            this.linkLabelText.Location = new System.Drawing.Point(50, 4);
            this.linkLabelText.Name = "linkLabelText";
            this.linkLabelText.Size = new System.Drawing.Size(82, 13);
            this.linkLabelText.TabIndex = 0;
            this.linkLabelText.TabStop = true;
            this.linkLabelText.Text = "dockingButton1";
            this.linkLabelText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelText_LinkClicked);
            // 
            // DockingButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabelText);
            this.Name = "DockingButton";
            this.Size = new System.Drawing.Size(180, 20);
            this.Resize += new System.EventHandler(this.DockingButton_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabelText;
    }
}
