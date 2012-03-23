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

namespace MLifterAudioBookGenerator.Forms
{
    public partial class SilenceLength : Form
    {
        /// <summary>
        /// Gets or sets the length of the silence.
        /// </summary>
        /// <value>The length of the silence.</value>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        public double Length
        {
            get { return Double.Parse(textBoxLength.Text); }
            set { textBoxLength.Text = Convert.ToString(value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SilenceLength"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        public SilenceLength()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the FormClosing event of the SilenceLength control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        private void SilenceLength_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
                this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Handles the Click event of the buttonOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-03-30</remarks>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool inputvalid = true;
            try
            {
                if (double.Parse(textBoxLength.Text) <= 0)
                    inputvalid = false;
            }
            catch
            {
                inputvalid = false;
            }

            if (inputvalid)
            {
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
            else
            {
                MessageBox.Show("Your input is not in a valid format, please correct it.", "Valid value required.");
                return;
            }
        }
    }
}
