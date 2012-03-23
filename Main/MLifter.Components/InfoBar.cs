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

namespace MLifter.Components
{
    /// <summary>
    /// A user control to display messages in a browser-like infobar.
    /// </summary>
    /// <remarks>Documented by Dev02, 2008-05-07</remarks>
    public partial class InfoBar : UserControl
    {
        private Control additionalSuspendControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoBar"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        public InfoBar()
        {
            Initialize();
            this.Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoBar"/> class.
        /// </summary>
        /// <param name="text">The text to set.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="docking">The docking style.</param>
        /// <param name="suspendParentControlsLayout">if set to <c>true</c> [suspend parent controls layout] to improve animation performance.</param>
        /// <param name="dontShowAgainVisible">if set to <c>true</c> [dont show again visible].</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        public InfoBar(string text, Control parent, DockStyle docking, bool suspendParentControlsLayout, bool dontShowAgainVisible)
            : this(text, parent, docking, suspendParentControlsLayout, dontShowAgainVisible, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoBar"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="docking">The docking.</param>
        /// <param name="suspendParentControlsLayout">if set to <c>true</c> [suspend parent controls layout].</param>
        /// <param name="dontShowAgainVisible">if set to <c>true</c> [dont show again visible].</param>
        /// <param name="additionalControlToSuspendLayout">The additional control to suspend layout.</param>
        /// <remarks>Documented by Dev05, 2009-04-15</remarks>
        public InfoBar(string text, Control parent, DockStyle docking, bool suspendParentControlsLayout, bool dontShowAgainVisible, Control additionalControlToSuspendLayout)
        {
            //check if parent already contains an infobar with the same text
            foreach (Control control in parent.Controls)
                if (control is InfoBar && control.Visible && control.Text == text)
                    return;

            additionalSuspendControl = additionalControlToSuspendLayout;

            this.SuspendParentControlsLayout = suspendParentControlsLayout;

            Initialize();
            parent.Controls.Add(this);
            this.Parent = parent;
            this.Text = text;
            this.DontShowAgainVisible = dontShowAgainVisible;

            this.Dock = docking;
            Animate(true);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        private void Initialize()
        {
            InitializeComponent();
            DontShowAgainVisible = false; //hide don't show again checkbox per default
            labelInfo.TextChanged += new EventHandler(labelInfo_TextChanged);
            labelInfo.FontChanged += new EventHandler(labelInfo_FontChanged);
            this.SizeChanged += new EventHandler(InfoBar_SizeChanged);
            buttonClose.Click += new EventHandler(buttonClose_Click);
        }

        /// <summary>
        /// Gets or sets the notification message, which the control should show.
        /// </summary>
        /// <value></value>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        [DefaultValue(""), Category("Appearance")]
        public override string Text
        {
            get
            {
                return labelInfo.Text;
            }
            set
            {
                labelInfo.Text = value;
                Visible = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the labelInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        void labelInfo_TextChanged(object sender, EventArgs e)
        {
            CheckTextSize();
        }

        /// <summary>
        /// Handles the FontChanged event of the labelInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        void labelInfo_FontChanged(object sender, EventArgs e)
        {
            CheckTextSize();
        }

        /// <summary>
        /// Handles the SizeChanged event of the InfoBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        void InfoBar_SizeChanged(object sender, EventArgs e)
        {
            CheckTextSize();
        }

        /// <summary>
        /// Checks the size of the text.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        private void CheckTextSize()
        {
            if (!string.IsNullOrEmpty(labelInfo.Text) && !timerAnimation.Enabled && this.Height != 0 && labelInfo.Height != 0)
            {
                this.Height = GetNecessaryTextHeight();
            }
        }

        /// <summary>
        /// Gets the height of the necessary text.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev02, 2008-05-21</remarks>
        private int GetNecessaryTextHeight()
        {
            //check if text is too long
            if (labelInfo.Width == 0)
                return 0;

            return labelInfo.GetPreferredSize(new Size(labelInfo.Width, 0)).Height + 8;
        }

        /// <summary>
        /// Occurs when [infobar was closed].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        public event EventHandler Closed;

        /// <summary>
        /// Raises the <see cref="E:Closed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        private void OnClosed(EventArgs e)
        {
            if (Closed != null)
                Closed(this, e);
        }

        /// <summary>
        /// Handles the Click event of the buttonClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        void buttonClose_Click(object sender, EventArgs e)
        {
            Animate(false);
            OnClosed(EventArgs.Empty);
        }

        DateTime animationStartTime;
        /// <summary>
        /// Starts the specified animation (in or out).
        /// </summary>
        /// <param name="animateIn">if set to <c>true</c> [animate in], else [animate out].</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        public void Animate(bool animateIn)
        {
            if (timerAnimation.Enabled)
            {
                timerAnimation.Stop();
                ResumeParentControls();
            }

            animationStartTime = DateTime.Now;

            SuspendParentControls(); //important to avoid flicker during docking/start of animation

            //start the animation
            if (animateIn)
            {
                //animate in
                timerAnimation.Tag = 1; //target direction
                this.Height = 0;
                this.Visible = true;
            }
            else
            {
                //animate out
                timerAnimation.Tag = -1; //target direction
            }
            timerAnimation.Start();
        }


        /// <summary>
        /// Delta Pixel for the animation.
        /// </summary>
        private static readonly int delta = 3;
        private int GetActualHeight()
        {
            if (timerAnimation == null || !(timerAnimation.Tag is int))
                return 0;

            int target = GetNecessaryTextHeight();
            int actual = 0;
            if ((int)timerAnimation.Tag <= 0)
                actual = Convert.ToInt32(target - (delta * (DateTime.Now - animationStartTime).TotalMilliseconds / timerAnimation.Interval));
            else
                actual = Convert.ToInt32(delta * (DateTime.Now - animationStartTime).TotalMilliseconds / timerAnimation.Interval);

            return actual > 0 ? actual < target ? actual : target : 0;
        }

        /// <summary>
        /// Handles the Tick event of the timerFade control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-07</remarks>
        private void timerFade_Tick(object sender, EventArgs e)
        {
            if (timerAnimation != null && timerAnimation.Tag is int)
            {
                int target = (int)timerAnimation.Tag;
                if (target > 0)
                {
                    //animate in
                    if (this.Height + delta >= GetNecessaryTextHeight())
                    {
                        timerAnimation.Stop();
                        CheckTextSize();
                        ResumeParentControls();
                    }
                    else
                        this.Height = GetActualHeight();
                }
                else
                {
                    //animate out
                    if ((this.Height = GetActualHeight()) <= 0)
                    {
                        timerAnimation.Stop();
                        this.Height = 0;
                        this.Visible = false;
                        ResumeParentControls(); //important to be before dispose
                        this.Dispose();
                    }
                }
            }
        }

        private int parentControlsLayoutSuspended = 0;

        /// <summary>
        /// Suspends the layout of all children of the parent (siblings) to improve performance.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-21</remarks>
        private void SuspendParentControls()
        {
            if (this.Parent != null && suspendParentControlsLayout)
            {
                this.parentControlsLayoutSuspended++;
                this.Parent.SuspendLayout();

                foreach (Control control in this.Parent.Controls)
                    if (control != null && control != this)
                        control.SuspendLayout();
            }

            if (this.additionalSuspendControl != null && suspendParentControlsLayout)
            {
                this.additionalSuspendControl.SuspendLayout();
                if (additionalSuspendControl is GradientPanel)
                    (additionalSuspendControl as GradientPanel).LayoutSuspended = true;

                foreach (Control control in this.additionalSuspendControl.Controls)
                    if (control != null && control != this)
                        control.SuspendLayout();
            }
        }

        /// <summary>
        /// Resumes the layout of all children of the parent (siblings).
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-21</remarks>
        private void ResumeParentControls()
        {
            if (this.Parent != null && suspendParentControlsLayout)
            {
                this.parentControlsLayoutSuspended--;
                if (this.parentControlsLayoutSuspended < 0)
                    this.parentControlsLayoutSuspended = 0;
                this.Parent.ResumeLayout();

                foreach (Control control in this.Parent.Controls)
                    if (control != null && control != this)
                        control.ResumeLayout();
            }

            if (this.additionalSuspendControl != null && suspendParentControlsLayout)
            {
                this.additionalSuspendControl.ResumeLayout();
                if (additionalSuspendControl is GradientPanel)
                    (additionalSuspendControl as GradientPanel).LayoutSuspended = false;

                foreach (Control control in this.additionalSuspendControl.Controls)
                    if (control != null && control != this)
                        control.ResumeLayout();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [parent controls layout suspended].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [parent controls layout suspended]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2009-05-14</remarks>
        public bool ParentControlsLayoutSuspended
        {
            get
            {
                if (!suspendParentControlsLayout)
                    return false;

                return this.parentControlsLayoutSuspended > 0;
            }
            set
            {
                if (value && this.parentControlsLayoutSuspended > 0)
                    SuspendParentControls();
                else if (!value)
                {
                    while (this.parentControlsLayoutSuspended > 0)
                        ResumeParentControls();
                }

            }
        }

        private bool suspendParentControlsLayout = false;

        /// <summary>
        /// Gets or sets a value indicating whether to [suspend parent children layout] to improve performance.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [suspend parent children layout]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-05-21</remarks>
        [Category("Behavior"), Description("Whether to [suspend the layout of all parent controls] to improve performance."), DefaultValue(true)]
        public bool SuspendParentControlsLayout
        {
            get { return suspendParentControlsLayout; }
            set { suspendParentControlsLayout = value; }
        }

        /// <summary>
        /// Cleans up all infobars on the selected control.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <remarks>Documented by Dev02, 2008-05-21</remarks>
        public static void CleanUpInfobars(Control parent)
        {
            if (parent != null)
            {
                foreach (Control control in parent.Controls)
                {
                    if (control is InfoBar)
                    {
                        InfoBar bar = (InfoBar)control;
                        bar.ParentControlsLayoutSuspended = false;
                        bar.Visible = false;
                        bar.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when [dont show again changed].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-06-09</remarks>
        public event EventHandler DontShowAgainChanged;

        /// <summary>
        /// Handles the CheckedChanged event of the checkBoxDontShowAgain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-06-09</remarks>
        private void checkBoxDontShowAgain_CheckedChanged(object sender, EventArgs e)
        {
            if (DontShowAgainChanged != null)
                DontShowAgainChanged(this, e);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dont show again].
        /// </summary>
        /// <value><c>true</c> if [dont show again]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-06-09</remarks>
        public bool DontShowAgain
        {
            get
            {
                return checkBoxDontShowAgain.Checked;
            }
            set
            {
                checkBoxDontShowAgain.Checked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dont show again visible].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dont show again visible]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-06-09</remarks>
        public bool DontShowAgainVisible
        {
            get
            {
                return checkBoxDontShowAgain.Visible;
            }
            set
            {
                if (checkBoxDontShowAgain.Visible != value)
                {
                    checkBoxDontShowAgain.Visible = value;
                    if (value)
                        labelInfo.Width -= checkBoxDontShowAgain.Width;
                    else
                        labelInfo.Width += checkBoxDontShowAgain.Width;
                }
            }
        }
    }
}
