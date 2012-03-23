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
namespace MLifter.Controls.Wizards.Print
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
            this.labelDescription = new System.Windows.Forms.Label();
            this.radioButtonChapters = new System.Windows.Forms.RadioButton();
            this.radioButtonIndividual = new System.Windows.Forms.RadioButton();
            this.radioButtonAll = new System.Windows.Forms.RadioButton();
            this.radioButtonWrong = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.buttonDontUseWizard = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radioButtonBoxes = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.labelDescription.Name = "labelDescription";
            // 
            // radioButtonChapters
            // 
            resources.ApplyResources(this.radioButtonChapters, "radioButtonChapters");
            this.radioButtonChapters.Name = "radioButtonChapters";
            this.radioButtonChapters.UseVisualStyleBackColor = true;
            this.radioButtonChapters.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonIndividual
            // 
            resources.ApplyResources(this.radioButtonIndividual, "radioButtonIndividual");
            this.radioButtonIndividual.Name = "radioButtonIndividual";
            this.radioButtonIndividual.UseVisualStyleBackColor = true;
            this.radioButtonIndividual.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonAll
            // 
            this.radioButtonAll.Checked = true;
            resources.ApplyResources(this.radioButtonAll, "radioButtonAll");
            this.radioButtonAll.Name = "radioButtonAll";
            this.radioButtonAll.TabStop = true;
            this.radioButtonAll.UseVisualStyleBackColor = true;
            this.radioButtonAll.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonWrong
            // 
            resources.ApplyResources(this.radioButtonWrong, "radioButtonWrong");
            this.radioButtonWrong.Name = "radioButtonWrong";
            this.radioButtonWrong.UseVisualStyleBackColor = true;
            this.radioButtonWrong.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MLifter.Controls.Properties.Resources.preferencesSystemWindows;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::MLifter.Controls.Properties.Resources.editFind;
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::MLifter.Controls.Properties.Resources.processStopBig;
            resources.ApplyResources(this.pictureBox4, "pictureBox4");
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::MLifter.Controls.Properties.Resources.systemFileManager;
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // buttonDontUseWizard
            // 
            resources.ApplyResources(this.buttonDontUseWizard, "buttonDontUseWizard");
            this.buttonDontUseWizard.Name = "buttonDontUseWizard";
            this.buttonDontUseWizard.UseVisualStyleBackColor = true;
            this.buttonDontUseWizard.Click += new System.EventHandler(this.buttonDontUseWizard_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MLifter.Controls.Properties.Resources.packageGeneric;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // radioButtonBoxes
            // 
            resources.ApplyResources(this.radioButtonBoxes, "radioButtonBoxes");
            this.radioButtonBoxes.Name = "radioButtonBoxes";
            this.radioButtonBoxes.UseVisualStyleBackColor = true;
            this.radioButtonBoxes.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // WelcomePage
            // 
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.radioButtonBoxes);
            this.Controls.Add(this.buttonDontUseWizard);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.radioButtonWrong);
            this.Controls.Add(this.radioButtonAll);
            this.Controls.Add(this.radioButtonIndividual);
            this.Controls.Add(this.radioButtonChapters);
            this.Controls.Add(this.labelDescription);
            resources.ApplyResources(this, "$this");
            this.HelpAvailable = true;
            this.Name = "WelcomePage";
            this.TopImage = global::MLifter.Controls.Properties.Resources.banner;
            this.Load += new System.EventHandler(this.WelcomePage_Load);
            this.Controls.SetChildIndex(this.labelDescription, 0);
            this.Controls.SetChildIndex(this.radioButtonChapters, 0);
            this.Controls.SetChildIndex(this.radioButtonIndividual, 0);
            this.Controls.SetChildIndex(this.radioButtonAll, 0);
            this.Controls.SetChildIndex(this.radioButtonWrong, 0);
            this.Controls.SetChildIndex(this.pictureBox2, 0);
            this.Controls.SetChildIndex(this.pictureBox3, 0);
            this.Controls.SetChildIndex(this.pictureBox4, 0);
            this.Controls.SetChildIndex(this.pictureBox5, 0);
            this.Controls.SetChildIndex(this.buttonDontUseWizard, 0);
            this.Controls.SetChildIndex(this.radioButtonBoxes, 0);
            this.Controls.SetChildIndex(this.pictureBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.RadioButton radioButtonChapters;
        private System.Windows.Forms.RadioButton radioButtonIndividual;
        private System.Windows.Forms.RadioButton radioButtonAll;
        private System.Windows.Forms.RadioButton radioButtonWrong;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Button buttonDontUseWizard;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton radioButtonBoxes;
    }
}
