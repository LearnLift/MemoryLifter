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
    partial class ButtonsPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonsPanel));
            this.gradientPanelButtonControlBackground = new MLifter.Components.GradientPanel();
            this.buttonDummyFocus = new System.Windows.Forms.Button();
            this.glassButtonQuestion = new MLifter.Components.GlassButton();
            this.glassButtonSelfAssesmentDoKnow = new MLifter.Components.GlassButton();
            this.glassButtonNextCard = new MLifter.Components.GlassButton();
            this.glassButtonSelfAssesmentDontKnow = new MLifter.Components.GlassButton();
            this.glassButtonPreviousCard = new MLifter.Components.GlassButton();
            this.gradientPanelButtonControlBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // gradientPanelButtonControlBackground
            // 
            this.gradientPanelButtonControlBackground.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.gradientPanelButtonControlBackground.Controls.Add(this.buttonDummyFocus);
            this.gradientPanelButtonControlBackground.Controls.Add(this.glassButtonQuestion);
            this.gradientPanelButtonControlBackground.Controls.Add(this.glassButtonSelfAssesmentDoKnow);
            this.gradientPanelButtonControlBackground.Controls.Add(this.glassButtonNextCard);
            this.gradientPanelButtonControlBackground.Controls.Add(this.glassButtonSelfAssesmentDontKnow);
            this.gradientPanelButtonControlBackground.Controls.Add(this.glassButtonPreviousCard);
            resources.ApplyResources(this.gradientPanelButtonControlBackground, "gradientPanelButtonControlBackground");
            this.gradientPanelButtonControlBackground.Gradient = null;
            this.gradientPanelButtonControlBackground.LayoutSuspended = false;
            this.gradientPanelButtonControlBackground.Name = "gradientPanelButtonControlBackground";
            // 
            // buttonDummyFocus
            // 
            resources.ApplyResources(this.buttonDummyFocus, "buttonDummyFocus");
            this.buttonDummyFocus.Name = "buttonDummyFocus";
            this.buttonDummyFocus.UseVisualStyleBackColor = true;
            this.buttonDummyFocus.Enter += new System.EventHandler(this.buttonDummyFocus_Enter);
            // 
            // glassButtonQuestion
            // 
            resources.ApplyResources(this.glassButtonQuestion, "glassButtonQuestion");
            this.glassButtonQuestion.CornerRadius = 20;
            this.glassButtonQuestion.FocusColor = System.Drawing.Color.White;
            this.glassButtonQuestion.GlowColor = System.Drawing.Color.White;
            this.glassButtonQuestion.Name = "glassButtonQuestion";
            this.glassButtonQuestion.OuterBorderColor = System.Drawing.Color.Transparent;
            this.glassButtonQuestion.ShowFocusBorder = true;
            this.glassButtonQuestion.SpecialSymbol = MLifter.Components.GlassButton.SpecialSymbols.Pause;
            this.glassButtonQuestion.SpecialSymbolColor = System.Drawing.Color.Empty;
            this.glassButtonQuestion.SpecialSymbolColorDisabled = System.Drawing.Color.Empty;
            this.glassButtonQuestion.SpecialSymbolColorEnabled = System.Drawing.Color.Empty;
            this.glassButtonQuestion.SpecialSymbolTransperency = 255;
            this.glassButtonQuestion.Click += new System.EventHandler(this.glassButtonQuestion_Click);
            // 
            // glassButtonSelfAssesmentDoKnow
            // 
            this.glassButtonSelfAssesmentDoKnow.AlternativeForm = true;
            this.glassButtonSelfAssesmentDoKnow.AlternativeFormDirection = MLifter.Components.GlassButton.Direction.Right;
            resources.ApplyResources(this.glassButtonSelfAssesmentDoKnow, "glassButtonSelfAssesmentDoKnow");
            this.glassButtonSelfAssesmentDoKnow.CornerRadius = 30;
            this.glassButtonSelfAssesmentDoKnow.FocusColor = System.Drawing.Color.White;
            this.glassButtonSelfAssesmentDoKnow.GlowColor = System.Drawing.Color.White;
            this.glassButtonSelfAssesmentDoKnow.Name = "glassButtonSelfAssesmentDoKnow";
            this.glassButtonSelfAssesmentDoKnow.OuterBorderColor = System.Drawing.Color.Transparent;
            this.glassButtonSelfAssesmentDoKnow.ShowFocusBorder = true;
            this.glassButtonSelfAssesmentDoKnow.SpecialSymbol = MLifter.Components.GlassButton.SpecialSymbols.Shuffle;
            this.glassButtonSelfAssesmentDoKnow.SpecialSymbolColor = System.Drawing.Color.Empty;
            this.glassButtonSelfAssesmentDoKnow.SpecialSymbolColorDisabled = System.Drawing.Color.Empty;
            this.glassButtonSelfAssesmentDoKnow.SpecialSymbolColorEnabled = System.Drawing.Color.Empty;
            this.glassButtonSelfAssesmentDoKnow.SpecialSymbolTransperency = 255;
            this.glassButtonSelfAssesmentDoKnow.Click += new System.EventHandler(this.glassButtonSelfAssesmentDoKnow_Click);
            this.glassButtonSelfAssesmentDoKnow.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glassButtonSelfAssesmentDoKnow_KeyDown);
            // 
            // glassButtonNextCard
            // 
            this.glassButtonNextCard.AlternativeForm = true;
            this.glassButtonNextCard.AlternativeFormDirection = MLifter.Components.GlassButton.Direction.Right;
            resources.ApplyResources(this.glassButtonNextCard, "glassButtonNextCard");
            this.glassButtonNextCard.CornerRadius = 3;
            this.glassButtonNextCard.FocusColor = System.Drawing.Color.Empty;
            this.glassButtonNextCard.GlowColor = System.Drawing.Color.White;
            this.glassButtonNextCard.Name = "glassButtonNextCard";
            this.glassButtonNextCard.OuterBorderColor = System.Drawing.Color.Transparent;
            this.glassButtonNextCard.ShowFocusBorder = true;
            this.glassButtonNextCard.SpecialSymbol = MLifter.Components.GlassButton.SpecialSymbols.Forward;
            this.glassButtonNextCard.SpecialSymbolColor = System.Drawing.Color.Empty;
            this.glassButtonNextCard.SpecialSymbolColorDisabled = System.Drawing.Color.Empty;
            this.glassButtonNextCard.SpecialSymbolColorEnabled = System.Drawing.Color.Empty;
            this.glassButtonNextCard.SpecialSymbolTransperency = 255;
            this.glassButtonNextCard.Click += new System.EventHandler(this.glassButtonNextCard_Click);
            this.glassButtonNextCard.Leave += new System.EventHandler(this.glassButtonNextCard_Leave);
            // 
            // glassButtonSelfAssesmentDontKnow
            // 
            this.glassButtonSelfAssesmentDontKnow.AlternativeForm = true;
            resources.ApplyResources(this.glassButtonSelfAssesmentDontKnow, "glassButtonSelfAssesmentDontKnow");
            this.glassButtonSelfAssesmentDontKnow.CornerRadius = 30;
            this.glassButtonSelfAssesmentDontKnow.FocusColor = System.Drawing.Color.White;
            this.glassButtonSelfAssesmentDontKnow.GlowColor = System.Drawing.Color.White;
            this.glassButtonSelfAssesmentDontKnow.Name = "glassButtonSelfAssesmentDontKnow";
            this.glassButtonSelfAssesmentDontKnow.OuterBorderColor = System.Drawing.Color.Transparent;
            this.glassButtonSelfAssesmentDontKnow.ShowFocusBorder = true;
            this.glassButtonSelfAssesmentDontKnow.SpecialSymbol = MLifter.Components.GlassButton.SpecialSymbols.Shuffle;
            this.glassButtonSelfAssesmentDontKnow.SpecialSymbolColor = System.Drawing.Color.Empty;
            this.glassButtonSelfAssesmentDontKnow.SpecialSymbolColorDisabled = System.Drawing.Color.Empty;
            this.glassButtonSelfAssesmentDontKnow.SpecialSymbolColorEnabled = System.Drawing.Color.Empty;
            this.glassButtonSelfAssesmentDontKnow.SpecialSymbolTransperency = 255;
            this.glassButtonSelfAssesmentDontKnow.Click += new System.EventHandler(this.glassButtonSelfAssesmentDontKnow_Click);
            this.glassButtonSelfAssesmentDontKnow.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glassButtonSelfAssesmentDontKnow_KeyDown);
            // 
            // glassButtonPreviousCard
            // 
            this.glassButtonPreviousCard.AlternativeFocusBorderColor = System.Drawing.Color.White;
            this.glassButtonPreviousCard.AlternativeForm = true;
            resources.ApplyResources(this.glassButtonPreviousCard, "glassButtonPreviousCard");
            this.glassButtonPreviousCard.CornerRadius = 3;
            this.glassButtonPreviousCard.FocusColor = System.Drawing.Color.Empty;
            this.glassButtonPreviousCard.GlowColor = System.Drawing.Color.White;
            this.glassButtonPreviousCard.Name = "glassButtonPreviousCard";
            this.glassButtonPreviousCard.OuterBorderColor = System.Drawing.Color.Transparent;
            this.glassButtonPreviousCard.ShowFocusBorder = true;
            this.glassButtonPreviousCard.SpecialSymbol = MLifter.Components.GlassButton.SpecialSymbols.Backward;
            this.glassButtonPreviousCard.SpecialSymbolColor = System.Drawing.Color.Empty;
            this.glassButtonPreviousCard.SpecialSymbolColorDisabled = System.Drawing.Color.Empty;
            this.glassButtonPreviousCard.SpecialSymbolColorEnabled = System.Drawing.Color.Empty;
            this.glassButtonPreviousCard.SpecialSymbolTransperency = 255;
            this.glassButtonPreviousCard.Click += new System.EventHandler(this.glassButtonPreviousCard_Click);
            this.glassButtonPreviousCard.Leave += new System.EventHandler(this.glassButtonPreviousCard_Leave);
            // 
            // ButtonsPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.gradientPanelButtonControlBackground);
            this.Name = "ButtonsPanel";
            resources.ApplyResources(this, "$this");
            this.gradientPanelButtonControlBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MLifter.Components.GlassButton glassButtonNextCard;
        private MLifter.Components.GlassButton glassButtonPreviousCard;
        private MLifter.Components.GlassButton glassButtonSelfAssesmentDontKnow;
        private MLifter.Components.GlassButton glassButtonSelfAssesmentDoKnow;
        private MLifter.Components.GlassButton glassButtonQuestion;
        private MLifter.Components.GradientPanel gradientPanelButtonControlBackground;
        private System.Windows.Forms.Button buttonDummyFocus;
    }
}
