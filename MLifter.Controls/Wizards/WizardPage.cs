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

namespace MLifter
{
    public partial class WizardPage : UserControl
    {
        private Wizard parentWizard;
        /// <summary>
        /// The wizard to which this page belongs.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public Wizard ParentWizard
        {
            get
            {
                return parentWizard != null ? parentWizard : FindForm() as Wizard;
            }
            set { parentWizard = value; }
        }

        private Image topImage;
        /// <summary>
        /// Gets or sets the top image - set <i>null</i> to hide.
        /// </summary>
        /// <value>The top image.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        [DefaultValue(null), Category("Appearance"), Description("This is the image, which will be displayed at the top of the page. Set to null to disable.")]
        public Image TopImage
        {
            get { return topImage; }
            set
            {
                topImage = value;
                pictureBoxTop.Image = topImage;
                pictureBoxTop.Visible = (topImage != null);
                Refresh();
            }
        }

        /// <summary>
        /// Gets the size of the top image frame.
        /// </summary>
        /// <value>The size of the top image.</value>
        /// <remarks>Documented by Dev02, 2008-01-14</remarks>
        public Size TopImageSize
        {
            get { return pictureBoxTop.Size; }
        }

        /// <summary>
        /// Gets or sets the top image size mode.
        /// </summary>
        /// <value>The top image size mode.</value>
        /// <remarks>Documented by Dev02, 2008-01-14</remarks>
        [DefaultValue(PictureBoxSizeMode.Normal), Category("Appearance"), Description("Set the picture box size mode of the image at the top of the page.")]
        public PictureBoxSizeMode TopImageSizeMode
        {
            get { return pictureBoxTop.SizeMode; }
            set { pictureBoxTop.SizeMode = value; }
        }

        private Image leftImage;
        /// <summary>
        /// Gets or sets the left image - set <i>null</i> to hide.
        /// </summary>
        /// <value>The left image.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        [DefaultValue(null), Category("Appearance"), Description("This is the image, which will be displayed at the left side of the page. Set to null to disable.")]
        public Image LeftImage
        {
            get { return leftImage; }
            set
            {
                leftImage = value;
                pictureBoxLeft.Image = leftImage;
                pictureBoxLeft.Visible = (leftImage != null);
                Refresh();
            }
        }

        /// <summary>
        /// Gets the size of the left image frame.
        /// </summary>
        /// <value>The size of the left image.</value>
        /// <remarks>Documented by Dev02, 2008-01-14</remarks>
        public Size LeftImageSize
        {
            get { return pictureBoxLeft.Size; }
        }

        /// <summary>
        /// Gets or sets the left image size mode.
        /// </summary>
        /// <value>The left image size mode.</value>
        /// <remarks>Documented by Dev02, 2008-01-14</remarks>
        [DefaultValue(PictureBoxSizeMode.Normal), Category("Appearance"), Description("Set the picture box size mode of the image at the left of the page.")]
        public PictureBoxSizeMode LeftImageSizeMode
        {
            get { return pictureBoxLeft.SizeMode; }
            set { pictureBoxLeft.SizeMode = value; }
        }

        private string headline = string.Empty;
        /// <summary>
        /// Gets or sets the headline, which is displayed at Top.
        /// </summary>
        /// <value>The headline.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        [DefaultValue(""), Category("Appearance"), Description("Gets or sets the headline, which is displayed at Top."), Localizable(true)]
        public string Headline
        {
            get { return headline; }
            set { headline = value; Refresh(); }
        }

        private bool helpAvailable = false;
        /// <summary>
        /// Gets or sets a value indicating whether a help available.
        /// </summary>
        /// <value><c>true</c> if [help available]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public bool HelpAvailable
        {
            get { return helpAvailable; }
            set { helpAvailable = value; OnPropertyChanged(WizardEventArgs.Empty); }
        }

        private bool backAllowed = true;
        /// <summary>
        /// Gets or sets a value indicating whether back is allowed.
        /// </summary>
        /// <value><c>true</c> if [back allowed]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public bool BackAllowed
        {
            get { return backAllowed; }
            set { backAllowed = value; OnPropertyChanged(WizardEventArgs.Empty); }
        }

        private bool nextAllowed = true;
        /// <summary>
        /// Gets or sets a value indicating whether next is allowed.
        /// </summary>
        /// <value><c>true</c> if [next allowed]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public bool NextAllowed
        {
            get { return nextAllowed; }
            set { nextAllowed = value; OnPropertyChanged(WizardEventArgs.Empty); }
        }

        private bool cancelAllowed = true;
        /// <summary>
        /// Gets or sets a value indicating whether cancel is allowed.
        /// </summary>
        /// <value><c>true</c> if [cancel allowed]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public bool CancelAllowed
        {
            get { return cancelAllowed; }
            set { cancelAllowed = value; OnPropertyChanged(WizardEventArgs.Empty); }
        }

        private bool lastStep = false;
        /// <summary>
        /// Gets or sets a value indicating whether this is the/a last step.
        /// </summary>
        /// <value><c>true</c> if [last step]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public bool LastStep
        {
            get { return lastStep; }
            set { lastStep = value; OnPropertyChanged(WizardEventArgs.Empty); }
        }

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public event EventHandler<WizardEventArgs> PropertyChanged;
        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        protected virtual void OnPropertyChanged(WizardEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        /// <summary>
        /// Occurs when a page is submitted.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public event EventHandler SubmitPage;
        /// <summary>
        /// Raises the <see cref="E:SubmitPage"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        protected virtual void OnSubmitPage(EventArgs e)
        {
            if (SubmitPage != null)
                SubmitPage(this, e);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardPage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public WizardPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the WizardPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        private void WizardPage_Load(object sender, EventArgs e)
        {
            pictureBoxTop.Visible = (TopImage != null);
            pictureBoxLeft.Visible = (LeftImage != null);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (pictureBoxTop.Visible)
            {
                Rectangle bottomRect = this.ClientRectangle;
                bottomRect.Y = 0;
                bottomRect.Height = pictureBoxTop.Height;
                ControlPaint.DrawBorder3D(e.Graphics, bottomRect, Border3DStyle.Etched, Border3DSide.Bottom);
            }

            if (headline.Length > 0)
                e.Graphics.DrawString(headline, new Font("Arial", 14), Brushes.Black, 10, 10);
        }

        /// <summary>
        /// Handles the Paint event of the pictureBoxTop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        private void pictureBoxTop_Paint(object sender, PaintEventArgs e)
        {
            if (headline.Length > 0)
                e.Graphics.DrawString(headline, new Font("Arial", 14), Brushes.Black, 10, 10);
        }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public virtual void ShowHelp() { }
        /// <summary>
        /// Called if the back-button is clicked.
        /// </summary>
        /// <returns><i>false</i> to abort, otherwise<i>true</i></returns>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public virtual bool GoBack() { return true; }
        /// <summary>
        /// Called if the next-button is clicked.
        /// </summary>
        /// <returns><i>false</i> to abort, otherwise<i>true</i></returns>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public virtual bool GoNext() { return true; }
        /// <summary>
        /// Called if the cancel-button is clicked.
        /// </summary>
        /// <returns><i>false</i> to abort, otherwise<i>true</i></returns>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        public virtual bool Cancel() { return true; }
    }
}
