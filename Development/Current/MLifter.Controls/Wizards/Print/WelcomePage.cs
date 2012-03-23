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

namespace MLifter.Controls.Wizards.Print
{
    public partial class WelcomePage : MLifter.WizardPage
    {
        public WizardPage BoxPage = new Wizards.Print.BoxSelectionPage();
        public WizardPage ChapterPage = new Wizards.Print.ChapterSelectionPage();
        public WizardPage IndividualPage = new Wizards.Print.IndividualSelectionPage();

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomePage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-21</remarks>
        public WelcomePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the WelcomePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        private void WelcomePage_Load(object sender, EventArgs e)
        {
            ParentWizard.Tag = new PrintSettings();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the radioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-21</remarks>
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RefreshPages();
        }

        /// <summary>
        /// Handles the Click event of the buttonDontUseWizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-21</remarks>
        private void buttonDontUseWizard_Click(object sender, EventArgs e)
        {
            ParentWizard.DialogResult = DialogResult.Ignore;
            ParentWizard.Close();
        }

        /// <summary>
        /// Refreshes the pages.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-12-21</remarks>
        private void RefreshPages()
        {
            if (radioButtonBoxes.Checked && !ParentWizard.Pages.Contains(BoxPage))
                ParentWizard.Pages.Insert(1, BoxPage);
            else if (!radioButtonBoxes.Checked && ParentWizard.Pages.Contains(BoxPage))
                ParentWizard.Pages.Remove(BoxPage);

            if (radioButtonChapters.Checked && !ParentWizard.Pages.Contains(ChapterPage))
                ParentWizard.Pages.Insert(1, ChapterPage);
            else if (!radioButtonChapters.Checked && ParentWizard.Pages.Contains(ChapterPage))
                ParentWizard.Pages.Remove(ChapterPage);

            if (radioButtonIndividual.Checked && !ParentWizard.Pages.Contains(IndividualPage))
                ParentWizard.Pages.Insert(1, IndividualPage);
            else if (!radioButtonIndividual.Checked && ParentWizard.Pages.Contains(IndividualPage))
                ParentWizard.Pages.Remove(IndividualPage);

            ParentWizard.UpdateButtonStates();
        }

        public override bool GoNext()
        {
            if (radioButtonAll.Checked)
                (ParentWizard.Tag as PrintSettings).Type = PrintType.All;
            else if (radioButtonChapters.Checked)
                (ParentWizard.Tag as PrintSettings).Type = PrintType.Chapter;
            else if (radioButtonBoxes.Checked)
                (ParentWizard.Tag as PrintSettings).Type = PrintType.Boxes;
            else if (radioButtonIndividual.Checked)
                (ParentWizard.Tag as PrintSettings).Type = PrintType.Individual;
            else
                (ParentWizard.Tag as PrintSettings).Type = PrintType.Wrong;

            return base.GoNext();
        }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/memo4f78.htm");
        }
    }
}

