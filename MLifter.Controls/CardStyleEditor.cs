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

using MLifter.DAL;
using MLifter.DAL.Interfaces;
using MLifter.BusinessLayer;

namespace MLifter.Controls
{
    public partial class CardStyleEditor : Form
    {
        public CardStyleEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the help namespace.
        /// </summary>
        /// <value>The help namespace.</value>
        /// <remarks>Documented by Dev02, 2008-03-07</remarks>
        public string HelpNamespace
        {
            get { return MainHelp.HelpNamespace; }
            set { MainHelp.HelpNamespace = value; }
        }

        /// <summary>
        /// Loads the style.
        /// </summary>
        /// <param name="actualCard">The actual card.</param>
        /// <param name="styleToEdit">The style to edit.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        public void LoadStyle(ICard actualCard, ICardStyle styleToEdit, Dictionary dictionary, object elementToStyle)
        {
            cardStyleEdit.Initialize(actualCard, styleToEdit, dictionary, elementToStyle);
        }

        /// <summary>
        /// Handles the Click event of the buttonSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the Click event of the buttonClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the FormClosing event of the CardStyleEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void CardStyleEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                cardStyleEdit.SaveChanges(true);
            else
                cardStyleEdit.RollbackChanges();
        }

        /// <summary>
        /// Handles the Resize event of the CardStyleEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-10-31</remarks>
        private void CardStyleEditor_Resize(object sender, EventArgs e)
        {
            cardStyleEdit.Height = Height - 65;
        }

        private void buttonClearStyles_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
    }
}
