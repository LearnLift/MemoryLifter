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
using System.Threading;

namespace MLifter.Controls.Wizards.Startup
{
    public partial class DictionaryPathPage : MLifter.WizardPage
    {
        ToolTip invalidCharsToolTip = new ToolTip();
        string DictionaryParentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string DefaultDictionaryPath = String.Empty;
        string DefaultDictionaryOldPath = String.Empty;
        List<Thread> updatethreads = new List<Thread>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryPathPage"/> class.
        /// </summary>
        /// <param name="dictionaryParentPath">The dictionary parent path.</param>
        /// <param name="dictionaryPath">The default dictionary path.</param>
        /// <param name="oldDictionaryPath">The old default dictionary path (before ML was localized).</param>
        /// <remarks>Documented by Dev03, 2008-02-26</remarks>
        public DictionaryPathPage(string dictionaryParentPath, string dictionaryPath, string oldDictionaryPath)
        {
            if (Directory.Exists(dictionaryParentPath))
                this.DictionaryParentPath = dictionaryParentPath;
            this.DefaultDictionaryPath = dictionaryPath;
            this.DefaultDictionaryOldPath = oldDictionaryPath;

            InitializeComponent();

            invalidCharsToolTip.Active = false;
            //invalidCharsToolTip.IsBalloon = true;
            invalidCharsToolTip.ShowAlways = true;
        }

        /// <summary>
        /// Handles the Load event of the DictionaryPathPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        private void DictionaryPathPage_Load(object sender, EventArgs e)
        {
            string path = Path.Combine(DictionaryParentPath, DefaultDictionaryOldPath);
            if (!Directory.Exists(path))
                path = Path.Combine(DictionaryParentPath, DefaultDictionaryPath);
            textBoxPath.Text = path;
            textBoxPath.Focus();
        }

        /// <summary>
        /// Handles the TextChanged event of the textBoxPath control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            bool valid = textBoxPath.Text.IndexOfAny(Path.GetInvalidPathChars()) == -1;

            if (valid)
                try
                {
                    new DirectoryInfo(textBoxPath.Text);
                }
                catch
                {
                    valid = false;
                }

            if (valid)
            {
                invalidCharsToolTip.Hide(textBoxPath);
                invalidCharsToolTip.Active = false;
                UpdateValues();
                NextAllowed = true;
            }
            else
            {
                invalidCharsToolTip.Active = true;
                invalidCharsToolTip.Show(Resources.DICPATH_INVALIDINPUT_TEXT, textBoxPath, 60, -20);
                NextAllowed = false;
            }
        }

        /// <summary>
        /// Updates the values.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        private void UpdateValues()
        {
            Thread thread = new Thread(delegate()
            {
                try
                {
                    DriveInfo info = new DriveInfo(textBoxPath.Text[0].ToString());
                    Invoke((MethodInvoker)delegate
                    {
                        labelFreeSpace.Text = Tools.FileSizeToString(info.AvailableFreeSpace);
                        labelTotalSpace.Text = Tools.FileSizeToString(info.TotalSize);
                    });
                }
                catch (Exception exp)
                {
                    if (!(exp is ThreadAbortException))
                        Invoke((MethodInvoker)delegate
                        {
                            labelFreeSpace.Text = "";
                            labelTotalSpace.Text = "";
                        });
                }

                try
                {
                    // [ML-1751] First start, number of LMs in new lm-folder counts ODX instead of MLM
                    int numberOfLearningModules = Directory.GetFiles(textBoxPath.Text, "*.odx", SearchOption.AllDirectories).Length;
                    numberOfLearningModules += Directory.GetFiles(textBoxPath.Text, "*.mlm", SearchOption.AllDirectories).Length;

                    Invoke((MethodInvoker)delegate { labelDictionariesInPath.Text = numberOfLearningModules.ToString(); });
                }
                catch (Exception exp)
                {
                    if (!(exp is ThreadAbortException))
                        Invoke((MethodInvoker)delegate { labelDictionariesInPath.Text = "0"; });
                }
            });
            thread.Name = "Directory Path Updater";
            thread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            thread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            updatethreads.Add(thread);
            thread.Start();
        }

        /// <summary>
        /// Handles the Click event of the buttonBrowse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (Directory.Exists(textBoxPath.Text))
                folderDialog.SelectedPath = textBoxPath.Text;
            else
                folderDialog.SelectedPath = DictionaryParentPath;

            if (folderDialog.ShowDialog() == DialogResult.OK)
                textBoxPath.Text = folderDialog.SelectedPath;
        }

        /// <summary>
        /// Gets or sets a value indicating whether copy demo dictionary.
        /// </summary>
        /// <value><c>true</c> if [copy demo dictionary]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        public bool CopyDemoDictionary { get { return checkBoxDemoDic.Checked; } set { checkBoxDemoDic.Checked = value; } }
        /// <summary>
        /// Gets or sets the dictionary path.
        /// </summary>
        /// <value>The dictionary path.</value>
        /// <remarks>Documented by Dev05, 2007-12-11</remarks>
        public string DictionaryPath { get { return textBoxPath.Text; } set { textBoxPath.Text = value; } }

        /// <summary>
        /// Called if the Help Button is clicked.
        /// </summary>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev03, 2008-02-22</remarks>
        public override void ShowHelp()
        {
            //TODO link the correct help topic id
            Help.ShowHelp(this.ParentForm, this.ParentWizard.HelpFile, HelpNavigator.Topic, "/html/Starting_Memory_Lifter.htm");
        }

        /// <summary>
        /// Releases all resources used by the <see cref="T:System.ComponentModel.Component"></see>.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-02-27</remarks>
        ~DictionaryPathPage()
        {
            AbortUpdateThreads();
        }

        /// <summary>
        /// Called if the next-button is clicked.
        /// </summary>
        /// <returns>
        /// 	<i>false</i> to abort, otherwise<i>true</i>
        /// </returns>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev02, 2008-03-03</remarks>
        public override bool GoNext()
        {
            AbortUpdateThreads();
            return base.GoNext();
        }

        /// <summary>
        /// Called if the cancel-button is clicked.
        /// </summary>
        /// <returns>
        /// 	<i>false</i> to abort, otherwise<i>true</i>
        /// </returns>
        /// <remarks>Documented by Dev05, 2007-11-21</remarks>
        /// <remarks>Documented by Dev02, 2008-03-03</remarks>
        public override bool Cancel()
        {
            AbortUpdateThreads();
            return base.Cancel();
        }

        /// <summary>
        /// Cancels the update threads.
        /// </summary>
        /// <remarks>Documented by Dev02, 2008-03-03</remarks>
        private void AbortUpdateThreads()
        {
            foreach (Thread thread in updatethreads)
            {
                if (thread != null && thread.IsAlive)
                    thread.Abort();
            }
        }
    }
}

