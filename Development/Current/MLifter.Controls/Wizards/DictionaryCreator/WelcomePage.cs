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
using System.IO;

using MLifter.Controls.Properties;

namespace MLifter.Controls.Wizards.DictionaryCreator
{
    public partial class WelcomePage : MLifter.WizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomePage"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        public WelcomePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [editable controls enabled].
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-28</remarks>
        public bool EditableControlsEnabled
        {
            get
            {
                return dictionaryProperties.EditableControlsEnabled;
            }
            set
            {
                dictionaryProperties.EditableControlsEnabled = value;
            }
        }

        /// <summary>
        /// Handles the Load event of the WelcomePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        private void WelcomePage_Load(object sender, EventArgs e) { dictionaryProperties.DictionaryLocationVisible = false; }

        /// <summary>
        /// Handles the VisibleChanged event of the WelcomePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-02-27</remarks>
        private void WelcomePage_VisibleChanged(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            if (Visible)
            {
                try
                {
                    if (ParentWizard == null)
                        return;
                    SourceSelectionPage sp = ParentWizard.Pages.Find(p => p is SourceSelectionPage) as SourceSelectionPage;
                    dictionaryProperties.DictionaryLocation = sp.ConnectionString.ConnectionString;
                }
                catch (NullReferenceException)
                {
                    dictionaryProperties.DictionaryLocation = MLifter.BusinessLayer.LearningModulesIndex.WritableConnections[0].ConnectionString;
                }
            }
        }

        /// <summary>
        /// Called if the next-button is clicked.
        /// </summary>
        /// <returns>
        /// 	<i>false</i> to abort, otherwise<i>true</i>
        /// </returns>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        public override bool GoNext()
        {
            if (String.IsNullOrEmpty(DictionaryName) || String.IsNullOrEmpty(DictionaryLocation) || (DictionaryName.Trim().Length == 0))
            {
                MessageBox.Show(Resources.NEWDIC_INVALIDINPUT_TEXT, Resources.NEWDIC_INVALIDINPUT_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return base.GoNext();
        }

        /// <summary>
        /// Gets or sets the dictionary category.
        /// </summary>
        /// <value>The dictionary category.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public MLifter.DAL.Category DictionaryCategory { get { return dictionaryProperties.DictionaryCategory; } set { dictionaryProperties.DictionaryCategory = value; } }

        /// <summary>
        /// Gets or sets the name of the dictionary.
        /// </summary>
        /// <value>The name of the dictionary.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryName { get { return dictionaryProperties.DictionaryName; } set { dictionaryProperties.DictionaryName = value; } }

        /// <summary>
        /// Gets or sets the dictionary author.
        /// </summary>
        /// <value>The dictionary author.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryAuthor { get { return dictionaryProperties.DictionaryAuthor; } set { dictionaryProperties.DictionaryAuthor = value; } }

        /// <summary>
        /// Gets or sets the dictionary Description.
        /// </summary>
        /// <value>The dictionary Description.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryDescription { get { return dictionaryProperties.DictionaryDescription; } set { dictionaryProperties.DictionaryDescription = value; } }

        /// <summary>
        /// Gets or sets the dictionary location.
        /// </summary>
        /// <value>The dictionary location.</value>
        /// <remarks>Documented by Dev02, 2008-01-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryLocation { get { return dictionaryProperties.DictionaryLocation; } set { dictionaryProperties.DictionaryLocation = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the control displays the dictionary location.
        /// </summary>
        /// <value><c>true</c> if [show path]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        [Category("Appearance"), Description("Gets or sets a value indicating whether the control displays the dictionary location.")]
        public bool DictionaryLocationVisible { get { return dictionaryProperties.DictionaryLocationVisible; } set { dictionaryProperties.DictionaryLocationVisible = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the dictionary name is shown as read only..
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dictionary name read only]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        [Category("Behavior"), Description("Gets or sets a value indicating whether the dictionary name is shown as read only.")]
        public bool DictionaryNameReadOnly { get { return dictionaryProperties.DictionaryNameReadOnly; } set { dictionaryProperties.DictionaryNameReadOnly = value; } }

        /// <summary>
        /// Gets or sets the title label.
        /// </summary>
        /// <value>The title.</value>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        [Category("Appearance"), Description("Gets or sets the Text for the title label.")]
        public string Title { get { return labelWelcome.Text; } set { labelWelcome.Text = value; } }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/memo9sqf.htm");
        }
    }
}

