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

namespace MLifter.Controls
{
    public partial class LinklabelButton : UserControl, IButtonControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinklabelButton"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        public LinklabelButton()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(this.Text))
                this.Text = this.GetType().Name;

            this.Appearance = LinklabelButtonAppearance.Button;
        }

        private LinklabelButtonAppearance appearance = LinklabelButtonAppearance.Button;

        /// <summary>
        /// Gets or sets the appearance.
        /// </summary>
        /// <value>The appearance.</value>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        [Category("Appearance"), DefaultValue(typeof(MLifter.Controls.LinklabelButton.LinklabelButtonAppearance), "Button"), Description("Switches through the different appearance modes of the control.")]
        public LinklabelButtonAppearance Appearance
        {
            get
            {
                return appearance;
            }
            set
            {
                if (value == LinklabelButtonAppearance.Button)
                {
                    button.BringToFront();
                    button.Visible = true;
                    linkLabel.Visible = false;
                }
                else
                {
                    linkLabel.BringToFront();
                    linkLabel.Visible = true;
                    button.Visible = false;
                }
                appearance = value;
            }
        }

        /// <summary>
        /// The appearance of the control.
        /// </summary>
        public enum LinklabelButtonAppearance
        {
            Button,
            Linklabel
        }

        /// <summary>
        /// The Text associated with this control.
        /// </summary>
        /// <value>The Text associated with this control.</value>
        /// <returns>The Text associated with this control.</returns>
        /// <remarks>Documented by Dev02, 2008-06-11</remarks>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        [Category("Appearance"), ReadOnly(false), Browsable(true), Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return Appearance == LinklabelButtonAppearance.Button ? button.Text : linkLabel.Text;
            }
            set
            {
                button.Text = linkLabel.Text = value;
            }
        }

        /// <summary>
        /// Handles the LinkClicked event of the linkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OnClick(EventArgs.Empty);
        }

        /// <summary>
        /// Handles the Click event of the button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        private void button_Click(object sender, EventArgs e)
        {
            OnClick(EventArgs.Empty);
        }

        /// <summary>
        /// Gets or sets the color used when displaying a normal link. 
        /// </summary>
        /// <value>The color of the link.</value>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        [Category("Appearance"), Browsable(true), ReadOnly(false), Description("Gets or sets the color used when displaying a normal link."), DefaultValue(typeof(Color), "0,0,255")]
        public Color LinkColor
        {
            get { return linkLabel.LinkColor; }
            set { linkLabel.LinkColor = value; }
        }

        /// <summary>
        /// Gets or sets the color used to display an active link. 
        /// </summary>
        /// <value>The color of the active link.</value>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        [Category("Appearance"), Browsable(true), ReadOnly(false), Description("Gets or sets the color used to display an active link. "), DefaultValue(typeof(Color), "255,0,0")]
        public Color ActiveLinkColor
        {
            get { return linkLabel.ActiveLinkColor; }
            set { linkLabel.ActiveLinkColor = value; }
        }

        /// <summary>
        /// Gets or sets the color used when displaying a link that that has been previously visited. 
        /// </summary>
        /// <value>The color of the visited link.</value>
        /// <remarks>Documented by Dev02, 2009-03-05</remarks>
        [Category("Appearance"), Browsable(true), ReadOnly(false), Description("Gets or sets the color used when displaying a link that that has been previously visited."), DefaultValue(typeof(Color), "96,100,32")]
        public Color VisitedLinkColor
        {
            get { return linkLabel.VisitedLinkColor; }
            set { linkLabel.VisitedLinkColor = value; }
        }


        #region IButtonControl Members

        /// <summary>
        /// Gets or sets the value returned to the parent form when the button is clicked.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Windows.Forms.DialogResult"/> values.
        /// </returns>
        /// <remarks>Documented by Dev02, 2009-04-16</remarks>
        public DialogResult DialogResult
        {
            get;
            set;
        }

        /// <summary>
        /// Notifies a control that it is the default button so that its appearance and behavior is adjusted accordingly.
        /// </summary>
        /// <param name="value">true if the control should behave as a default button; otherwise false.</param>
        /// <remarks>Documented by Dev02, 2009-04-16</remarks>
        public void NotifyDefault(bool value)
        {
            //the link label does not support this
            button.NotifyDefault(value);
        }

        /// <summary>
        /// Generates a <see cref="E:System.Windows.Forms.Control.Click"/> event for the control.
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-04-16</remarks>
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        #endregion
    }
}
