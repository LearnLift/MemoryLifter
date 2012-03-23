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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MLifter.DAL;
using MLifter.Properties;
using MLifter.BusinessLayer;
using MLifter.Controls;

namespace MLifter
{
    public partial class StyleEditor : Form
    {
        private string current_Style;
        private string current_Default;
        private string backup_Style;
        private string backup_Default;

        private const string m_BackupString = "_Original";
        private const string m_GlobalStyle = "style.xsl";
        private const string m_SystemFolder = "System";

        private int card_id;

        /// <summary>
        /// Constructor of class
        /// </summary>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-19</remarks>
        public StyleEditor()
        {
            InitializeComponent();
            MLifter.Classes.Help.SetHelpNameSpace(MainHelp);
        }

        private static MLifter.BusinessLayer.Dictionary Dictionary
        {
            get
            {
                return MainForm.LearnLogic.Dictionary;
            }
        }

        private void Preview()
        {
            xslStyleEdit.Display = (radioButtonQuestion.Checked ? DisplayTyp.Question : rbCorrect.Checked ? DisplayTyp.AnswerCorrect : DisplayTyp.AnswerWrong);
        }

        /// <summary>
        /// Loads selected answer and question for selected card and displays as well as previews stylesheet
        /// </summary>
        /// <param name="sender">Number of selected card</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-20</remarks>
        public void EditXSL(int selected_card)
        {
            card_id = selected_card;

            string path = Path.GetDirectoryName(MainForm.styleHandler.CurrentStyle.QuestionStylesheetPath);
            current_Style = Path.Combine(path, m_GlobalStyle);
            xslStyleEdit.InitializeFile(current_Style, selected_card, Dictionary);
            current_Default = Path.Combine(path.Remove(path.LastIndexOf(Path.DirectorySeparatorChar)), Path.Combine(m_SystemFolder, m_GlobalStyle));

            if (!File.Exists(current_Style) || !File.Exists(current_Default))
            {
                MessageBox.Show(Resources.STYLESHEET_NOT_AVAILABLE_TEXT, Resources.STYLESHEET_NOT_AVAILABLE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            backup_Style = Path.Combine(Path.GetDirectoryName(current_Style), Path.GetFileNameWithoutExtension(current_Style) + m_BackupString + Path.GetExtension(current_Style));
            backup_Default = Path.Combine(Path.GetDirectoryName(current_Default), Path.GetFileNameWithoutExtension(current_Default) + m_BackupString + Path.GetExtension(current_Default));

            if (!File.Exists(backup_Style))
                File.Copy(current_Style, backup_Style);
            if (!File.Exists(backup_Default))
                File.Copy(current_Default, backup_Default);

            selectStylesheet.Items.Add(Resources.XSL_STYLE);
            selectStylesheet.Items.Add(Resources.XSL_DEFAULT);
            selectStylesheet.SelectedIndex = 0;
            this.ShowDialog();
        }

        /// <summary>
        /// Saves stylesheet of question or answer and closes dialog
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-20</remarks>        
        private void btnClose_Click(object sender, EventArgs e)
        {
            xslStyleEdit.IsClosing = true;
            xslStyleEdit.Save();

            this.Close();
        }

        /// <summary>
        /// If index has changed current answer/question is saved and new question/answer is loaded. Afterwards redirection to button preview
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-20</remarks>        
        private void selectStylesheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectStylesheet.SelectedIndex == 0)
                xslStyleEdit.InitializeFile(current_Style, card_id, Dictionary);
            else
                xslStyleEdit.InitializeFile(current_Default, card_id, Dictionary);
            Preview();
        }

        /// <summary>
        /// Restores default values
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-20</remarks>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            File.Copy(backup_Default, current_Default, true);
            File.Copy(backup_Style, current_Style, true);
            selectStylesheet_SelectedIndexChanged(this, null);
        }

        /// <summary>
        /// If selection of radiobuttons has changed -> redirection to button preview
        /// </summary>
        /// <param name="sender">Sender of object</param>
        /// <param name="e">Contains event data</param>
        /// <returns>No return value</returns>
        /// <exceptions>Does not throw any exception.</exceptions>
        /// <remarks>Documented by Dev00, 2007-07-20</remarks>        
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Preview();
        }

        /// <summary>
        /// Handles the FormClosed event of the StyleEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-08</remarks>
        private void XSLPreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            //File.Delete(backup_Style);
            //File.Delete(backup_Default);
        }

        private void StyleEditor_Shown(object sender, EventArgs e)
        {
            Preview();
        }
    }
}
