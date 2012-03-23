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
using System.Globalization;

namespace MLifter.AudioTools
{
    public partial class LanguageForm : Form
    {
        private DictionaryManagement dictionaryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageForm"/> class.
        /// </summary>
        /// <param name="dictionaryManager">The dictionary manager.</param>
        /// <remarks>Documented by Dev05, 2007-08-22</remarks>
        public LanguageForm(DictionaryManagement dictionaryManager)
        {
            this.dictionaryManager = dictionaryManager;
            InitializeComponent();

            LoadLanguages();
        }

        /// <summary>
        /// Loads the languages.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-09-11</remarks>
        private void LoadLanguages()
        {
            comboBoxAnswerCulture.Items.Clear();
            comboBoxQuestionCulture.Items.Clear();

            comboBoxAnswerCulture.Items.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures));
            comboBoxAnswerCulture.SelectedItem = dictionaryManager.AnswerCulture;
            comboBoxQuestionCulture.Items.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures));
            comboBoxQuestionCulture.SelectedItem = dictionaryManager.QuestionCulture;
        }

        /// <summary>
        /// Handles the Click event of the buttonOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-08-22</remarks>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            dictionaryManager.AnswerCulture = comboBoxAnswerCulture.SelectedItem as CultureInfo;
            dictionaryManager.QuestionCulture = comboBoxQuestionCulture.SelectedItem as CultureInfo;

            Close();
        }
    }
}
