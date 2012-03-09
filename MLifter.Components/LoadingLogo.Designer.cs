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
