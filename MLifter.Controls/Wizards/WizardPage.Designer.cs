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
namespace MLifter
{
    partial class WizardPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardPage));
            this.pictureBoxLeft = new System.Windows.Forms.PictureBox();
            this.pictureBoxTop = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBoxLeft, "pictureBoxLeft");
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.TabStop = false;
            // 
            // pictureBoxTop
            // 
            this.pictureBoxTop.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBoxTop, "pictureBoxTop");
            this.pictureBoxTop.Name = "pictureBoxTop";
            this.pictureBoxTop.TabStop = false;
            this.pictureBoxTop.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxTop_Paint);
            // 
            // WizardPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.pictureBoxTop);
            this.Controls.Add(this.pictureBoxLeft);
            resources.ApplyResources(this, "$this");
            this.Name = "WizardPage";
            this.Load += new System.EventHandler(this.WizardPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLeft;
        private System.Windows.Forms.PictureBox pictureBoxTop;
    }
}
