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

namespace MLifter.Components
{
    /// <summary>
    /// Display ML logo (and loading animation).
    /// </summary>
    /// <remarks>Documented by Dev05, 2009-03-10</remarks>
    public partial class LoadingLogo : UserControl
    {
        private bool isLoading = true;
        /// <summary>
        /// Gets or sets a value indicating whether this displays "Loading...".
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this displays "Loading..."; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Documented by Dev05, 2009-03-10</remarks>
        [Description("Displays \"Loading...\""), Browsable(true), DefaultValue(true)]
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                loadingCircleMain.Visible = value;
                loadingCircleMain.Active = value;
                labelLoading.Visible = value;
                BackgroundImage = value ? Resources.MLSplashScreen_144x33_white : Resources.MLSplashScreen;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingLogo"/> class.
        /// </summary>
        /// <remarks>Documented by Dev05, 2009-03-10</remarks>
        public LoadingLogo()
        {
            InitializeComponent();

            loadingCircleMain.InnerCircleRadius = 7;
            loadingCircleMain.NumberSpoke = 36;
            loadingCircleMain.OuterCircleRadius = 8;
            loadingCircleMain.RotationSpeed = 20;
            loadingCircleMain.SpokeThickness = 4;
        }

        private void LoadingLogo_Load(object sender, EventArgs e)
        {

        }



    }
}
