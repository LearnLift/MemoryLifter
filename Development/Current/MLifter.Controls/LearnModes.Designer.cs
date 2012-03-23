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
    partial class LearnModes
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearnModes));
			this.GBLearning = new System.Windows.Forms.GroupBox();
			this.LModeImageRecognition = new System.Windows.Forms.CheckBox();
			this.LModeListeningComprehension = new System.Windows.Forms.CheckBox();
			this.LModeSentences = new System.Windows.Forms.CheckBox();
			this.LModeMultipleChoice = new System.Windows.Forms.CheckBox();
			this.LModeStandard = new System.Windows.Forms.CheckBox();
			this.LblModes = new System.Windows.Forms.Label();
			this.RGDirection = new System.Windows.Forms.GroupBox();
			this.RBDirectionMixed = new MLifter.Controls.RadioCheckbox();
			this.RBDirectionAnswerQuestion = new MLifter.Controls.RadioCheckbox();
			this.RBDirectionQuestionAnswer = new MLifter.Controls.RadioCheckbox();
			this.groupBoxMultipleChoice = new System.Windows.Forms.GroupBox();
			this.numericUpDownMaxNumberOfCorrectAnswers = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownNumberOfChoices = new System.Windows.Forms.NumericUpDown();
			this.labelMaxNumberOfCorrectAnswers = new System.Windows.Forms.Label();
			this.checkBoxAllowRandomDistractors = new System.Windows.Forms.CheckBox();
			this.checkBoxAllowMultipleCorrectAnswers = new System.Windows.Forms.CheckBox();
			this.labelNumberOfChoices = new System.Windows.Forms.Label();
			this.GBLearning.SuspendLayout();
			this.RGDirection.SuspendLayout();
			this.groupBoxMultipleChoice.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxNumberOfCorrectAnswers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumberOfChoices)).BeginInit();
			this.SuspendLayout();
			// 
			// GBLearning
			// 
			resources.ApplyResources(this.GBLearning, "GBLearning");
			this.GBLearning.Controls.Add(this.LModeImageRecognition);
			this.GBLearning.Controls.Add(this.LModeListeningComprehension);
			this.GBLearning.Controls.Add(this.LModeSentences);
			this.GBLearning.Controls.Add(this.LModeMultipleChoice);
			this.GBLearning.Controls.Add(this.LModeStandard);
			this.GBLearning.Controls.Add(this.LblModes);
			this.GBLearning.Name = "GBLearning";
			this.GBLearning.TabStop = false;
			// 
			// LModeImageRecognition
			// 
			resources.ApplyResources(this.LModeImageRecognition, "LModeImageRecognition");
			this.LModeImageRecognition.Name = "LModeImageRecognition";
			// 
			// LModeListeningComprehension
			// 
			resources.ApplyResources(this.LModeListeningComprehension, "LModeListeningComprehension");
			this.LModeListeningComprehension.Name = "LModeListeningComprehension";
			// 
			// LModeSentences
			// 
			resources.ApplyResources(this.LModeSentences, "LModeSentences");
			this.LModeSentences.Name = "LModeSentences";
			// 
			// LModeMultipleChoice
			// 
			resources.ApplyResources(this.LModeMultipleChoice, "LModeMultipleChoice");
			this.LModeMultipleChoice.Name = "LModeMultipleChoice";
			// 
			// LModeStandard
			// 
			resources.ApplyResources(this.LModeStandard, "LModeStandard");
			this.LModeStandard.Name = "LModeStandard";
			// 
			// LblModes
			// 
			resources.ApplyResources(this.LblModes, "LblModes");
			this.LblModes.Name = "LblModes";
			// 
			// RGDirection
			// 
			resources.ApplyResources(this.RGDirection, "RGDirection");
			this.RGDirection.Controls.Add(this.RBDirectionMixed);
			this.RGDirection.Controls.Add(this.RBDirectionAnswerQuestion);
			this.RGDirection.Controls.Add(this.RBDirectionQuestionAnswer);
			this.RGDirection.Name = "RGDirection";
			this.RGDirection.TabStop = false;
			// 
			// RBDirectionMixed
			// 
			resources.ApplyResources(this.RBDirectionMixed, "RBDirectionMixed");
			this.RBDirectionMixed.Name = "RBDirectionMixed";
			// 
			// RBDirectionAnswerQuestion
			// 
			resources.ApplyResources(this.RBDirectionAnswerQuestion, "RBDirectionAnswerQuestion");
			this.RBDirectionAnswerQuestion.Name = "RBDirectionAnswerQuestion";
			// 
			// RBDirectionQuestionAnswer
			// 
			resources.ApplyResources(this.RBDirectionQuestionAnswer, "RBDirectionQuestionAnswer");
			this.RBDirectionQuestionAnswer.Name = "RBDirectionQuestionAnswer";
			// 
			// groupBoxMultipleChoice
			// 
			resources.ApplyResources(this.groupBoxMultipleChoice, "groupBoxMultipleChoice");
			this.groupBoxMultipleChoice.Controls.Add(this.numericUpDownMaxNumberOfCorrectAnswers);
			this.groupBoxMultipleChoice.Controls.Add(this.numericUpDownNumberOfChoices);
			this.groupBoxMultipleChoice.Controls.Add(this.labelMaxNumberOfCorrectAnswers);
			this.groupBoxMultipleChoice.Controls.Add(this.checkBoxAllowRandomDistractors);
			this.groupBoxMultipleChoice.Controls.Add(this.checkBoxAllowMultipleCorrectAnswers);
			this.groupBoxMultipleChoice.Controls.Add(this.labelNumberOfChoices);
			this.groupBoxMultipleChoice.Name = "groupBoxMultipleChoice";
			this.groupBoxMultipleChoice.TabStop = false;
			// 
			// numericUpDownMaxNumberOfCorrectAnswers
			// 
			resources.ApplyResources(this.numericUpDownMaxNumberOfCorrectAnswers, "numericUpDownMaxNumberOfCorrectAnswers");
			this.numericUpDownMaxNumberOfCorrectAnswers.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.numericUpDownMaxNumberOfCorrectAnswers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownMaxNumberOfCorrectAnswers.Name = "numericUpDownMaxNumberOfCorrectAnswers";
			this.numericUpDownMaxNumberOfCorrectAnswers.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// numericUpDownNumberOfChoices
			// 
			resources.ApplyResources(this.numericUpDownNumberOfChoices, "numericUpDownNumberOfChoices");
			this.numericUpDownNumberOfChoices.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.numericUpDownNumberOfChoices.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownNumberOfChoices.Name = "numericUpDownNumberOfChoices";
			this.numericUpDownNumberOfChoices.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.numericUpDownNumberOfChoices.ValueChanged += new System.EventHandler(this.numericUpDownNumberOfChoices_ValueChanged);
			// 
			// labelMaxNumberOfCorrectAnswers
			// 
			resources.ApplyResources(this.labelMaxNumberOfCorrectAnswers, "labelMaxNumberOfCorrectAnswers");
			this.labelMaxNumberOfCorrectAnswers.Name = "labelMaxNumberOfCorrectAnswers";
			// 
			// checkBoxAllowRandomDistractors
			// 
			resources.ApplyResources(this.checkBoxAllowRandomDistractors, "checkBoxAllowRandomDistractors");
			this.checkBoxAllowRandomDistractors.Name = "checkBoxAllowRandomDistractors";
			this.checkBoxAllowRandomDistractors.UseVisualStyleBackColor = true;
			// 
			// checkBoxAllowMultipleCorrectAnswers
			// 
			resources.ApplyResources(this.checkBoxAllowMultipleCorrectAnswers, "checkBoxAllowMultipleCorrectAnswers");
			this.checkBoxAllowMultipleCorrectAnswers.Name = "checkBoxAllowMultipleCorrectAnswers";
			this.checkBoxAllowMultipleCorrectAnswers.UseVisualStyleBackColor = true;
			// 
			// labelNumberOfChoices
			// 
			resources.ApplyResources(this.labelNumberOfChoices, "labelNumberOfChoices");
			this.labelNumberOfChoices.Name = "labelNumberOfChoices";
			// 
			// LearnModes
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.groupBoxMultipleChoice);
			this.Controls.Add(this.GBLearning);
			this.Controls.Add(this.RGDirection);
			this.Name = "LearnModes";
			resources.ApplyResources(this, "$this");
			this.GBLearning.ResumeLayout(false);
			this.RGDirection.ResumeLayout(false);
			this.RGDirection.PerformLayout();
			this.groupBoxMultipleChoice.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxNumberOfCorrectAnswers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumberOfChoices)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GBLearning;
        private System.Windows.Forms.CheckBox LModeImageRecognition;
        private System.Windows.Forms.CheckBox LModeListeningComprehension;
        private System.Windows.Forms.CheckBox LModeSentences;
        private System.Windows.Forms.CheckBox LModeMultipleChoice;
        private System.Windows.Forms.CheckBox LModeStandard;
        private System.Windows.Forms.Label LblModes;
        private System.Windows.Forms.GroupBox RGDirection;
        private MLifter.Controls.RadioCheckbox RBDirectionMixed;
        private MLifter.Controls.RadioCheckbox RBDirectionAnswerQuestion;
        private MLifter.Controls.RadioCheckbox RBDirectionQuestionAnswer;
        private System.Windows.Forms.GroupBox groupBoxMultipleChoice;
        private System.Windows.Forms.CheckBox checkBoxAllowRandomDistractors;
        private System.Windows.Forms.CheckBox checkBoxAllowMultipleCorrectAnswers;
        private System.Windows.Forms.Label labelNumberOfChoices;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxNumberOfCorrectAnswers;
        private System.Windows.Forms.NumericUpDown numericUpDownNumberOfChoices;
        private System.Windows.Forms.Label labelMaxNumberOfCorrectAnswers;
    }
}
