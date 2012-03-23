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
namespace MLifter.Controls.Wizards.DictionaryCreator
{
    partial class WelcomePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomePage));
            this.labelWelcome = new System.Windows.Forms.Label();
            this.dictionaryProperties = new MLifter.Controls.DictionaryProperties();
            this.SuspendLayout();
            // 
            // labelWelcome
            // 
            resources.ApplyResources(this.labelWelcome, "labelWelcome");
            this.labelWelcome.Name = "labelWelcome";
            // 
            // dictionaryProperties
            // 
            this.dictionaryProperties.DictionaryLocationVisible = true;
            this.dictionaryProperties.DictionaryNameReadOnly = false;
            resources.ApplyResources(this.dictionaryProperties, "dictionaryProperties");
            this.dictionaryProperties.Name = "dictionaryProperties";
            // 
            // WelcomePage
            // 
            this.Controls.Add(this.labelWelcome);
            this.Controls.Add(this.dictionaryProperties);
            resources.ApplyResources(this, "$this");
            this.HelpAvailable = true;
            this.LeftImage = global::MLifter.Controls.Properties.Resources.setup;
            this.Name = "WelcomePage";
            this.Load += new System.EventHandler(this.WelcomePage_Load);
            this.VisibleChanged += new System.EventHandler(this.WelcomePage_VisibleChanged);
            this.Controls.SetChildIndex(this.dictionaryProperties, 0);
            this.Controls.SetChildIndex(this.labelWelcome, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelWelcome;
        private DictionaryProperties dictionaryProperties;
    }
}
