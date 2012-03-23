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
    partial class Wizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wizard));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.tableLayoutPanelPages = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.AutoEllipsis = true;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::MLifter.Controls.Properties.Resources.processStop;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonBack
            // 
            this.buttonBack.AutoEllipsis = true;
            this.buttonBack.Image = global::MLifter.Controls.Properties.Resources.goPrevious;
            resources.ApplyResources(this.buttonBack, "buttonBack");
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.AutoEllipsis = true;
            this.buttonNext.Image = global::MLifter.Controls.Properties.Resources.goNext;
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoEllipsis = true;
            this.buttonHelp.Image = global::MLifter.Controls.Properties.Resources.helpBrowser;
            resources.ApplyResources(this.buttonHelp, "buttonHelp");
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // tableLayoutPanelPages
            // 
            resources.ApplyResources(this.tableLayoutPanelPages, "tableLayoutPanelPages");
            this.tableLayoutPanelPages.Name = "tableLayoutPanelPages";
            // 
            // Wizard
            // 
            this.AcceptButton = this.buttonNext;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.buttonCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanelPages);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Wizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Wizard_HelpButtonClicked);
            this.Shown += new System.EventHandler(this.Wizard_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Wizard_FormClosing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Wizard_HelpRequested);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPages;
    }
}
