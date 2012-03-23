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
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Controls
{
    partial class PropertiesForm
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesForm));
            this.tabControlProperties = new System.Windows.Forms.TabControl();
            this.tabPageProperties = new System.Windows.Forms.TabPage();
            this.dictionaryProperties = new MLifter.Controls.Wizards.DictionaryCreator.WelcomePage();
            this.tabPageInformation = new System.Windows.Forms.TabPage();
            this.dictionaryInfos = new MLifter.Controls.Wizards.DictionaryCreator.InfoPage();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.dictionaryCaptions = new MLifter.Controls.DictionaryCaptions();
            this.tabPageLearningOptions = new System.Windows.Forms.TabPage();
            this.learnModes = new MLifter.Controls.LearnModes();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPageAdvanced = new System.Windows.Forms.TabPage();
            this.buttonAddEditStyle = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutSoundCommands = new System.Windows.Forms.TableLayoutPanel();
            this.buttonChange = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSoundFile = new System.Windows.Forms.TextBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.groupBoxAnswer = new System.Windows.Forms.GroupBox();
            this.listboxCommentaryAnswer = new System.Windows.Forms.ListBox();
            this.groupBoxQuestion = new System.Windows.Forms.GroupBox();
            this.listboxCommentaryQuestion = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.ToolTipControl = new System.Windows.Forms.ToolTip(this.components);
            this.MainHelp = new System.Windows.Forms.HelpProvider();
            this.tabControlProperties.SuspendLayout();
            this.tabPageProperties.SuspendLayout();
            this.tabPageInformation.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageLearningOptions.SuspendLayout();
            this.tabPageAdvanced.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutSoundCommands.SuspendLayout();
            this.groupBoxAnswer.SuspendLayout();
            this.groupBoxQuestion.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlProperties
            // 
            this.tabControlProperties.Controls.Add(this.tabPageProperties);
            this.tabControlProperties.Controls.Add(this.tabPageInformation);
            this.tabControlProperties.Controls.Add(this.tabPageGeneral);
            this.tabControlProperties.Controls.Add(this.tabPageLearningOptions);
            this.tabControlProperties.Controls.Add(this.tabPageAdvanced);
            resources.ApplyResources(this.tabControlProperties, "tabControlProperties");
            this.tabControlProperties.Name = "tabControlProperties";
            this.tabControlProperties.SelectedIndex = 0;
            this.MainHelp.SetShowHelp(this.tabControlProperties, ((bool)(resources.GetObject("tabControlProperties.ShowHelp"))));
            // 
            // tabPageProperties
            // 
            this.tabPageProperties.Controls.Add(this.dictionaryProperties);
            resources.ApplyResources(this.tabPageProperties, "tabPageProperties");
            this.tabPageProperties.Name = "tabPageProperties";
            this.MainHelp.SetShowHelp(this.tabPageProperties, ((bool)(resources.GetObject("tabPageProperties.ShowHelp"))));
            this.tabPageProperties.UseVisualStyleBackColor = true;
            // 
            // dictionaryProperties
            // 
            this.dictionaryProperties.BackAllowed = true;
            this.dictionaryProperties.CancelAllowed = true;
            this.dictionaryProperties.DictionaryLocationVisible = false;
            this.dictionaryProperties.DictionaryNameReadOnly = true;
            resources.ApplyResources(this.dictionaryProperties, "dictionaryProperties");
            this.dictionaryProperties.EditableControlsEnabled = true;
            this.dictionaryProperties.HelpAvailable = false;
            this.dictionaryProperties.LastStep = false;
            this.dictionaryProperties.LeftImage = ((System.Drawing.Image)(resources.GetObject("dictionaryProperties.LeftImage")));
            this.dictionaryProperties.Name = "dictionaryProperties";
            this.dictionaryProperties.NextAllowed = true;
            this.MainHelp.SetShowHelp(this.dictionaryProperties, ((bool)(resources.GetObject("dictionaryProperties.ShowHelp"))));
            this.dictionaryProperties.Title = "DictionaryName";
            // 
            // tabPageInformation
            // 
            this.tabPageInformation.Controls.Add(this.dictionaryInfos);
            resources.ApplyResources(this.tabPageInformation, "tabPageInformation");
            this.tabPageInformation.Name = "tabPageInformation";
            this.MainHelp.SetShowHelp(this.tabPageInformation, ((bool)(resources.GetObject("tabPageInformation.ShowHelp"))));
            this.tabPageInformation.UseVisualStyleBackColor = true;
            // 
            // dictionaryInfos
            // 
            this.dictionaryInfos.BackAllowed = true;
            this.dictionaryInfos.CancelAllowed = true;
            resources.ApplyResources(this.dictionaryInfos, "dictionaryInfos");
            this.dictionaryInfos.HelpAvailable = false;
            this.dictionaryInfos.LastStep = false;
            this.dictionaryInfos.LeftImage = ((System.Drawing.Image)(resources.GetObject("dictionaryInfos.LeftImage")));
            this.dictionaryInfos.Name = "dictionaryInfos";
            this.dictionaryInfos.NextAllowed = true;
            this.MainHelp.SetShowHelp(this.dictionaryInfos, ((bool)(resources.GetObject("dictionaryInfos.ShowHelp"))));
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.dictionaryCaptions);
            resources.ApplyResources(this.tabPageGeneral, "tabPageGeneral");
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.MainHelp.SetShowHelp(this.tabPageGeneral, ((bool)(resources.GetObject("tabPageGeneral.ShowHelp"))));
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // dictionaryCaptions
            // 
            resources.ApplyResources(this.dictionaryCaptions, "dictionaryCaptions");
            this.dictionaryCaptions.Name = "dictionaryCaptions";
            this.MainHelp.SetShowHelp(this.dictionaryCaptions, ((bool)(resources.GetObject("dictionaryCaptions.ShowHelp"))));
            // 
            // tabPageLearningOptions
            // 
            this.tabPageLearningOptions.Controls.Add(this.learnModes);
            this.tabPageLearningOptions.Controls.Add(this.label8);
            resources.ApplyResources(this.tabPageLearningOptions, "tabPageLearningOptions");
            this.tabPageLearningOptions.Name = "tabPageLearningOptions";
            this.MainHelp.SetShowHelp(this.tabPageLearningOptions, ((bool)(resources.GetObject("tabPageLearningOptions.ShowHelp"))));
            this.tabPageLearningOptions.UseVisualStyleBackColor = true;
            // 
            // learnModes
            // 
            this.learnModes.AnswerCaption = "Answer";
            this.learnModes.EditableControlsEnabled = true;
            resources.ApplyResources(this.learnModes, "learnModes");
            this.learnModes.MultipleChoiceOptionsVisible = false;
            this.learnModes.MultipleDirections = true;
            this.learnModes.Name = "learnModes";
            this.learnModes.QueryDirection = MLifter.DAL.Interfaces.EQueryDirection.Answer2Question;
            this.learnModes.QuestionCaption = "Question";
            this.MainHelp.SetShowHelp(this.learnModes, ((bool)(resources.GetObject("learnModes.ShowHelp"))));
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.MainHelp.SetShowHelp(this.label8, ((bool)(resources.GetObject("label8.ShowHelp"))));
            // 
            // tabPageAdvanced
            // 
            this.tabPageAdvanced.Controls.Add(this.buttonAddEditStyle);
            this.tabPageAdvanced.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.tabPageAdvanced, "tabPageAdvanced");
            this.tabPageAdvanced.Name = "tabPageAdvanced";
            this.MainHelp.SetShowHelp(this.tabPageAdvanced, ((bool)(resources.GetObject("tabPageAdvanced.ShowHelp"))));
            this.tabPageAdvanced.UseVisualStyleBackColor = true;
            // 
            // buttonAddEditStyle
            // 
            resources.ApplyResources(this.buttonAddEditStyle, "buttonAddEditStyle");
            this.buttonAddEditStyle.Name = "buttonAddEditStyle";
            this.MainHelp.SetShowHelp(this.buttonAddEditStyle, ((bool)(resources.GetObject("buttonAddEditStyle.ShowHelp"))));
            this.buttonAddEditStyle.UseVisualStyleBackColor = true;
            this.buttonAddEditStyle.Click += new System.EventHandler(this.buttonStyle_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutSoundCommands);
            this.groupBox1.Controls.Add(this.groupBoxAnswer);
            this.groupBox1.Controls.Add(this.groupBoxQuestion);
            this.groupBox1.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.MainHelp.SetShowHelp(this.groupBox1, ((bool)(resources.GetObject("groupBox1.ShowHelp"))));
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutSoundCommands
            // 
            resources.ApplyResources(this.tableLayoutSoundCommands, "tableLayoutSoundCommands");
            this.tableLayoutSoundCommands.Controls.Add(this.buttonChange, 3, 0);
            this.tableLayoutSoundCommands.Controls.Add(this.buttonDelete, 4, 0);
            this.tableLayoutSoundCommands.Controls.Add(this.label1, 0, 0);
            this.tableLayoutSoundCommands.Controls.Add(this.textBoxSoundFile, 1, 0);
            this.tableLayoutSoundCommands.Controls.Add(this.buttonPlay, 2, 0);
            this.tableLayoutSoundCommands.Name = "tableLayoutSoundCommands";
            this.MainHelp.SetShowHelp(this.tableLayoutSoundCommands, ((bool)(resources.GetObject("tableLayoutSoundCommands.ShowHelp"))));
            // 
            // buttonChange
            // 
            this.buttonChange.Image = global::MLifter.Controls.Properties.Resources.mediaRecord;
            resources.ApplyResources(this.buttonChange, "buttonChange");
            this.buttonChange.Name = "buttonChange";
            this.MainHelp.SetShowHelp(this.buttonChange, ((bool)(resources.GetObject("buttonChange.ShowHelp"))));
            this.ToolTipControl.SetToolTip(this.buttonChange, resources.GetString("buttonChange.ToolTip"));
            this.buttonChange.UseVisualStyleBackColor = true;
            this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Image = global::MLifter.Controls.Properties.Resources.delete;
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.MainHelp.SetShowHelp(this.buttonDelete, ((bool)(resources.GetObject("buttonDelete.ShowHelp"))));
            this.ToolTipControl.SetToolTip(this.buttonDelete, resources.GetString("buttonDelete.ToolTip"));
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.MainHelp.SetShowHelp(this.label1, ((bool)(resources.GetObject("label1.ShowHelp"))));
            // 
            // textBoxSoundFile
            // 
            resources.ApplyResources(this.textBoxSoundFile, "textBoxSoundFile");
            this.textBoxSoundFile.Name = "textBoxSoundFile";
            this.textBoxSoundFile.ReadOnly = true;
            this.MainHelp.SetShowHelp(this.textBoxSoundFile, ((bool)(resources.GetObject("textBoxSoundFile.ShowHelp"))));
            this.ToolTipControl.SetToolTip(this.textBoxSoundFile, resources.GetString("textBoxSoundFile.ToolTip"));
            // 
            // buttonPlay
            // 
            this.buttonPlay.Image = global::MLifter.Controls.Properties.Resources.mediaPlaybackStart;
            resources.ApplyResources(this.buttonPlay, "buttonPlay");
            this.buttonPlay.Name = "buttonPlay";
            this.MainHelp.SetShowHelp(this.buttonPlay, ((bool)(resources.GetObject("buttonPlay.ShowHelp"))));
            this.ToolTipControl.SetToolTip(this.buttonPlay, resources.GetString("buttonPlay.ToolTip"));
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // groupBoxAnswer
            // 
            this.groupBoxAnswer.Controls.Add(this.listboxCommentaryAnswer);
            resources.ApplyResources(this.groupBoxAnswer, "groupBoxAnswer");
            this.groupBoxAnswer.Name = "groupBoxAnswer";
            this.MainHelp.SetShowHelp(this.groupBoxAnswer, ((bool)(resources.GetObject("groupBoxAnswer.ShowHelp"))));
            this.groupBoxAnswer.TabStop = false;
            // 
            // listboxCommentaryAnswer
            // 
            resources.ApplyResources(this.listboxCommentaryAnswer, "listboxCommentaryAnswer");
            this.listboxCommentaryAnswer.Items.AddRange(new object[] {
            resources.GetString("listboxCommentaryAnswer.Items"),
            resources.GetString("listboxCommentaryAnswer.Items1"),
            resources.GetString("listboxCommentaryAnswer.Items2"),
            resources.GetString("listboxCommentaryAnswer.Items3"),
            resources.GetString("listboxCommentaryAnswer.Items4"),
            resources.GetString("listboxCommentaryAnswer.Items5")});
            this.listboxCommentaryAnswer.Name = "listboxCommentaryAnswer";
            this.MainHelp.SetShowHelp(this.listboxCommentaryAnswer, ((bool)(resources.GetObject("listboxCommentaryAnswer.ShowHelp"))));
            this.listboxCommentaryAnswer.SelectedIndexChanged += new System.EventHandler(this.listbox_SelectedIndexChanged);
            this.listboxCommentaryAnswer.DoubleClick += new System.EventHandler(this.listboxCommentary_DoubleClick);
            // 
            // groupBoxQuestion
            // 
            this.groupBoxQuestion.Controls.Add(this.listboxCommentaryQuestion);
            resources.ApplyResources(this.groupBoxQuestion, "groupBoxQuestion");
            this.groupBoxQuestion.Name = "groupBoxQuestion";
            this.MainHelp.SetShowHelp(this.groupBoxQuestion, ((bool)(resources.GetObject("groupBoxQuestion.ShowHelp"))));
            this.groupBoxQuestion.TabStop = false;
            // 
            // listboxCommentaryQuestion
            // 
            resources.ApplyResources(this.listboxCommentaryQuestion, "listboxCommentaryQuestion");
            this.listboxCommentaryQuestion.Items.AddRange(new object[] {
            resources.GetString("listboxCommentaryQuestion.Items"),
            resources.GetString("listboxCommentaryQuestion.Items1"),
            resources.GetString("listboxCommentaryQuestion.Items2"),
            resources.GetString("listboxCommentaryQuestion.Items3"),
            resources.GetString("listboxCommentaryQuestion.Items4"),
            resources.GetString("listboxCommentaryQuestion.Items5")});
            this.listboxCommentaryQuestion.Name = "listboxCommentaryQuestion";
            this.MainHelp.SetShowHelp(this.listboxCommentaryQuestion, ((bool)(resources.GetObject("listboxCommentaryQuestion.ShowHelp"))));
            this.listboxCommentaryQuestion.SelectedIndexChanged += new System.EventHandler(this.listbox_SelectedIndexChanged);
            this.listboxCommentaryQuestion.DoubleClick += new System.EventHandler(this.listboxCommentary_DoubleClick);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.MainHelp.SetShowHelp(this.label6, ((bool)(resources.GetObject("label6.ShowHelp"))));
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.MainHelp.SetShowHelp(this.buttonOk, ((bool)(resources.GetObject("buttonOk.ShowHelp"))));
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.MainHelp.SetShowHelp(this.buttonCancel, ((bool)(resources.GetObject("buttonCancel.ShowHelp"))));
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // OpenDialog
            // 
            resources.ApplyResources(this.OpenDialog, "OpenDialog");
            // 
            // PropertiesForm
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.tabControlProperties);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainHelp.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.MainHelp.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesForm";
            this.MainHelp.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.tabControlProperties.ResumeLayout(false);
            this.tabPageProperties.ResumeLayout(false);
            this.tabPageInformation.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageLearningOptions.ResumeLayout(false);
            this.tabPageAdvanced.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutSoundCommands.ResumeLayout(false);
            this.tableLayoutSoundCommands.PerformLayout();
            this.groupBoxAnswer.ResumeLayout(false);
            this.groupBoxQuestion.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TabControl tabControlProperties;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageAdvanced;
        private System.Windows.Forms.TabPage tabPageProperties;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listboxCommentaryAnswer;
        private System.Windows.Forms.OpenFileDialog OpenDialog;
        private ToolTip ToolTipControl;
        private TabPage tabPageLearningOptions;
        private Label label8;
        private MLifter.Controls.LearnModes learnModes;
        private MLifter.Controls.DictionaryCaptions dictionaryCaptions;
        private GroupBox groupBox1;
        private GroupBox groupBoxAnswer;
        private ListBox listboxCommentaryQuestion;
        private GroupBox groupBoxQuestion;
        private Button buttonPlay;
        private Label label1;
        private TextBox textBoxSoundFile;
        private Button buttonDelete;
        private Button buttonChange;
        private MLifter.Controls.Wizards.DictionaryCreator.WelcomePage dictionaryProperties;
        private TabPage tabPageInformation;
        private MLifter.Controls.Wizards.DictionaryCreator.InfoPage dictionaryInfos;
        private HelpProvider MainHelp;
        private Button buttonAddEditStyle;
        private TableLayoutPanel tableLayoutSoundCommands;
    }
}
