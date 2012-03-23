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

namespace MLifter.AudioTools
{
    public partial class LayoutSelectorForm : Form
    {
        /// <summary>
        /// Gets or sets a value indicating whether [keyboard layout].
        /// </summary>
        /// <value><c>true</c> if [keyboard layout]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-05-26</remarks>
        public bool KeyboardLayout
        {
            get
            {
                return radioButtonKeyboard.Checked;
            }
            set
            {
                if (value)
                    radioButtonKeyboard.Checked = true;
                else
                    radioButtonNumPad.Checked = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ask again].
        /// </summary>
        /// <value><c>true</c> if [ask again]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-05-26</remarks>
        public bool AskAgain
        {
            get
            {
                return !checkBoxAskAgain.Checked;
            }
            set
             {
                checkBoxAskAgain.Checked = !value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSelectorForm"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-26</remarks>
        public LayoutSelectorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the buttonOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-26</remarks>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
