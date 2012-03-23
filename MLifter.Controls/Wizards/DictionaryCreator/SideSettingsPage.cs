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
using System.Globalization;

namespace MLifter.Controls.Wizards.DictionaryCreator
{
    public partial class SideSettingsPage : MLifter.WizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SideSettingsPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        public SideSettingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the answer title.
        /// </summary>
        /// <value>The answer title.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        public string AnswerTitle { get { return dictionaryCaptions.AnswerTitle; } set { dictionaryCaptions.AnswerTitle = value; } }

        /// <summary>
        /// Gets or sets the question title.
        /// </summary>
        /// <value>The question title.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        public string QuestionTitle { get { return dictionaryCaptions.QuestionTitle; } set { dictionaryCaptions.QuestionTitle = value; } }

        /// <summary>
        /// Gets or sets the answer culture.
        /// </summary>
        /// <value>The answer culture.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        public CultureInfo AnswerCulture { get { return dictionaryCaptions.AnswerCulture; } set { dictionaryCaptions.AnswerCulture = value; } }

        /// <summary>
        /// Gets or sets the question culture.
        /// </summary>
        /// <value>The question culture.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        public CultureInfo QuestionCulture { get { return dictionaryCaptions.QuestionCulture; } set { dictionaryCaptions.QuestionCulture = value; } }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/memo9sqf.htm");
        }
    }
}

