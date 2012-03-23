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
using System.IO;

using MLifter.Controls;
using MLifter.Controls.Properties;

namespace MLifter.Controls
{
    public partial class DictionaryProperties : UserControl
    {
        string dictionaryPath = string.Empty;
        string dictionaryParentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryProperties"/> class.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-10</remarks>
        public DictionaryProperties()
        {
            InitializeComponent();

            textBoxTitle.Text = Resources.NEWDIC_NEW;

            //comboBoxCategory.Items.AddRange(MLifter.DAL.Category.CategoryNames);
            //comboBoxCategory.SelectedIndex = 7;

            comboBoxCategory.Items.AddRange(MLifter.BusinessLayer.Dictionary.Categories.ToArray());
            comboBoxCategory.SelectedItem = MLifter.BusinessLayer.Dictionary.DefaultCategory;
        }

        /// <summary>
        /// Gets or sets the dictionary path.
        /// </summary>
        /// <value>The dictionary path.</value>
        /// <remarks>Documented by Dev02, 2008-01-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryPath
        {
            get { return dictionaryPath; }
            set { dictionaryPath = value; }
        }

        /// <summary>
        /// Gets or sets the dictionary parent path.
        /// </summary>
        /// <value>The dictionary parent path.</value>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryParentPath
        {
            get { return dictionaryParentPath; }
            set { dictionaryParentPath = value; UpdateLocation(); }
        }

        /// <summary>
        /// Gets or sets the enabled-property for all editable controls!
        /// Enables or disables the category/author/description control
        /// </summary>
        /// <remarks>Documented by Dev08, 2009-04-28</remarks>
        [Category("Appearance"), Description("Gets or sets the enabled-property for all editable controls")]
        public bool EditableControlsEnabled
        {
            get
            {
                return comboBoxCategory.Enabled;
            }
            set
            {
                comboBoxCategory.Enabled = value;
                textBoxAuthor.ReadOnly = textBoxDescription.ReadOnly = textBoxLocation.ReadOnly = !value;
            }
        }

        /// <summary>
        /// Handles the Load event of the DictionaryProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev02, 2008-01-10</remarks>
        private void DictionaryProperties_Load(object sender, EventArgs e)
        {
            textBoxTitle.Focus();

            if (string.IsNullOrEmpty(dictionaryPath))
                UpdateLocation();
            else
                textBoxLocation.Text = DictionaryPath;
        }

        /// <summary>
        /// Handles the Enter event of the textBoxTitle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        private void textBoxTitle_Enter(object sender, EventArgs e)
        {
            if (textBoxTitle.Text == Resources.NEWDIC_NEW)
                textBoxTitle.SelectAll();
        }

        /// <summary>
        /// Handles the MouseUp event of the textBoxTitle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        private void textBoxTitle_MouseUp(object sender, MouseEventArgs e)
        {
            //if (textBoxTitle.Text == Resources.NEWDIC_NEW)
            //    textBoxTitle.SelectAll();
        }

        /// <summary>
        /// Updates the dictionary location field.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-01-15</remarks>
        private void UpdateLocation()
        {
            if (DictionaryLocationVisible)
            {
                if (Directory.Exists(dictionaryParentPath))
                {
                    textBoxLocation.Text = Path.Combine(dictionaryParentPath, DictionaryName);
                    textBoxLocation.SelectionLength = 0;
                    textBoxLocation.SelectionStart = textBoxLocation.Text.Length;
                }
            }
        }

        /// <summary>
        /// Gets or sets the dictionary category.
        /// </summary>
        /// <value>The dictionary category.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public MLifter.DAL.Category DictionaryCategory
        {
            get
            {
                return (MLifter.DAL.Category)comboBoxCategory.SelectedItem;
            }
            set
            {
                if (!comboBoxCategory.Items.Contains(value))
                    comboBoxCategory.Items.Add(value);

                comboBoxCategory.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the dictionary.
        /// </summary>
        /// <value>The name of the dictionary.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryName { get { return textBoxTitle.Text; } set { textBoxTitle.Text = value; } }

        /// <summary>
        /// Gets or sets the dictionary author.
        /// </summary>
        /// <value>The dictionary author.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryAuthor { get { return textBoxAuthor.Text; } set { textBoxAuthor.Text = value; } }

        /// <summary>
        /// Gets or sets the dictionary Description.
        /// </summary>
        /// <value>The dictionary Description.</value>
        /// <remarks>Documented by Dev05, 2007-12-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryDescription { get { return textBoxDescription.Text; } set { textBoxDescription.Text = value; } }

        /// <summary>
        /// Gets or sets the dictionary location.
        /// </summary>
        /// <value>The dictionary location.</value>
        /// <remarks>Documented by Dev02, 2008-01-10</remarks>
        [Browsable(false), ReadOnly(true)]
        public string DictionaryLocation { get { return textBoxLocation.Text; } set { textBoxLocation.Text = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the control displays the dictionary location.
        /// </summary>
        /// <value><c>true</c> if [show path]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev02, 2008-01-10</remarks>
        [Category("Appearance"), Description("Gets or sets a value indicating whether the control displays the dictionary location.")]
        public bool DictionaryLocationVisible
        {
            get { return textBoxLocation.Visible; }
            set { textBoxLocation.Visible = buttonBrowse.Visible = label5.Visible = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dictionary name is shown as read only..
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [dictionary name read only]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev02, 2008-01-11</remarks>
        [Category("Behavior"), Description("Gets or sets a value indicating whether the dictionary name is shown as read only.")]
        public bool DictionaryNameReadOnly { get { return textBoxTitle.ReadOnly; } set { textBoxTitle.ReadOnly = value; } }

        /// <summary>
        /// Handles the Click event of the buttonBrowse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-17</remarks>
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (textBoxLocation.Visible)
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();

                if (Directory.Exists(textBoxLocation.Text))
                    folderDialog.SelectedPath = textBoxLocation.Text;
                else
                    folderDialog.SelectedPath = dictionaryParentPath;

                if (folderDialog.ShowDialog() == DialogResult.OK && Directory.Exists(folderDialog.SelectedPath))
                {
                    dictionaryParentPath = new DirectoryInfo(folderDialog.SelectedPath).FullName;
                    UpdateLocation();
                }
            }
        }
    }
}
