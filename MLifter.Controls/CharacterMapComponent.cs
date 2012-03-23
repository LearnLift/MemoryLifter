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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace MLifter.Controls.LearningWindow
{
    public partial class CharacterMapComponent : Component, IDisposable
    {
        private CharacterForm characterform = null;
        private Control currentControl = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMapComponent"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public CharacterMapComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterMapComponent"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public CharacterMapComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Handles the ControlRemoved event of the target control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ControlEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        void target_ControlRemoved(object sender, ControlEventArgs e)
        {
            e.Control.Enter -= new EventHandler(control_Enter);
        }

        /// <summary>
        /// Handles the ControlAdded event of the target control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ControlEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        void target_ControlAdded(object sender, ControlEventArgs e)
        {
            RegisterControl(e.Control);
        }

        /// <summary>
        /// Registers the controls.
        /// </summary>
        /// <param name="controls">The controls.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public void RegisterControls(System.Windows.Forms.Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                RegisterControl(control);
            }
        }

        /// <summary>
        /// Registers the form.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="closeCharactermapWithForm">if set to <c>true</c> [close charactermap with form].</param>
        /// <remarks>Documented by Dev02, 2008-05-13</remarks>
        public void RegisterForm(Form form, bool closeCharactermapWithForm)
        {
            RegisterControl(form);
            if (closeCharactermapWithForm)
                form.FormClosing += new FormClosingEventHandler(form_FormClosing);
        }

        /// <summary>
        /// Handles the FormClosing event of the form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Visible)
                this.Visible = false;
        }

        /// <summary>
        /// Registers the control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public void RegisterControl(Control control)
        {
            if (control != null)
            {
                if (control.Controls.Count > 0)
                {
                    control.ControlAdded += new ControlEventHandler(target_ControlAdded);
                    control.ControlRemoved += new ControlEventHandler(target_ControlRemoved);
                    RegisterControls(control.Controls);
                }

                if (CharacterForm.CanControl(control))
                    control.Enter += new EventHandler(control_Enter);
            }
        }

        /// <summary>
        /// Handles the Enter event of the control control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        void control_Enter(object sender, EventArgs e)
        {
            currentControl = (Control)sender;
            System.Diagnostics.Debug.WriteLine("CharacterMapComponent CurrentControl: " + currentControl.Name);
            if (this.Visible)
                characterform.CurrentControl = currentControl;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CharacterMap is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        [Description("Gets or sets a value indicating whether the CharacterMap is visible."), DefaultValue(false)]
        public bool Visible
        {
            get
            {
                return (characterform != null && characterform.Visible);
            }
            set
            {
                if (value != Visible)
                {
                    if (value)
                    {
                        characterform = new CharacterForm();
                        characterform.VisibleChanged += new EventHandler(characterform_VisibleChanged);
                        characterform.FormClosed += new FormClosedEventHandler(characterform_FormClosed);
                        if (currentControl != null)
                            characterform.CurrentControl = currentControl;
                        characterform.Show();
                    }
                    else
                    {
                        characterform.Close();
                        characterform.Dispose();
                        characterform = null;
                    }
                    if (VisibleChanged != null)
                        VisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the CharacterMap is focused.
        /// </summary>
        /// <value><c>true</c> if focused; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-05-13</remarks>
        public bool Focused
        {
            get
            {
                return (characterform != null && characterform.Focused);
            }
        }

        /// <summary>
        /// Handles the FormClosed event of the characterform control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        void characterform_FormClosed(object sender, FormClosedEventArgs e)
        {
            //to trigger the visiblechanged event
            characterform.Visible = false;
        }

        /// <summary>
        /// Handles the VisibleChanged event of the characterform control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        void characterform_VisibleChanged(object sender, EventArgs e)
        {
            if (VisibleChanged != null)
                VisibleChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when [visible changed].
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        public event EventHandler VisibleChanged;

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by the <see cref="T:System.ComponentModel.Component"></see>.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-05-08</remarks>
        void IDisposable.Dispose()
        {
            if (this.Visible)
                this.Visible = false;
        }

        #endregion
    }
}
