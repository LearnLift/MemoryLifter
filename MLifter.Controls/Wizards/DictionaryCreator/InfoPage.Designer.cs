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
    partial class InfoPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoPage));
            this.labelWelcome = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelNumberOfCards = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelUsedDiskSpace = new System.Windows.Forms.Label();
            this.textBoxDictionaryLocation = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelUsedDiskFiles = new System.Windows.Forms.Label();
            this.labelScore = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelHighscore = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelWelcome
            // 
            resources.ApplyResources(this.labelWelcome, "labelWelcome");
            this.labelWelcome.Name = "labelWelcome";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // labelNumberOfCards
            // 
            resources.ApplyResources(this.labelNumberOfCards, "labelNumberOfCards");
            this.labelNumberOfCards.Name = "labelNumberOfCards";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // labelUsedDiskSpace
            // 
            resources.ApplyResources(this.labelUsedDiskSpace, "labelUsedDiskSpace");
            this.labelUsedDiskSpace.Name = "labelUsedDiskSpace";
            // 
            // textBoxDictionaryLocation
            // 
            resources.ApplyResources(this.textBoxDictionaryLocation, "textBoxDictionaryLocation");
            this.textBoxDictionaryLocation.Name = "textBoxDictionaryLocation";
            this.textBoxDictionaryLocation.ReadOnly = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // labelUsedDiskFiles
            // 
            resources.ApplyResources(this.labelUsedDiskFiles, "labelUsedDiskFiles");
            this.labelUsedDiskFiles.Name = "labelUsedDiskFiles";
            // 
            // labelScore
            // 
            resources.ApplyResources(this.labelScore, "labelScore");
            this.labelScore.Name = "labelScore";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // labelHighscore
            // 
            resources.ApplyResources(this.labelHighscore, "labelHighscore");
            this.labelHighscore.Name = "labelHighscore";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // InfoPage
            // 
            this.Controls.Add(this.labelHighscore);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.labelScore);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelUsedDiskFiles);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxDictionaryLocation);
            this.Controls.Add(this.labelUsedDiskSpace);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelNumberOfCards);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelWelcome);
            resources.ApplyResources(this, "$this");
            this.HelpAvailable = true;
            this.LeftImage = global::MLifter.Controls.Properties.Resources.setup;
            this.Name = "InfoPage";
            this.Controls.SetChildIndex(this.labelWelcome, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.labelNumberOfCards, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.labelUsedDiskSpace, 0);
            this.Controls.SetChildIndex(this.textBoxDictionaryLocation, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.labelUsedDiskFiles, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.labelScore, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.labelHighscore, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWelcome;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelNumberOfCards;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelUsedDiskSpace;
        private System.Windows.Forms.TextBox textBoxDictionaryLocation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelUsedDiskFiles;
        private System.Windows.Forms.Label labelScore;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelHighscore;
        private System.Windows.Forms.Label label7;
    }
}
