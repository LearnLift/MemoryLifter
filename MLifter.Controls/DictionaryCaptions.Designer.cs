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
namespace MLifter.Controls
{
    partial class DictionaryCaptions
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictionaryCaptions));
			this.groupBoxAnswer = new System.Windows.Forms.GroupBox();
			this.comboBoxAnswerCulture = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxAnswerHeadline = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxQuestion = new System.Windows.Forms.GroupBox();
			this.comboBoxQuestionCulture = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxQuestionHeadline = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBoxAnswer.SuspendLayout();
			this.groupBoxQuestion.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxAnswer
			// 
			this.groupBoxAnswer.Controls.Add(this.comboBoxAnswerCulture);
			this.groupBoxAnswer.Controls.Add(this.label4);
			this.groupBoxAnswer.Controls.Add(this.textBoxAnswerHeadline);
			this.groupBoxAnswer.Controls.Add(this.label5);
			resources.ApplyResources(this.groupBoxAnswer, "groupBoxAnswer");
			this.groupBoxAnswer.Name = "groupBoxAnswer";
			this.groupBoxAnswer.TabStop = false;
			// 
			// comboBoxAnswerCulture
			// 
			resources.ApplyResources(this.comboBoxAnswerCulture, "comboBoxAnswerCulture");
			this.comboBoxAnswerCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAnswerCulture.FormattingEnabled = true;
			this.comboBoxAnswerCulture.Name = "comboBoxAnswerCulture";
			this.comboBoxAnswerCulture.Sorted = true;
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// textBoxAnswerHeadline
			// 
			resources.ApplyResources(this.textBoxAnswerHeadline, "textBoxAnswerHeadline");
			this.textBoxAnswerHeadline.Name = "textBoxAnswerHeadline";
			this.textBoxAnswerHeadline.Enter += new System.EventHandler(this.textBoxHeadline_Enter);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// groupBoxQuestion
			// 
			this.groupBoxQuestion.Controls.Add(this.comboBoxQuestionCulture);
			this.groupBoxQuestion.Controls.Add(this.label3);
			this.groupBoxQuestion.Controls.Add(this.textBoxQuestionHeadline);
			this.groupBoxQuestion.Controls.Add(this.label2);
			resources.ApplyResources(this.groupBoxQuestion, "groupBoxQuestion");
			this.groupBoxQuestion.Name = "groupBoxQuestion";
			this.groupBoxQuestion.TabStop = false;
			// 
			// comboBoxQuestionCulture
			// 
			resources.ApplyResources(this.comboBoxQuestionCulture, "comboBoxQuestionCulture");
			this.comboBoxQuestionCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxQuestionCulture.FormattingEnabled = true;
			this.comboBoxQuestionCulture.Name = "comboBoxQuestionCulture";
			this.comboBoxQuestionCulture.Sorted = true;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// textBoxQuestionHeadline
			// 
			resources.ApplyResources(this.textBoxQuestionHeadline, "textBoxQuestionHeadline");
			this.textBoxQuestionHeadline.Name = "textBoxQuestionHeadline";
			this.textBoxQuestionHeadline.Enter += new System.EventHandler(this.textBoxHeadline_Enter);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// DictionaryCaptions
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.groupBoxAnswer);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBoxQuestion);
			this.Name = "DictionaryCaptions";
			resources.ApplyResources(this, "$this");
			this.Load += new System.EventHandler(this.DictionaryCaptions_Load);
			this.groupBoxAnswer.ResumeLayout(false);
			this.groupBoxAnswer.PerformLayout();
			this.groupBoxQuestion.ResumeLayout(false);
			this.groupBoxQuestion.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAnswer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxAnswerCulture;
        private System.Windows.Forms.TextBox textBoxAnswerHeadline;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxQuestion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxQuestionCulture;
        private System.Windows.Forms.TextBox textBoxQuestionHeadline;
        private System.Windows.Forms.Label label2;
    }
}
