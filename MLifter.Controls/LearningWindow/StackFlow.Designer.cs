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
    partial class StackFlow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StackFlow));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.rtbStack4 = new System.Windows.Forms.RichTextBox();
            this.rtbStack3 = new System.Windows.Forms.RichTextBox();
            this.rtbStack2 = new System.Windows.Forms.RichTextBox();
            this.rtbStack1 = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.rtbStack4, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.rtbStack3, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.rtbStack2, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.rtbStack1, 0, 0);
            this.tableLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // rtbStack4
            // 
            resources.ApplyResources(this.rtbStack4, "rtbStack4");
            this.rtbStack4.Name = "rtbStack4";
            this.rtbStack4.ReadOnly = true;
            this.rtbStack4.Text = global::MLifter.Controls.Properties.Resources.TOOLTIP_FRMFIELDS_LISTBOXSELFIELDS;
            // 
            // rtbStack3
            // 
            resources.ApplyResources(this.rtbStack3, "rtbStack3");
            this.rtbStack3.Name = "rtbStack3";
            this.rtbStack3.ReadOnly = true;
            this.rtbStack3.Text = global::MLifter.Controls.Properties.Resources.TOOLTIP_FRMFIELDS_LISTBOXSELFIELDS;
            // 
            // rtbStack2
            // 
            resources.ApplyResources(this.rtbStack2, "rtbStack2");
            this.rtbStack2.Name = "rtbStack2";
            this.rtbStack2.ReadOnly = true;
            this.rtbStack2.Text = global::MLifter.Controls.Properties.Resources.TOOLTIP_FRMFIELDS_LISTBOXSELFIELDS;
            // 
            // rtbStack1
            // 
            resources.ApplyResources(this.rtbStack1, "rtbStack1");
            this.rtbStack1.Name = "rtbStack1";
            this.rtbStack1.ReadOnly = true;
            this.rtbStack1.Text = global::MLifter.Controls.Properties.Resources.TOOLTIP_FRMFIELDS_LISTBOXSELFIELDS;
            // 
            // StackFlow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel);
            resources.ApplyResources(this, "$this");
            this.Name = "StackFlow";
            this.FontChanged += new System.EventHandler(this.StackFlow_FontChanged);
            this.ForeColorChanged += new System.EventHandler(this.StackFlow_ForeColorChanged);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.RichTextBox rtbStack1;
        private System.Windows.Forms.RichTextBox rtbStack4;
        private System.Windows.Forms.RichTextBox rtbStack3;
        private System.Windows.Forms.RichTextBox rtbStack2;

    }
}
