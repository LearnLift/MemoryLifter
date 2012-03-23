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
    partial class QuestionPanel
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuestionPanel));
			this.gradientPanelQuestion = new MLifter.Components.GradientPanel();
			this.labelQuestion = new System.Windows.Forms.Label();
            this.webBrowserQuestion = new MLifter.Components.MLifterWebBrowser();
			this.gradientPanelQuestion.SuspendLayout();
			this.SuspendLayout();
			// 
			// gradientPanelQuestion
			// 
			this.gradientPanelQuestion.BackColor = System.Drawing.Color.Transparent;
			this.gradientPanelQuestion.Controls.Add(this.labelQuestion);
			this.gradientPanelQuestion.Controls.Add(this.webBrowserQuestion);
			resources.ApplyResources(this.gradientPanelQuestion, "gradientPanelQuestion");
			this.gradientPanelQuestion.Gradient = null;
			this.gradientPanelQuestion.LayoutSuspended = false;
			this.gradientPanelQuestion.Name = "gradientPanelQuestion";
			// 
			// labelQuestion
			// 
			this.labelQuestion.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.labelQuestion, "labelQuestion");
			this.labelQuestion.ForeColor = System.Drawing.Color.White;
			this.labelQuestion.Name = "labelQuestion";
			// 
			// webBrowserQuestion
			// 
			resources.ApplyResources(this.webBrowserQuestion, "webBrowserQuestion");
			this.webBrowserQuestion.IsWebBrowserContextMenuEnabled = false;
			this.webBrowserQuestion.MinimumSize = new System.Drawing.Size(27, 25);
			this.webBrowserQuestion.Name = "webBrowserQuestion";
			this.webBrowserQuestion.WebBrowserShortcutsEnabled = false;
			this.webBrowserQuestion.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserQuestion_Navigating);
			// 
			// QuestionPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.gradientPanelQuestion);
			this.Name = "QuestionPanel";
			resources.ApplyResources(this, "$this");
			this.gradientPanelQuestion.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelQuestion;
        private MLifter.Components.GradientPanel gradientPanelQuestion;
        private MLifter.Components.MLifterWebBrowser webBrowserQuestion;
    }
}
