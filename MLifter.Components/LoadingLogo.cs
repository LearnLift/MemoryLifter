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
