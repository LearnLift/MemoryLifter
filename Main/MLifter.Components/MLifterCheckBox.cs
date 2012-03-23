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
using MLifter.Components.Properties;
using System.Net;

namespace MLifter.Components
{
    public partial class MLifterCheckBox : UserControl
    {

        #region properties
        private int number;
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>The number.</value>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        [Category("Appearance"), Description("The number of the Button."), DefaultValue(1)]
        public int Number
        {
            get { return number; }
            set
            {
                number = value;
                checkButtonNumber.Text = "&" + value + ".";
            }
        }
        private Image imageChecked;
        [DefaultValue(typeof(Image), null), Category("Appearance-CheckedImage"), Description("The Image for the checked control")]
        public Image ImageChecked
        {
            get { return imageChecked; }
            set
            {

                imageChecked = value;
                checkButtonNumber.ImageChecked = value;
            }
        }
        private Image backGroundCheckBox;
        [DefaultValue(typeof(Image), null), Category("Appearance-CheckedImage"), Description("The Image for the checked control")]
        public Image BackGroundCheckBox
        {
            get { return backGroundCheckBox; }
            set
            {
                backGroundCheckBox = value;
                checkButtonNumber.BackGroundCheckBox = value;
            }
        }
        /// <summary>
        /// The text associated with this control.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        [Category("Appearance"), Description("The text associated with this control."), DefaultValue("MLifterCheckBox")]
        public override string Text
        {
            get { return WebUtility.HtmlEncode(labelText.Text); }
            set { labelText.Text = WebUtility.HtmlDecode(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MLifterCheckBox"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        [Category("Appearance"), Description("Gets or sets a value indicating whether this MLifterCheckBox is checked."), DefaultValue(false)]
        public bool Checked { get { return checkButtonNumber.Checked; } set { checkButtonNumber.Checked = value; } }
        #endregion properties

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MLifterCheckBox"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        public MLifterCheckBox()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();
        }
        #endregion constructor

        #region checked_CheckButton
        /// <summary>
        /// Occurs when checked changed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        public event EventHandler CheckedChanged;
        /// <summary>
        /// Raises the <see cref="E:CheckedChange"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        protected virtual void OnCheckedChange(EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged(this, e);
        }
        /// <summary>
        /// Handles the CheckedChanged event of the checkButtonNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-14</remarks>
        private void checkButtonNumber_CheckedChanged(object sender, EventArgs e)
        {
            OnCheckedChange(e);
        }
        #endregion checked_CheckButton

        #region clickEvents

        /// <summary>
        /// Handles the Click event of the labelText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-15</remarks>
        private void labelText_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
        }

        /// <summary>
        /// Handles the Click event of the MLifterCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-15</remarks>
        private void MLifterCheckBox_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
        }
        #endregion clickEvents

        #region keyEvents
        /// <summary>
        /// Handles the KeyUp event of the checkButtonNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-04-16</remarks>
        private void checkButtonNumber_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }
        /// <summary>
        /// Handles the KeyDown event of the checkButtonNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2009-04-16</remarks>
        private void checkButtonNumber_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }
        #endregion keyEvents

        #region override
        /// <summary>
        /// Retrieves the size of a rectangular area into which a control can be fitted.
        /// </summary>
        /// <param name="proposedSize">The custom-sized area for a control.</param>
        /// <returns>
        /// An ordered pair of type <see cref="T:System.Drawing.Size"/> representing the width and height of a rectangle.
        /// </returns>
        /// <remarks>Documented by Dev05, 2009-04-15</remarks>
        public override Size GetPreferredSize(Size proposedSize)
        {
            Size labelSize = labelText.GetPreferredSize(proposedSize);

            return new Size(labelSize.Width + Width - labelText.Width, labelSize.Height + Height - labelText.Height);
        }
        #endregion override

        #region alginmentChanged
        /// <summary>
        /// Handles the RightToLeftChanged event of the MLifterCheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-05-04</remarks>
        private void MLifterCheckBox_RightToLeftChanged(object sender, EventArgs e)
        {
            checkButtonNumber.Anchor = AnchorStyles.None;
            checkButtonNumber.Left = RightToLeft == RightToLeft.Yes ? Width - checkButtonNumber.Width - 10 : 10;
            checkButtonNumber.Anchor = RightToLeft == RightToLeft.Yes ? AnchorStyles.Right : AnchorStyles.Left;

            labelText.Anchor = AnchorStyles.None;
            labelText.Left = RightToLeft == RightToLeft.Yes ? 10 : checkButtonNumber.Width + 20;
            labelText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }
        #endregion alginmentChanged
    }
}
