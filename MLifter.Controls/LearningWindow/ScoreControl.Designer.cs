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
namespace MLifter.Controls.LearningWindow
{
    partial class ScoreControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScoreControl));
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.labelKnown = new System.Windows.Forms.Label();
            this.colorProgressBarKnown = new MLifter.Components.ColorProgressBar();
            this.tableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            resources.ApplyResources(this.tableLayoutPanelMain, "tableLayoutPanelMain");
            this.tableLayoutPanelMain.Controls.Add(this.pictureBoxIcon, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.labelKnown, 2, 0);
            this.tableLayoutPanelMain.Controls.Add(this.colorProgressBarKnown, 1, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            // 
            // pictureBoxIcon
            // 
            resources.ApplyResources(this.pictureBoxIcon, "pictureBoxIcon");
            this.pictureBoxIcon.Image = global::MLifter.Controls.Properties.Resources.star;
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.TabStop = false;
            // 
            // labelKnown
            // 
            resources.ApplyResources(this.labelKnown, "labelKnown");
            this.labelKnown.Name = "labelKnown";
            // 
            // colorProgressBarKnown
            // 
            resources.ApplyResources(this.colorProgressBarKnown, "colorProgressBarKnown");
            this.colorProgressBarKnown.BackColor = System.Drawing.Color.Transparent;
            this.colorProgressBarKnown.BarColor = System.Drawing.Color.Red;
            this.colorProgressBarKnown.BorderColor = System.Drawing.Color.Black;
            this.colorProgressBarKnown.FillStyle = MLifter.Components.ColorProgressBar.FillStyles.Solid;
            this.colorProgressBarKnown.Maximum = 100;
            this.colorProgressBarKnown.Minimum = 0;
            this.colorProgressBarKnown.Name = "colorProgressBarKnown";
            this.colorProgressBarKnown.Step = 1;
            this.colorProgressBarKnown.Value = 100;
            // 
            // ScoreControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanelMain);
            resources.ApplyResources(this, "$this");
            this.Name = "ScoreControl";
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label labelKnown;
        private MLifter.Components.ColorProgressBar colorProgressBarKnown;
    }
}
