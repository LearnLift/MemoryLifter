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
    partial class MLifterCheckBox
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
            this.labelText = new System.Windows.Forms.Label();
            this.checkButtonNumber = new MLifter.Components.CheckButton();
            this.SuspendLayout();
            // 
            // labelText
            // 
            this.labelText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelText.Location = new System.Drawing.Point(75, 9);
            this.labelText.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(352, 80);
            this.labelText.TabIndex = 0;
            this.labelText.Text = "Text";
            this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelText.Click += new System.EventHandler(this.labelText_Click);
            // 
            // checkButtonNumber
            // 
            this.checkButtonNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkButtonNumber.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkButtonNumber.AutoSize = true;
            this.checkButtonNumber.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.checkButtonNumber.CrossColor = System.Drawing.Color.Red;
            this.checkButtonNumber.CrossMargin = 6;
            this.checkButtonNumber.ImageChecked = global::MLifter.Components.Properties.Resources.CheckButtonCross3;
            this.checkButtonNumber.Location = new System.Drawing.Point(10, 34);
            this.checkButtonNumber.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.checkButtonNumber.Name = "checkButtonNumber";
            this.checkButtonNumber.Size = new System.Drawing.Size(55, 35);
            this.checkButtonNumber.TabIndex = 1;
            this.checkButtonNumber.Text = "10.";
            this.checkButtonNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkButtonNumber.UseVisualStyleBackColor = true;
            this.checkButtonNumber.KeyUp += new System.Windows.Forms.KeyEventHandler(this.checkButtonNumber_KeyUp);
            this.checkButtonNumber.CheckedChanged += new System.EventHandler(this.checkButtonNumber_CheckedChanged);
            this.checkButtonNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkButtonNumber_KeyDown);
            // 
            // MLifterCheckBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkButtonNumber);
            this.Controls.Add(this.labelText);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "MLifterCheckBox";
            this.Size = new System.Drawing.Size(434, 102);
            this.Click += new System.EventHandler(this.MLifterCheckBox_Click);
            this.RightToLeftChanged += new System.EventHandler(this.MLifterCheckBox_RightToLeftChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelText;
        private CheckButton checkButtonNumber;
    }
}
