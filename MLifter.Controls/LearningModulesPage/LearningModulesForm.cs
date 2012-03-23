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
using MLifter.DAL.Interfaces;
using MLifter.BusinessLayer;
using MLifter.DAL;
using MLifter.Components;
using MLifter.Controls.LearningModulesPage;

namespace MLifter.Controls
{
    public partial class LearningModulesForm : Form
    {
        private LearningModulesIndexEntry selectedConnection;
        private DriveDetector driveDetector;
        /// <summary>
        /// Gets or sets the selected connection.
        /// </summary>
        /// <value>The selected connection.</value>
        public LearningModulesIndexEntry SelectedConnection
        {
            get { return selectedConnection; }
            set { selectedConnection = value; }
        }

        public bool IsUsedDragAndDrop { get; set; }

        private bool createNewLearningModule = false;
        /// <summary>
        /// Gets or sets a value indicating whether create a new learning module or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if create new learning module was selected; otherwise, <c>false</c>.
        /// </value>
        public bool CreateNewLearningModule
        {
            get { return createNewLearningModule; }
        }

        /// <summary>
        /// Sets the general configuration path.
        /// </summary>
        /// <value>The general configuration path.</value>
        public string GeneralConfigurationPath
        {
            set
            {
                learningModulesPageMain.GeneralConfigurationPath = value;
            }
        }

        /// <summary>
        /// Sets the user configuration path.
        /// </summary>
        /// <value>The user configuration path.</value>
        public string UserConfigurationPath
        {
            set
            {
                learningModulesPageMain.UserConfigurationPath = value;
            }
        }

        /// <summary>
        /// Sets the synced learning module path.
        /// </summary>
        /// <value>The synced learning module path.</value>
        public string SyncedLearningModulePath
        {
            set
            {
                LearningModulesPage.LearningModulesPage.SyncedLearningModulePath = value;
            }
        }

        public HelpProvider MainHelpObject
        {
            get
            {
                return MainHelp;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether first use.
        /// </summary>
        /// <value><c>true</c> if [first use]; otherwise, <c>false</c>.</value>
        /// <remarks>Documented by Dev05, 2009-05-22</remarks>
        public bool FirstUse { get { return learningModulesPageMain.FirstUse; } set { learningModulesPageMain.FirstUse = value; } }

        /// <summary>
        /// Gets a value indicating whether [show start page at startup].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show start page at startup; otherwise open last learning module.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-09</remarks>
        public bool ShowStartPageAtStartup { get { return learningModulesPageMain.ShowStartPageAtStartup; } set { learningModulesPageMain.ShowStartPageAtStartup = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningModulesForm"/> class.
        /// </summary>
        public LearningModulesForm(string helpfile)
        {
            InitializeComponent();
            MainHelp.HelpNamespace = helpfile;

            this.CancelButton = learningModulesPageMain.ButtonCancel;

            if (!learningModulesPageMain.PersistencyLoaded && (Screen.GetWorkingArea(this).Height < 750 || Screen.GetWorkingArea(this).Width < 1005))
            {
                this.Height = 550;
                this.Width = 795;
            }
            else if (learningModulesPageMain.PersistencyLoaded)
            {
                if (!learningModulesPageMain.Maximized)
                {
                    learningModulesPageMain.Loading = true;
                    this.Size = new Size(learningModulesPageMain.PersistancySize.Width + Width - ClientSize.Width, learningModulesPageMain.PersistancySize.Height + Height - ClientSize.Height);
                    learningModulesPageMain.Loading = false;
                }
                else
                    this.WindowState = FormWindowState.Maximized;
            }

            this.DialogResult = DialogResult.Abort;
        }

        /// <summary>
        /// Loads the learning modules.
        /// </summary>
        public void LoadLearningModules()
        {
            learningModulesPageMain.LoadLearningModules();

            driveDetector = new DriveDetector(this);
            driveDetector.DeviceArrived += new DriveDetectorEventHandler(driveDetector_DeviceArrived);
            driveDetector.DeviceRemoved += new DriveDetectorEventHandler(driveDetector_DeviceRemoved);
        }

        /// <summary>
        /// WNDs the proc.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <remarks>Documented by Dev05, 2009-04-01</remarks>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (driveDetector != null)
                driveDetector.WndProc(ref m);
        }

        /// <summary>
        /// Handles the DeviceRemoved event of the driveDetector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.Components.DriveDetectorEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-01</remarks>
        private void driveDetector_DeviceRemoved(object sender, DriveDetectorEventArgs e)
        {
            learningModulesPageMain.RemoveDrive(e.Drive);
        }

        /// <summary>
        /// Handles the DeviceArrived event of the driveDetector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MLifter.Components.DriveDetectorEventArgs"/> instance containing the event data.</param>
        /// <remarks>Documented by Dev05, 2009-04-01</remarks>
        private void driveDetector_DeviceArrived(object sender, DriveDetectorEventArgs e)
        {
            learningModulesPageMain.AddDrive(e.Drive);
        }

        private void learningModulesPageMain_LearningModuleSelected(object sender, LearningModuleSelectedEventArgs e)
        {
            selectedConnection = e.LearnModule;
            IsUsedDragAndDrop = e.IsUsedDragAndDrop;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void learningModulesPageMain_CancelPressed(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void learningModulesPageMain_NewLearningModulePressed(object sender, System.EventArgs e)
        {
            createNewLearningModule = true;

            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void LearningModulesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            learningModulesPageMain.Close();
        }

        private void LearningModulesForm_SizeChanged(object sender, EventArgs e)
        {
            learningModulesPageMain.Maximized = (this.WindowState == FormWindowState.Maximized);
        }
    }
}
