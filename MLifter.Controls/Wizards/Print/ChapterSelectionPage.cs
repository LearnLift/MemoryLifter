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

using MLifter.BusinessLayer;

namespace MLifter.Controls.Wizards.Print
{
    public partial class ChapterSelectionPage : MLifter.WizardPage
    {
        private Dictionary dic;
        /// <summary>
        /// Gets or sets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        public Dictionary Dictionary
        {
            get { return dic; }
            set { dic = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChapterSelectionPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        public ChapterSelectionPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the ChapterSelectionPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-27</remarks>
        private void ChapterSelectionPage_Load(object sender, EventArgs e)
        {
            chapterFrame.Prepare(dic);
        }

        public override bool GoNext()
        {
            (ParentWizard.Tag as PrintSettings).IDs.Clear();
            (ParentWizard.Tag as PrintSettings).IDs.AddRange(chapterFrame.SelChapters);

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

