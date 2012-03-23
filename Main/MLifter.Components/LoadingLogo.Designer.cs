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
namespace MLifter.Components
{
    partial class LoadingLogo
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingLogo));
            this.labelLoading = new System.Windows.Forms.Label();
            this.loadingCircleMain = new MRG.Controls.UI.LoadingCircle();
            this.SuspendLayout();
            // 
            // labelLoading
            // 
            this.labelLoading.AccessibleDescription = null;
            this.labelLoading.AccessibleName = null;
            resources.ApplyResources(this.labelLoading, "labelLoading");
            this.labelLoading.Name = "labelLoading";
            // 
            // loadingCircleMain
            // 
            this.loadingCircleMain.AccessibleDescription = null;
            this.loadingCircleMain.AccessibleName = null;
            this.loadingCircleMain.Active = true;
            resources.ApplyResources(this.loadingCircleMain, "loadingCircleMain");
            this.loadingCircleMain.BackgroundImage = null;
            this.loadingCircleMain.Color = System.Drawing.Color.DimGray;
            this.loadingCircleMain.CustomImage = null;
            this.loadingCircleMain.Font = null;
            this.loadingCircleMain.InnerCircleRadius = 7;
            this.loadingCircleMain.Name = "loadingCircleMain";
            this.loadingCircleMain.NumberSpoke = 36;
            this.loadingCircleMain.OuterCircleRadius = 8;
            this.loadingCircleMain.RotationSpeed = 20;
            this.loadingCircleMain.SpokeThickness = 4;
            this.loadingCircleMain.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MLifter;
            // 
            // LoadingLogo
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::MLifter.Components.Properties.Resources.MLSplashScreen_144x33_white;
            this.Controls.Add(this.labelLoading);
            this.Controls.Add(this.loadingCircleMain);
            this.DoubleBuffered = true;
            this.Font = null;
            this.Name = "LoadingLogo";
            this.Load += new System.EventHandler(this.LoadingLogo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MRG.Controls.UI.LoadingCircle loadingCircleMain;
        private System.Windows.Forms.Label labelLoading;

    }
}
