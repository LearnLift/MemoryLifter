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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace MLifter.Controls
{
    public partial class DictionaryCaptions : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryCaptions"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        public DictionaryCaptions()
        {
            InitializeComponent();

            textBoxQuestionHeadline.Text = Properties.Resources.NEWDIC_QUESTION;
            textBoxAnswerHeadline.Text = Properties.Resources.NEWDIC_ANSWER;

            comboBoxAnswerCulture.Items.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures));
            comboBoxAnswerCulture.SelectedItem = System.Threading.Thread.CurrentThread.CurrentCulture;
            comboBoxQuestionCulture.Items.AddRange(CultureInfo.GetCultures(CultureTypes.AllCultures));
            comboBoxQuestionCulture.SelectedItem = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        /// Handles the Load event of the DictionaryCaptions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        private void DictionaryCaptions_Load(object sender, EventArgs e)
        {
            textBoxQuestionHeadline.Focus();
        }

        /// <summary>
        /// Handles the Enter event of the textBoxHeadline control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-09</remarks>
        private void textBoxHeadline_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string newDicText = textBox.Name.Contains("Question") ? Properties.Resources.NEWDIC_QUESTION : Properties.Resources.NEWDIC_ANSWER;

            if (textBox.Text == newDicText)
                textBox.SelectAll();
        }

        /// <summary>
        /// Gets or sets the answer title.
        /// </summary>
        /// <value>The answer title.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string AnswerTitle { get { return textBoxAnswerHeadline.Text; } set { textBoxAnswerHeadline.Text = value; } }

        /// <summary>
        /// Gets or sets the question title.
        /// </summary>
        /// <value>The question title.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string QuestionTitle { get { return textBoxQuestionHeadline.Text; } set { textBoxQuestionHeadline.Text = value; } }

        /// <summary>
        /// Gets or sets the answer culture.
        /// </summary>
        /// <value>The answer culture.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public CultureInfo AnswerCulture { get { return comboBoxAnswerCulture.SelectedItem as CultureInfo; } set { comboBoxAnswerCulture.SelectedItem = value; } }

        /// <summary>
        /// Gets or sets the question culture.
        /// </summary>
        /// <value>The question culture.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public CultureInfo QuestionCulture { get { return comboBoxQuestionCulture.SelectedItem as CultureInfo; } set { comboBoxQuestionCulture.SelectedItem = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [editable controls enabled].
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-28</remarks>
        [Browsable(false), ReadOnly(true)]
        public bool EditableControlsEnabled
        {
            get
            {
                return textBoxQuestionHeadline.Enabled;
            }
            set
            {
                textBoxQuestionHeadline.ReadOnly = textBoxAnswerHeadline.ReadOnly = !value;
                comboBoxQuestionCulture.Enabled = comboBoxAnswerCulture.Enabled = value;
            }
        }
    }
}
